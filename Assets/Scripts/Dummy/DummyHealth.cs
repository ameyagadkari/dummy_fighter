using UnityEngine;
using UnityEngine.UI;

namespace Dummy
{
    public class DummyHealth : MonoBehaviour
    {
        private const float BasicDamage = 0.1f;
        private Image _healthImage;

        private void Awake()
        {
            _healthImage = GetComponent<Image>();
        }

        public void ApplyDamage(int multiplier)
        {
            _healthImage.fillAmount -= multiplier * BasicDamage;
        }

        public float GetHealth()
        {
            return _healthImage.fillAmount;
        }

        public void ResetHealth()
        {
            _healthImage.fillAmount = 1.0f;
        }
    }
}
