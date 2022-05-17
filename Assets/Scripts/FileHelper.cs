using System.IO;

namespace GamestureTask
{
    public static class FileHelper
    {
        public static bool IsImage(this FileSystemInfo fileInfo)
        {
            var extension = fileInfo.Extension;
            return extension is ".jpg" or ".jpeg" or ".png";
        }
    }
}