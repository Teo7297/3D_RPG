using UnityEngine;
using RPG.Attributes;
using RPG.Movement;
using RPG.Combat;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField]
        private CursorMapping[] cursorMappings;

        private Health health;
        

        enum CursorType
        {
            None,
            Movement,
            Combat
        }

        [Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

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

            SetCursorType(CursorType.None);
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
                SetCursorType(CursorType.Combat);
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
                SetCursorType(CursorType.Movement);
                return true;
            }
            return false;
        }

        private void SetCursorType(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (var mapping in cursorMappings)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

