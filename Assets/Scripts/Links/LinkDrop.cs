using UnityEngine;
using UnityEngine.EventSystems;

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
            eventData.pointerDrag.transform.SetParent(_newParentTransform);
        }
    }
}

