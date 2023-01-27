using System;
using System.Threading.Tasks;
using YellowDuck.Api.Constants;
using YellowDuck.Api.Extensions;
using YellowDuck.Api.Gql.Schema.Types;
using YellowDuck.Api.Services.Files;

namespace YellowDuck.Api.Helpers
{
    public static class MutationHelper
    {
        public static Task CheckPictureMaybe(Maybe<string> id, IFileManager fileManager)
        {
            return id.IfSet(v => CheckPicture(v, fileManager), Task.CompletedTask);
        }

        public static async Task CheckPicture(string id, IFileManager fileManager)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var writeTime = await fileManager.Exists(FileContainers.Images, id);
            if (writeTime == false)
                throw new Exception();
        }

        public static Task CheckFileMaybe(Maybe<string> id, IFileManager fileManager)
        {
            return id.IfSet(v => CheckFileMaybe(v, fileManager), Task.CompletedTask);
        }

        public static async Task CheckFileMaybe(string id, IFileManager fileManager)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var writeTime = await fileManager.Exists(FileContainers.Files, id);
            if (writeTime == false)
                throw new Exception();
        }
    }
}
