using UnityEngine;
using ScriptableObjectTags;

public class TaggableTest : MonoBehaviour
{
    public TagManager tagManager;  // Inspector에서 할당
    private Taggable taggable;

    void Awake()
    {
        // Taggable 컴포넌트 추가
        taggable = gameObject.GetComponent<Taggable>();
    }

    void Start()
    {
        if (tagManager == null || tagManager.Tags.Count == 0)
        {
            Debug.LogError("TagManager is not assigned or contains no tags.");
            return;
        }

        // 첫 번째 태그를 가져옴
        TagData firstTag = tagManager.Tags[0];
        Debug.Log($"Adding Tag: {firstTag.TagName}");

        // 태그 추가
        taggable.AddTag(firstTag);

        // 태그가 추가되었는지 확인
        if (taggable.HasTag(firstTag))
        {
            Debug.Log($"Tag '{firstTag.TagName}' has been successfully added to the GameObject.");
        }
        else
        {
            Debug.LogError($"Failed to add tag '{firstTag.TagName}' to the GameObject.");
        }

        // 모든 태그 출력
        Debug.Log("All Tags:");
        foreach (var tag in taggable.ObjectTags)
        {
            Debug.Log(tag.TagName);
        }

        // 태그 제거
        taggable.RemoveTag(firstTag);
        Debug.Log($"Removed Tag: {firstTag.TagName}");

        // 태그가 제거되었는지 확인
        if (!taggable.HasTag(firstTag))
        {
            Debug.Log($"Tag '{firstTag.TagName}' has been successfully removed from the GameObject.");
        }
        else
        {
            Debug.LogError($"Failed to remove tag '{firstTag.TagName}' from the GameObject.");
        }
    }
}
