using System;
using System.Collections.Generic;
using UnityEngine;

namespace Links
{
    public class LinksManager : MonoBehaviour
    {
        public static LinksManager Instance => _instance ?? (_instance = FindObjectOfType<LinksManager>());

        private static LinksManager _instance;

        public struct LinksHolder
        {
            public Stack<Tuple<int, Transform>> Links;
            public Transform ParentTransform;

            public LinksHolder(Transform parentTransform)
            {
                ParentTransform = parentTransform;
                Links = new Stack<Tuple<int, Transform>>();
            }
        }

        public LinksHolder Unused;
        public LinksHolder InUse;

        private void Awake()
        {
            Unused = new LinksHolder(GameObject.Find("Undo Link List").GetComponent<Transform>());
            InUse = new LinksHolder(GameObject.FindWithTag("DropZone").transform.GetChild(0));
        }

        public void SwitchList(ref LinksHolder fromLinksHolder, ref LinksHolder toLinksHolder,
            bool useChildIndex = false)
        {
            if (fromLinksHolder.Links.Count <= 0) return;
            var linkTransform = fromLinksHolder.Links.Pop();
            linkTransform.Item2.SetParent(toLinksHolder.ParentTransform);
            linkTransform.Item2.gameObject.SetActive(false);
            if (useChildIndex)
            {
                linkTransform.Item2.SetSiblingIndex(linkTransform.Item1);
                linkTransform.Item2.gameObject.SetActive(true);
            }
            toLinksHolder.Links.Push(linkTransform);
        }

        public void AddNewLink(int siblingIndex, Transform linkTransform)
        {
            InUse.Links.Push(Tuple.Create(siblingIndex, linkTransform));
            foreach (var link in Unused.Links)
            {
                Destroy(link.Item2.gameObject);
            }
            Unused.Links.Clear();
        }
    }
}