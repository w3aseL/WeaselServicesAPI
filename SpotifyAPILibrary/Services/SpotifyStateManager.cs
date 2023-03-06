using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyAPILibrary
{
    public class SpotifyStateManager
    {
        private ILogger<SpotifyStateManager> _logger;
        private Dictionary<int, SpotifyPlayerActiveState> activeStates;
        private ReaderWriterLockSlim _lock;
        private SpotifySessionJobQueue _queue;

        public SpotifyStateManager(ILogger<SpotifyStateManager> logger, SpotifySessionJobQueue queue)
        {
            _logger = logger;
            activeStates = new Dictionary<int, SpotifyPlayerActiveState>();
            _lock = new ReaderWriterLockSlim();
            _queue = queue;
        }

        ~SpotifyStateManager()
        {
            if(_lock != null) _lock.Dispose();
        }

        public SpotifyPlayerActiveState GetActiveState(int accountId)
        {
            _lock.EnterReadLock();

            try
            {
                return activeStates[accountId].ShallowClone();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public (bool, string) UpdateActiveState(int accountId, CurrentlyPlayingContext? ctx)
        {
            _lock.EnterWriteLock();

            try
            {
                SpotifyPlayerActiveState state;
                var hasKey = activeStates.TryGetValue(accountId, out state);

                if (!hasKey)
                {
                    state = new SpotifyPlayerActiveState(accountId, _queue);
                    activeStates.Add(accountId, state);
                }

                return state.UpdateActiveState(ctx);
            }
            finally { _lock.ExitWriteLock(); }
        }
    }
}
