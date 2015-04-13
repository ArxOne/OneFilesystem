#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Session
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Maintains a pool of sessions per client
    /// </summary>
    /// <typeparam name="TSession">The type of the session.</typeparam>
    public class ClientSessionProvider<TSession> : IDisposable
    {
        private readonly Func<TSession, bool> _isValid;
        private readonly Queue<TSession> _sessions = new Queue<TSession>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSessionProvider{TSession}"/> class.
        /// </summary>
        /// <param name="isValid">The is valid.</param>
        public ClientSessionProvider(Func<TSession, bool> isValid)
        {
            _isValid = isValid;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (_sessions)
            {
                while (_sessions.Count > 0)
                {
                    var session = _sessions.Dequeue() as IDisposable;
                    if (session != null)
                        session.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets or creates a valid client session.
        /// </summary>
        /// <param name="createSession">The create session.</param>
        /// <returns></returns>
        public ClientSession<TSession> Get(Func<TSession> createSession)
        {
            // first, try to dequeue an active session
            lock (_sessions)
            {
                while (_sessions.Count > 0)
                {
                    var session = _sessions.Dequeue();
                    if (_isValid(session))
                        return new ClientSession<TSession>(session, this);
                }
            }
            // finally, create a new one
            return new ClientSession<TSession>(createSession(), this);
        }

        /// <summary>
        /// Releases the specified client session.
        /// </summary>
        /// <param name="clientSession">The client session.</param>
        public void Release(ClientSession<TSession> clientSession)
        {
            if (_isValid(clientSession.Session))
            {
                lock (_sessions)
                {
                    // in case this is disposed twice
                    if (!_sessions.Contains(clientSession.Session))
                        _sessions.Enqueue(clientSession.Session);
                }
            }
        }
    }
}
