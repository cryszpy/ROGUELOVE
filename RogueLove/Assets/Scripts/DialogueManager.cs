using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    [SerializeField] private Image dialogueSprite;

    [SerializeField] private TMP_Text nameText;

    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private Animator animator;

    private Queue<string> sentences;

    public DialogueList callDialogueList;

    [SerializeField] private GameObject continueButton;

    [SerializeField] private Dialogue currentDialogue;

    [SerializeField] private List<Animator> uiElements;

    [Header("VARIABLES")]

    public List<Dialogue> priority;

    [SerializeField] private float textCPS;

    [SerializeField] private GAMESTATE previousGameState;

    public bool movedDialogue = false;

    public bool playingDialogue = false;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void CheckIfEmpty(List<Dialogue> list, List<Dialogue> seenList) {

        // If dialogue type list has run out of available dialogue, then—
        if (list.Count == 0 && seenList.Count != 0) {

            // Reset dialogue and reshuffle
            ResetDialogue(seenList, list);

            Debug.Log("All dialogues of type: " + list + " have been exhausted! Reshuffled.");
        }
    }

    public void AddRandomNoReqDialogue(DialogueList dialogueList) {
        
        CheckIfEmpty(dialogueList.noRequirements, dialogueList.seenNoRequirements);

        // Gets a random piece of dialogue of a specific type
        int rand = UnityEngine.Random.Range(0, dialogueList.noRequirements.Count - 1);

        // Adds that piece of dialogue to the priority list
        priority.Add(dialogueList.noRequirements[rand]);

        // Removes it from available pool
        DiscardDialogue(dialogueList.noRequirements[rand], dialogueList.noRequirements, dialogueList.seenNoRequirements);

        Debug.Log("Added random no requirements dialogue!");
    }

    public void DiscardDialogue(Dialogue dialogue, List<Dialogue> dialogues, List<Dialogue> seenDialogues) {

        seenDialogues.Add(dialogue);
        dialogues.Remove(dialogue);
        movedDialogue = true;
    }

    public void ResetDialogue(List<Dialogue> swapFrom, List<Dialogue> swapTo) {
        foreach (Dialogue dialogue in swapFrom.ToArray()) {
            swapTo.Add(dialogue);
            swapFrom.Remove(dialogue);
        }
        ShuffleList(swapTo);
    }

    public void ShuffleList(List<Dialogue> list) {

        for(int i = list.Count - 1; i > 0; i--) {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[j], list[i]) = (list[i], list[j]);
        }
    }

    private void FindReferences() {

        // UI ANIMATOR REFERENCES

        if (GameStateManager.GetStage() != 0) {

            if (GameObject.FindGameObjectWithTag("EnergyBar").TryGetComponent<Animator>(out var energybar)) {
                uiElements.Add(energybar);
            } else {
                Debug.LogError("Could not find energy bar script!");
            }

            if (GameObject.FindGameObjectWithTag("PlayerHealth").TryGetComponent<Animator>(out var healthbar)) {
                uiElements.Add(healthbar);
            } else {
                Debug.LogError("Could not find health bar script!");
            }

            if (GameObject.FindGameObjectWithTag("CoinsUI").TryGetComponent<Animator>(out var coins)) {
                uiElements.Add(coins);
            } else {
                Debug.LogError("Could not find coins UI script!");
            }

            if (GameObject.FindGameObjectWithTag("AmmoBar").TryGetComponent<Animator>(out var ammobar)) {
                uiElements.Add(ammobar);
            } else {
                Debug.LogError("Could not find ammo bar script!");
            }
        }
    
        // DIALOGUE UI REFERENCES

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
    }

    public void StartDialogue (Dialogue dialogue) {
        Debug.Log("Started conversation with + " + dialogue.characterName);
        playingDialogue = true;

        FindReferences();

        // Enables the continue button
        if (!continueButton.activeSelf) {
            continueButton.SetActive(true);
        }

        if (uiElements.Count != 0) {
            foreach (Animator element in uiElements) {
                element.SetTrigger("Out");
            }
        }

        currentDialogue = dialogue;

        priority.Remove(dialogue);

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

        if (uiElements.Count != 0) {
            foreach (Animator element in uiElements) {
                element.SetTrigger("In");
            }
        }

        // Disables the continue button
        if (continueButton.activeSelf) {
            continueButton.SetActive(false);
        }

        // Clears the priority list of any queued dialogue
        priority.Clear();

        // Plays closing dialogue box animation
        animator.SetBool("IsOpen", false);

        // Resets game state to previous state
        GameStateManager.SetState(previousGameState);

        playingDialogue = false;

        Debug.Log("End of conversation.");
    }
}
