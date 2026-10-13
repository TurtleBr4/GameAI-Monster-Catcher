
using UnityEngine;

public class OverworldBattleTrigger : MonoBehaviour
{
    [Header("Encounter")]
    [SerializeField] private Monster[] enemySpecies;
    [SerializeField] private bool triggerOnce = true;

    [SerializeField]private bool hasTriggered;
    [Header("Enemy AI")]
    [SerializeField] private EnemyAILevel aiLevel =
        EnemyAILevel.BasicWildMonster;
    
    public bool debugFlag = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (debugFlag && GameManager.instance.playerParty.monsters.Count == 0)
        {
            GameManager.instance.AddMonsterToPlayerParty(enemySpecies[0]);
        }

        if (enemySpecies == null || enemySpecies.Length == 0)
        {
            Debug.LogError("No enemy species assigned to encounter.");
            return;
        }

        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager is missing.");
            return;
        }

        //create a new enemy party for this encounter.
        Party enemyParty = new Party();

        foreach (Monster species in enemySpecies)
        {
            if (species == null)
                continue;

            MonsterInstance monster =
                GameManager.instance.CreateMonster(species);

            if (!enemyParty.AddMonster(monster))
            {
                Debug.LogWarning(
                    "Could not add enemy. The party may be full.");
                break;
            }
        }

        if (enemyParty.monsters.Count == 0)
        {
            Debug.LogError("Encounter has no valid enemies.");
            return;
        }

        //give the enemy party to the battle system.
        GameManager.instance.takeInEnemyBattleInformation(
            enemyParty);
        
        GameManager.instance.SetBattleAILevel(aiLevel);

        //start the battle.
        GameManager.instance.toggleState(GameState.Battle);

        hasTriggered = triggerOnce;
    }
}
