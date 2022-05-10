using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private RectTransform foreground;
        [SerializeField]
        private Canvas canvas;
        private Health health;

        private void Start()
        {
            health = GetComponentInParent<Health>();
        }

        private void Update()
        {
            var fraction = health.GetFraction();
            if (Mathf.Approximately(1f, fraction) || Mathf.Approximately(0f, fraction))
            {
                canvas.enabled = false;
                return;
            }
            canvas.enabled = true;
            foreground.localScale = new Vector3(fraction, 1, 1);
        }
    }
}