using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class R_StateMachine : MonoBehaviour
{
    private Dictionary<Type, R_BaseState> states;
    public R_BaseState currentState;
    public R_BaseState CurrentState
    {
        get
        {
            return currentState;
        }
        private set
        {
            currentState = value;
        }
    }

    public void SetStates(Dictionary<Type, R_BaseState> states)
    {
        this.states = states;
    }

    private void Update()
    {
        if (CurrentState == null)
        {
            CurrentState = states.Values.First();
        }
        else
        {
            var nextState = CurrentState.StateUpdate();
            if (nextState != null && nextState != CurrentState.GetType())
            {
                SwitchToState(nextState);
            }
        }
    }

    private void SwitchToState(Type nextState)
    {
        CurrentState.StateExit();
        CurrentState = states[nextState];
        CurrentState.StateEnter();
    }
}
