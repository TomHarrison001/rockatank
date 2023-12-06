using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class R_BaseState : MonoBehaviour
{
    public float T;
    protected float t;
    public abstract void SetState(R_StateManager stateManager);
}
