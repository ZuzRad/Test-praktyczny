
using UnityEngine;

namespace AFSInterview
{
    public class WaitState : IUnitState
    {
        public void Enter(Unit unit)
        {
            Debug.Log("<color=orange>" + unit.name + " state: Wait</color>");
        }

        public void Exit(Unit unit)
        {
            Debug.Log("<color=orange>Turn " + unit.name + "</color>");
        }
    }
}
