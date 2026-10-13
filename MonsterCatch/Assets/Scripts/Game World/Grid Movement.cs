using System.Collections;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    private bool walking;  //useful for animating
    private int direction; //useful for animating
    [SerializeField]
    private float walkspeed;
    [SerializeField]
    private float runspeed;  

    public bool moving; //useful for animating
    private bool moveoverride; //for when the player is having dialog and stuff

    private void Awake()
    {
        moveoverride = false;
        moving = false;
    }

    private void FixedUpdate()
    {
        //check if sprinting in between movement
        if (!moving && !moveoverride)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                walking = false;
            }
            else
            {
                walking = true;
            }
            //check where to move to so long as the player isnt already moving
            if (Input.GetKey(KeyCode.W))
            {
                if (direction == 0)
                {
                    if (!Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y+0.5f), Vector2.up, 0.5f))
                    {
                        StartCoroutine(Move(0));
                        //add walk/run animation here, all necessary variables should be assigned already
                    }
                }
                else
                {
                    direction = 0;
                    Turn(0);
                }
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (direction == 1)
                {
                    if (!Physics2D.Raycast(new Vector2(transform.position.x-0.5f, transform.position.y), Vector2.left, 0.5f))
                    {
                        StartCoroutine(Move(1));
                        //add walk/run animation here, all necessary variables should be assigned already
                    }
                }
                else
                {
                    direction = 1;
                    Turn(1);
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (direction == 2)
                {
                    if (!Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y-0.5f), Vector2.down, 0.5f))
                    {
                        StartCoroutine(Move(2));
                        //add walk/run animation here, all necessary variables should be assigned already
                    }
                }
                else
                {
                    direction = 2;
                    Turn(2);
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (direction == 3)
                {
                    if (!Physics2D.Raycast(new Vector2(transform.position.x+0.5f,transform.position.y), Vector2.right, 0.5f))
                    {
                        StartCoroutine(Move(3));
                        //add walk/run animation here, all necessary variables should be assigned already
                    }
                }
                else
                {
                    direction = 3;
                    Turn(3);
                }
            }
        }
    }

    IEnumerator Move(int direction)
    {
        moving = true;
        //finds where to move to based on the direction
        Vector3 newpos = gameObject.transform.position;
        switch(direction)
        {
            case 0:
                newpos.y += 1;
                break;
            case 1:
                newpos.x -= 1;
                break;
            case 2:
                newpos.y -= 1;
                break;
            case 3:
                newpos.x += 1;
                break;
        }
        //moves towards it until its so close snapping position isn't visible
        while ((newpos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            if (walking)
            {
                transform.position = Vector3.MoveTowards(transform.position, newpos, walkspeed * Time.fixedDeltaTime);
                yield return null;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, newpos, runspeed * Time.fixedDeltaTime);
                yield return null;
            }
        }
        //snaps position to exact coords
        transform.position = newpos;
        moving = false;
    }
    
    private void Turn(int direction) //does nothing for now, will be used to change the direction of the sprite later
    {
        return;
    }

    public void SetMoveOverride(bool moveoverride)
    {
        this.moveoverride = moveoverride;
    }
}
