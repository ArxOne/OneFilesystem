#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Represents a path
    /// </summary>
    [DebuggerDisplay("{Uri}")]
    public class OnePath
    {
        private static readonly char[] Separators = { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar };

        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        public string Protocol { get; private set; }
        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; private set; }
        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        public int? Port { get; private set; }
        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public IList<string> Path { get; private set; }

        /// <summary>
        /// Gets the name (which is actually the last part of the path).
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                if (Path.Count > 0)
                    return Path[Path.Count - 1];
                return null;
            }
        }

        /// <summary>
        /// Gets the path as URI.
        /// </summary>
        /// <value>
        /// The URI.
        /// </value>
        public Uri Uri
        {
            get
            {
                return new Uri(string.Format("{0}://{1}{2}{3}/{4}",
                    Protocol, Host,
                    Port.HasValue ? ":" : "", Port.HasValue ? Port.Value.ToString() : "",
                    string.Join("/", Path)));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePath"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <exception cref="System.ArgumentNullException">uri</exception>
        public OnePath(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            Init(uri);
        }

        /// <summary>
        /// Initializes instance with the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        private void Init(Uri uri)
        {
            Protocol = uri.Scheme;
            Host = uri.Host;
            if (uri.Port >= 0)
                Port = uri.Port;
            Path = MakePath(uri.AbsolutePath.Split('/'));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePath"/> class.
        /// </summary>
        /// <param name="localPathOrUri">The local path or URI.</param>
        /// <exception cref="System.ArgumentNullException">localPathOrUri</exception>
        public OnePath(string localPathOrUri)
        {
            if (localPathOrUri == null)
                throw new ArgumentNullException("localPathOrUri");

            // first, let's check for a disguised URI
            if (localPathOrUri.Contains("://"))
            {
                try
                {
                    Init(new Uri(localPathOrUri));
                    return;
                }
                catch (UriFormatException)
                {
                }
            }

            // otherwise, this is a local path
            var fullPath = System.IO.Path.GetFullPath(localPathOrUri);
            var allParts = fullPath.Split(Separators);
            Protocol = Uri.UriSchemeFile;
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
                Host = "localhost";
                Path = MakePath(allParts);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnePath"/> class.
        /// </summary>
        /// <param name="protocol">The protocol.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="path">The path.</param>
        protected OnePath(string protocol, string host, int? port, IEnumerable<string> path)
        {
            Protocol = protocol;
            Host = host;
            Port = port;
            Path = new ReadOnlyCollection<string>(path.ToList());
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as OnePath;
            if (other == null)
                return false;
            return Protocol == other.Protocol
                   && Host == other.Host
                   && Port == other.Port
                   && Path.SequenceEqual(other.Path);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var h = Protocol.GetHashCode() ^ Host.GetHashCode() ^ Port.GetHashCode();
            foreach (var part in Path)
                h ^= part.GetHashCode();
            return h;
        }

        /// <summary>
        /// Makes the path clean, by eliminated empty parts, single and double dot specific parts.
        /// </summary>
        /// <param name="pathParts">The path parts.</param>
        /// <returns></returns>
        private static IList<string> MakePath(IEnumerable<string> pathParts)
        {
            var path = new List<string>();
            foreach (var part in pathParts)
            {
                if (part == "." || part == "")
                    continue;
                if (part == "..")
                {
                    if (path.Count > 0)
                        path.RemoveAt(path.Count - 1);
                    continue;
                }

                path.Add(part);
            }

            return new ReadOnlyCollection<string>(path);
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <returns></returns>
        public OnePath GetParent()
        {
            if (Path.Count == 0)
                return null;

            return new OnePath(Protocol, Host, Port, Path.Take(Path.Count - 1));
        }

        /// <summary>
        /// Gets the root (protocol and host without any path).
        /// </summary>
        /// <returns></returns>
        public OnePath GetRoot()
        {
            if (Path.Count == 0)
                return this;

            return new OnePath(Protocol, Host, Port, new string[0]);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="entryName">Name of the entry.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static OnePath operator +(OnePath path, string entryName)
        {
            return new OnePath(path.Protocol, path.Host, path.Port, path.Path.Concat(MakePath(entryName.Split(Separators))));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Uri"/> to <see cref="OnePath"/>.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator OnePath(Uri uri)
        {
            return new OnePath(uri);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="OnePath"/>.
        /// </summary>
        /// <param name="pathOrUri">The path or URI.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator OnePath(string pathOrUri)
        {
            return new OnePath(pathOrUri);
        }
    }

}
