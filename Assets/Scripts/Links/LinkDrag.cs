using UnityEngine;
using UnityEngine.EventSystems;

namespace Links
{
    public class LinkDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private CanvasGroup _canvasGroup;
        private Vector3 _startPosition;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = false;
            _startPosition = transform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
            var hitGameObject = eventData.pointerCurrentRaycast.gameObject;
            if (hitGameObject == null) return;
            if (hitGameObject.CompareTag("DropZone"))
            {
                
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {    
            _canvasGroup.blocksRaycasts = true;
            transform.position = _startPosition;
        }
    }
}
