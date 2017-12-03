using System;
using System.IO;
using Links;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public enum SceneNames { Editor, Fight }

    public static Manager Instance => _instance ?? (_instance = FindObjectOfType<Manager>());

    private static Manager _instance;
    private const string SaveFileNameWithExtensionWithPrecedingBlackSlash = @"\dummy_fighter.sav";
    private const string SaveFolderNameWithPrecedingBlackSlash = @"\Assets\StreamingAssets";
    private string _currentDirectory;
    private string _absoluteFilePath;
    private const int TotalNumberOfLinks = 4;
    private GameObject[] _links;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _currentDirectory = Directory.GetCurrentDirectory();
        if (!Directory.Exists(_currentDirectory + SaveFolderNameWithPrecedingBlackSlash))
        {
            Directory.CreateDirectory(_currentDirectory + SaveFolderNameWithPrecedingBlackSlash);
        }
        _absoluteFilePath = _currentDirectory + SaveFolderNameWithPrecedingBlackSlash +
                               SaveFileNameWithExtensionWithPrecedingBlackSlash;
        _links = new GameObject[TotalNumberOfLinks];
        _links[0] = GameObject.Find("LinkThink");
        Assert.IsNotNull(_links[0]);
        _links[1] = GameObject.Find("LinkWatch");
        Assert.IsNotNull(_links[1]);
        _links[2] = GameObject.Find("LinkAttack");
        Assert.IsNotNull(_links[2]);
        _links[3] = GameObject.Find("LinkDodge");
        Assert.IsNotNull(_links[3]);
    }

    private void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        if (File.Exists(_absoluteFilePath))
        {
            File.Delete(_absoluteFilePath);
        }
        using (var streamWriter = new StreamWriter(_absoluteFilePath))
        {
            var linksArray = LinksManager.Instance.InUse.Links.ToArray();
            Array.Reverse(linksArray);
            foreach (var link in linksArray)
            {
                var linkInfo = link.Item2.GetComponent<LinkInfo>();
                streamWriter.WriteLine((byte)linkInfo.LinkTypeName);
                streamWriter.WriteLine(link.Item1);
                if (linkInfo.LinkTypeName == LinkInfo.LinkType.Think || linkInfo.LinkTypeName == LinkInfo.LinkType.Watch)
                {
                    streamWriter.WriteLine(linkInfo.InCaseValues.IntToWriteRead);
                }
            }
            streamWriter.Flush();
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(_absoluteFilePath)) return;
        using (var streamReader = new StreamReader(_absoluteFilePath))
        {
            while (streamReader.Peek() >= 0)
            {
                var linkTypeString = streamReader.ReadLine();
                Assert.IsNotNull(linkTypeString);
                var linkType = (LinkInfo.LinkType)int.Parse(linkTypeString);

                var siblingIndexString = streamReader.ReadLine();
                Assert.IsNotNull(siblingIndexString);
                var siblingIndex = int.Parse(siblingIndexString);

                var inCaseValues = new LinkInfo.InCase();
                if (linkType == LinkInfo.LinkType.Think || linkType == LinkInfo.LinkType.Watch)
                {
                    var inCaseValuesString = streamReader.ReadLine();
                    Assert.IsNotNull(inCaseValuesString);
                    inCaseValues.IntToWriteRead = uint.Parse(inCaseValuesString);
                }
                ReCreateLink(linkType, siblingIndex, inCaseValues);
            }
        }
    }

    private void ReCreateLink(LinkInfo.LinkType linkType, int siblingIndex, LinkInfo.InCase inCaseValues)
    {
        var linkGameObject = Instantiate(_links[(int)linkType - 1], LinksManager.Instance.InUse.ParentTransform);
        linkGameObject.name = _links[(int)linkType - 1].name;
        linkGameObject.transform.SetSiblingIndex(siblingIndex);
        var linkInfo = linkGameObject.GetComponent<LinkInfo>();
        linkInfo.LayoutElementScript.enabled = true;
        linkInfo.LinkDragScript.enabled = false;
        linkInfo.CanvasGroupScript.blocksRaycasts = true;
        if (linkInfo.LinkTypeName == LinkInfo.LinkType.Think ||
            linkInfo.LinkTypeName == LinkInfo.LinkType.Watch)
        {
            var goToDisplay = linkGameObject.transform.GetChild(1).gameObject;
            var rectTransform = goToDisplay.GetComponent<RectTransform>();
            goToDisplay.SetActive(true);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            goToDisplay.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = inCaseValues.Idle.ToString();
            goToDisplay.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = inCaseValues.Attack.ToString();
            goToDisplay.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = inCaseValues.Dodge.ToString();
            linkInfo.InCaseValues = inCaseValues;
        }
        LinksManager.Instance.AddNewLink(siblingIndex, linkGameObject.transform);
    }
}
