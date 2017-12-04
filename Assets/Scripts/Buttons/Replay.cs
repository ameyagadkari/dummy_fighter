using UnityEngine;
using UnityEngine.UI;

namespace Buttons
{
    public class Replay : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClickReplay);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        private static void OnClickReplay()
        {
            Manager.Instance.Player.ReplayGame();
            Manager.Instance.Enemy.ReplayGame();
        }
    }
}
