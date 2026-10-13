using UnityEngine;

public enum MoveCategory
{
    Physical,
    Special,
    Status
}

[CreateAssetMenu(fileName = "New Move", menuName = "Battle/Move")]
public class MonsterMove : ScriptableObject
{
    [Header("Basic Info")]
    public string moveName;
    [TextArea] public string description;

    [Header("Combat")]
    public MoveCategory category;
    public int power;
    public int accuracy = 100;
}
