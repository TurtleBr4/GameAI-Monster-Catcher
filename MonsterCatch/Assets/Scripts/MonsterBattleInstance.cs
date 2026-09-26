using UnityEngine;

public class MonsterBattleInstance : MonoBehaviour
{
    [Header("Data")]
    public Monster currentMonster;
    public string monsterName;
    public int currentHealth;
    public int maxHealth;
    public int baseDefense;
    public int baseSpeed;
    public int baseSpecial;
    public int baseAttack;
}
