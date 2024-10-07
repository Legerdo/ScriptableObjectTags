using UnityEditor;
using UnityEngine;

namespace ScriptableObjectTags
{
    [CustomEditor(typeof(TagManager))]
    public class TagManagerEditor : Editor
    {
        private SerializedProperty tags;

        private void OnEnable()
        {
            tags = serializedObject.FindProperty("tags");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Tag Management", EditorStyles.boldLabel);

            if (GUILayout.Button("Add New Tag"))
            {
                string tagManagerPath = AssetDatabase.GetAssetPath(target);
                string folderPath = System.IO.Path.GetDirectoryName(tagManagerPath);

                if (folderPath == null)
                {
                    Debug.LogError("Failed to locate the TagManager path.");
                    return;
                }

                // 새로운 태그 생성
                TagData newTag = CreateInstance<TagData>();
                newTag.SetTagName($"NewTag_{System.Guid.NewGuid()}"); // 각 태그에 고유한 이름을 부여

                string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/NewTag.asset"); // 고유 경로 생성
                AssetDatabase.CreateAsset(newTag, assetPath); // 새 태그를 고유 경로에 생성
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                ((TagManager)target).AddTag(newTag);

                // 변경 사항을 적용 및 저장
                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.PropertyField(tags, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
