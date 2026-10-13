
using UnityEngine;

public class BattleActionResolver : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;

    private void Awake()
    {
        if (battleManager == null)
            battleManager = BattleManager.instance;
    }

    public void ResolveMove(
        MonsterInstance attacker,
        MonsterMove move,
        bool isPlayer)
    {
        if (battleManager == null)
        {
            Debug.LogError("BattleManager is missing.");
            return;
        }

        if (attacker == null || move == null)
        {
            Debug.LogError("Attacker or move is missing.");
            battleManager.CompleteAction();
            return;
        }

        MonsterInstance target = GetTarget(isPlayer);

        if (target == null)
        {
            Debug.Log("No valid targets remain.");
            battleManager.CheckBattleEnd();
            battleManager.CompleteAction();
            return;
        }

        Debug.Log(attacker.monsterName + " used " +
                  move.moveName + "!");

        if (Random.Range(1, 101) > move.accuracy)
        {
            Debug.Log(move.moveName + " missed!");
            FinishAction();
            return;
        }

        if (move.category == MoveCategory.Status)
        {
            Debug.Log("Status effects are not implemented yet.");
            FinishAction();
            return;
        }

        int attackStat;
        int defenseStat;

        if (move.category == MoveCategory.Physical)
        {
            attackStat = attacker.baseAttack;
            defenseStat = target.baseDefense;
        }
        else
        {
            // Special is used for both special attack and defense.
            attackStat = attacker.baseSpecial;
            defenseStat = target.baseSpecial;
        }

        int damage = Mathf.Max(
            1,
            move.power + attackStat / 2 - defenseStat / 2);

        target.currentHealth = Mathf.Max(
            0,
            target.currentHealth - damage);

        Debug.Log(target.monsterName + " took " +
                  damage + " damage. HP: " +
                  target.currentHealth + "/" +
                  target.maxHealth);

        if (target.currentHealth <= 0)
        {
            Debug.Log(target.monsterName + " fainted!");
        }

        FinishAction();
    }

    private MonsterInstance GetTarget(bool isPlayer)
    {
        MonsterInstance[] targets = isPlayer
            ? battleManager.enemySlots
            : battleManager.playerSlots;

        int slotCount = battleManager.isDoubleBattle ? 2 : 1;

        for (int i = 0; i < slotCount; i++)
        {
            if (targets[i] != null &&
                targets[i].currentHealth > 0)
            {
                return targets[i];
            }
        }

        return null;
    }

    private void FinishAction()
    {
        battleManager.CheckBattleEnd();

        if (battleManager.battleIsActive())
            battleManager.CompleteAction();
    }
}
