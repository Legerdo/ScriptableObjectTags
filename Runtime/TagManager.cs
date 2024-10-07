using UnityEngine;
using System;
using System.Collections.Generic;

namespace ScriptableObjectTags
{
    /// <summary>
    /// 모든 태그를 관리하는 매니저입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "TagManager", menuName = "TagSystem/TagManager")]
    public class TagManager : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private List<TagData> tags = new List<TagData>(); // 관리되는 모든 태그 목록

        private Dictionary<string, TagData> tagDictionary = new Dictionary<string, TagData>(); // 태그 이름을 키로 하는 딕셔너리

        #endregion

        #region Events

        /// <summary>
        /// 새로운 태그가 추가되었을 때 호출됩니다.
        /// </summary>
        public event Action<TagData> OnTagAdded;

        /// <summary>
        /// 태그가 제거되었을 때 호출됩니다.
        /// </summary>
        public event Action<TagData> OnTagRemoved;

        #endregion

        #region Properties

        /// <summary>
        /// 현재 매니저에 등록된 모든 태그를 반환합니다.
        /// </summary>
        public IReadOnlyList<TagData> Tags => tags.AsReadOnly();

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// ScriptableObject가 활성화될 때 태그 딕셔너리를 재구성합니다.
        /// </summary>
        private void OnEnable()
        {
            RebuildTagDictionary();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 새로운 태그를 매니저에 추가합니다.
        /// </summary>
        /// <param name="newTag">추가할 태그 데이터</param>
        public void AddTag(TagData newTag)
        {
            if (newTag == null)
            {
                Debug.LogWarning("추가하려는 태그가 null입니다.");
                return;
            }

            if (string.IsNullOrEmpty(newTag.TagName))
            {
                Debug.LogWarning("태그 이름이 비어 있습니다.");
                return;
            }

            if (tagDictionary.ContainsKey(newTag.TagName))
            {
                Debug.LogWarning($"태그 이름 '{newTag.TagName}'이 이미 존재합니다.");
                return;
            }

            tags.Add(newTag);
            tagDictionary[newTag.TagName] = newTag; // 딕셔너리에 직접 추가
            OnTagAdded?.Invoke(newTag);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// 매니저에서 특정 태그를 제거합니다.
        /// </summary>
        /// <param name="tagToRemove">제거할 태그 데이터</param>
        public void RemoveTag(TagData tagToRemove)
        {
            if (tagToRemove == null)
            {
                Debug.LogWarning("제거하려는 태그가 null입니다.");
                return;
            }

            if (tags.Remove(tagToRemove))
            {
                tagDictionary.Remove(tagToRemove.TagName); // 딕셔너리에서 직접 제거
                OnTagRemoved?.Invoke(tagToRemove);

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
            else
            {
                Debug.LogWarning("해당 태그가 매니저에 존재하지 않습니다.");
            }
        }

        /// <summary>
        /// 특정 태그가 매니저에 존재하는지 확인합니다.
        /// </summary>
        /// <param name="tag">확인할 태그 데이터</param>
        /// <returns>존재하면 true, 아니면 false</returns>
        public bool HasTag(TagData tag)
        {
            return tag != null && tagDictionary.ContainsKey(tag.TagName);
        }

        /// <summary>
        /// 태그 이름으로 태그 데이터를 검색합니다.
        /// </summary>
        /// <param name="tagName">검색할 태그 이름</param>
        /// <returns>해당 이름의 태그 데이터, 없으면 null</returns>
        public TagData GetTagByName(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                Debug.LogWarning("태그 이름이 비어 있습니다.");
                return null;
            }

            tagDictionary.TryGetValue(tagName, out TagData tagData);
            return tagData;
        }

        /// <summary>
        /// 특정 부모 태그의 모든 자식 태그를 재귀적으로 가져옵니다.
        /// </summary>
        /// <param name="parentTag">부모 태그</param>
        /// <returns>모든 자식 태그 목록</returns>
        public List<TagData> GetAllChildTags(TagData parentTag)
        {
            if (parentTag == null)
            {
                Debug.LogWarning("부모 태그가 null입니다.");
                return new List<TagData>();
            }

            List<TagData> allChildTags = new List<TagData>();

            void AddChildTagsRecursive(TagData currentTag)
            {
                foreach (var child in currentTag.ChildTags)
                {
                    if (child != null && !allChildTags.Contains(child))
                    {
                        allChildTags.Add(child);
                        AddChildTagsRecursive(child);
                    }
                }
            }

            AddChildTagsRecursive(parentTag);
            return allChildTags;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 태그 목록을 기반으로 태그 딕셔너리를 재구성합니다.
        /// </summary>
        private void RebuildTagDictionary()
        {
            tagDictionary.Clear();
            foreach (var tag in tags)
            {
                if (tag != null && !string.IsNullOrEmpty(tag.TagName) && !tagDictionary.ContainsKey(tag.TagName))
                {
                    tagDictionary.Add(tag.TagName, tag);
                }
            }
        }

        #endregion
    }
}
