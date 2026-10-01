using System;
using UnityEngine;

public class Party : MonoBehaviour
{
    [Header(("Basic Settings"))] [SerializeField]
    private int partySize;
    public static readonly int maxPartySize = 6;
    [SerializeField] private int activeSlots;
    
    [Header("Dynamic Info")]
    [SerializeField] private MonsterBattleInstance[] partyMonsters = new MonsterBattleInstance[maxPartySize];
    [SerializeField] private bool allDead = false;

    /// <summary>
    /// Call this function when the game transitions to a battle, once per party (so once from the player side and once for the enemy)
    /// </summary>
    /// <param name="monsters"></param>
    public void initializeParty(MonsterBattleInstance[] monsters) 
    {
        Array.Copy(monsters, partyMonsters, monsters.Length);
        partySize = monsters.Length;
    }
}
