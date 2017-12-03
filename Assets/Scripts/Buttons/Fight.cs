using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Buttons
{
    public class Fight : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        private void OnEnable()
        {
            _button.onClick.AddListener(OnClickFight);
        }
        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }
        private static void OnClickFight()
        {
            Manager.Instance.SaveGame();
            SceneManager.LoadScene((int)Manager.SceneNames.Fight);
        }
    }
}
