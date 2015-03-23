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
    /// Protocol exception (bad arguments, for example)
    /// </summary>
    public class OneFilesystemProtocolException : OneFilesystemException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneFilesystemProtocolException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public OneFilesystemProtocolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}