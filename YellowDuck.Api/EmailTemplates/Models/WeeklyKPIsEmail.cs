using System.Collections.Generic;
using System.Linq;
using YellowDuck.Api.DbModel.Entities;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.DbModel.Enums;
using YellowDuck.Api.Services.Mailer;

namespace YellowDuck.Api.EmailTemplates.Models
{
    public class WeeklyKPIsEmail : EmailModel
    {
        public override string Subject => $"KPIs pour MutuAli";

        public int UserCount { get; set; }
        public List<IGrouping<OrganizationType, AppUser>> UserByOranizationType { get; set; }
        public List<IGrouping<Industry, AppUser>> UserByIndustry { get; set; }


        public int AdCount { get; set; }
        public List<IGrouping<OrganizationType, Ad>> AdByOrganizationType { get; set; }
        public List<IGrouping<AdCategory, Ad>> AdByCategory { get; set; }
        public List<IGrouping<string, Ad>> AdByRegion { get; set; }
        public List<IGrouping<string, Ad>> AdByPostalCode { get; set; }

        public WeeklyKPIsEmail(string to) : base(to) { }
    }
}
