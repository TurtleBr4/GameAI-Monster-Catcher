using Mono.Cecil;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogNPC : MonoBehaviour
{
    [SerializeField]
    private List<string> lines;
    [SerializeField]
    private int direction;
    [SerializeField]
    private GridMovement player;
    [SerializeField]
    private float sightrange;
    [SerializeField]
    private TextMeshProUGUI textbox;
    [SerializeField]
    private GameObject textbackground;

    private bool linesread;
    private bool readinglines;
    private int currentline;


    private void Awake()
    {
        linesread = false;
        readinglines = false;
        currentline = 0;
    }

    private void Update()
    {
        if (!linesread && !player.moving)
        {
            switch (direction) //please please please for the love of god do not place animals in front of npcs and then ask me why this breaks, i will crash out
            {
                case 0:
                    if(Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.5f), Vector2.up, sightrange-0.5f))
                    {
                        StartDialogue();
                    }
                    break;
                case 1:
                    if (Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y), Vector2.left, sightrange-0.5f))
                    {
                        StartDialogue();
                    }
                    break;
                case 2:
                    if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.5f), Vector2.down, sightrange - 0.5f))
                    {
                        StartDialogue();
                    }
                    break;
                case 3:
                    if (Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y), Vector2.right, sightrange - 0.5f))
                    {
                        StartDialogue();
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
    }
    
    private void StartDialogue()
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
        else if (currentline ==  lines.Count - 1)
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
}
