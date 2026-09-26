using UnityEngine;

public class Party : MonoBehaviour
{
    [Header(("Basic Settings"))] [SerializeField]
    private int partySize;
    public static readonly int maxPartySize = 6;
    
    [Header("Dynamic Info")]
    [SerializeField] private MonsterBattleInstance[] partyMonsters = new MonsterBattleInstance[maxPartySize];
    [SerializeField] private bool allDead = false;
}
