using UnityEngine;

public class Manager : MonoBehaviour
{
    public enum SceneNames
    {
        Editor,
        Fight
    }

    public static Manager Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<Manager>()); }
    }

    private static Manager _instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
