using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.FixedUI
{
    public class ToastUI : UIBase
    {
        public override UIType GetUIType() => UIType.ToastUI;
        [SerializeField] private Text txtMessage;

        public void Init(string message)
        {
            txtMessage.color = Color.white;
            txtMessage.text = message;
            StartCoroutine(FadeText());
        }

        private IEnumerator FadeText()
        {
            yield return new WaitForSeconds(1f);
            txtMessage.DOFade(0, 0.3f).OnComplete(Close);
        }

    }
}