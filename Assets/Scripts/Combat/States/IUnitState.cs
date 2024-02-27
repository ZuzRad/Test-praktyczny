using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AFSInterview
{
    public interface IUnitState
    {
        void Enter(Unit unit);
        void Exit(Unit unit);
    }
}
