using System.Linq;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Services.System;

namespace YellowDuck.Api.Extensions
{
    /// <summary>
    /// Extensions pour les requêtes d'annonces
    /// </summary>
    public static class AdQueryExtensions
    {
        /// <summary>
        /// Applique le filtrage d'accès selon le type d'utilisateur
        /// </summary>
        /// <param name="query">La requête d'annonces à filtrer</param>
        /// <param name="currentUserAccessor">L'accesseur pour obtenir les informations de l'utilisateur actuel</param>
        /// <returns>La requête filtrée selon les permissions de l'utilisateur</returns>
        public static IQueryable<Ad> ApplyUserAccessFilter(this IQueryable<Ad> query, ICurrentUserAccessor currentUserAccessor)
        {
            if (currentUserAccessor.IsUserType(UserType.Admin))
            {
                // Admin voit TOUT (publiées, non publiées, admin-only)
                return query;
            }

            var currentUserId = currentUserAccessor.GetCurrentUserId();

            if (currentUserId != null)
            {
                // Utilisateur connecté : publiées + ses propres non publiées, mais pas admin-only
                return query.Where(x => (x.IsPublish || x.UserId == currentUserId) && !x.IsAdminOnly);
            }

            // Utilisateur non connecté : seulement publiées et pas admin-only
            return query.Where(x => x.IsPublish && !x.IsAdminOnly);
        }
    }
}

