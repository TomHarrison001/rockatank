using System;
using System.Collections.Generic;
using UnityEngine;

public class R_Rules
{
    public void AddRule(R_Rule rule)
    {
        GetRules.Add(rule);
    }

    public List<R_Rule> GetRules { get; } = new List<R_Rule>();
}
