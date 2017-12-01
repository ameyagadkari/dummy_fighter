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
            draggedGameObject.transform.SetParent(_newParentTransform);
            draggedGameObject.GetComponent<LayoutElement>().enabled = true;
            draggedGameObject.GetComponent<LinkDrag>().enabled = false;

            var draggedGameObjectClone = Instantiate(draggedGameObject, originalParentTransform);
            draggedGameObjectClone.name = originalName;
            draggedGameObjectClone.GetComponent<LayoutElement>().enabled = false;
            draggedGameObjectClone.GetComponent<LinkDrag>().enabled = true;
            draggedGameObjectClone.GetComponent<CanvasGroup>().blocksRaycasts = true;
            draggedGameObjectClone.transform.SetSiblingIndex((int)draggedGameObjectClone.GetComponent<LinkInfo>().LinkTypeName);
        }
    }
}

