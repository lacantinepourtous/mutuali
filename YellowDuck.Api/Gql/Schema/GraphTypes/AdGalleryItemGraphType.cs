using GraphQL.Conventions;
using System.Threading.Tasks;
using YellowDuck.Api.DbModel.Entities.Ads;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Interfaces;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Services.Files;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class AdGalleryItemGraphType : LazyGraphType<AdGalleryItem>
    {

        public AdGalleryItemGraphType(AdGalleryItem galleryItem) : base(galleryItem)
        {
            Id = galleryItem.GetIdentifier();
        }

        public Id Id { get; }

        public async Task<string> Src(
            [Inject] ImageUrlProvider urlProvider,
            ImageFormat format = null)
        {
            var pictureFileId = await WithData(x => x.PictureFileId);
            return urlProvider.GetImageUrl(pictureFileId, format);
        }
        public Task<string> Alt => WithData(x => x.Alt);
    }
}
