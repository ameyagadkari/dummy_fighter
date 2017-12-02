using Links;
using UnityEngine;
using UnityEngine.UI;

namespace Buttons
{
    public class Transaction : MonoBehaviour
    {
        public static Text SelectedText { get; private set; }
        public static LinkInfo SelectedLinkInfo { get; private set; }
        public static InCaseType SelectedInCaseTypeName { get; private set; }

        public enum InCaseType : byte { Idle, Attack, Dodge }

        public InCaseType InCaseTypeName;

        private Button _button;
        private Image _image;
        private Text _text;
        private LinkInfo _linkInfo;
        private static Image _previousImage;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            _text = transform.GetChild(0).GetComponent<Text>();
            _linkInfo = transform.parent.parent.GetComponent<LinkInfo>();
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
            SelectedText = _text;
            SelectedLinkInfo = _linkInfo;
            SelectedInCaseTypeName = InCaseTypeName;
            _image.color = Color.green;
        }
    }
}
