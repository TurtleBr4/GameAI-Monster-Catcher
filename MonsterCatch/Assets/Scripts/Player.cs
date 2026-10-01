using System;
using UnityEngine;


public enum FacingDirection
{
    Up,
    Down,
    Left,
    Right
}
public class Player : MonoBehaviour
{
    public bool isRunning = false;
    public float moveSpeed;
    public float runMult = 1.5f;
    public FacingDirection facingDirection;
    public CharacterController characterController;

    private void FixedUpdate()
    {
        doMove();   
    }

    private void doMove()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            return;
        }
        
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        float speed = moveSpeed;
        
        if (Input.GetButton("Jump")) //i didnt feel like changing the name, sue me
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (isRunning)
        {
            speed *= runMult;
        }
        else
        {
            speed = moveSpeed;
        }
        
        

        Vector2 movement = new Vector2(x * speed, y  * speed);
        characterController.Move(gateMovement(movement));
    }

    private Vector2 gateMovement(Vector2 movement)
    {
        if (MathF.Abs(movement.x) > MathF.Abs(movement.y))
        {
            if (movement.x > 0)
            {
                facingDirection = FacingDirection.Right;
            }
            else
            {
                facingDirection = FacingDirection.Left;
            }
            return new Vector2(movement.x, 0);
        }
        else
        {
            if (movement.y > 0)
            {
                facingDirection = FacingDirection.Up;
            }
            else
            {
                facingDirection = FacingDirection.Down;
            }
            return new Vector2(0, movement.y);
        }
    }
}
