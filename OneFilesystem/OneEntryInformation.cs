
namespace ArxOne.OneFilesystem
{
    using System;

    /// <summary>
    /// Information about file or directory
    /// </summary>
    public class OneEntryInformation : OnePath
    {
        /// <summary>
        /// Gets a value indicating whether this instance is directory.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is directory; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirectory { get; private set; }

        /// <summary>
        /// Gets the length.
        /// Null for directories
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public long? Length { get; private set; }

        /// <summary>
        /// Gets the creation time UTC.
        /// </summary>
        /// <value>
        /// The creation time UTC.
        /// </value>
        public DateTime? CreationTimeUtc { get; private set; }
        /// <summary>
        /// Gets the last write time UTC.
        /// </summary>
        /// <value>
        /// The last write time UTC.
        /// </value>
        public DateTime? LastWriteTimeUtc { get; private set; }
        /// <summary>
        /// Gets the last access time UTC.
        /// </summary>
        /// <value>
        /// The last access time UTC.
        /// </value>
        public DateTime? LastAccessTimeUtc { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneEntryInformation" /> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="isDirectory">if set to <c>true</c> [is directory].</param>
        /// <param name="length">The length.</param>
        /// <param name="creationTimeUtc">The creation time UTC.</param>
        /// <param name="lastWriteTimeUtc">The last write time UTC.</param>
        /// <param name="lastAccessTimeUtc">The last access time UTC.</param>
        public OneEntryInformation(OnePath path, bool isDirectory, long? length = null,
            DateTime? creationTimeUtc = null, DateTime? lastWriteTimeUtc = null, DateTime? lastAccessTimeUtc = null)
            : base(path.Protocol, path.Host, path.Port, path.Path)
        {
            IsDirectory = isDirectory;
            Length = length;
            CreationTimeUtc = creationTimeUtc;
            LastWriteTimeUtc = lastWriteTimeUtc;
            LastAccessTimeUtc = lastAccessTimeUtc;
        }
    }
}

