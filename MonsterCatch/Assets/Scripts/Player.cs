
using UnityEngine;
using UnityEngine.Serialization;

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
    public float moveSpeed = 5f;
    public float runMult = 1.5f;
    public FacingDirection facingDirection;

    [FormerlySerializedAs("characterController")]
    public Rigidbody2D rb;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (rb == null)
            Debug.LogError("Player is missing a Rigidbody2D!", this);
    }

    private void FixedUpdate()
    {
        doMove();
    }

    private void doMove()
    {
        if (rb == null)
            return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        isRunning = Input.GetButton("Jump");

        float speed = isRunning ? moveSpeed * runMult : moveSpeed;

        Vector2 movement = new Vector2(x, y);

        // Prevent diagonal movement from being faster.
        movement = movement.normalized * speed;

        if (movement.sqrMagnitude > 0)
        {
            movement = gateMovement(movement);
        }

        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
    }

    private Vector2 gateMovement(Vector2 movement)
    {
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            facingDirection = movement.x > 0
                ? FacingDirection.Right
                : FacingDirection.Left;

            return new Vector2(movement.x, 0);
        }
        else
        {
            facingDirection = movement.y > 0
                ? FacingDirection.Up
                : FacingDirection.Down;

            return new Vector2(0, movement.y);
        }
    }
}
