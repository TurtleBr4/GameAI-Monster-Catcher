
using UnityEngine;

public class OverworldBattleTrigger : MonoBehaviour
{
    [Header("Encounter")]
    [SerializeField] private Monster[] enemySpecies;
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

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

        // Create a new enemy party for this encounter.
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

        // Give the enemy party to the battle system.
        GameManager.instance.takeInEnemyBattleInformation(
            enemyParty);

        // Start the battle.
        GameManager.instance.toggleState(GameState.Battle);

        hasTriggered = triggerOnce;
    }
}
