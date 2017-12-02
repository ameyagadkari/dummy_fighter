using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Links
{
    public class LinkDrop : MonoBehaviour, IDropHandler
    {
        private Transform _newParentTransform;

        private void Start()
        {
            _newParentTransform = transform.GetChild(0).GetChild(0);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!eventData.pointerDrag.CompareTag("Link")) return;

            var draggedGameObject = eventData.pointerDrag;
            var originalParentTransform = draggedGameObject.transform.parent;
            var originalName = draggedGameObject.name;

            // Make the dragged link part of the Chain Inspector 
            {
                draggedGameObject.transform.SetParent(_newParentTransform);
                draggedGameObject.transform.SetSiblingIndex(LinkDrag.DummySlotTransform.GetSiblingIndex());
                LinkDrag.DummySlotTransform.gameObject.SetActive(false);
                LinkDrag.LinkTransformsInChainInspector = null;
                var linkInfo = draggedGameObject.GetComponent<LinkInfo>();
                linkInfo.LayoutElementScript.enabled = true;
                linkInfo.LinkDragScript.enabled = false;
                if (linkInfo.LinkTypeName == LinkInfo.LinkType.Think ||
                    linkInfo.LinkTypeName == LinkInfo.LinkType.Watch)
                {
                    var goToDisplay = draggedGameObject.transform.GetChild(1).gameObject;
                    var rectTransform = goToDisplay.GetComponent<RectTransform>();
                    goToDisplay.SetActive(true);
                    rectTransform.offsetMin = Vector2.zero;
                    rectTransform.offsetMax = Vector2.zero;
                }
            }

            // Re-create the link in the link library
            {
                var draggedGameObjectClone = Instantiate(draggedGameObject, originalParentTransform);
                draggedGameObjectClone.name = originalName;
                var linkInfo = draggedGameObjectClone.GetComponent<LinkInfo>();
                linkInfo.LayoutElementScript.enabled = false;
                linkInfo.LinkDragScript.enabled = true;
                linkInfo.CanvasGroupScript.blocksRaycasts = true;
                draggedGameObjectClone.transform.SetSiblingIndex((int)linkInfo.LinkTypeName);
                if (linkInfo.LinkTypeName == LinkInfo.LinkType.Think ||
                    linkInfo.LinkTypeName == LinkInfo.LinkType.Watch)
                {
                    draggedGameObjectClone.transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }
}

