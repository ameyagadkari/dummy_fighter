using System;
using System.IO;
using Dummy;
using Links;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
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
    private LinkInfo[] _playerLinkInfoArray;

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
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoadingFinished;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoadingFinished;
    }

    private void OnLevelLoadingFinished(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case (int)SceneNames.Editor:
                LoadGame();
                break;
            case (int)SceneNames.Fight:
                InitializeActorControllers();
                break;
        }
    }

    public void SaveGame(bool fetchPlayerLinkInfo = false)
    {  
        if (File.Exists(_absoluteFilePath))
        {
            File.Delete(_absoluteFilePath);
        }
        using (var streamWriter = new StreamWriter(_absoluteFilePath))
        {
            var linksArray = LinksManager.Instance.InUse.Links.ToArray();
            Array.Reverse(linksArray);
            if (fetchPlayerLinkInfo)
            {
                _playerLinkInfoArray = new LinkInfo[linksArray.Length];
                for (var i = 0; i < linksArray.Length; i++)
                {
                    _playerLinkInfoArray[i] = linksArray[i].Item2.GetComponent<LinkInfo>();
                }
            }
            foreach (var link in linksArray)
            {
                var linkInfo = link.Item2.GetComponent<LinkInfo>();
                streamWriter.WriteLine((byte)linkInfo.LinkTypeName);
                streamWriter.WriteLine(link.Item1);
                if (linkInfo.LinkTypeName != LinkInfo.LinkType.Think &&
                    linkInfo.LinkTypeName != LinkInfo.LinkType.Watch) continue;
                streamWriter.WriteLine(linkInfo.InCaseValues.Idle);
                streamWriter.WriteLine(linkInfo.InCaseValues.Attack);
                streamWriter.WriteLine(linkInfo.InCaseValues.Dodge);
            }
            streamWriter.Flush();
        }
    }

    private void LoadGame()
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
                    var idle = streamReader.ReadLine();
                    Assert.IsNotNull(idle);
                    inCaseValues.Idle = int.Parse(idle);

                    var attack = streamReader.ReadLine();
                    Assert.IsNotNull(attack);
                    inCaseValues.Attack = int.Parse(attack);

                    var dodge = streamReader.ReadLine();
                    Assert.IsNotNull(dodge);
                    inCaseValues.Dodge = int.Parse(dodge);
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

    private void InitializeActorControllers()
    {
        var linksArray = LinksManager.Instance.InUse.Links.ToArray();
        Array.Reverse(linksArray);
        var length = linksArray.Length;
        var enemyLinkInfoArray = new LinkInfo[length];
        for (var i = 0; i < length; i++)
        {
            enemyLinkInfoArray[i] = LinkInfo.GetRandomLink();
        }

        // Process player's links
        {
            var dummyController = GameObject.Find("Player").transform.GetChild(0).GetComponent<DummyController>();
            dummyController.ProcessLinks(ref _playerLinkInfoArray, ref enemyLinkInfoArray);
        }

        // Process enemy's links
        {
            var dummyController = GameObject.Find("Enemy").transform.GetChild(0).GetComponent<DummyController>();
            dummyController.ProcessLinks(ref enemyLinkInfoArray, ref _playerLinkInfoArray);
        }
    }
}
