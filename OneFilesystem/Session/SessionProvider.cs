#region OneFilesystem
// OneFilesystem
// (to rule them all... Or at least some...)
// https://github.com/ArxOne/OneFilesystem
// Released under MIT license http://opensource.org/licenses/MIT
#endregion
namespace ArxOne.OneFilesystem.Session
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Maintains a pool of sessions, over multiple hosts
    /// </summary>
    public class SessionProvider<TSession> : IDisposable
    {
        private readonly IDictionary<Tuple<string, int, string>, ClientSessionProvider<TSession>> _clientSessionProviders
            = new ConcurrentDictionary<Tuple<string, int, string>, ClientSessionProvider<TSession>>();

        private readonly Func<TSession, bool> _isValid;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionProvider{TSession}"/> class.
        /// </summary>
        /// <param name="isValid">The is valid.</param>
        public SessionProvider(Func<TSession, bool> isValid)
        {
            _isValid = isValid;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            lock (_clientSessionProviders)
            {
                foreach (var clientSessionProvider in _clientSessionProviders.Values)
                    clientSessionProvider.Dispose();
                _clientSessionProviders.Clear();
            }
        }

        /// <summary>
        /// Gets or creates a valid client session, related to host and port.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="createSession">The create session.</param>
        /// <returns></returns>
        public ClientSession<TSession> Get(string host, int port, string userName, Func<TSession> createSession)
        {
            var key = Tuple.Create(host, port, userName);
            ClientSessionProvider<TSession> clientSessionProvider;
            lock (_clientSessionProviders)
            {
                if (!_clientSessionProviders.TryGetValue(key, out clientSessionProvider))
                    _clientSessionProviders[key] = clientSessionProvider = new ClientSessionProvider<TSession>(_isValid);
            }
            return clientSessionProvider.Get(createSession);
        }
    }
}