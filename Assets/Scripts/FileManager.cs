using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GamestureTask
{
    public static class FileManager
    {
        public static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (var newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        public static List<FileInfo> GetImageFiles(string directoryPath, SearchOption searchOption)
        {
            var imageFiles = new List<FileInfo>();
            if (!Directory.Exists(directoryPath)) return imageFiles;
            var imagesDirectory = new DirectoryInfo(directoryPath);
            var files = imagesDirectory.GetFiles("*", searchOption);
            imageFiles.AddRange(files.Where(fileInfo => fileInfo.IsImage()));
            return imageFiles;
        }
    }
}