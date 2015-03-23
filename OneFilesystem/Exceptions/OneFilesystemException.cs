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
        public OneFilesystemException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
