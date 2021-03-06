using UnityEngine;
using RPG.Attributes;
using RPG.Movement;
using RPG.Combat;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField]
        private CursorMapping[] cursorMappings;
        [SerializeField]
        private float maxNavMeshProjectionDistance = 1f;
        [SerializeField]
        private float raycastRadius = 1f;


        private Health health;


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
            if (InteractWithUI()) { return; }
            if (health.IsDead())
            {
                SetCursorType(CursorType.None);
                return;
            }
            if (InteractWithComponent()) { return; }
            if (InteractWithMovement()) { return; }
            //If we are here we are hovering the mouse over the void

            SetCursorType(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject()) //! This refers to UI gameobjects!!
            {
                SetCursorType(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent()
        {
            //? RaycastAll returns hits in non predictable order

            RaycastHit[] hits = SortRaycastAll();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (var raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursorType(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] SortRaycastAll()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithMovement()
        {
            Vector3 target;
            bool hasHit = RaycastNavmesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) { return false; }

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                SetCursorType(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavmesh(out Vector3 target)
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit)
            {
                target = new Vector3();
                return false;
            }

            NavMeshHit navMeshHit;
            var hasCastToNavMesh = NavMesh.SamplePosition(hit.point,
                                                          out navMeshHit,
                                                          maxNavMeshProjectionDistance,
                                                          NavMesh.AllAreas);
            if (!hasCastToNavMesh)
            {
                target = new Vector3();
                return false;
            }
            target = navMeshHit.position;

            return true;
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
                if (mapping.type == type)
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

