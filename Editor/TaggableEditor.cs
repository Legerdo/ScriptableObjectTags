using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace ScriptableObjectTags
{
    [CustomEditor(typeof(Taggable))]
    public class TaggableEditor : Editor
    {
        private int selectedIndex = 0; // 선택된 인덱스를 저장하는 변수
        private string searchQuery = ""; // 검색어 저장

        private SerializedProperty objectTagsProperty; // SerializedProperty for objectTags

        private void OnEnable()
        {
            // objectTagsProperty를 SerializedObject에서 가져옵니다.
            objectTagsProperty = serializedObject.FindProperty("objectTags");
        }

        public override void OnInspectorGUI()
        {
            // SerializedObject 업데이트
            serializedObject.Update();

            Taggable taggable = (Taggable)target;

            // TagManager를 인스펙터에서 설정할 수 있도록 추가
            taggable.tagManager = (TagManager)EditorGUILayout.ObjectField("Tag Manager", taggable.tagManager, typeof(TagManager), false);

            if (taggable.tagManager == null)
            {
                EditorGUILayout.HelpBox("Please assign a TagManager.", MessageType.Error);
                return;
            }

            EditorGUILayout.LabelField("Taggable Object", EditorStyles.boldLabel);

            // 현재 태그 목록 표시
            if (objectTagsProperty.arraySize > 0)
            {
                EditorGUILayout.LabelField("Current Tags:");
                for (int i = objectTagsProperty.arraySize - 1; i >= 0; i--)
                {
                    SerializedProperty tagProperty = objectTagsProperty.GetArrayElementAtIndex(i);
                    TagData tag = tagProperty.objectReferenceValue as TagData;

                    if (tag != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("- " + tag.TagName);

                        if (GUILayout.Button("Remove", GUILayout.Width(60)))
                        {
                            // Undo 등록하여 변경 사항 기록
                            Undo.RecordObject(taggable, "Remove Tag");
                            objectTagsProperty.DeleteArrayElementAtIndex(i);
                            serializedObject.ApplyModifiedProperties(); // 변경 사항을 즉시 적용
                            Repaint(); // 인스펙터를 강제로 다시 그리기
                            break;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("No tags assigned.");
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Add Tag:");

            // 검색어 입력 필드 추가
            searchQuery = EditorGUILayout.TextField("Search Tag", searchQuery);

            // 검색어에 따라 필터링된 태그 목록 생성
            var availableTags = taggable.tagManager.Tags
                .Where(t => !taggable.ObjectTags.Contains(t) && t.TagName.ToLower().Contains(searchQuery.ToLower()))
                .ToList();

            if (availableTags.Count > 0)
            {
                // 태그 선택 UI 생성
                string[] tagOptions = availableTags.Select(tag => tag.TagName).ToArray();

                // 현재 선택된 인덱스를 유지하도록 selectedIndex를 사용
                selectedIndex = EditorGUILayout.Popup("Select Tag", selectedIndex, tagOptions);

                // 선택한 태그를 추가하는 버튼
                if (GUILayout.Button("Add Selected Tag") && selectedIndex >= 0 && selectedIndex < availableTags.Count)
                {
                    // Undo 등록하여 변경 사항 기록
                    Undo.RecordObject(taggable, "Add Tag");

                    // objectTags에 새 태그 추가
                    objectTagsProperty.InsertArrayElementAtIndex(objectTagsProperty.arraySize);
                    objectTagsProperty.GetArrayElementAtIndex(objectTagsProperty.arraySize - 1).objectReferenceValue = availableTags[selectedIndex];

                    serializedObject.ApplyModifiedProperties(); // 변경 사항을 즉시 적용
                    Repaint(); // 인스펙터를 강제로 다시 그리기

                    selectedIndex = 0; // 태그 추가 후 초기화
                    searchQuery = ""; // 검색어 초기화
                }

                // 모든 태그를 추가하는 버튼
                if (GUILayout.Button("Add All Tags"))
                {
                    Undo.RecordObject(taggable, "Add All Tags");
                    foreach (var tag in availableTags)
                    {
                        objectTagsProperty.InsertArrayElementAtIndex(objectTagsProperty.arraySize);
                        objectTagsProperty.GetArrayElementAtIndex(objectTagsProperty.arraySize - 1).objectReferenceValue = tag;
                    }
                    serializedObject.ApplyModifiedProperties(); // 변경 사항을 즉시 적용
                    Repaint(); // 인스펙터를 강제로 다시 그리기
                    searchQuery = ""; // 검색어 초기화
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No available tags to add.", MessageType.Info);
            }

            // 모든 태그 제거 버튼
            if (GUILayout.Button("Clear All Tags"))
            {
                // Undo 등록하여 변경 사항 기록
                Undo.RecordObject(taggable, "Clear All Tags");

                objectTagsProperty.ClearArray();
                serializedObject.ApplyModifiedProperties(); // 변경 사항을 즉시 적용
                Repaint(); // 인스펙터를 강제로 다시 그리기
            }

            // SerializedObject의 변경 사항을 인스펙터에 반영
            serializedObject.ApplyModifiedProperties();
        }
    }
}
