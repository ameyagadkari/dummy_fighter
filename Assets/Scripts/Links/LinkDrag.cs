using System.Collections.Generic;
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
        private static List<Transform> _linkTransformsInChainInspector;
        public static List<Transform> LinkTransformsInChainInspector
        {
            get
            {
                if (_linkTransformsInChainInspector != null) return _linkTransformsInChainInspector;
                _linkTransformsInChainInspector = new List<Transform>();
                foreach (Transform transform in DummySlotTransform.parent.transform)
                {
                    _linkTransformsInChainInspector.Add(transform);
                }
                return _linkTransformsInChainInspector;
            }
            set { _linkTransformsInChainInspector = value; }
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
                var rayCastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, rayCastResults);
                foreach (var rayCastResult in rayCastResults)
                {
                    if (rayCastResult.gameObject.CompareTag("DropZone"))
                    {
                        DummySlotTransform.gameObject.SetActive(true);
                        DummySlotTransform.SetAsLastSibling();
                        var length = LinkTransformsInChainInspector.Count - 1;
                        for (var i = 0; i < length; i++)
                        {
                            if (!(transform.position.x < LinkTransformsInChainInspector[i].position.x)) continue;
                            DummySlotTransform.SetSiblingIndex(LinkTransformsInChainInspector[i].GetSiblingIndex());
                            break;
                        }
                        break;
                    }
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
