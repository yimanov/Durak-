using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField]
    protected Image _cardView;

    private VisualData _cardVisualData;

    public void Initialize(VisualData data)
    {
        _cardVisualData = data;
    }

    /// <summary>
    /// Set face view for card
    /// </summary>
    public void FaceUp()
    {
        _cardView.sprite = _cardVisualData.Face;
    }

    /// <summary>
    /// Set back view for card
    /// </summary>
    public void FaceDown()
    {
        _cardView.sprite = _cardVisualData.Back;
    }
}