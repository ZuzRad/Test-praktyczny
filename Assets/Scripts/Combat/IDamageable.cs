using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFSInterview
{
    public interface IDamageable
    {
        void TakeDamage(float damage);
        void ApplyDamage(Unit target, float damage);

        void Die();
    }
}
