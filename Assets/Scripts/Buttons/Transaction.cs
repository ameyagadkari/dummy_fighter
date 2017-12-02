using UnityEngine;
using UnityEngine.UI;

namespace Buttons
{
    public class Transaction : MonoBehaviour
    {
        private Button _button;
        private Image _image;
        private static Image _previousImage;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }
        private void OnEnable()
        {
            _button.onClick.AddListener(OnClickTransaction);
        }
        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }
        private void OnClickTransaction()
        {
            if (_previousImage != null)
            {
                _previousImage.color = Color.white;
            }
            _previousImage = _image;
            _image.color = Color.green;
        }
    }
}
