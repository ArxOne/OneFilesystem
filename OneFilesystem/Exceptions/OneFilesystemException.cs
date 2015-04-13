#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Exceptions
{
    using System;
    using System.IO;

    /// <summary>
    /// Base file exception
    /// </summary>
    public class OneFilesystemException : IOException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneFilesystemException"/> class.
        /// </summary>
        /// <param name="message">Message d'erreur qui explique la raison de l'exception.</param>
        /// <param name="innerException">Exception ayant provoqué l'exception en cours.Si le paramètre <paramref name="innerException" /> n'est pas null, l'exception actuelle est levée dans un bloc catch qui gère l'exception interne.</param>
        public OneFilesystemException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
