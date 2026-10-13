
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private GameObject moveMenu;

    [Header("Move Buttons")]
    [SerializeField] private Button[] moveButtons = new Button[4];
    [SerializeField] private TMP_Text[] moveLabels = new TMP_Text[4];

    private UnityAction[] buttonListeners;

    private MonsterInstance currentMonster;
    private bool waitingForMove;

    private void Start()
    {
        if (battleManager == null)
            battleManager = BattleManager.instance;

        if (battleManager == null)
            throw new UnityException("BattleManager is missing.");

        if (moveMenu == null)
            throw new UnityException("Move menu is missing.");

        buttonListeners = new UnityAction[moveButtons.Length];

        battleManager.OnActionReady += HandleActionReady;

        moveMenu.SetActive(false);

        for (int i = 0; i < moveButtons.Length; i++)
        {
            if (moveButtons[i] == null)
                continue;

            int index = i;

            buttonListeners[i] = () => SelectMove(index);
            moveButtons[i].onClick.AddListener(buttonListeners[i]);
        }
    }

    private void OnDestroy()
    {
        if (battleManager != null)
            battleManager.OnActionReady -= HandleActionReady;

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
        MonsterInstance monster, bool isPlayer)
    {
        moveMenu.SetActive(false);
        waitingForMove = false;
        currentMonster = null;

        if (!isPlayer)
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
                i < currentMonster.moves.Length &&
                currentMonster.moves[i] != null;

            moveButtons[i].gameObject.SetActive(hasMove);

            if (hasMove && i < moveLabels.Length &&
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

        MonsterMove selectedMove =
            currentMonster.moves[index];

        if (selectedMove == null)
            return;

        MonsterInstance attacker = currentMonster;

        waitingForMove = false;
        currentMonster = null;
        moveMenu.SetActive(false);

        BattleActionResolver resolver =
            FindFirstObjectByType<BattleActionResolver>();

        if (resolver == null)
        {
            Debug.LogError("BattleActionResolver is missing.");
            battleManager.CompleteAction();
            return;
        }

        resolver.ResolveMove(attacker, selectedMove, true);
    }
}
