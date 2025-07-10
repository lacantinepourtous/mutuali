using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.BackgroundJobs
{
    public class SendKPIsEmail
    {
        private const int MIN_USERS_FOR_SPECIFICATION_GROUP = 5;

        private readonly AppDbContext db;
        private readonly IMailer mailer;
        private readonly IConfiguration config;

        public SendKPIsEmail(AppDbContext db, IMailer mailer, IConfiguration config)
        {
            this.db = db;
            this.mailer = mailer;
            this.config = config;
        }

        public static void RegisterJob(IConfiguration config)
        {
            RecurringJob.AddOrUpdate<SendKPIsEmail>(
                x => x.Run(),
                Cron.Weekly(DayOfWeek.Monday, 2, 0),
                TimeZoneInfo.FindSystemTimeZoneById(config["systemLocalTimezone"]));
        }

        public async Task Run()
        {
            var ads = await db.Ads
                .Include(x => x.Address)
                .Include(x => x.User).ThenInclude(x => x.Profile)
                .Where(x => x.IsPublish)
                .ToListAsync();

            var users = await db.Users.Include(x => x.Profile).ToListAsync();

            // Traitement des utilisateurs par type d'organisation
            var userByOrganizationType = users.GroupBy(x => x.Profile.OrganizationType)
                .OrderBy(x => x.Key == OrganizationType.Other ? 1 : 0)
                .ThenBy(x => x.Key)
                .ToList();
            var userByOrganizationTypeOtherSpecification = ProcessOtherSpecifications(
                users.Where(x => x.Profile.OrganizationType == OrganizationType.Other),
                x => x.Profile.OrganizationTypeOtherSpecification);

            // Calculer le reste pour les utilisateurs par type d'organisation
            var userOrganizationTypeOtherCount = users.Count(u => u.Profile.OrganizationType == OrganizationType.Other);
            var userOrganizationTypeOtherSpecificationCount = userByOrganizationTypeOtherSpecification.Sum(g => g.Count());
            var userOrganizationTypeOtherRemaining = userOrganizationTypeOtherCount - userOrganizationTypeOtherSpecificationCount;

            // Traitement des utilisateurs par secteur d'activité
            var userByIndustry = users.GroupBy(x => x.Profile.Industry)
                .OrderBy(x => x.Key == Industry.Other ? 1 : 0)
                .ThenBy(x => x.Key)
                .ToList();

            // Traitement des annonces par type d'organisation
            var adByOrganizationType = ads.GroupBy(x => x.User.Profile.OrganizationType)
                .OrderBy(x => x.Key == OrganizationType.Other ? 1 : 0)
                .ThenBy(x => x.Key)
                .ToList();
            var adByOrganizationTypeOtherSpecification = ProcessOtherSpecifications(
                ads.Where(x => x.User.Profile.OrganizationType == OrganizationType.Other),
                x => x.User.Profile.OrganizationTypeOtherSpecification);

            // Calculer le reste pour les annonces par type d'organisation
            var adOrganizationTypeOtherCount = ads.Count(a => a.User.Profile.OrganizationType == OrganizationType.Other);
            var adOrganizationTypeOtherSpecificationCount = adByOrganizationTypeOtherSpecification.Sum(g => g.Count());
            var adOrganizationTypeOtherRemaining = adOrganizationTypeOtherCount - adOrganizationTypeOtherSpecificationCount;

            // Traitement des annonces par catégorie (filtrer les valeurs vides)
            var adByCategory = ads.GroupBy(x => x.Category)
                .Where(g => !string.IsNullOrWhiteSpace(g.Key.ToString()))
                .OrderBy(x => x.Key == AdCategory.Other ? 1 : 0)
                .ThenBy(x => x.Key)
                .ToList();

            // Traitement des annonces par région (filtrer les valeurs vides et nettoyer les noms)
            var adByRegion = ads.GroupBy(x => NormalizeCityName(x.Address.Locality))
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .OrderBy(x => x.Key)
                .ToList();

            // Traitement des annonces par code postal (filtrer les valeurs vides)
            var adByPostalCode = ads.GroupBy(x => x.Address.PostalCode)
                .Where(g => !string.IsNullOrWhiteSpace(g.Key))
                .OrderBy(x => x.Key)
                .ToList();

            var model = new WeeklyKPIsEmail(config["kpisEmailRecipient"])
            {
                UserCount = users.Count,
                UserByOranizationType = userByOrganizationType,
                UserByOrganizationTypeOtherSpecification = userByOrganizationTypeOtherSpecification,
                UserOrganizationTypeOtherRemaining = userOrganizationTypeOtherRemaining,
                UserByIndustry = userByIndustry,
                AdCount = ads.Count,
                AdByCategory = adByCategory,
                AdByOrganizationType = adByOrganizationType,
                AdByOrganizationTypeOtherSpecification = adByOrganizationTypeOtherSpecification,
                AdOrganizationTypeOtherRemaining = adOrganizationTypeOtherRemaining,
                AdByRegion = adByRegion,
                AdByPostalCode = adByPostalCode,
            };

            await mailer.Send(model);
        }

        private List<IGrouping<string, T>> ProcessOtherSpecifications<T>(
            IEnumerable<T> items,
            Func<T, string> specificationSelector)
        {
            // Grouper par spécification
            var groupsBySpecification = items
                .Where(x => !string.IsNullOrWhiteSpace(specificationSelector(x)))
                .GroupBy(specificationSelector)
                .ToList();

            // Identifier les groupes avec suffisamment d'éléments
            var popularGroups = groupsBySpecification
                .Where(g => g.Count() >= MIN_USERS_FOR_SPECIFICATION_GROUP)
                .ToList();

            return popularGroups;
        }

        private string NormalizeCityName(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                return cityName;

            // Convertir en minuscules et supprimer les espaces en début/fin
            var normalized = cityName.Trim().ToLowerInvariant();

            // Remplacer les caractères accentués
            normalized = normalized
                .Replace("à", "a").Replace("â", "a").Replace("ä", "a")
                .Replace("é", "e").Replace("è", "e").Replace("ê", "e").Replace("ë", "e")
                .Replace("î", "i").Replace("ï", "i")
                .Replace("ô", "o").Replace("ö", "o")
                .Replace("ù", "u").Replace("û", "u").Replace("ü", "u")
                .Replace("ç", "c")
                .Replace("ñ", "n");

            // Supprimer les espaces multiples et les remplacer par un seul espace
            normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"\s+", " ");

            // Capitaliser la première lettre de chaque mot
            normalized = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(normalized);

            return normalized;
        }
    }
}
