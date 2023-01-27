using System.ComponentModel.DataAnnotations.Schema;

namespace YellowDuck.Api.DbModel.Entities.Ads
{
    public class AdGalleryItem : IHaveLongIdentifier
    {
        public long Id { get; set; }
        public long AdId { get; set; }

        public string PictureFileId { get; set; }
        public string Alt { get; set; }
    }
}
