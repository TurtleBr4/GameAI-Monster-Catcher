using System.Collections.Generic;

[System.Serializable]
public class Party
{
    public const int MaxPartySize = 6;

    public List<MonsterInstance> monsters =
        new List<MonsterInstance>();

    public bool AddMonster(MonsterInstance monster)
    {
        if (monster == null)
            return false;

        if (monsters.Count >= MaxPartySize)
            return false;

        monsters.Add(monster);
        return true;
    }

    public MonsterInstance GetMonster(int index)
    {
        if (index < 0 || index >= monsters.Count)
            return null;

        return monsters[index];
    }

    public bool HasLivingMonsters()
    {
        foreach (MonsterInstance monster in monsters)
        {
            if (monster != null && monster.currentHealth > 0)
                return true;
        }

        return false;
    }
}