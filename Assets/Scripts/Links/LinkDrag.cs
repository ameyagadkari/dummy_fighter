using UnityEngine;
using UnityEngine.EventSystems;

namespace Links
{
    public class LinkDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private CanvasGroup _canvasGroup;
        private Vector3 _startPosition;
        private static Transform _dummySlotTransform;
        public static Transform DummySlotTransform
        {
            get
            {
                return _dummySlotTransform ?? (_dummySlotTransform = GameObject.FindGameObjectWithTag("Slot").transform);
            }
        }

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            DummySlotTransform.gameObject.SetActive(false);
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
            if (hitGameObject == null)
            {
                DummySlotTransform.gameObject.SetActive(false);
            }
            else
            {
                if (hitGameObject.CompareTag("DropZone"))
                {
                    DummySlotTransform.gameObject.SetActive(true);
                    DummySlotTransform.SetAsLastSibling();
                    var linkTransformsInChainInspector =
                        DummySlotTransform.parent.GetComponentsInChildren<Transform>();
                    var length = linkTransformsInChainInspector.Length - 1;
                    for (var i = 0; i < length; i++)
                    {
                        if (!(transform.position.x < linkTransformsInChainInspector[i].position.x)) continue;
                        DummySlotTransform.SetSiblingIndex(linkTransformsInChainInspector[i].GetSiblingIndex());
                        break;
                    }
                }
                else
                {
                    DummySlotTransform.gameObject.SetActive(false);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _canvasGroup.blocksRaycasts = true;
            transform.position = _startPosition;
        }
    }
}
