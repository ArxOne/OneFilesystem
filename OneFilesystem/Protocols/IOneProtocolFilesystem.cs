
namespace ArxOne.OneFilesystem.Protocols
{
    public interface IOneProtocolFilesystem : IOneFilesystem
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
