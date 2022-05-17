using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

namespace GamestureTask.Editor
{
    public static class BuildPostprocessor
    {
        public const string COPY_IMAGES_EDITOR_PREF = "";
        public const string MENU_PATH = "My Menu/Build Postprocessor";

        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            var rootDirectoryPath = Path.GetDirectoryName(pathToBuiltProject);
            var imagesDirectoryPath = Path.Combine(rootDirectoryPath, ImageLoader.IMAGES_DIRECTORY_NAME);
            if (!Directory.Exists(imagesDirectoryPath))
            {
                Directory.CreateDirectory(imagesDirectoryPath);
            }

            if (EditorPrefs.GetBool(COPY_IMAGES_EDITOR_PREF))
            {
                FileManager.CopyFilesRecursively(ImageLoader.IMAGES_DIRECTORY_NAME, imagesDirectoryPath);
            }
        }
    }
}