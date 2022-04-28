using System;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update()
        {
            print(health.GetPercentage());
            GetComponent<TextMeshProUGUI>().SetText(
                String.Format("{0:0}%", health.GetPercentage()) // :0 means 0 decimal digits
            );
        }
    }
}