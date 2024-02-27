namespace AFSInterview
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.TestTools;

    public class Unit : MonoBehaviour
    {
        [Header("Basics")]
        [SerializeField] protected List<Attributes> attributes;
        [SerializeField] protected int hp = 10;
        [SerializeField] protected int armor = 2;
        [SerializeField] public int nbOfTeam = 1;

        [Header("Attack")]
        [SerializeField] protected int attackInterval = 1;
        [SerializeField] protected int attackDamage = 1;
        [SerializeField] protected int optionalAttackDamage = 1;
        [SerializeField] protected bool isRanged = false;

        [Header("Movement")]
        [SerializeField] protected float moveSpeed = 5f;

        private Unit targetUnit;
        private bool isMoving = false;

        public UnityEvent onTurnEnd = new UnityEvent();

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
                    Attack(nearestUnit);
                }
                else
                {
                    targetUnit = nearestUnit;
                    isMoving = true;
                }
            }
        }

        private void Attack(Unit target)
        {
            Debug.Log("<color=green>Attacking nearest unit! " + target + "</color>");
            StartCoroutine(WaitAndEndTurn(2f));
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
                Attack(target);
            }
        }

        private void Update()
        {
            if (isMoving)
                MoveToTarget(targetUnit);
        }
    }
}
