using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.BackgroundJobs
{
    public class SendKPIsEmail
    {
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
                TimeZoneInfo.FindSystemTimeZoneById(config["systemLocalTimezone"]));;
        }

        public async Task Run()
        {
            var ads = await db.Ads
                .Include(x => x.Address)
                .Include(x => x.User).ThenInclude(x => x.Profile)
                .Where(x => x.IsPublish)
                .ToListAsync();

            var users = await db.Users.Include(x => x.Profile).ToListAsync();

            var model = new WeeklyKPIsEmail(config["kpisEmailRecipient"])
            {
                UserCount = users.Count,
                UserByOranizationType = users.GroupBy(x => x.Profile.OrganizationType).ToList(),
                UserByIndustry = users.GroupBy(x => x.Profile.Industry).ToList(),
                AdCount = ads.Count,
                AdByCategory = ads.GroupBy(x => x.Category).ToList(),
                AdByOrganizationType = ads.GroupBy(x => x.User.Profile.OrganizationType).ToList(),
                AdByRegion = ads.GroupBy(x => x.Address.Locality).ToList(),
                AdByPostalCode = ads.GroupBy(x => x.Address.PostalCode).ToList(),
                UserByPostalCode = users.GroupBy(x => x.Profile.PostalCode).ToList()
            };

            await mailer.Send(model);
        }
    }
}
