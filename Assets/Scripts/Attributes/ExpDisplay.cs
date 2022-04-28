using System;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class ExpDisplay : MonoBehaviour
    {
        Experience exp;

        private void Awake()
        {
            exp = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            GetComponent<TextMeshProUGUI>().SetText(
                String.Format("{0:0}", exp.ExperiencePoints) // :0 means 0 decimal digits
            );
        }
    }
}