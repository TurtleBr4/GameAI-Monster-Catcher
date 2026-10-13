using UnityEngine;

[System.Serializable]
public class MonsterInstance
{
    public const int MaxMoves = 4;

    public Monster species;
    public string monsterName;

    [Header("Stats")]
    public int currentHealth;
    public int maxHealth;
    public int baseAttack;
    public int baseDefense;
    public int baseSpeed;
    public int baseSpecial;
    [Header("Types")]
    public Element primaryType;
    public bool hasSecondaryType;
    public Element secondaryType;

    public MonsterMove[] moves = new MonsterMove[MaxMoves];

    public MonsterInstance(Monster monster)
    {
        if (monster == null)
        {
            throw new UnityException("Cannot create an instance from a null Monster.");
        }

        species = monster;
        monsterName = monster.Name;

        maxHealth = Mathf.Max(1, monster.baseHealth);
        currentHealth = maxHealth;

        baseAttack = monster.baseAttack;
        baseDefense = monster.baseDefense;
        baseSpeed = monster.baseSpeed;
        baseSpecial = monster.baseSpecial;

        moves = new MonsterMove[MaxMoves];

        if (monster.startingMoves == null)
        {
            return;
        }

        int moveCount = Mathf.Min(
            monster.startingMoves.Length,
            MaxMoves);

        for (int i = 0; i < moveCount; i++)
        {
            moves[i] = monster.startingMoves[i];
        }
    }

    public bool IsFainted()
    {
        return currentHealth <= 0;
    }

    public void HealToFull()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
    }
}