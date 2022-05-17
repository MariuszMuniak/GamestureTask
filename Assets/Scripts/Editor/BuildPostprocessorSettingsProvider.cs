using UnityEditor;
using UnityEngine;

namespace GamestureTask.Editor
{
    public class BuildPostprocessorSettingsProvider : SettingsProvider
    {
        public BuildPostprocessorSettingsProvider(string path, SettingsScope scopes = SettingsScope.User) : base(path, scopes)
        {
        }

        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);
            GUILayout.Space(20f);
            var canCopyImages = EditorPrefs.GetBool(BuildPostprocessor.COPY_IMAGES_EDITOR_PREF);
            var changedCanCopyImages = EditorGUILayout.Toggle("Copy images on build", canCopyImages);
            if (canCopyImages != changedCanCopyImages)
            {
                EditorPrefs.SetBool(BuildPostprocessor.COPY_IMAGES_EDITOR_PREF, changedCanCopyImages);
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateBuildPostprocessorSettingsProvider()
        {
            var buildPostprocessorSettingsProvider = new BuildPostprocessorSettingsProvider(BuildPostprocessor.MENU_PATH);
            return buildPostprocessorSettingsProvider;
        }
    }
}