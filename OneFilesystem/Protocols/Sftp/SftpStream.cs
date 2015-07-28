#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion

namespace ArxOne.OneFilesystem.Protocols.Sftp
{
    using System;
    using System.IO;
    using IO;

    /// <summary>
    /// FTP stream wrapper
    /// </summary>
    internal class SftpStream : TranslateStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SftpStream"/> class.
        /// </summary>
        /// <param name="innerStream">The inner stream.</param>
        public SftpStream(Stream innerStream)
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
        protected override TResult Translate<TResult>(Func<TResult> func) => SftpProtocolFilesystem.Process(func);
    }
}
