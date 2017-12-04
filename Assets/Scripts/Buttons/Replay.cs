using Dummy;
using UnityEngine;
using UnityEngine.UI;

namespace Buttons
{
    public class Replay : MonoBehaviour
    {
        private Button _button;
        private DummyController _player;
        private DummyController _enemy;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _player = GameObject.Find("Player").transform.GetChild(0).GetComponent<DummyController>();
            _enemy = GameObject.Find("Enemy").transform.GetChild(0).GetComponent<DummyController>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClickReplay);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnClickReplay()
        {
            _player.ReplayGame();
            _enemy.ReplayGame();
        }
    }
}
