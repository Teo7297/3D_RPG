using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField]
        private float maxSpeed = 5.66f;

        private NavMeshAgent navMeshAgent;
        private Health health;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            //We cancel the attack when we start moving
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            //get velocity from navMeshAgent (we get the global velocity!)
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;

            //convert velocity from global to local (we just need to know how fast we are moving, the global value may be influenced by other things!)
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            //Get the speed on the z axis
            float speed = localVelocity.z;

            //Pass the value to the animator
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }
    }
}
