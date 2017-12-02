using UnityEngine;
using UnityEngine.UI;

namespace Buttons
{
    public class Redo : MonoBehaviour
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        private void OnEnable()
        {
            _button.onClick.AddListener(OnClickRedo);
        }
        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }
        private static void OnClickRedo()
        {
            LinksManager.Instance.SwitchList(ref LinksManager.Instance.Unused, ref LinksManager.Instance.InUse, true);
        }
    }
}
