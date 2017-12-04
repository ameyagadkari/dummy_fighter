using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
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
    public DummyController Player;
    public DummyController Enemy;
    public bool HasGameStarted;
    public bool HasGameFinished;

    private DummyHealth _playerHealth;
    private DummyHealth _enemyHealth;

    private static Manager _instance;
    private const string SaveFileNameWithExtensionWithPrecedingBlackSlash = @"\dummy_fighter.sav";
    private string _absoluteFilePath;
    private const int TotalNumberOfLinks = 4;
    private GameObject[] _links;
    private LinkInfo[] _playerLinkInfoArray;
    private bool _startCoroutine;
    private bool _canDamageBeCalculated;
    private int _currentStateNumber;

    private readonly DamageMultipliers[,] _damageMultipliersMatrix =
    {
        { new DamageMultipliers(0),new DamageMultipliers(0x0100),new DamageMultipliers(0) },
        { new DamageMultipliers(1),new DamageMultipliers(0x0303),new DamageMultipliers(0) },
        { new DamageMultipliers(0),new DamageMultipliers(0),new DamageMultipliers(0) }
    };

    // C# unions
    [StructLayout(LayoutKind.Explicit)]
    public class DamageMultipliers
    {
        [FieldOffset(0)]
        public readonly byte Player;

        [FieldOffset(1)]
        public readonly byte Enemy;

        [FieldOffset(0)]
        private readonly ushort Unused;

        internal DamageMultipliers(ushort value)
        {
            Unused = value;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _absoluteFilePath = Application.streamingAssetsPath +
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

    private IEnumerator Timer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _canDamageBeCalculated = true;
        _startCoroutine = true;
    }

    private void OnLevelLoadingFinished(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case (int)SceneNames.Editor:
                LoadGame();
                break;
            case (int)SceneNames.Fight:
                Player = GameObject.Find("Player").transform.GetChild(0).GetComponent<DummyController>();
                Enemy = GameObject.Find("Enemy").transform.GetChild(0).GetComponent<DummyController>();
                _playerHealth = GameObject.Find("PlayerHealthBar").GetComponent<DummyHealth>();
                _enemyHealth = GameObject.Find("EnemyHealthBar").GetComponent<DummyHealth>();
                InitializeActorControllers();
                ResetManager();
                break;
        }
    }

    public void ResetManager()
    {
        StopAllCoroutines();
        HasGameStarted = false;
        HasGameFinished = false;
        _startCoroutine = true;
        _canDamageBeCalculated = false;
        _currentStateNumber = 0;
        _playerHealth.ResetHealth();
        _enemyHealth.ResetHealth();
    }

    private void Update()
    {
        if (HasGameFinished) return;
        HasGameFinished = _playerHealth?.GetHealth() <= 0.0f || _enemyHealth?.GetHealth() <= 0.0f;
        if (HasGameFinished)
        {
            Player.SetWinLoseState(_playerHealth.GetHealth() >= _enemyHealth.GetHealth());
            Enemy.SetWinLoseState(_enemyHealth.GetHealth() >= _playerHealth.GetHealth());
            return;
        }
        if (_canDamageBeCalculated && _currentStateNumber < Enemy.DummyStatesArray.Length)
        {
            _canDamageBeCalculated = false;
            var damageMultipliers = _damageMultipliersMatrix[(int)Enemy.DummyStatesArray[_currentStateNumber], (int)Player.DummyStatesArray[_currentStateNumber]];
            _enemyHealth.ApplyDamage(damageMultipliers.Enemy);
            _playerHealth.ApplyDamage(damageMultipliers.Player);
            _currentStateNumber++;
        }
        else if (Enemy != null && _currentStateNumber == Enemy.DummyStatesArray.Length && Enemy.CurrentStateNumber == Enemy.DummyStatesArray.Length && Player.CurrentStateNumber == Player.DummyStatesArray.Length)
        {
            Player.SetWinLoseState(_playerHealth.GetHealth() >= _enemyHealth.GetHealth());
            Enemy.SetWinLoseState(_enemyHealth.GetHealth() >= _playerHealth.GetHealth());
            _currentStateNumber++;
            HasGameFinished = true;
        }
        if (!_startCoroutine || !HasGameStarted) return;
        _startCoroutine = false;
        StartCoroutine(Timer(1.0f));
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
