

public static class TypeChart
{
    public static float GetEffectiveness(
        Element attackType,
        Element defenderType)
    {
        // Triangle 1: Fire > Nature > Water > Fire
        if (attackType == Element.Fire &&
            defenderType == Element.Nature)
            return 2f;

        if (attackType == Element.Nature &&
            defenderType == Element.Water)
            return 2f;

        if (attackType == Element.Water &&
            defenderType == Element.Fire)
            return 2f;

        // Triangle 2: Ice > Metal > Thunder > Ice
        if (attackType == Element.Ice &&
            defenderType == Element.Metal)
            return 2f;

        if (attackType == Element.Metal &&
            defenderType == Element.Thunder)
            return 2f;

        if (attackType == Element.Thunder &&
            defenderType == Element.Ice)
            return 2f;

        // Triangle 3: Wind > Aether > Earth > Wind
        if (attackType == Element.Wind &&
            defenderType == Element.Aether)
            return 2f;

        if (attackType == Element.Aether &&
            defenderType == Element.Earth)
            return 2f;

        if (attackType == Element.Earth &&
            defenderType == Element.Wind)
            return 2f;

        // Reverse matchups are resisted.
        if (attackType == Element.Nature &&
            defenderType == Element.Fire)
            return 0.5f;

        if (attackType == Element.Water &&
            defenderType == Element.Nature)
            return 0.5f;

        if (attackType == Element.Fire &&
            defenderType == Element.Water)
            return 0.5f;

        if (attackType == Element.Metal &&
            defenderType == Element.Ice)
            return 0.5f;

        if (attackType == Element.Thunder &&
            defenderType == Element.Metal)
            return 0.5f;

        if (attackType == Element.Ice &&
            defenderType == Element.Thunder)
            return 0.5f;

        if (attackType == Element.Aether &&
            defenderType == Element.Wind)
            return 0.5f;

        if (attackType == Element.Earth &&
            defenderType == Element.Aether)
            return 0.5f;

        if (attackType == Element.Wind &&
            defenderType == Element.Earth)
            return 0.5f;

        //different triangles have no advantage over each other.
        return 1f;
    }
}
