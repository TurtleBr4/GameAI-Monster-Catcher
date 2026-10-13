using UnityEngine;

public enum MoveCategory
{
    Physical,
    Special,
    Status
}

public enum Element
{
    Fire,
    Water,
    Nature,
    Ice,
    Metal,
    Thunder,
    Wind,
    Aether,
    Earth,
    None
}

[CreateAssetMenu(fileName = "New Move", menuName = "Battle/Move")]
public class MonsterMove : ScriptableObject
{
    [Header("Basic Info")]
    public string moveName;
    [TextArea] public string description;

    [Header("Combat")]
    public MoveCategory category;
    public Element type;
    public int power;
    public int accuracy = 100;
}
