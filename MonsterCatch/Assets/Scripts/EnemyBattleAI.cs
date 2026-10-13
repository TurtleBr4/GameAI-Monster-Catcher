
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAILevel
{
    BasicWildMonster,
    BasicTrainerBattle
}

public class EnemyBattleAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private EnemyAILevel aiLevel =
        EnemyAILevel.BasicWildMonster;

    [Header("References")]
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private BattleActionResolver actionResolver;

    private void Start()
    {
        if (battleManager == null)
            battleManager = BattleManager.instance;

        if (battleManager == null)
            throw new UnityException("BattleManager is missing.");

        if (actionResolver == null)
            actionResolver = FindFirstObjectByType<BattleActionResolver>();

        if (actionResolver == null)
            throw new UnityException("BattleActionResolver is missing.");

        battleManager.OnActionReady += HandleActionReady;
    }

    private void OnDestroy()
    {
        if (battleManager != null)
            battleManager.OnActionReady -= HandleActionReady;
    }

    private void HandleActionReady(
        MonsterInstance monster,
        bool isPlayer)
    {
        if (isPlayer || monster == null)
            return;

        if (!battleManager.battleIsActive() ||
            monster.currentHealth <= 0)
        {
            return;
        }

        List<MonsterMove> availableMoves =
            new List<MonsterMove>();

        foreach (MonsterMove move in monster.moves)
        {
            if (move != null)
                availableMoves.Add(move);
        }

        if (availableMoves.Count == 0)
        {
            Debug.LogWarning(
                monster.monsterName + " has no available moves.");

            battleManager.CompleteAction(monster);
            return;
        }

        MonsterMove selectedMove;

        if (aiLevel == EnemyAILevel.BasicWildMonster)
        {
            selectedMove = SelectRandomMove(availableMoves);
        }
        else
        {
            selectedMove = SelectBestMove(monster, availableMoves);
        }

        actionResolver.ResolveMove(monster, selectedMove, false);
    }

    private MonsterMove SelectRandomMove(
        List<MonsterMove> availableMoves)
    {
        int index = Random.Range(0, availableMoves.Count);
        return availableMoves[index];
    }

    private MonsterMove SelectBestMove(
        MonsterInstance attacker,
        List<MonsterMove> availableMoves)
    {
        MonsterInstance target = GetTarget();

        if (target == null)
            return SelectRandomMove(availableMoves);

        float bestScore = float.MinValue;
        List<MonsterMove> bestMoves = new List<MonsterMove>();

        foreach (MonsterMove move in availableMoves)
        {
            float score = EvaluateMove(attacker, target, move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMoves.Clear();
                bestMoves.Add(move);
            }
            else if (score == bestScore)
            {
                bestMoves.Add(move);
            }
        }

        if (bestMoves.Count == 0)
            return SelectRandomMove(availableMoves);

        int index = Random.Range(0, bestMoves.Count);

        Debug.Log(
            attacker.monsterName + " selected " +
            bestMoves[index].moveName +
            " with an estimated score of " + bestScore + ".");

        return bestMoves[index];
    }

    private float EvaluateMove(
        MonsterInstance attacker,
        MonsterInstance target,
        MonsterMove move)
    {
        if (move.category == MoveCategory.Status)
            return 0f;

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

        int estimatedDamage = Mathf.Max(
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

        float accuracy = Mathf.Clamp(move.accuracy, 0, 100) / 100f;

        return estimatedDamage * effectiveness * accuracy;
    }

    private MonsterInstance GetTarget()
    {
        MonsterInstance[] targets = battleManager.playerSlots;
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

    public void SetAILevel(EnemyAILevel level)
    {
        aiLevel = level;
    }
}
