using System;
using Unity.VisualScripting;
using UnityEngine;

public enum GameState
{
    Overworld,
    Battle,
    Dialog
}

        
public class GameManager : MonoBehaviour
{
    public const float controllerDeadzone = .2f;
    
    public static GameManager instance;
    private GameState gameState;
    public Party playerParty;
    public BattleManager battleManager;
    
    
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
        if (playerParty == null)
        {
            throw new UnityException("playerParty is null");
        }
    }

    public void toggleState(GameState state)
    {
        if (state == gameState)
        {
            throw new UnityException("togglerState is already active"); //this code assumes you are changing states
        }
        switch (state)
        {
            case GameState.Overworld:
                break;
            case GameState.Battle:
                if (battleManager.isBattleReady())
                {
                    battleManager.startBattle();
                }
                else
                {
                    throw new UnityException("BattleManager is not ready");
                }
                break;
            case GameState.Dialog:
                break;
        }

    }
}
