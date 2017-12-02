using UnityEngine;
using UnityEngine.UI;

namespace Buttons
{
    public class Undo : MonoBehaviour
    {
        private Button _button;
       
        private void Awake()
        {
            _button = GetComponent<Button>();     
        }
        private void OnEnable()
        {
            _button.onClick.AddListener(OnClickUndo);
        }
        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }
        private static void OnClickUndo()
        {
            
        }
    }
}
