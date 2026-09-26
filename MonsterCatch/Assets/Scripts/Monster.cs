using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Scriptable Objects/Monster")]
public class Monster : ScriptableObject
{
    [Header("Basic Info")]
    public string Name;
    public Sprite Image;

    public int baseHealth;
    public int baseAttack;
    public int baseDefense;
    public int baseSpeed;
    public int baseSpecial;
}
