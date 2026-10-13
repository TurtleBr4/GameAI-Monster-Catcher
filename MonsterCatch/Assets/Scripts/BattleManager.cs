using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Party player;
    [SerializeField] private Party enemy;

    public static BattleManager instance;

    public GameObject battleScene;
    public GameObject doubleScene;
    public GameObject singleScene;

    //active battle positions, not the entire party
    public MonsterInstance[] enemySlots = new MonsterInstance[2];
    public MonsterInstance[] playerSlots = new MonsterInstance[2];

    public bool isDoubleBattle = false;

    public event Action<MonsterInstance, bool> OnActionReady;

    [Header("Turn Timing")]
    [SerializeField] private float speedMultiplier = 100f;
    [SerializeField] private float minimumTurnTime = 0.25f;

    private class Combatant
    {
        public MonsterInstance monster;
        public bool isPlayer;
        public float timer;
        public bool ready;

        public Combatant(
            MonsterInstance monster,
            bool isPlayer,
            float timer)
        {
            this.monster = monster;
            this.isPlayer = isPlayer;
            this.timer = timer;
            ready = false;
        }
    }

    private readonly List<Combatant> combatants =
        new List<Combatant>();

    private readonly Queue<Combatant> readyQueue =
        new Queue<Combatant>();

    private Combatant currentCombatant;
    private bool battleActive;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (battleScene != null)
            battleScene.SetActive(false);

        if (singleScene != null)
            singleScene.SetActive(false);

        if (doubleScene != null)
            doubleScene.SetActive(false);
    }

    private void Update()
    {
        if (!battleActive)
            return;

        UpdateTimers();
        ProcessReadyQueue();
    }

    public void setEnemy(Party p)
    {
        enemy = p;
    }

    public bool isBattleReady()
    {
        if (player == null && GameManager.instance != null)
            player = GameManager.instance.playerParty;

        return player != null && enemy != null;
    }

    public void startBattle()
    {
        //always use the player's current party
        if (GameManager.instance != null)
            player = GameManager.instance.playerParty;

        if (player == null || enemy == null)
        {
            throw new UnityException(
                "Player or enemy party is missing.");
        }

        if (player.monsters == null || enemy.monsters == null)
        {
            throw new UnityException(
                "A party's monster list is missing.");
        }

        int playerLivingCount = CountLivingMonsters(player);
        int enemyLivingCount = CountLivingMonsters(enemy);

        if (playerLivingCount == 0 || enemyLivingCount == 0)
        {
            throw new UnityException(
                "Both parties must have at least one living monster.");
        }

        //automatically select single or double battle
        //both parties must have at least two living monsters
        isDoubleBattle =
            playerLivingCount >= 2 &&
            enemyLivingCount >= 2;

        combatants.Clear();
        readyQueue.Clear();
        currentCombatant = null;

        Array.Clear(playerSlots, 0, playerSlots.Length);
        Array.Clear(enemySlots, 0, enemySlots.Length);

        //fill active slots with living monsters
        int activeCount = isDoubleBattle ? 2 : 1;

        FillActiveSlots(player, playerSlots, activeCount);
        FillActiveSlots(enemy, enemySlots, activeCount);

        //register the active monsters for turn scheduling
        for (int i = 0; i < activeCount; i++)
        {
            if (playerSlots[i] == null || enemySlots[i] == null)
            {
                throw new UnityException(
                    "Failed to fill the active battle slots.");
            }

            AddCombatant(playerSlots[i], true);
            AddCombatant(enemySlots[i], false);
        }

        // Activate the battle scene and the correct layout.
        if (battleScene != null)
            battleScene.SetActive(true);

        if (singleScene != null)
            singleScene.SetActive(!isDoubleBattle);

        if (doubleScene != null)
            doubleScene.SetActive(isDoubleBattle);

        battleActive = true;

        Debug.Log(isDoubleBattle
            ? "Starting a double battle!"
            : "Starting a single battle!");
    }

    private int CountLivingMonsters(Party party)
    {
        if (party == null || party.monsters == null)
            return 0;

        int count = 0;

        foreach (MonsterInstance monster in party.monsters)
        {
            if (monster != null && monster.currentHealth > 0)
                count++;
        }

        return count;
    }

    private void FillActiveSlots(
        Party party,
        MonsterInstance[] slots,
        int activeCount)
    {
        int nextSlot = 0;

        foreach (MonsterInstance monster in party.monsters)
        {
            if (monster == null || monster.currentHealth <= 0)
                continue;

            slots[nextSlot] = monster;
            nextSlot++;

            if (nextSlot >= activeCount)
                break;
        }
    }

    private void AddCombatant(
        MonsterInstance monster, bool isPlayer)
    {
        float interval = GetTurnInterval(monster.baseSpeed);

        combatants.Add(
            new Combatant(monster, isPlayer, interval));
    }

    private float GetTurnInterval(int speed)
    {
        if (speed <= 0)
            return float.MaxValue;

        return Mathf.Max(
            minimumTurnTime,
            speedMultiplier / speed);
    }

    private void UpdateTimers()
    {
        foreach (Combatant combatant in combatants)
        {
            if (combatant.ready ||
                combatant.monster == null ||
                combatant.monster.currentHealth <= 0)
            {
                continue;
            }

            combatant.timer -= Time.deltaTime;

            if (combatant.timer <= 0f)
            {
                combatant.timer = 0f;
                combatant.ready = true;
                readyQueue.Enqueue(combatant);
            }
        }
    }

    private void ProcessReadyQueue()
    {
        if (currentCombatant != null)
            return;

        while (readyQueue.Count > 0)
        {
            Combatant next = readyQueue.Dequeue();

            if (next.monster == null ||
                next.monster.currentHealth <= 0)
            {
                continue;
            }

            currentCombatant = next;

            OnActionReady?.Invoke(
                currentCombatant.monster,
                currentCombatant.isPlayer);

            return;
        }
    }

    public void CompleteAction()
    {
        if (currentCombatant == null)
            return;

        if (!battleActive)
        {
            currentCombatant = null;
            return;
        }

        if (currentCombatant.monster != null &&
            currentCombatant.monster.currentHealth > 0)
        {
            currentCombatant.timer =
                GetTurnInterval(
                    currentCombatant.monster.baseSpeed);

            currentCombatant.ready = false;
        }

        currentCombatant = null;
    }

    public bool battleIsActive()
    {
        return battleActive;
    }

    public void CheckBattleEnd()
    {
        if (!HasLivingMonsters(player))
        {
            Debug.Log("Player has no remaining monsters!");
            endBattle();
        }
        else if (!HasLivingMonsters(enemy))
        {
            Debug.Log("Enemy has no remaining monsters!");
            endBattle();
        }
    }

    private bool HasLivingMonsters(Party party)
    {
        return CountLivingMonsters(party) > 0;
    }

    public void endBattle()
    {
        if (!battleActive)
            return;

        battleActive = false;

        //save the players final hp after the battle
        if (GameManager.instance != null)
        {
            GameManager.instance.SaveParty();
        }

        combatants.Clear();
        readyQueue.Clear();
        currentCombatant = null;

        if (battleScene != null)
            battleScene.SetActive(false);

        if (singleScene != null)
            singleScene.SetActive(false);

        if (doubleScene != null)
            doubleScene.SetActive(false);

        Array.Clear(playerSlots, 0, playerSlots.Length);
        Array.Clear(enemySlots, 0, enemySlots.Length);

        Debug.Log("Battle ended. Player party saved.");
    }
}