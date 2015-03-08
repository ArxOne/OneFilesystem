
namespace ArxOne.OneFilesystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Protocols;

    public class OneFilesystem : IOneFilesystem
    {
        /// <summary>
        /// All known filesystems
        /// </summary>
        public static readonly IOneProtocolFilesystem[] Filesystems = { new FileProtocolFilesystem() };

        private readonly Dictionary<string, IList<IOneProtocolFilesystem>> _protocolFilesystemsByProtocol;
        private readonly IList<IOneProtocolFilesystem> _nullFilesystems;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneFilesystem"/> class.
        /// </summary>
        /// <param name="protocolFilesystems">The protocol filesystems.</param>
        public OneFilesystem(IOneProtocolFilesystem[] protocolFilesystems = null)
        {
            var validProtocolFilesystems = protocolFilesystems ?? Filesystems;
            _protocolFilesystemsByProtocol = validProtocolFilesystems.Where(p => p.Protocol != null)
                .GroupBy(p => p.Protocol)
                .ToDictionary(p => p.Key, p => (IList<IOneProtocolFilesystem>)p.ToList());
            _nullFilesystems = validProtocolFilesystems.Where(p => p.Protocol == null).ToList();
        }

        /// <summary>
        /// Gets the filesystem.
        /// </summary>
        /// <param name="onePath">The one path.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        private IOneFilesystem GetFilesystem(OnePath onePath)
        {
            IOneFilesystem filesystem;
            IList<IOneProtocolFilesystem> filesystems;
            if (_protocolFilesystemsByProtocol.TryGetValue(onePath.Protocol, out filesystems))
            {
                filesystem = filesystems.FirstOrDefault(f => f.Handle(onePath));
                if (filesystem != null)
                    return filesystem;
            }

            filesystem = _nullFilesystems.FirstOrDefault(f => f.Handle(onePath));
            if (filesystem != null)
                return filesystem;

            throw new NotSupportedException(string.Format("Found no filesystem to handle URI '{0}'", onePath));
        }

        /// <summary>
        /// Enumerates the entries.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>
        /// A list, or null if the directory is not found (if the directoryPath points to a file, an empty list is returned)
        /// </returns>
        public IEnumerable<OneEntryInformation> EnumerateEntries(OnePath directoryPath)
        {
            return GetFilesystem(directoryPath).EnumerateEntries(directoryPath);
        }

        /// <summary>
        /// Gets the information.
        /// </summary>
        /// <param name="entryPath"></param>
        /// <returns></returns>
        public OneEntryInformation GetInformation(OnePath entryPath)
        {
            // A quick check to avoid annoying everyone
            var entryInformation = entryPath as OneEntryInformation;
            if (entryInformation != null)
                return entryInformation;
            return GetFilesystem(entryPath).GetInformation(entryPath);
        }

        public Stream OpenRead(OnePath filePath)
        {
            return GetFilesystem(filePath).OpenRead(filePath);
        }

        public bool Delete(OnePath entryPath)
        {
            return GetFilesystem(entryPath).Delete(entryPath);
        }

        public Stream CreateFile(OnePath filePath)
        {
            return GetFilesystem(filePath).CreateFile(filePath);
        }

        public bool CreateDirectory(OnePath directoryPath)
        {
            return GetFilesystem(directoryPath).CreateDirectory(directoryPath);
        }
    }
}
