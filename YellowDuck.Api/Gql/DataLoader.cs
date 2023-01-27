using YellowDuck.Api.Requests.Queries;
using GraphQL.DataLoader;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YellowDuck.Api.Gql
{
    public class DataLoader : DataLoaderContext
    {
        private readonly IMediator mediator;

        public DataLoader(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task<TResult> LoadOne<TQuery, TResult, TKey>(TKey id) where TQuery : IRequest<IDictionary<TKey, TResult>>, IIdListQuery<TKey>, new()
        {
            return LoadOne(typeof(TQuery).FullName, DoLoad, id);

            Task<IDictionary<TKey, TResult>> DoLoad(IEnumerable<TKey> ids, CancellationToken cancellationToken)
            {
                var query = new TQuery { Ids = ids };
                return mediator.Send(query, cancellationToken);
            }
        }

        public Task<TResult> LoadOne<TQuery, TResult, TGroup, TKey>(TGroup group, TKey id, Func<TGroup, string> stringifyGroup = null)
            where TQuery : IRequest<IDictionary<TKey, TResult>>, IIdListQuery<TKey>, IHaveGroup<TGroup>, new()
        {
            if (stringifyGroup == null) stringifyGroup = x => x.ToString();

            return LoadOne($"{typeof(TQuery).FullName}:{stringifyGroup(group)}", DoLoad, id);

            Task<IDictionary<TKey, TResult>> DoLoad(IEnumerable<TKey> ids, CancellationToken cancellationToken)
            {
                var query = new TQuery { Group = group, Ids = ids };
                return mediator.Send(query, cancellationToken);
            }
        }

        public Task<IEnumerable<TResult>> LoadCollection<TQuery, TResult, TKey>(TKey id) where TQuery : IRequest<ILookup<TKey, TResult>>, IIdListQuery<TKey>, new()
        {
            return LoadCollection(typeof(TQuery).FullName, DoLoad, id);

            Task<ILookup<TKey, TResult>> DoLoad(IEnumerable<TKey> ids, CancellationToken cancellationToken)
            {
                var query = new TQuery { Ids = ids };
                return mediator.Send(query, cancellationToken);
            }
        }

        public Task<IEnumerable<TResult>> LoadCollection<TQuery, TResult, TGroup, TKey>(TGroup group, TKey id, Func<TGroup, string> stringifyGroup = null)
            where TQuery : IRequest<ILookup<TKey, TResult>>, IIdListQuery<TKey>, IHaveGroup<TGroup>, new()
        {
            if (stringifyGroup == null) stringifyGroup = x => x.ToString();

            return LoadCollection($"{typeof(TQuery).FullName}:{stringifyGroup(group)}", DoLoad, id);

            Task<ILookup<TKey, TResult>> DoLoad(IEnumerable<TKey> ids, CancellationToken cancellationToken)
            {
                var query = new TQuery { Group = group, Ids = ids };
                return mediator.Send(query, cancellationToken);
            }
        }


        private Task<TResult> LoadOne<TKey, TResult>(string loaderKey, Func<IEnumerable<TKey>, CancellationToken, Task<IDictionary<TKey, TResult>>> loader, TKey key)
        {
            ForceDispatch();
            return this.GetOrAddBatchLoader(loaderKey, loader).LoadAsync(key);
        }

        private Task<IEnumerable<TResult>> LoadCollection<TKey, TResult>(string loaderKey, Func<IEnumerable<TKey>, CancellationToken, Task<ILookup<TKey, TResult>>> loader, TKey key)
        {
            ForceDispatch();
            return this.GetOrAddCollectionBatchLoader(loaderKey, loader).LoadAsync(key);
        }


        private bool isDispatchScheduled;

        /// <summary>
        /// HACK: Ceci est un workaround pas propre.
        /// Dans certains cas le DataLoader de GraphQL dotnet ne trigger pas le dispatch correctement.
        /// Ce code s'assure qu'on fait un dispatch après un petit délais.
        /// </summary>
        private void ForceDispatch()
        {
            const int dispatchDelayMs = 50;

            if (isDispatchScheduled) return;

            isDispatchScheduled = true;

            Task.Delay(dispatchDelayMs)
                .ContinueWith(async x =>
                {
                    isDispatchScheduled = false;
                    await DispatchAllAsync(CancellationToken.None);
                });
        }
    }
}