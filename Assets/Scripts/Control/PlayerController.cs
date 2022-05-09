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
        private float maxNavPathLength = 40f;

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
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
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
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
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

            // calculate path and return false if too long
            NavMeshPath path = new NavMeshPath();
            //! path passed by ref and modified inside CalculatePath
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath) { return false; }
            if (path.status != NavMeshPathStatus.PathComplete) { return false; }  //! this disables unreachable zones paths

            if (GetPathLength(path) > maxNavPathLength) { return false; }

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            var total = 0f;
            if (path.corners.Length < 2) { return total; }
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
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

