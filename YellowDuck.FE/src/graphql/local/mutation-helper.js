// Helper pour encapsuler la logique d'une mutation :
// On lit l'entrée en cache, on la met à jour, on la ré-écrit dans la cache
// Et on laisse la réactivité faire sa magie

export default {
  // cache  : la cache d'Apollo
  // params : la query et ses variables : { query:Object, variables:Object }
  // update : delegate de mise à jour de l'entrée en cache : (x) => { ... }
  updateCache(cache, params, update) {
    const data = cache.readQuery(params);

    update(data);

    cache.writeQuery({ ...params, data });

    return data;
  }
};
