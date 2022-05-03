using UnityEngine;
using RPG.Attributes;
using RPG.Movement;
using RPG.Combat;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {

        private Health health;

        private void Awake()
        {
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (health.IsDead()) { return; }
            if (InteractWithCombat()) { return; } //if we can attack we don't move
            if (InteractWithMovement()) { return; }
            //If we are here we are hovering the mouse over the void
        }

        private bool InteractWithCombat()
        {
            //Returns an array of all the items the ray hits
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target is null) { continue; }

                if (!GetComponent<Fighter>().CanAttack(target.gameObject)) { continue; }

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Fighter>().Attack(target.gameObject);
                }
                //we want to NOT move even when only hovering over an enemy
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point, 1f);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

