using YellowDuck.Api.DbModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System;

namespace YellowDuck.Api.Requests.Queries.Rating
{
    public class GetUserAverageRating : IRequestHandler<GetUserAverageRating.Query, double>
    {
        private readonly AppDbContext db;

        public GetUserAverageRating(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<double> Handle(Query request, CancellationToken cancellationToken)
        {
            var userRatings = await db.UserRatings
                .Where(x => x.UserId == request.UserId)
                .Select(x => new
                {
                    CommunicationRating = (int)x.CommunicationRating,
                    RespectRating = (int)x.RespectRating,
                    OverallRating = (int)x.OverallRating
                })
                .ToListAsync(cancellationToken);

            // Entity Framework can't translate the SelectMany with array creation directly to SQL.
            var allRatings = userRatings
                .SelectMany(x => new[] { x.CommunicationRating, x.RespectRating, x.OverallRating })
                .Where(r => r > 0)
                .ToList();

            if (!allRatings.Any())
            {
                return 0;
            }

            var avg = allRatings.Average();
            return Math.Round(avg, 1);
        }

        public class Query : IRequest<double>
        {
            public string UserId { get; set; }
        }
    }
}