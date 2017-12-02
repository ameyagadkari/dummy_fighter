using UnityEngine;
using UnityEngine.UI;

namespace Links
{
    public class LinkInfo : MonoBehaviour
    {
        public enum LinkType : byte { Think = 1, Watch, Attack, Dodge }

        public LinkType LinkTypeName;

        [System.Serializable]
        public struct InCase
        {
            public byte Idle;
            public byte Attack;
            public byte Dodge;
        }

        public InCase InCaseValues;

        // Used for quick access to respective behavior scripts 
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
