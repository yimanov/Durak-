using UnityEngine;
using UnityEngine.EventSystems;

namespace Durak
{
    public class CardComponent : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Transform Holder;
        public CardData Data;
        public VisualData VisualData;
        public CardView View;

        private bool _isFaceUp;

        public int CardId => Data.Priority + (int)Data.Suit;

        public bool IsActive;
        public float StartPosY;

        private SessionManager _sessionManager;

        public virtual void InitializeComponents()
        {
            _sessionManager = LazySingleton<SessionManager>.Instance;

            View.Initialize(VisualData);
            transform.ResetTransformLocal();
            StartPosY = transform.localPosition.y;
        }

        public void Lock()
        {
            IsActive = false;
        }

        public void Unlock()
        {
            IsActive = true;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1 || !IsActive)
            {
                return;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1 || !IsActive)
            {
                return;
            }

            transform.position += (Vector3)(eventData.delta);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (Input.touchCount > 1 || !IsActive)
            {
                return;
            }

            _sessionManager.OnValidateCard(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Input.touchCount > 1 || !IsActive)
            {
                return;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Input.touchCount > 1 || !IsActive)
            {
                return;
            }
        }

        /// <summary>
        /// Is current view face?
        /// </summary>
        public bool IsFaceUp()
        {
            return _isFaceUp;
        }

        public void FaceUp()
        {
            _isFaceUp = true;

            View.FaceUp();
        }

        public void FaceDown()
        { 
            View.FaceDown();

            _isFaceUp = false;
        }

        public void UpdateView()
        {
            if (IsFaceUp())
            {
                View.FaceUp();
            }
            else
            {
                View.FaceDown();
            }
        }
    }
}