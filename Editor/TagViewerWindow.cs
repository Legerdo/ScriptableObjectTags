using UnityEditor;
using UnityEngine;

namespace ScriptableObjectTags
{
    public class TagViewerWindow : EditorWindow
    {
        [MenuItem("Window/Tag Viewer")]
        public static void ShowWindow()
        {
            GetWindow<TagViewerWindow>("Tag Viewer");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Tag Viewer", EditorStyles.boldLabel);

            var allTaggableObjects = FindObjectsOfType<Taggable>();

            foreach (var taggable in allTaggableObjects)
            {
                EditorGUILayout.LabelField(taggable.name);

                foreach (var tag in taggable.ObjectTags)
                {
                    EditorGUILayout.LabelField(" - " + tag.TagName);
                }
            }
        }
    }
}
