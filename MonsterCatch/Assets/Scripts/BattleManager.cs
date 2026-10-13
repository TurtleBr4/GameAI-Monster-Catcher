
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

    // Active battle positions, not the entire party.
    public MonsterInstance[] enemySlots = new MonsterInstance[2];
    public MonsterInstance[] playerSlots = new MonsterInstance[2];

    public bool isDoubleBattle = false;

    public event Action<MonsterInstance, bool> OnActionReady;
    public event Action OnBattleStarted;
    public event Action OnBattleStateChanged;

    [Header("Turn Timing")]
    [SerializeField] private float speedMultiplier = 10f;
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

    private bool battleActive;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
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
    
    public void NotifyBattleStateChanged()
    {
        OnBattleStateChanged?.Invoke();
    }
    public void startBattle()
    {
        if (GameManager.instance != null)
            player = GameManager.instance.playerParty;

        if (player == null || enemy == null)
            throw new UnityException(
                "Player or enemy party is missing.");

        if (player.monsters == null || enemy.monsters == null)
            throw new UnityException(
                "A party's monster list is missing.");

        int playerLivingCount = CountLivingMonsters(player);
        int enemyLivingCount = CountLivingMonsters(enemy);

        if (playerLivingCount == 0 || enemyLivingCount == 0)
            throw new UnityException(
                "Both parties must have at least one living monster.");

        isDoubleBattle =
            playerLivingCount >= 2 &&
            enemyLivingCount >= 2;

        combatants.Clear();
        readyQueue.Clear();

        Array.Clear(playerSlots, 0, playerSlots.Length);
        Array.Clear(enemySlots, 0, enemySlots.Length);

        int activeCount = isDoubleBattle ? 2 : 1;

        FillActiveSlots(player, playerSlots, activeCount);
        FillActiveSlots(enemy, enemySlots, activeCount);

        for (int i = 0; i < activeCount; i++)
        {
            if (playerSlots[i] == null || enemySlots[i] == null)
                throw new UnityException(
                    "Failed to fill the active battle slots.");

            AddCombatant(playerSlots[i], true);
            AddCombatant(enemySlots[i], false);
        }

        if (battleScene != null)
            battleScene.SetActive(true);

        if (singleScene != null)
            singleScene.SetActive(!isDoubleBattle);

        if (doubleScene != null)
            doubleScene.SetActive(isDoubleBattle);

        battleActive = true;

        OnBattleStarted?.Invoke();
        OnBattleStateChanged?.Invoke();

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
        MonsterInstance monster,
        bool isPlayer)
    {
        float interval = GetTurnInterval(monster.baseSpeed);

        // Begin at half charge so the opening actions happen sooner.
        float initialTimer = interval * 0.5f;

        combatants.Add(
            new Combatant(monster, isPlayer, initialTimer));
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
        // Do not globally lock actions while a player is selecting a move.
        while (readyQueue.Count > 0 && battleActive)
        {
            Combatant next = readyQueue.Dequeue();

            if (next.monster == null ||
                next.monster.currentHealth <= 0)
            {
                next.ready = false;
                continue;
            }

            OnActionReady?.Invoke(
                next.monster,
                next.isPlayer);
        }
    }

    public void CompleteAction(MonsterInstance monster)
    {
        if (monster == null || !battleActive)
            return;

        foreach (Combatant combatant in combatants)
        {
            if (combatant.monster != monster)
                continue;

            if (monster.currentHealth > 0)
            {
                combatant.timer =
                    GetTurnInterval(monster.baseSpeed);

                combatant.ready = false;
            }

            return;
        }
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
            return;
        }

        if (!HasLivingMonsters(enemy))
        {
            Debug.Log("Enemy has no remaining monsters!");
            endBattle();
            return;
        }

        ReplaceFaintedMonsters();
    }

    private bool HasLivingMonsters(Party party)
    {
        return CountLivingMonsters(party) > 0;
    }

    private bool ReplaceFaintedMonsters()
    {
        bool replacedAny = false;
        int activeCount = isDoubleBattle ? 2 : 1;

        for (int i = 0; i < activeCount; i++)
        {
            if (playerSlots[i] == null ||
                playerSlots[i].currentHealth <= 0)
            {
                playerSlots[i] =
                    FindNextLivingMonster(player, playerSlots);

                if (playerSlots[i] != null)
                {
                    Debug.Log(
                        "Player sent out " +
                        playerSlots[i].monsterName + "!");

                    AddCombatantIfMissing(playerSlots[i], true);
                    replacedAny = true;
                }
            }

            if (enemySlots[i] == null ||
                enemySlots[i].currentHealth <= 0)
            {
                enemySlots[i] =
                    FindNextLivingMonster(enemy, enemySlots);

                if (enemySlots[i] != null)
                {
                    Debug.Log(
                        "Enemy sent out " +
                        enemySlots[i].monsterName + "!");

                    AddCombatantIfMissing(enemySlots[i], false);
                    replacedAny = true;
                }
            }
        }

        if (replacedAny)
            OnBattleStateChanged?.Invoke();

        return replacedAny;
    }

    private MonsterInstance FindNextLivingMonster(
        Party party,
        MonsterInstance[] activeSlots)
    {
        foreach (MonsterInstance monster in party.monsters)
        {
            if (monster == null || monster.currentHealth <= 0)
                continue;

            bool alreadyActive = false;

            foreach (MonsterInstance active in activeSlots)
            {
                if (active == monster)
                {
                    alreadyActive = true;
                    break;
                }
            }

            if (!alreadyActive)
                return monster;
        }

        return null;
    }

    private void AddCombatantIfMissing(
        MonsterInstance monster,
        bool isPlayer)
    {
        foreach (Combatant combatant in combatants)
        {
            if (combatant.monster == monster &&
                combatant.isPlayer == isPlayer)
            {
                // A previously registered monster may have fainted
                // and later been revived. Ensure its timer can run.
                if (monster.currentHealth > 0 && combatant.ready)
                {
                    combatant.ready = false;
                    combatant.timer =
                        GetTurnInterval(monster.baseSpeed);
                }

                return;
            }
        }

        AddCombatant(monster, isPlayer);
    }

    public void endBattle()
    {
        if (!battleActive)
            return;

        battleActive = false;

        if (GameManager.instance != null)
            GameManager.instance.SaveParty();

        combatants.Clear();
        readyQueue.Clear();

        if (battleScene != null)
            battleScene.SetActive(false);

        if (singleScene != null)
            singleScene.SetActive(false);

        if (doubleScene != null)
            doubleScene.SetActive(false);

        Array.Clear(playerSlots, 0, playerSlots.Length);
        Array.Clear(enemySlots, 0, enemySlots.Length);

        OnBattleStateChanged?.Invoke();

        Debug.Log("Battle ended. Player party saved.");
    }
}
