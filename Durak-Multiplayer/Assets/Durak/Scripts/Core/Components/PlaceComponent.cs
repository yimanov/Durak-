using UnityEngine;
using UnityEngine.UI;

public class PlaceComponent : MonoBehaviour
{
    public RectTransform Transform;
    public HorizontalLayoutGroup Group;
    public LayoutElement LayoutElmt;
    public Image ActivePlace;

    [HideInInspector] public int PlayerId;
    public bool IsActive { get { return gameObject.activeInHierarchy; } }

    /// <summary>
    /// Call LayoutRebuilder class for update transfom.
    /// </summary>
    public void Rebuild()
    {
        LayoutRebuilder.MarkLayoutForRebuild(Transform);
    }

    /// <summary>
    /// Set sprite and set enable activity object.
    /// </summary>
    /// <param name="spriteState"></param>
    public void HighlightPlace(Sprite spriteState)
    {
        if (spriteState == null)
        {
            ActivePlace.enabled = false;
            return;
        }

        ActivePlace.sprite = spriteState;
        ActivePlace.enabled = true;
    }

    /// <summary>
    /// Activate or deactivate place.
    /// </summary>
    /// <param name="state"></param>
    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    /// <summary>
    /// Clear place childs.
    /// </summary>
    public void Clear()
    {
        foreach (Transform child in Transform)
        {
            Destroy(child.gameObject);
        }
    }
}
