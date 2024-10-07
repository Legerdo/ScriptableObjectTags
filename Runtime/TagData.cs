using UnityEngine;
using System.Collections.Generic;

namespace ScriptableObjectTags
{
    /// <summary>
    /// 태그 데이터를 정의하는 ScriptableObject입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "NewTagData", menuName = "TagSystem/TagData")]
    public class TagData : ScriptableObject
    {
        #region Fields

        [SerializeField]
        private string tagName; // 태그의 이름

        [Tooltip("이 태그의 부모 태그가 있을 경우 지정합니다.")]
        [SerializeField]
        private TagData parentTag; // 부모 태그

        [SerializeField]
        private List<TagData> childTags = new List<TagData>(); // 자식 태그 목록

        #endregion

        #region Properties

        /// <summary>
        /// 태그의 이름을 반환합니다.
        /// </summary>
        public string TagName => tagName;

        /// <summary>
        /// 부모 태그를 반환합니다.
        /// </summary>
        public TagData ParentTag => parentTag;

        /// <summary>
        /// 자식 태그 목록을 읽기 전용으로 반환합니다.
        /// </summary>
        public IReadOnlyList<TagData> ChildTags => childTags.AsReadOnly();

        #endregion

        #region Public Methods

        /// <summary>
        /// 태그의 이름을 변경합니다.
        /// </summary>
        /// <param name="newName">새로운 태그 이름</param>
        public void SetTagName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                Debug.LogWarning("태그 이름은 비어 있을 수 없습니다.");
                return;
            }

            tagName = newName;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        /// <summary>
        /// 자식 태그를 추가합니다.
        /// </summary>
        /// <param name="childTag">추가할 자식 태그 데이터</param>
        public void AddChildTag(TagData childTag)
        {
            if (childTag == null)
            {
                Debug.LogWarning("추가하려는 자식 태그가 null입니다.");
                return;
            }

            if (!childTags.Contains(childTag))
            {
                childTags.Add(childTag);
                childTag.SetParentTag(this);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
            else
            {
                Debug.LogWarning($"태그 '{childTag.TagName}'는 이미 자식 태그로 추가되어 있습니다.");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 부모 태그를 설정합니다.
        /// </summary>
        /// <param name="parent">설정할 부모 태그</param>
        private void SetParentTag(TagData parent)
        {
            parentTag = parent;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        #endregion
    }
}
