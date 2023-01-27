using GraphQL.Conventions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel;
using YellowDuck.Api.EmailTemplates.Models;
using YellowDuck.Api.Services.Mailer;
using static YellowDuck.Api.EmailTemplates.Models.DailyConversationNotificationSummaryEmail;

namespace YellowDuck.Api.BackgroundJobs
{
    public class SendDailyConversationNotificationEmail
    {
        private readonly AppDbContext db;
        private readonly IMailer mailer;

        public SendDailyConversationNotificationEmail(AppDbContext db, IMailer mailer)
        {
            this.db = db;
            this.mailer = mailer;
        }

        public static void RegisterJob(IConfiguration config)
        {
            RecurringJob.AddOrUpdate<SendDailyConversationNotificationEmail>(
                x => x.Run(),
                Cron.Daily(3, 30),
                TimeZoneInfo.FindSystemTimeZoneById(config["systemLocalTimezone"]));
        }

        public async Task Run()
        {
            var notifications = db.ConversationNotifications.ToArray();
            var byUserId = notifications.GroupBy(x => x.UserId);

            foreach (var userId in byUserId)
            {
                var user = db.Users.Where(x => x.Id == userId.Key).Include(x => x.Profile).First();

                var byConversationId = userId.GroupBy(x => x.ConversationId);

                var model = new DailyConversationNotificationSummaryEmail(user.Email)
                {
                    FirstName = user.Profile.FirstName,
                    LastName = user.Profile.LastName,
                    Conversations = byConversationId.Select(x => {
                        var conversation = db.Conversations.Where(y => y.Id == x.Key)
                            .Include(y => y.Ad).ThenInclude(y => y.Translations)
                            .First();
                        var bodies = new List<string>();

                        var messageAuthor = db.Users.Where(y => y.Id == x.First().NotificationCreator).Include(x => x.Profile).First();

                        foreach (var notification in x)
                        {
                            bodies.Add(notification.Body);
                        }

                        return new Conversation()
                        {
                            Id = Id.New<Conversation>(conversation.Id).ToString(),
                            Bodies = bodies,
                            UserFrom = messageAuthor.Profile.PublicName,
                            Ad = new Ad()
                            {
                                Id = Id.New<Ad>(conversation.Ad.Id).ToString(),
                                Title = conversation.Ad.Translations.First().Title
                            }
                        };
                    }).ToList()
                };

                await mailer.Send(model);
            }

            db.ConversationNotifications.RemoveRange(notifications);
            await db.SaveChangesAsync();
        }
    }
}
