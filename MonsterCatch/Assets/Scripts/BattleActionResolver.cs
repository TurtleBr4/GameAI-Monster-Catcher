
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
            battleManager = BattleManager.instance;

        if (attacker == null || !battleManager.battleIsActive())
            return;

        if (move == null)
        {
            Debug.Log(attacker.monsterName + " has no available move!");
            FinishAction(attacker);
            return;
        }

        MonsterInstance target = GetTarget(isPlayer);

        if (target == null)
        {
            FinishAction(attacker);
            return;
        }

        Debug.Log(
            attacker.monsterName + " used " + move.moveName + "!");

        if (Random.Range(1, 101) > move.accuracy)
        {
            Debug.Log(move.moveName + " missed!");
            FinishAction(attacker);
            return;
        }

        if (move.category == MoveCategory.Status)
        {
            Debug.Log("Status moves are not implemented yet.");
            FinishAction(attacker);
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
            attackStat = attacker.baseSpecial;
            defenseStat = target.baseSpecial;
        }

        int damage = Mathf.Max(
            1,
            move.power + attackStat / 2 - defenseStat / 2);

        float effectiveness = TypeChart.GetEffectiveness(
            move.type,
            target.primaryType);

        if (target.hasSecondaryType)
        {
            effectiveness *= TypeChart.GetEffectiveness(
                move.type,
                target.secondaryType);
        }

        damage = Mathf.Max(
            1,
            Mathf.RoundToInt(damage * effectiveness));

        target.TakeDamage(damage);

        Debug.Log(
            target.monsterName + " took " + damage + " damage!");

        if (effectiveness > 1f)
            Debug.Log("It's super effective!");
        else if (effectiveness < 1f)
            Debug.Log("It's not very effective...");

        if (target.IsFainted())
            Debug.Log(target.monsterName + " fainted!");

        FinishAction(attacker);
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

    private void FinishAction(MonsterInstance attacker)
    {
        battleManager.CheckBattleEnd();
        battleManager.NotifyBattleStateChanged();

        if (battleManager.battleIsActive())
            battleManager.CompleteAction(attacker);
    }
}
