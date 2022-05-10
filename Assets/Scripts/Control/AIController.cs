using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;
using UnityEngine;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField]
        private float chaseDistance = 5f;
        [SerializeField]
        private float suspicionTime = 3f;
        [SerializeField]
        private float aggroCooldownTime = 3f;
        [SerializeField]
        private PatrolPath patrolPath;
        [SerializeField]
        private float waypointTolerance = 1f;
        [SerializeField]
        private float waypointDwellTime = 3f;
        [SerializeField]
        [Range(0, 1)]
        private float patrolSpeedFraction = .2f;
        [SerializeField]
        private float shoutDistance = 10f;

        private Fighter fighter;
        private GameObject player;
        private Health health;
        private Mover mover;

        private LazyValue<Vector3> guardPosition;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float timeSinceAggravated = Mathf.Infinity;

        private int currentWaypointIndex = 0;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {
            if (health.IsDead()) { return; }

            if (IsAggravated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer <= suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        // Called from enemy unityEvent onTakeDamage
        public void Aggravate()
        {
            timeSinceAggravated = 0f;
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void UpdateTimers()
        {
            var deltaTime = Time.deltaTime;
            timeSinceLastSawPlayer += deltaTime;
            timeSinceArrivedAtWaypoint += deltaTime;
            timeSinceAggravated += deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies()
        {
            //? Spherecast is the same as ray but shoots a sphere

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (var hit in hits)
            {
                print(hit.transform);
                var aiController = hit.transform.GetComponent<AIController>();
                if (aiController is null) { continue; }

                aiController.Aggravate();
            }
        }

        private bool IsAggravated()
        {
            var distance = Vector3.Distance(transform.position, player.transform.position);
            return
                distance <= chaseDistance ||
                timeSinceAggravated <= aggroCooldownTime;
        }

        //Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
            // UnityEditor.Handles // --> versione migliorata
        }
    }
}

