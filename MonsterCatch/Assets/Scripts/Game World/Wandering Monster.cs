using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WanderingMonster : MonoBehaviour
{
    [SerializeField]
    private List<Vector2> pathpoints;
    [SerializeField]
    private float sightrange;
    [SerializeField]
    private GridMovement player;

    private int direction;
    private bool pathblocked;
    private bool battling;
    private bool moving;
    private bool hasbattled;
    private int targetpathpoint;

    private void Start()
    {
        pathblocked = false;
        battling = false;
        moving = false;
        hasbattled = false;
        targetpathpoint = 0;
        direction = 0;
    }

    private void Update()
    {
        //figure out where to go next
        if (!moving && !battling)
        {
            //check if target point has been reached
            if (transform.position.x == pathpoints[targetpathpoint].x && transform.position.y == pathpoints[targetpathpoint].y)
            {
                if (targetpathpoint == pathpoints.Count - 1)
                {
                    targetpathpoint = 0;
                }
                else
                {
                    targetpathpoint++;
                }
            }
            //check direction to move, this works only if the path is only straight lines, its a grid so please keep that in mind
            //handle movement on x axis
            if (pathpoints[targetpathpoint].y - transform.position.y == 0)
            {
                //if moving right
                if (pathpoints[targetpathpoint].x - transform.position.x <= 0)
                {
                    direction = 1;
                }
                //if moving left
                else
                {
                    direction = 3;
                }
            }
            //handle movement on y axis
            if (pathpoints[targetpathpoint].x - transform.position.x == 0)
            {
                //if moving down
                if (pathpoints[targetpathpoint].y - transform.position.y <= 0)
                {
                    direction = 2;
                }
                //if moving up
                else
                {
                    direction = 0;
                }
            }
        }
        //figure out if the path ahead is blocked before moving
        if (!moving && !battling)
        {
            switch (direction)
            {
                case 0:
                    pathblocked = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.6f), Vector2.up, 0.5f);
                    break;
                case 1:
                    pathblocked = Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y), Vector2.left, 0.5f);
                    break;
                case 2:
                    pathblocked = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.6f), Vector2.down, 0.5f);
                    break;
                case 3:
                    pathblocked = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y), Vector2.right, 0.5f);
                    break;
            }
        }
        if (pathblocked && !hasbattled && !moving)
        {
            StartBattle();
        }
        if (!pathblocked && !moving)
        {
            StartCoroutine(Move(direction));
        }
    }

    IEnumerator Move(int direction)
    {
        moving = true;
        //finds where to move to based on the direction
        Vector3 newpos = gameObject.transform.position;
        switch (direction)
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
            transform.position = Vector3.MoveTowards(transform.position, newpos, 0.2f * Time.fixedDeltaTime);
            yield return null;
        }
        //snaps position to exact coords
        transform.position = newpos;
        moving = false;
    }

    private void StartBattle()
    {
        hasbattled = true;
        player.SetMoveOverride(true);
        //call to battle script goes here
        //for now this is gonna call end battle directly
        Invoke(nameof(EndBattle), 1f);
    }

    public void EndBattle()
    {
        player.SetMoveOverride(false);
        gameObject.SetActive(false);
    }
}
