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
            try
            {
                var ratings = db.UserRatings.Where(x => x.UserId == request.UserId);
                if (ratings.Count() > 0)
                {
                    var average = await ratings.AverageAsync(x => ((double)x.CommunicationRating + (double)x.FiabilityRating + (double)x.RespectRating) / 3, cancellationToken: cancellationToken);
                    return Math.Round(average, 1);
                }
            }
            catch (Exception)
            {
                return 0;
            }

            return 0;
        }

        public class Query : IRequest<double>
        {
            public string UserId { get; set; }
        }
    }
}