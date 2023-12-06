using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_StateManager : MonoBehaviour
{
    public enum State { Search, Attack, Chase }
    public State state;
    private float t = 0f;

    private void Start()
    {
        SetStateToSearch();
    }

    private void Update()
    {
        
    }

    public void SetStateToSearch()
    {
        if (state != State.Search)
        {
            StartCoroutine(SearchSet());
        }
    }

    public void SetStateToAttack()
    {
        if (state != State.Attack)
        {
            StartCoroutine(AttackSet());
        }
    }

    public void SetStateToChase()
    {
        if (state != State.Chase)
        {
            StartCoroutine(ChaseSet());
        }
    }

    private IEnumerator SearchSet()
    {
        yield return new WaitForSeconds(t);
        state = State.Search;
    }

    private IEnumerator AttackSet()
    {
        yield return new WaitForSeconds(t);
        state = State.Attack;
    }

    private IEnumerator ChaseSet()
    {
        yield return new WaitForSeconds(t);
        state = State.Chase;
    }
}
