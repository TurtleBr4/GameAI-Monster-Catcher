using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Battle/Monster")]
public class Monster : ScriptableObject
{
    [Header("Identity")]
    public string speciesId;
    public string Name;
    public Sprite image;
    [Header("Types")]
    public Element primaryType;
    public bool hasSecondaryType;
    public Element secondaryType;
    [Header("Base Stats")]
    public int baseHealth = 20;
    public int baseAttack = 10;
    public int baseDefense = 10;
    public int baseSpeed = 10;
    public int baseSpecial = 10;

    [Header("Starting Moves")]
    public MonsterMove[] startingMoves = new MonsterMove[4];
}