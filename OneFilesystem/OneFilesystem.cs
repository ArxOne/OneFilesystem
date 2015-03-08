
namespace ArxOne.OneFilesystem
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Protocols;

    public class OneFilesystem : IOneFilesystem
    {
        public static readonly IOneProtocolFilesystem[] Filesystems = new IOneProtocolFilesystem[] { new FileProtocolFilesystem() };

        private readonly Dictionary<string, IList<IOneProtocolFilesystem>> _protocolFilesystemsByProtocol;
        private readonly IList<IOneProtocolFilesystem> _nullFilesystems;

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

        public IEnumerable<OneEntryInformation> EnumerateEntries(OnePath directoryPath)
        {
            return GetFilesystem(directoryPath).EnumerateEntries(directoryPath);
        }

        public OneEntryInformation GetInformation(OnePath entryPath)
        {
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

        public void CreateDirectory(OnePath directoryPath)
        {
            GetFilesystem(directoryPath).CreateFile(directoryPath);
        }
    }
}
