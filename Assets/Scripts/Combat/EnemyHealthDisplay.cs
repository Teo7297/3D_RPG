using System;
using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update()
        {
            if (fighter.GetTarget() is null)
            {
                GetComponent<TextMeshProUGUI>().SetText("N.A.");
            }
            else
            {
                Health health = fighter.GetTarget();
                GetComponent<TextMeshProUGUI>().SetText(
                    String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints()) // :0 means 0 decimal digits
                );
            }
        }
    }
}