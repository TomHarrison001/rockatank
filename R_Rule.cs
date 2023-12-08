using System;
using System.Collections.Generic;

public class R_Rule
{
    public string atecedentA, atecedentB;
    public Type consequentState;
    public enum Predicate { AND, OR, NAND };
    public Predicate compare;

    public R_Rule(string atecedentA, string atecedentB, Type consequentState, Predicate compare)
    {
        this.atecedentA = atecedentA;
        this.atecedentB = atecedentB;
        this.consequentState = consequentState;
        this.compare = compare;
    }

    public Type CheckRule(Dictionary<string, bool> stats)
    {
        bool atecedentABool = stats[atecedentA];
        bool atecedentBBool = stats[atecedentB];

        switch (compare)
        {
            case Predicate.AND:
                return (atecedentABool && atecedentBBool) ? consequentState : null;
            case Predicate.OR:
                return (atecedentABool || atecedentBBool) ? consequentState : null;
            case Predicate.NAND:
                return (!atecedentABool && !atecedentBBool) ? consequentState : null;
            default:
                return null;
        }
    }
}
