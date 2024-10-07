using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ScriptableObjectTags
{
    /// <summary>
    /// 컴포넌트에 태그를 할당할 수 있는 기능을 제공합니다.
    /// </summary>
    public class Taggable : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        public TagManager tagManager; // 인스펙터에서 TagManager를 지정할 수 있습니다.

        [SerializeField]
        private List<TagData> objectTags = new List<TagData>(); // 객체에 할당된 태그 목록

        #endregion

        #region Properties

        /// <summary>
        /// 객체에 할당된 태그들을 읽기 전용으로 반환합니다.
        /// </summary>
        public IReadOnlyList<TagData> ObjectTags => objectTags.AsReadOnly();

        #endregion

        #region Public Methods

        /// <summary>
        /// 새로운 태그를 객체에 추가합니다.
        /// </summary>
        /// <param name="tag">추가할 태그 데이터</param>
        public void AddTag(TagData tag)
        {
            if (tag == null)
            {
                Debug.LogWarning("추가하려는 태그가 null입니다.");
                return;
            }

            if (tagManager == null)
            {
                Debug.LogWarning("TagManager가 설정되지 않았습니다.");
                return;
            }

            if (!objectTags.Contains(tag))
            {
                objectTags.Add(tag);
                MarkDirty();
            }
            else
            {
                Debug.LogWarning($"태그 '{tag.TagName}'는 이미 할당되어 있습니다.");
            }
        }

        /// <summary>
        /// 객체에서 특정 태그를 제거합니다.
        /// </summary>
        /// <param name="tag">제거할 태그 데이터</param>
        public void RemoveTag(TagData tag)
        {
            if (tag == null)
            {
                Debug.LogWarning("제거하려는 태그가 null입니다.");
                return;
            }

            if (objectTags.Contains(tag))
            {
                objectTags.Remove(tag);
                MarkDirty();
            }
            else
            {
                Debug.LogWarning($"태그 '{tag.TagName}'가 객체에 존재하지 않습니다.");
            }
        }

        /// <summary>
        /// 객체가 특정 태그를 가지고 있는지 확인합니다.
        /// </summary>
        /// <param name="tag">확인할 태그 데이터</param>
        /// <returns>태그를 가지고 있으면 true, 아니면 false</returns>
        public bool HasTag(TagData tag)
        {
            return tag != null && objectTags.Contains(tag);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 변경 사항을 에디터에 알리기 위해 객체를 Dirty 상태로 표시합니다.
        /// </summary>
        private void MarkDirty()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        #endregion
    }
}
