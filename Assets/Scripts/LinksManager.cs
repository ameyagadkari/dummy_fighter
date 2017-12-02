using System.Collections.Generic;
using UnityEngine;

public class LinksManager : MonoBehaviour
{
    public static LinksManager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<LinksManager>()); }
    }

    private static LinksManager _instance;

    public struct LinksHolder
    {
        public Stack<Transform> Links;
        public Transform ParentTransform;

        public LinksHolder(Transform parentTransform)
        {
            ParentTransform = parentTransform;
            Links = new Stack<Transform>();
        }
    }

    public LinksHolder Unused;
    public LinksHolder InUse;

    private void Awake()
    {
        Unused = new LinksHolder(GameObject.Find("Undo Link List").GetComponent<Transform>());
        InUse = new LinksHolder(GameObject.FindWithTag("DropZone").transform.GetChild(0));
    }
}
