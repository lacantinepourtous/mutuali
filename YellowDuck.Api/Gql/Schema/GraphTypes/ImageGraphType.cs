using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Services.Files;
using GraphQL.Conventions;

namespace YellowDuck.Api.Gql.Schema.GraphTypes
{
    public class ImageGraphType
    {
        private readonly string fileId;

        public ImageGraphType(string fileId)
        {
            this.fileId = fileId;
        }

        public Id Id => Id.New<ImageGraphType>(fileId);
        public string Url([Inject] ImageUrlProvider urlProvider, ImageFormat format)
        {
            return urlProvider.GetImageUrl(fileId, format);
        }
    }
}