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
			var adRatings = await db.AdRatings
				.Where(x => x.AdId == request.AdId)
				.Select(x => new
				{
					ComplianceRating = (int)x.ComplianceRating,
					QualityRating = (int)x.QualityRating,
					OverallRating = (int)x.OverallRating
				})
				.ToListAsync(cancellationToken);

            // Entity Framework can't translate the SelectMany with array creation directly to SQL.
			var allRatings = adRatings
				.SelectMany(x => new[] { x.ComplianceRating, x.QualityRating, x.OverallRating })
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
            public long AdId { get; set; }
        }
    }
}