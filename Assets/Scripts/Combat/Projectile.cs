using RPG.Attributes;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField]
        private UnityEvent onHit;

        private Health target;

        private float damage;

        private GameObject instigator;
        private Vector3 direction;
        private Vector3 previousPosition;

        private CapsuleCollider targetCollider;
        private CapsuleCollider selfCollider;

        private void Awake()
        {
            selfCollider = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            if (target is null) { return; }
            transform.LookAt(GetAimPosition());

            direction = (GetAimPosition() - transform.position).normalized;
            previousPosition = transform.position;

            HitIfShotFromInsideCollider();

            Destroy(gameObject, lifeTime);
        }

        private void Update()
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
            this.targetCollider = target.GetComponent<CapsuleCollider>();
            this.instigator = instigator;
            this.damage = damage;
        }

        private Vector3 GetAimPosition()
        {
            var position = target.transform.position;

            if (targetCollider is null) { return position; }

            return new Vector3(
                position.x,
                position.y + (targetCollider.height * .5f),
                position.z);
        }

        private void HitIfShotFromInsideCollider()
        {
            if (targetCollider.bounds.Contains(transform.position))
            {
                HitTarget();
            }
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

            HitTarget();
        }

        private void HitTarget()
        {
            target.TakeDamage(instigator, damage);
            onHit.Invoke();

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
            selfCollider.enabled = false;
            transform.parent = target.transform;
            target = null;
        }
    }
}