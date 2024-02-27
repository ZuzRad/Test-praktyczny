using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AFSInterview
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        private Vector3 targetPosition;
        private bool isMoving = true;
        public void InitiateProjectile(Vector3 newTarget)
        {
            targetPosition = newTarget;
        }

        private void MoveToTarget()
        {
            if (Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                isMoving = false;
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (isMoving)
                MoveToTarget();
        }
    }
}
