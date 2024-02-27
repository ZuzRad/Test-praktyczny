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
        [SerializeField] private Attributes strongAgainst;
        [SerializeField] private float optionalAttackDamage = 2;
        [SerializeField] private GameObject projectilePrefab;

        [Header("Movement")]
        [SerializeField] private bool isRanged = false;
        [SerializeField] private float moveSpeed = 5f;

        private Unit targetUnit;
        private bool isMoving = false;
        private int currentAttackInterval;
        private IUnitState currentState;

        public UnityEvent onTurnEnd = new();

        private void Awake()
        {
            currentAttackInterval = 1;
        }

        public void ChangeState(IUnitState newState)
        {
            if (currentState != null)
            {
                currentState.Exit(this);
            }

            currentState = newState;
            currentState.Enter(this);
        }

        public bool CheckIfAttributeInList(Attributes attributeToFind) 
        {
            return attributes.Contains(attributeToFind);
        }
        public void ExecuteTurn()
        {
            if (currentAttackInterval == 1)
            {
                ChangeState(new AttackState());
                currentAttackInterval = attackInterval;
            }
            else
            {
                StartCoroutine(WaitAndEndTurn());
                currentAttackInterval--;
            }
        }
        public void FindUnitToAttack()
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

        private IEnumerator WaitAndEndTurn() 
        {
            ChangeState(new WaitState());
            yield return new WaitForSeconds(3f);
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
                Debug.Log("<color=green>" + this.name + " died</color>");
                Die();
            }
            else
            {
                Debug.Log("<color=green>" + this.name + " taking damage: " + finalDamage + "</color>");
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
            Debug.Log("<color=green>Attacking nearest unit: " + target.name + "</color>");

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

            StartCoroutine(WaitAndEndTurn());
        }
    }
}
