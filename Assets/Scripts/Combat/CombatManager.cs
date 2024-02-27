using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace AFSInterview
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private List<Unit> army1;
        [SerializeField] private List<Unit> army2;

        private List<Unit> allUnits = new();
        private Queue<Unit> turnOrder;
        private int currentTurnIndex = 0;
        private Unit currentUnit;
        public static CombatManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }
        void Start()
        {
            allUnits.AddRange(army1);
            allUnits.AddRange(army2);
            ShuffleTurnOrder();

            ExecuteNextTurn();
        }

        private void ShuffleTurnOrder()
        {
            turnOrder = new Queue<Unit>(allUnits.OrderBy(x => Random.value));
        }

        public void ExecuteNextTurn()
        {
            if (currentTurnIndex < allUnits.Count)
            {
                if(currentUnit)
                    currentUnit.onTurnEnd.RemoveListener(ExecuteNextTurn);

                currentUnit = turnOrder.Dequeue();

                Debug.Log("<color=yellow>TURN " + (currentTurnIndex+1) + " now unit " + currentUnit + "</color>");


                currentUnit.onTurnEnd.AddListener(ExecuteNextTurn);

                currentUnit.ExecuteTurn();
                turnOrder.Enqueue(currentUnit);
                currentTurnIndex++;
            }
            else
            {
                currentTurnIndex = 0;
                ExecuteNextTurn();
            }
        }

        public void RemoveUnitFromTurnOrder(Unit unit)
        {
            if (turnOrder.Contains(unit))
            {
                turnOrder = new Queue<Unit>(turnOrder.Where(x => x != unit));
                allUnits.Remove(unit);
            }
        }
    }
}
