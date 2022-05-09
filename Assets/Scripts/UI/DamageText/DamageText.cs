using UnityEngine;
using TMPro;
using System;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI damageText;

        public void DestroyText()
        {
            Destroy(gameObject);
        }

        public void SetValue(float amount)
        {
            damageText.SetText(String.Format("{0:0}", amount));
        }
    }
}