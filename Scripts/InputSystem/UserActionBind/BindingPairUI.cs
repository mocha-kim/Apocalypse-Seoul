using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace InputSystem.UserActionBind
{
    public class BindingPairUI : MonoBehaviour
    {
        public bool selected = false;

        public Text actionLabel;
        public Text codeLabel;
        public Button codeButton;
        public Image buttonImage;

        public void Select()
        {
            selected = true;
            buttonImage.color = Color.green;
        }

        public void Deselect()
        {
            selected = false;
            buttonImage.color = Color.white;
        }

        public void InitLabels(string actionText, string codeText)
        {
            actionLabel.text = actionText;
            codeLabel.text = codeText;
        }

        public void SetCodeLabel(string text)
        {
            codeLabel.text = text;
        }

        public void AddButtonListener(UnityAction method)
        {
            codeButton.onClick.AddListener(method);
        }
    }
}