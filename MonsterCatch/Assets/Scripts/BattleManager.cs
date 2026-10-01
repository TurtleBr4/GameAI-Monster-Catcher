using System;
using UnityEngine;

public enum TurnState
{
   Player, Enemy
}
public class BattleManager : MonoBehaviour
{
   [SerializeField] private Party player;
   [SerializeField] private Party enemy;
   
   public static BattleManager instance;
   
   public GameObject battleScene;
   public GameObject doubleScene;
   public GameObject singleScene;
   
   
   public MonsterBattleInstance[] enemySlots;
   public MonsterBattleInstance[] playerSlots;
   public bool isDoubleBattle = false;
   
   public TurnState turnState = TurnState.Player;

   private void Awake()
   {
      instance = this;
      DontDestroyOnLoad(gameObject);
   }

   private void Start()
   {
      battleScene.SetActive(false);
   }

  

   public void setEnemy(Party p)
   {
      enemy = p;
   }

   public bool isBattleReady() //sanity check
   {
      if (player == null || enemy == null)
      {
         return false;
      }
      return true;
   }
   
   
   public void startBattle()//
   {
      if (player == null) //extra validation 
      {
         player = GameManager.instance.playerParty;
      }

      if (isDoubleBattle) //fill our battle slots
      {
         enemySlots[0] = enemy.getMonsters()[0];
         enemySlots[1] = enemy.getMonsters()[1];
         
         playerSlots[0] = player.getMonsters()[0];
         playerSlots[1] = player.getMonsters()[1];

         int enemySpeed = 0;
         int playerSpeed = 0;
         
         foreach (MonsterBattleInstance m in enemySlots)
         {
            if (m.baseSpeed > enemySpeed)
            {
               enemySpeed = m.baseSpeed;
            }
         }
         foreach (MonsterBattleInstance m in playerSlots)
         {
            if (m.baseSpeed > playerSpeed)
            {
               playerSpeed = m.baseSpeed;
            }
         }

         if (playerSpeed > enemySpeed)
         {
            turnState = TurnState.Player;
         }
         else
         {
            turnState = TurnState.Enemy;
         }
      }
      else
      {
         enemySlots[0] = enemy.getMonsters()[0];
         playerSlots[0] = player.getMonsters()[0];
      }
      
      
      
      
   }

   public void endBattle()
   {
      
   }

}
