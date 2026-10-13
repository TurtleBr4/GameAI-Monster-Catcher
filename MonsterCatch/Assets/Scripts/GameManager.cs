using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum GameState
{
    Overworld,
    Battle,
    Dialog
}

public class GameManager : MonoBehaviour
{
    public const float controllerDeadzone = 0.2f;

    public static GameManager instance;

    [SerializeField] private GameState gameState = GameState.Overworld;

    public Party playerParty = new Party();
    public BattleManager battleManager;

    [Header("Save System")]
    [SerializeField] private Monster[] monsterDatabase;

    private string SavePath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                "player_party.json");
        }
    }

    public GameState CurrentGameState
    {
        get { return gameState; }
    }

    [Serializable]
    private class PartySaveData
    {
        public List<MonsterSaveData> monsters =
            new List<MonsterSaveData>();
    }

    [Serializable]
    private class MonsterSaveData
    {
        public string speciesId;
        public int currentHealth;
    }

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
            return;
        }

        if (playerParty == null)
        {
            playerParty = new Party();
        }
    }

    private void Start()
    {
        if (battleManager == null)
        {
            battleManager = BattleManager.instance;
        }

        if (battleManager == null)
        {
            Debug.LogError("BattleManager is missing.");
        }

        LoadParty();
    }

    public MonsterInstance CreateMonster(Monster species)
    {
        if (species == null)
        {
            Debug.LogError("Cannot create a monster from a null species.");
            return null;
        }

        return new MonsterInstance(species);
    }

    public bool AddMonsterToPlayerParty(Monster species)
    {
        if (playerParty == null)
        {
            playerParty = new Party();
        }

        MonsterInstance newMonster = CreateMonster(species);

        if (newMonster == null)
        {
            return false;
        }

        if (!playerParty.AddMonster(newMonster))
        {
            Debug.LogWarning(
                "Could not add monster. The party may be full.");

            return false;
        }

        SaveParty();

        Debug.Log(
            "Added " + newMonster.monsterName +
            " to the player's party.");

        return true;
    }

    public void SaveParty()
    {
        if (playerParty == null || playerParty.monsters == null)
        {
            Debug.LogError("Cannot save: player party is missing.");
            return;
        }

        PartySaveData saveData = new PartySaveData();

        foreach (MonsterInstance monster in playerParty.monsters)
        {
            if (monster == null || monster.species == null)
                continue;

            if (string.IsNullOrWhiteSpace(monster.species.speciesId))
            {
                Debug.LogError(
                    "Cannot save " + monster.monsterName +
                    ": its species has no species ID.");

                continue;
            }

            MonsterSaveData monsterData = new MonsterSaveData();

            monsterData.speciesId = monster.species.speciesId;
            monsterData.currentHealth = monster.currentHealth;

            saveData.monsters.Add(monsterData);
        }

        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SavePath, json);

            Debug.Log("Party saved to: " + SavePath);
        }
        catch (Exception exception)
        {
            Debug.LogError("Failed to save party: " + exception.Message);
        }
    }

    public bool LoadParty()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No party save found. Keeping the current party.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            PartySaveData saveData =
                JsonUtility.FromJson<PartySaveData>(json);

            if (saveData == null || saveData.monsters == null)
            {
                Debug.LogError("Party save data is invalid.");
                return false;
            }

            Party loadedParty = new Party();

            foreach (MonsterSaveData monsterData in saveData.monsters)
            {
                if (loadedParty.monsters.Count >= Party.MaxPartySize)
                    break;

                if (monsterData == null ||
                    string.IsNullOrWhiteSpace(monsterData.speciesId))
                {
                    continue;
                }

                Monster species = FindSpecies(monsterData.speciesId);

                if (species == null)
                {
                    Debug.LogWarning(
                        "Could not find monster species with ID: " +
                        monsterData.speciesId);

                    continue;
                }

                MonsterInstance monster = CreateMonster(species);

                if (monster == null)
                    continue;

                monster.currentHealth = Mathf.Clamp(
                    monsterData.currentHealth,
                    0,
                    monster.maxHealth);

                loadedParty.AddMonster(monster);
            }

            playerParty = loadedParty;

            Debug.Log(
                "Party loaded. Monster count: " +
                playerParty.monsters.Count);

            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError("Failed to load party: " + exception.Message);
            return false;
        }
    }

    private Monster FindSpecies(string speciesId)
    {
        if (monsterDatabase == null)
            return null;

        foreach (Monster species in monsterDatabase)
        {
            if (species != null && species.speciesId == speciesId)
            {
                return species;
            }
        }

        return null;
    }

    public void takeInEnemyBattleInformation(Party p)
    {
        if (p == null)
        {
            Debug.LogError("Enemy party is null.");
            return;
        }

        if (battleManager == null)
        {
            battleManager = BattleManager.instance;
        }

        if (battleManager == null)
        {
            Debug.LogError("BattleManager is missing.");
            return;
        }

        battleManager.setEnemy(p);
    }

    public void toggleState(GameState state)
    {
        if (state == gameState)
        {
            Debug.LogWarning("That game state is already active.");
            return;
        }

        if (state == GameState.Battle)
        {
            if (battleManager == null)
            {
                battleManager = BattleManager.instance;
            }

            if (battleManager == null || !battleManager.isBattleReady())
            {
                Debug.LogError("BattleManager is not ready.");
                return;
            }

            battleManager.startBattle();
        }

        gameState = state;
    }

    private void OnApplicationQuit()
    {
        if (instance == this)
        {
            SaveParty();
        }
    }

    public void HealMonster(MonsterInstance monster)
    {
        if (monster == null)
        {
            Debug.LogWarning("Cannot heal a null monster.");
            return;
        }

        monster.HealToFull();
        SaveParty();

        Debug.Log(monster.monsterName + " was fully healed!");
    }

    public void HealParty()
    {
        if (playerParty == null || playerParty.monsters == null)
        {
            Debug.LogError("Cannot heal party: player party is missing.");
            return;
        }

        foreach (MonsterInstance monster in playerParty.monsters)
        {
            if (monster == null)
                continue;

            monster.HealToFull();
        }

        SaveParty();

        Debug.Log("The player's party has been fully healed!");
    }
}