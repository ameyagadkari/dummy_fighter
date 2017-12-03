using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Links
{
    public class LinkInfo : MonoBehaviour
    {
        public enum LinkType : byte { Think = 1, Watch, Attack, Dodge }

        public LinkType LinkTypeName;

        [StructLayout(LayoutKind.Explicit)]
        [Serializable]
        public struct InCase
        {
            [FieldOffset(0)]
            public byte Idle;
            [FieldOffset(1)]
            public byte Attack;
            [FieldOffset(2)]
            public byte Dodge;
            [FieldOffset(3)]
            public readonly byte UnusedByte;
            [FieldOffset(0)]
            public uint IntToWriteRead;
        }

        public InCase InCaseValues;

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
