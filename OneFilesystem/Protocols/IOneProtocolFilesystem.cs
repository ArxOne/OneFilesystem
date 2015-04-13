#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Protocols
{
    using System;

    /// <summary>
    /// Interface for handled protocol and related filesystem.
    /// Implement this to create your own custom filesystem and add it as paramter to <see cref="OneFilesystem"/> constructor.
    /// </summary>
    public interface IOneProtocolFilesystem : IDisposable, IOneFilesystem
    {
        /// <summary>
        /// Gets the protocol.
        /// If this is null, then the Handle() method has to be called to see if the file is handled here
        /// </summary>
        /// <value>
        /// The protocol.
        /// </value>
        string Protocol { get; }

        /// <summary>
        /// Indicates if this filesystem handles the given protocol.
        /// </summary>
        /// <param name="entryPath">The entry path.</param>
        /// <returns></returns>
        bool Handle(OnePath entryPath);
    }
}
