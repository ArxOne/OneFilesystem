#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion

namespace ArxOne.OneFilesystem.IO
{
    using System.IO;

    /// <summary>
    /// Extensions to <see cref="Stream"/>
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads all data from stream (as much as possible).
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static int ReadAll(this Stream stream, byte[] buffer, int offset, int count)
        {
            var bytesRead = 0;
            while (count > 0)
            {
                var r = stream.Read(buffer, offset, count);
                if (r == 0)
                    break;

                bytesRead += r;
                offset += r;
                count -= r;
            }
            return bytesRead;
        }
    }
}
