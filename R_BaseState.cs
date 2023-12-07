using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class R_BaseState : MonoBehaviour
{
    public abstract Type StateUpdate();
    public abstract Type StateEnter();
    public abstract Type StateExit();
}
