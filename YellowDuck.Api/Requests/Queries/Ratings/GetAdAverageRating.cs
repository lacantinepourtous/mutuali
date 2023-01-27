using YellowDuck.Api.DbModel;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using System;

namespace YellowDuck.Api.Requests.Queries.Rating
{
    public class GetAdAverageRating : IRequestHandler<GetAdAverageRating.Query, double>
    {
        private readonly AppDbContext db;

        public GetAdAverageRating(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<double> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                var ratings = db.AdRatings.Where(x => x.AdId == request.AdId);
                if (ratings.Count() > 0)
                {
                    var average = await ratings.AverageAsync(x => ((double)x.CleanlinessRating + (double)x.ComplianceRating + (double)x.SecurityRating) / 3, cancellationToken: cancellationToken);
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
            public long AdId { get; set; }
        }
    }
}