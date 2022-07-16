#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Durak
{
    [RequireComponent(typeof(CardComponent))]
    public class CardGizmos : MonoBehaviour
    {
        [SerializeField]
        private bool _isGizmosEnable = true;

        [SerializeField]
        private CardComponent _cardComponent;
        [SerializeField]
        private Vector3 _offset;

        public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!_isGizmosEnable)
            {
                return;
            }
            if (!Application.isMobilePlatform && Application.isPlaying)
            {
                if (_cardComponent != null)
                {
                    GUIStyle style = new GUIStyle() { normal = new GUIStyleState() { textColor = Color.blue }, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold};
                    
                    string priority = _cardComponent.Data.Priority.ToString();
                    string suit = _cardComponent.Data.Suit.ToString();
                    string lockStatus = _cardComponent.IsActive ? "<color=green>Unlocked</color>" : "<color=red>Locked</color>";

                    Handles.Label(transform.position + _offset, string.Format("{0}|{1}\n{2}", suit, priority, lockStatus), style);
                }
            }
#endif
        }
    }
}