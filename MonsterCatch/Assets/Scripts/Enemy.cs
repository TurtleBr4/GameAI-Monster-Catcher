using UnityEngine;

public enum EnemyOverworldBehavior
{
    Static,
    Roam,
    Path
}
public class Enemy : MonoBehaviour
{
    public Party data;
    public EnemyOverworldBehavior behavior;
    public int lineOfSight;
    public FacingDirection facingDirection;


    public void doAiStuff()
    {
        switch (behavior)
        {
            case EnemyOverworldBehavior.Static:
                break;
            case EnemyOverworldBehavior.Roam:
                break;
            case EnemyOverworldBehavior.Path:
                break;
        }
    }

    public void checkLineOfSight()
    {
        Vector2 dir;
        switch (facingDirection)
        {
            case FacingDirection.Left:
                dir = Vector2.left;
                break;
            case FacingDirection.Right:
                dir = Vector2.right;
                break;
            case FacingDirection.Up:
                dir = Vector2.up;
                break;
            case FacingDirection.Down:
                dir = Vector2.down;
                break;
            default:
                dir = Vector2.zero;
                break;
        }
        
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, lineOfSight);

        if (hit.collider.transform.CompareTag("Player"))
        {
            BattleManager.instance.setEnemy(data);
            GameManager.instance.toggleState(GameState.Battle);
        }

    }

}
