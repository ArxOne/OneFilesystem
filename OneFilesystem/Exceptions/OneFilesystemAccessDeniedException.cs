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
    /// Exception for access denied (may occur everywhere)
    /// </summary>
    public  class OneFilesystemAccessDeniedException : OneFilesystemProtocolException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneFilesystemAccessDeniedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public OneFilesystemAccessDeniedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}