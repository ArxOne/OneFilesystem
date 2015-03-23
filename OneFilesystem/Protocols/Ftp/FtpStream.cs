#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Protocols.Ftp
{
    using System;
    using System.IO;
    using ArxOne.Ftp.Exceptions;
    using Exceptions;
    using IO;

    /// <summary>
    /// FTP stream wrapper
    /// </summary>
    internal class FtpStream : TranslateStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FtpStream"/> class.
        /// </summary>
        /// <param name="innerStream">The inner stream.</param>
        public FtpStream(Stream innerStream)
            : base(innerStream)
        {
        }

        /// <summary>
        /// Translates exceptions.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        /// <exception cref="ArxOne.OneFilesystem.Exceptions.OneFilesystemTransportException"></exception>
        protected override TResult Translate<TResult>(Func<TResult> func)
        {
            try
            {
                return func();
            }
            catch (FtpTransportException e)
            {
                throw new OneFilesystemTransportException(null, e);
            }
            catch (FtpProtocolException e)
            {
                throw new OneFilesystemProtocolException(null, e);
            }
        }
    }
}
