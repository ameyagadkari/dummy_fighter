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
            public int Idle;
            public int Attack;
            public int Dodge;
        }

        public InCase InCaseValues;

        // Random Link info generation
        public static LinkInfo GetRandomLink()
        {
            // WARNING: Using new to prevent code repetition
            var returnValue = new LinkInfo();
            // !WARNING
            returnValue.LinkTypeName = (LinkType)Random.Range(1, 5);
            if (returnValue.LinkTypeName != LinkType.Think && returnValue.LinkTypeName != LinkType.Watch)
            {
                return returnValue;
            }
            returnValue.InCaseValues.Idle = Random.Range(int.MinValue, int.MaxValue);
            returnValue.InCaseValues.Attack = Random.Range(int.MinValue, int.MaxValue);
            returnValue.InCaseValues.Dodge = Random.Range(int.MinValue, int.MaxValue);
            return returnValue;
        }

        // Used for quick access to respective behavior scripts 

        public CanvasGroup CanvasGroupScript { get; private set; }
        public LayoutElement LayoutElementScript { get; private set; }
        public LinkDrag LinkDragScript { get; private set; }

        private void Awake()
        {
            CanvasGroupScript = GetComponent<CanvasGroup>();
            LayoutElementScript = GetComponent<LayoutElement>();
            LinkDragScript = GetComponent<LinkDrag>();
        }
    }
}
