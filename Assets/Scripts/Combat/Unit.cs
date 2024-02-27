namespace AFSInterview
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class Unit : MonoBehaviour, IDamageable
    {
        [Header("Basics")]
        [SerializeField] private List<Attributes> attributes;
        [SerializeField] private float healthPoints = 10;
        [SerializeField] private float armor = 2;
        [SerializeField] public int nbOfTeam = 1;

        [Header("Attack")]
        [SerializeField] private int attackInterval = 1;
        [SerializeField] private float attackDamage = 1;
        [SerializeField] private bool isRanged = false;
        [SerializeField] private Attributes strongAgainst;
        [SerializeField] private float optionalAttackDamage = 2;
        [SerializeField] private GameObject projectilePrefab;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;

        private Unit targetUnit;
        private bool isMoving = false;
        private int currentAttackInterval;

        public UnityEvent onTurnEnd = new();

        private void Start()
        {
            currentAttackInterval = 1;
        }

        public bool CheckIfAttributeInList(Attributes attributeToFind) 
        {
            return attributes.Contains(attributeToFind);
        }
        public void ExecuteTurn()
        {
            if (currentAttackInterval == 1)
            {
                FindUnitToAttack();
                currentAttackInterval = attackInterval;
            }
            else
            {
                StartCoroutine(WaitAndEndTurn(2f));
                currentAttackInterval--;
            }
        }
        private void FindUnitToAttack()
        {
            Unit[] allUnits = FindObjectsOfType<Unit>();

            Unit nearestUnit = null;
            float nearestDistance = float.MaxValue;

            foreach (Unit unit in allUnits)
            {
                if (unit.nbOfTeam != nbOfTeam)
                {
                    float distance = Vector3.Distance(transform.position, unit.transform.position);

                    if (distance < nearestDistance)
                    {
                        nearestUnit = unit;
                        nearestDistance = distance;
                    }
                }
            }

            if (nearestUnit != null)
            {
                if (isRanged)
                {
                    ApplyDamage(nearestUnit, attackDamage);
                }
                else
                {
                    targetUnit = nearestUnit;
                    isMoving = true;
                }
            }
        }

        private IEnumerator WaitAndEndTurn(float time) 
        {
            yield return new WaitForSeconds(time);
            onTurnEnd.Invoke();
        }

        private void MoveToTarget(Unit target)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > 1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                isMoving = false;
                ApplyDamage(target, attackDamage);
            }
        }

        private void Update()
        {
            if (isMoving)
                MoveToTarget(targetUnit);
        }

        public void TakeDamage(float damage)
        {
            float finalDamage = Mathf.Max(damage - armor, 1);
            healthPoints -= finalDamage;

            if (healthPoints <= 0)
            {
                Die();
            }
            else
            {
                Debug.Log("<color=green>Taking damage: " + finalDamage + "</color>");
            }
        }

        public void Die()
        {
            onTurnEnd.RemoveAllListeners();
            CombatManager.Instance.RemoveUnitFromTurnOrder(this);
            Destroy(gameObject);
        }

        public void ApplyDamage(Unit target,float damage)
        {
            Debug.Log("<color=green>Attacking nearest unit! " + target + "</color>");

            if (target.CheckIfAttributeInList(strongAgainst))
            {
                target.TakeDamage(optionalAttackDamage);
            }
            else
            {
                target.TakeDamage(damage);
            }

            if (isRanged)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                
                if (projectile.TryGetComponent<Projectile>(out var projectileComponent))
                {
                    projectileComponent.InitiateProjectile(target.transform.position);
                }
            }

            StartCoroutine(WaitAndEndTurn(2f));
        }
    }
}
