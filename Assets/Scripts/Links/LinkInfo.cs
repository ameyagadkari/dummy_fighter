using UnityEngine;
using UnityEngine.UI;

namespace Links
{
    public class LinkInfo : MonoBehaviour
    {
        public enum LinkType : byte { Think = 1, Watch, Attack, Dodge }

        public LinkType LinkTypeName;

        [HideInInspector]
        public CanvasGroup CanvasGroupScript;
        [HideInInspector]
        public LayoutElement LayoutElementScript;
        [HideInInspector]
        public LinkDrag LinkDragScript;

        private void Awake()
        {
            CanvasGroupScript = GetComponent<CanvasGroup>();
            LayoutElementScript = GetComponent<LayoutElement>();
            LinkDragScript = GetComponent<LinkDrag>();
        }
    }
}
