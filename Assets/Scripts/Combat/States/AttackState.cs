
using UnityEngine;

namespace AFSInterview
{
    public class AttackState : IUnitState
    {
        public void Enter(Unit unit)
        {
            Debug.Log("<color=orange>" + unit.name + " state: Attack</color>");
            unit.FindUnitToAttack();
        }

        public void Exit(Unit unit)
        {
        }
    }
}
