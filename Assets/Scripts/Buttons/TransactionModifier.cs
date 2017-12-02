using UnityEngine;
using UnityEngine.UI;

namespace Buttons
{
    public class TransactionModifier : MonoBehaviour
    {
        public enum TransactionModifierType : byte { Unknown, Plus, Minus }

        public TransactionModifierType TransactionModifierTypeName;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        private void OnEnable()
        {
            _button.onClick.AddListener(OnClickTransactionModifier);
        }
        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }
        private void OnClickTransactionModifier()
        {
            if (Transaction.SelectedText == null) return;
            switch (TransactionModifierTypeName)
            {
                case TransactionModifierType.Plus:
                    switch (Transaction.SelectedInCaseTypeName)
                    {
                        case Transaction.InCaseType.Idle:
                            Transaction.SelectedLinkInfo.InCaseValues.Idle++;
                            Transaction.SelectedText.text = Transaction.SelectedLinkInfo.InCaseValues.Idle.ToString();
                            break;
                        case Transaction.InCaseType.Attack:
                            Transaction.SelectedLinkInfo.InCaseValues.Attack++;
                            Transaction.SelectedText.text = Transaction.SelectedLinkInfo.InCaseValues.Attack.ToString();
                            break;
                        case Transaction.InCaseType.Dodge:
                            Transaction.SelectedLinkInfo.InCaseValues.Dodge++;
                            Transaction.SelectedText.text = Transaction.SelectedLinkInfo.InCaseValues.Dodge.ToString();
                            break;
                    }
                    break;
                case TransactionModifierType.Minus:
                    switch (Transaction.SelectedInCaseTypeName)
                    {
                        case Transaction.InCaseType.Idle:
                            Transaction.SelectedLinkInfo.InCaseValues.Idle--;
                            Transaction.SelectedText.text = Transaction.SelectedLinkInfo.InCaseValues.Idle.ToString();
                            break;
                        case Transaction.InCaseType.Attack:
                            Transaction.SelectedLinkInfo.InCaseValues.Attack--;
                            Transaction.SelectedText.text = Transaction.SelectedLinkInfo.InCaseValues.Attack.ToString();
                            break;
                        case Transaction.InCaseType.Dodge:
                            Transaction.SelectedLinkInfo.InCaseValues.Dodge--;
                            Transaction.SelectedText.text = Transaction.SelectedLinkInfo.InCaseValues.Dodge.ToString();
                            break;
                    }
                    break;
            }
        }
    }
}
