#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Exceptions
{
    using System;

    /// <summary>
    /// Transport exception (network)
    /// </summary>
    public class OneFilesystemTransportException : OneFilesystemException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneFilesystemTransportException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public OneFilesystemTransportException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}