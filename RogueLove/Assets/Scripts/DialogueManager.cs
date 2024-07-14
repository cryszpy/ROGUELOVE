using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    [SerializeField]
    private Image dialogueSprite;

    [SerializeField]
    private TMP_Text nameText;

    [SerializeField]
    private TMP_Text dialogueText;

    [SerializeField]
    private Animator animator;

    private Queue<string> sentences;

    public DialogueList dialogueList;

    [SerializeField]
    private GameObject continueButton;

    [SerializeField]
    private Dialogue currentDialogue;

    [Header("VARIABLES")]

    [SerializeField]
    private float textCPS;

    [SerializeField]
    private GAMESTATE previousGameState;

    /* private static bool playDialogue = false;
    public static bool PlayDialogue { get => playDialogue; set => playDialogue = value; } */

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void AddRandomDialogueOfType(DialogueType type) {

        // Checks if the added dialogue type has run out of available options, and resets if so
        CheckEmpty(type);

        // Gets a random piece of dialogue of a specific type
        int rand = UnityEngine.Random.Range(0, dialogueList.GetDialogue(type).Count - 1);

        // Adds that piece of dialogue to the end of the priority queue (highest priority) and removes it from available pool
        dialogueList.DiscardDialogue(dialogueList.GetDialogue(type)[rand], type);

        Debug.Log("Added random dialogue of type: " + type);
    }

    private void CheckEmpty(DialogueType type) {

        // If dialogue type list has run out of available dialogue, then—
        if (dialogueList.GetDialogue(type).Count == 0 && dialogueList.GetSeenDialogue(type).Count != 0) {

            // Reset dialogue and reshuffle
            dialogueList.ResetDialogue(dialogueList.GetSeenDialogue(type), dialogueList.GetDialogue(type));

            Debug.Log("All dialogues of type: " + type + " have been exhausted! Reshuffled.");
        }
    }

    public void StartDialogue (Dialogue dialogue) {
        Debug.Log("Started conversation with + " + dialogue.characterName);
        currentDialogue = dialogue;

        dialogueList.priority.Remove(dialogue);

        if (continueButton == null) {
            continueButton = GameObject.FindGameObjectWithTag("ContinueButton");
            if (continueButton.TryGetComponent<Button>(out var button)) {
                button.onClick.AddListener(DisplayNextSentence);
                Debug.Log("Assigned continueButton onClick event!");
            }
            //continueButton.SetActive(false);
            Debug.Log("DialogueManager continueButton is null! Reassigned.");
        }
        if (animator == null) {
            animator = GameObject.FindGameObjectWithTag("DialogueBox").GetComponent<Animator>();
            Debug.Log("DialogueManager animator is null! Reassigned.");
        }
        if (nameText == null) {
            nameText = GameObject.FindGameObjectWithTag("DialogueName").GetComponent<TextMeshProUGUI>();
            Debug.Log("DialogueManager nameText is null! Reassigned.");
        }
        if (dialogueText == null) {
            dialogueText = GameObject.FindGameObjectWithTag("DialogueDialogue").GetComponent<TextMeshProUGUI>();
            Debug.Log("DialogueManager dialogueText is null! Reassigned.");
        }
        if (dialogueSprite == null) {
            dialogueSprite = GameObject.FindGameObjectWithTag("DialogueSprite").GetComponent<Image>();
            Debug.Log("DialogueManager dialogueSprite is null! Reassigned.");
        }

        animator.SetBool("IsOpen", true);

        // Save previous game state and set current game state to MENU (disables input and enemies)
        previousGameState = GameStateManager.GetState();
        GameStateManager.SetState(GAMESTATE.MENU);

        // Sets the character's sprite for this piece of dialogue
        dialogueSprite.sprite = dialogue.characterSprite;

        // Sets the character's name for this piece of dialogue
        nameText.text = dialogue.characterName;

        // Clears the sentences list (just in case lol)
        sentences.Clear();

        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {

        // If dialogue has ended and it is not a choice / question dialogue then end and return
        if (sentences.Count == 0 && currentDialogue.choices.Length == 0) {
            EndDialogue();
            return;
        } 
        // If dialogue has ended and it is a choice / question dialogue then display choices
        else if (sentences.Count == 0 && currentDialogue.choices.Length != 0) {
            Debug.Log("DISPLAY CHOICES");
            return;
        }

        // Removes current sentence from the queue of sentences to be displayed
        string sentence = sentences.Dequeue();

        // Stops any currently displaying sentences if player clicks continue mid-way through
        StopAllCoroutines();

        // Types the currently queued sentence
        StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence (string sentence) {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray()) {

            dialogueText.text += letter;

            yield return new WaitForSeconds(textCPS);
        }
    }

    void EndDialogue() {

        // Disables the continue button
        if (continueButton.activeSelf) {
            continueButton.SetActive(false);
        }

        // Clears the priority list of any queued dialogue
        dialogueList.priority.Clear();

        // Plays closing dialogue box animation
        animator.SetBool("IsOpen", false);

        // Resets game state to previous state
        GameStateManager.SetState(previousGameState);

        Debug.Log("End of conversation.");
    }
}
