
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private BattleActionResolver actionResolver;
    [SerializeField] private GameObject moveMenu;

    [Header("Move Buttons")]
    [SerializeField] private Button[] moveButtons = new Button[4];
    [SerializeField] private TMP_Text[] moveLabels = new TMP_Text[4];

    [Header("Player Active Monster UI")]
    [SerializeField] private Image playerMonsterSprite;
    [SerializeField] private TMP_Text playerMonsterName;
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private Slider playerHealthBar;

    [Header("Enemy Active Monster UI")]
    [SerializeField] private Image enemyMonsterSprite;
    [SerializeField] private TMP_Text enemyMonsterName;
    [SerializeField] private TMP_Text enemyHealthText;
    [SerializeField] private Slider enemyHealthBar;

    private UnityAction[] buttonListeners;

    private readonly Queue<MonsterInstance> waitingPlayers =
        new Queue<MonsterInstance>();

    private readonly HashSet<MonsterInstance> queuedPlayers =
        new HashSet<MonsterInstance>();

    private MonsterInstance currentMonster;
    private bool waitingForMove;

    private void Start()
    {
        if (battleManager == null)
            battleManager = BattleManager.instance;

        if (battleManager == null)
            throw new UnityException("BattleManager is missing.");

        if (actionResolver == null)
            actionResolver =
                FindFirstObjectByType<BattleActionResolver>();

        if (actionResolver == null)
            throw new UnityException("BattleActionResolver is missing.");

        if (moveMenu == null)
            throw new UnityException("Move menu is missing.");

        buttonListeners = new UnityAction[moveButtons.Length];

        battleManager.OnActionReady += HandleActionReady;
        battleManager.OnBattleStarted += RefreshBattleUI;
        battleManager.OnBattleStateChanged += RefreshBattleUI;

        moveMenu.SetActive(false);

        for (int i = 0; i < moveButtons.Length; i++)
        {
            if (moveButtons[i] == null)
                continue;

            int index = i;

            buttonListeners[i] = () => SelectMove(index);
            moveButtons[i].onClick.AddListener(buttonListeners[i]);
        }

        RefreshBattleUI();
    }

    private void OnDestroy()
    {
        if (battleManager != null)
        {
            battleManager.OnActionReady -= HandleActionReady;
            battleManager.OnBattleStarted -= RefreshBattleUI;
            battleManager.OnBattleStateChanged -= RefreshBattleUI;
        }

        if (buttonListeners == null)
            return;

        for (int i = 0; i < buttonListeners.Length; i++)
        {
            if (moveButtons[i] != null &&
                buttonListeners[i] != null)
            {
                moveButtons[i].onClick.RemoveListener(
                    buttonListeners[i]);
            }
        }
    }

    private void HandleActionReady(
        MonsterInstance monster,
        bool isPlayer)
    {
        RefreshBattleUI();

        // The AI handles enemy actions. Do not close the player's menu.
        if (!isPlayer || monster == null)
            return;

        if (waitingForMove && currentMonster == monster)
            return;

        if (queuedPlayers.Contains(monster))
            return;

        if (waitingForMove)
        {
            waitingPlayers.Enqueue(monster);
            queuedPlayers.Add(monster);
            return;
        }

        SetCurrentMonster(monster);
    }

    private void SetCurrentMonster(MonsterInstance monster)
    {
        if (monster == null || monster.currentHealth <= 0)
            return;

        currentMonster = monster;
        waitingForMove = true;

        DisplayMoves();
    }

    private void DisplayMoves()
    {
        for (int i = 0; i < moveButtons.Length; i++)
        {
            if (moveButtons[i] == null)
                continue;

            bool hasMove =
                currentMonster != null &&
                currentMonster.moves != null &&
                i < currentMonster.moves.Length &&
                currentMonster.moves[i] != null;

            moveButtons[i].gameObject.SetActive(hasMove);

            if (hasMove &&
                i < moveLabels.Length &&
                moveLabels[i] != null)
            {
                moveLabels[i].text =
                    currentMonster.moves[i].moveName;
            }
        }

        moveMenu.SetActive(true);
    }

    private void SelectMove(int index)
    {
        if (!waitingForMove || currentMonster == null)
            return;

        if (index < 0 || index >= currentMonster.moves.Length)
            return;

        MonsterMove selectedMove = currentMonster.moves[index];

        if (selectedMove == null)
            return;

        MonsterInstance attacker = currentMonster;

        waitingForMove = false;
        currentMonster = null;
        moveMenu.SetActive(false);

        actionResolver.ResolveMove(attacker, selectedMove, true);

        RefreshBattleUI();
        ShowNextQueuedPlayer();
    }

    private void ShowNextQueuedPlayer()
    {
        if (!battleManager.battleIsActive() || waitingForMove)
            return;

        while (waitingPlayers.Count > 0)
        {
            MonsterInstance next = waitingPlayers.Dequeue();
            queuedPlayers.Remove(next);

            if (next == null || next.currentHealth <= 0)
                continue;

            SetCurrentMonster(next);
            return;
        }
    }

    public void RefreshBattleUI()
    {
        if (battleManager == null)
            return;

        UpdateMonsterDisplay(
            battleManager.playerSlots[0],
            playerMonsterSprite,
            playerMonsterName,
            playerHealthText,
            playerHealthBar);

        UpdateMonsterDisplay(
            battleManager.enemySlots[0],
            enemyMonsterSprite,
            enemyMonsterName,
            enemyHealthText,
            enemyHealthBar);
    }

    private void UpdateMonsterDisplay(
        MonsterInstance monster,
        Image spriteImage,
        TMP_Text nameText,
        TMP_Text healthText,
        Slider healthBar)
    {
        bool hasMonster =
            monster != null &&
            monster.species != null &&
            monster.currentHealth > 0;

        if (spriteImage != null)
        {
            spriteImage.gameObject.SetActive(hasMonster);

            if (hasMonster)
                spriteImage.sprite = monster.species.image;
        }

        if (nameText != null)
            nameText.text = hasMonster ? monster.monsterName : "";

        if (healthText != null)
        {
            healthText.text = hasMonster
                ? monster.currentHealth + " / " + monster.maxHealth
                : "";
        }

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(hasMonster);

            if (hasMonster)
            {
                healthBar.minValue = 0;
                healthBar.maxValue = monster.maxHealth;
                healthBar.value = monster.currentHealth;
            }
        }
    }
}
