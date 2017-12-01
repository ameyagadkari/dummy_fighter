using UnityEngine;

namespace Links
{
    public class LinkInfo : MonoBehaviour
    {
        public enum LinkType { Think = 1, Watch, Attack, Dodge }

        public LinkType LinkTypeName;
    }
}
