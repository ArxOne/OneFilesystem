#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Session
{
    using System;

    /// <summary>
    /// Holds a client session
    /// </summary>
    /// <typeparam name="TSession">The type of the session.</typeparam>
    public class ClientSession<TSession> : IDisposable
    {
        private readonly ClientSessionProvider<TSession> _sessionProvider;
        public TSession Session { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSession{TSession}"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="sessionProvider">The session provider.</param>
        public ClientSession(TSession session, ClientSessionProvider<TSession> sessionProvider)
        {
            _sessionProvider = sessionProvider;
            Session = session;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _sessionProvider.Release(this);
        }
    }
}