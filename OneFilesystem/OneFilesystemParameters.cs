#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion

namespace ArxOne.OneFilesystem
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Parameters to OneFilesystem
    /// </summary>
    public class OneFilesystemParameters
    {
        /// <summary>
        /// Gets or sets the connect proxy.
        /// </summary>
        /// <value>
        /// The connect proxy.
        /// </value>
        public Func<EndPoint, Socket> ConnectProxy { get; set; }

        /// <summary>
        /// Default parameters (for lazy people... Like me)
        /// </summary>
        public readonly static OneFilesystemParameters Default = new OneFilesystemParameters();
    }
}
