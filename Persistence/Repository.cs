using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Persistence
{
    public interface IRepository<TKey, TVal>
    {
        Task<TVal> Get(TKey key);
        Task<TVal> Save(TKey key, TVal val);
        Task<IEnumerable<TVal>> GetAll();
    }

    public class Repository<TKey, TVal> : IRepository<TKey, TVal> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        private readonly IReliableStateManager _stateManager;
        private readonly string _name;

        public Repository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
            _name = typeof(TVal).Name;
        }

        public  Task<string> Test => Task.FromResult("Tested");

        public async Task<TVal> Get(TKey key)
        {
            var dictionary = await _stateManager.GetOrAddAsync<IReliableDictionary<TKey, TVal>>(_name);

            var cancellationToken = CancellationToken.None; // todo optional parameter

            cancellationToken.ThrowIfCancellationRequested();

            using (var tx = _stateManager.CreateTransaction())
            {
                var result = await dictionary.TryGetValueAsync(tx, key);
                return result.HasValue ? result.Value : default(TVal);
            }
        }

        public async Task<TVal> Save(TKey key, TVal val)
        {
            var dictionary = await _stateManager.GetOrAddAsync<IReliableDictionary<TKey, TVal>>(_name);

            var cancellationToken = CancellationToken.None; // todo optional parameter

            cancellationToken.ThrowIfCancellationRequested();

            using (var tx = _stateManager.CreateTransaction())
            {
                var res = await dictionary.AddOrUpdateAsync(tx, key, (k) => val, (k, v) => val);
                await tx.CommitAsync();
                return res;
            }
        }

        public async Task<IEnumerable<TVal>> GetAll()
        {
            var dictionary = await _stateManager.GetOrAddAsync<IReliableDictionary<TKey, TVal>>(_name);

            var cancellationToken = CancellationToken.None; // todo optional parameter

            cancellationToken.ThrowIfCancellationRequested();

            using (var tx = _stateManager.CreateTransaction())
            {
                var en = await dictionary.CreateEnumerableAsync(tx);
                var e = en.GetAsyncEnumerator();

                var vals = new List<TVal>();
                while (await e.MoveNextAsync(cancellationToken))
                {
                    vals.Add(e.Current.Value);
                }

                return vals;
            }
        }
    }
}
