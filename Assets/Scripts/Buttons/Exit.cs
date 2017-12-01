using UnityEngine;
using UnityEngine.UI;


namespace Buttons
{
    public class Exit : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();      
        }
        private void OnEnable()
        {
            _button.onClick.AddListener(QuitApp);
        }
        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }
        private static void QuitApp()
        {
            Application.Quit();
        }
    }
}
