using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StationaryTrainer : MonoBehaviour
{
    [SerializeField]
    private float sightrange;
    [SerializeField]
    private GridMovement player;
    [SerializeField]
    private TextMeshProUGUI textbox;
    [SerializeField]
    private GameObject textbackground;
    [SerializeField]
    private List<string> lines;
    [SerializeField]
    private int direction;

    private bool readinglines;
    private bool linesread;
    private bool hasbattled;
    private int currentline;

    private void Awake()
    {
        readinglines = false;
        linesread = false;
        hasbattled = false;
        currentline = 0;
    }

    private void Update()
    {
        if (!linesread && !player.moving)
        {
            switch (direction) //please please please for the love of god do not place animals in front of npcs and then ask me why this breaks, i will crash out
            {
                case 0:
                    if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.up, sightrange - 0.5f))
                    {
                        ReadLines();
                    }
                    break;
                case 1:
                    if (Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y), Vector2.left, sightrange - 0.5f))
                    {
                        ReadLines();
                    }
                    break;
                case 2:
                    if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.down, sightrange - 0.5f))
                    {
                        ReadLines();
                    }
                    break;
                case 3:
                    if (Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y), Vector2.right, sightrange - 0.5f))
                    {
                        ReadLines();
                    }
                    break;
            }
        }
        if (readinglines)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextLine();
            }
        }
        if (linesread && !readinglines && !player.moving && !hasbattled)
        {
            StartBattle();
        }
    }

    private void ReadLines()
    {
        linesread = true;
        readinglines = true;
        player.SetMoveOverride(true);
        textbox.gameObject.SetActive(true);
        textbackground.SetActive(true);
        textbox.text = lines[0];
    }

    private void NextLine()
    {
        if (currentline < lines.Count - 1)
        {
            currentline++;
            textbox.text = lines[currentline];
        }
        else if (currentline == lines.Count - 1)
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        readinglines = false;
        player.SetMoveOverride(false);
        textbox.gameObject.SetActive(false);
        textbackground.SetActive(false);
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
    }
}

