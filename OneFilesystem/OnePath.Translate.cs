#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion

namespace ArxOne.OneFilesystem
{
    using System;
    using System.Linq;
    using System.Threading;

    partial class OnePath
    {
        private const string Localhost = "localhost";

        /// <summary>
        /// Initializes instance with the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        private void Init(Uri uri)
        {
            Protocol = uri.Scheme;
            Host = uri.Host;
            if (Host == "")
                Host = null;
            if (uri.Port >= 0)
                Port = uri.Port;
            Path = MakePath(uri.AbsolutePath.Split('/'));
        }

        /// <summary>
        /// Loads the path from a given URI.
        /// </summary>
        /// <param name="localPathOrUri">The local path or URI.</param>
        /// <returns>true if OK</returns>
        private bool LoadUri(string localPathOrUri)
        {
            // first, let's check for a disguised URI
            if (localPathOrUri.Contains("://"))
            {
                try
                {
                    Init(new Uri(localPathOrUri));
                    return true;
                }
                catch (UriFormatException)
                {
                    var parts = localPathOrUri.Split(new[] { "://" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 1)
                    {
                        Protocol = parts[0];
                        Host = "";
                        Path = new string[0];
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Loads the network root.
        /// </summary>
        /// <param name="localPathOrUri">The local path or URI.</param>
        /// <returns>true if OK</returns>
        private bool LoadNetworkRootWin32(string localPathOrUri)
        {
            // otherwise, this is a local path
            // first, look for a simple network root "\\"
            if (localPathOrUri == @"\\")
            {
                Protocol = Uri.UriSchemeFile;
                Host = "";
                Path = new string[0];
                return true;
            }
            return false;
        }

        /// <summary>
        /// Loads the network server.
        /// </summary>
        /// <param name="localPathOrUri">The local path or URI.</param>
        /// <returns></returns>
        private bool LoadNetworkServerWin32(string localPathOrUri)
        {
            // then, for a server "\\server" or "\\server\"
            if (localPathOrUri.StartsWith(@"\\"))
            {
                var server = localPathOrUri.Trim('\\');
                if (!server.Contains('\\'))
                {
                    Protocol = Uri.UriSchemeFile;
                    Host = server;
                    Path = new string[0];
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Loads the win32.
        /// </summary>
        /// <param name="localPathOrUri">The local path or URI.</param>
        /// <returns>true if OK</returns>
        private bool LoadWin32(string localPathOrUri)
        {
            Protocol = Uri.UriSchemeFile;
            // otherwise, standard path
            var fullPath = System.IO.Path.GetFullPath(localPathOrUri);
            var allParts = fullPath.Split(Separators);
            if (allParts.Length > 2 && allParts[0] == "" && allParts[1] == "")
            {
                // remote path
                var partsList = allParts.ToList();
                while (partsList.Count > 0 && string.IsNullOrEmpty(partsList[0]))
                    partsList.RemoveAt(0);
                Host = partsList[0];
                Path = MakePath(partsList.Skip(1));
            }
            else
            {
                // localpath here
                Host = Localhost;
                Path = MakePath(allParts);
            }
            return true;
        }

        /// <summary>
        /// Gets the literal path, OS style.
        /// </summary>
        /// <returns></returns>
        private string GetLiteralWin32()
        {
            if (!string.Equals(Protocol, "file", StringComparison.InvariantCultureIgnoreCase))
                return null;
            if (Host == "")
                return @"\\";
            var localpath = string.Join(@"\", Path);
            if (IsLocalHost)
                return localpath;
            if (localpath == "")
                return string.Format(@"\\{0}", Host);
            return string.Format(@"\\{0}\{1}", Host, localpath);
        }

        private bool LoadRootWin32(string localPathOrUri)
        {
            if (localPathOrUri != "")
                return false;

            Protocol = "";
            Host = "";
            Path = new string[0];
            return true;
        }

        /// <summary>
        /// Gets the literal URI.
        /// </summary>
        /// <returns></returns>
        private string GetLiteralUri()
        {
            return string.Format("{0}://{1}{2}{3}/{4}",
                Protocol, Host ?? "",
                Port.HasValue ? ":" : "", Port.HasValue ? Port.Value.ToString() : "",
                string.Join("/", Path));
        }

        /// <summary>
        /// Gets the literal root.
        /// </summary>
        /// <returns></returns>
        private string GetLiteralRoot()
        {
            if (Protocol == "")
                return "";
            return null;
        }
    }
}
