using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1f;
        [SerializeField]
        private float lifeTime = 1f;
        [SerializeField]
        private bool shouldChaseTarget = false;
        [SerializeField]
        private GameObject hitEffect;

        private Health target;

        private float damage;

        private GameObject instigator;
        private Vector3 direction;
        private Vector3 previousPosition;

        private void Start()
        {
            if (target is null) { return; }
            transform.LookAt(GetAimPosition());

            direction = (GetAimPosition() - transform.position).normalized;
            previousPosition = transform.position;

            Destroy(gameObject, lifeTime);
        }

        private void FixedUpdate()
        {
            if (target is null) { return; }
            if (shouldChaseTarget && !target.IsDead())
                transform.LookAt(GetAimPosition());

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            
            CheckHit();
            
            previousPosition = transform.position;
        }


        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.instigator = instigator;
            this.damage = damage;
        }

        private Vector3 GetAimPosition()
        {
            var targetCapsule = target.GetComponent<CapsuleCollider>();
            var position = target.transform.position;

            if (targetCapsule is null) { return position; }

            return new Vector3(
                position.x,
                position.y + (targetCapsule.height * .5f),
                position.z);
        }

        private void CheckHit()
        {
            RaycastHit hit;
            if (Physics.Raycast(previousPosition, direction, out hit, Vector3.Distance(transform.position, previousPosition)))
            {
                CheckTargetValidity(hit);
            }
        }

        private void CheckTargetValidity(RaycastHit hit)
        {
            if (hit.collider.GetComponent<Health>() != target) { return; }

            transform.position = hit.point;

            target.TakeDamage(instigator, damage);

            DestroyHead();

            if (hitEffect != null)
                Instantiate(hitEffect, transform.position, Quaternion.identity);

            foreach (var part in GetComponentsInChildren<ParticleSystem>())
                part.Stop();

            StopProjectile();
        }

        private void DestroyHead()
        {
            var head = GetComponentInChildren<DestroyOnImpact>();
            if (head != null)
                Destroy(head.gameObject);
        }

        private void StopProjectile()
        {
            GetComponent<CapsuleCollider>().enabled = false;
            transform.parent = target.transform;
            target = null;

        }
    }
}