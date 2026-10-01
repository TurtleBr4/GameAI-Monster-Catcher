using System;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
   [SerializeField] private Party player;
   [SerializeField] private Party enemy;

   private void Awake()
   {
      DontDestroyOnLoad(gameObject);
   }

   public bool isDoubleBattle = false;


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
      
   }

   public void endBattle()
   {
      
   }

}
