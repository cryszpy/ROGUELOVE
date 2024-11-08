using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")]

    public PlayerController player;

    [SerializeField] private Image dialogueSprite;

    [SerializeField] private TMP_Text nameText;

    [SerializeField] private TMP_Text dialogueText;

    [SerializeField] private Animator animator;

    private Queue<string> sentences;

    public DialogueList callDialogueList;

    [SerializeField] private GameObject continueButton;

    [SerializeField] private Dialogue currentDialogue;

    [SerializeField] private GameObject choicesBar;
    [SerializeField] private GameObject choicePrefab;
    [SerializeField] private List<GameObject> choiceButtonsList;

    [SerializeField] private List<Animator> uiElements;

    public SceneInfo sceneInfo;

    [Header("VARIABLES")]

    public List<Dialogue> priority;

    [SerializeField] private float textCPS;

    [SerializeField] private GAMESTATE previousGameState;

    [SerializeField] private float callDialogueChance;

    [Tooltip("Index 1 -> Support calls, Index 2 -> Quest calls, Index 3 -> Shop calls, Index 4 -> Trade calls")]
    [SerializeField] private float[] callChances;

    public bool movedDialogue = false;

    public bool playingDialogue = false;
    public bool playingChoices = false;

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

    public void AddRandomNoReqDialogue(List<Dialogue> list, List<Dialogue> seenList) {
        
        CheckIfEmpty(list, seenList);

        // Gets a random piece of dialogue of a specific type
        int rand = UnityEngine.Random.Range(0, list.Count - 1);

        // Adds that piece of dialogue to the priority list
        priority.Add(list[rand]);

        // Removes it from available pool
        DiscardDialogue(list[rand], list, seenList);

        Debug.Log("Added random no requirements dialogue from list: " + list);
    }

    public void AddCallDialogue(DialogueList dialogueList, PlayerController player) {

        int counter = 0;

        List<Dialogue> fulfilledReqs = new();

        // REQUIREMENTS DIALOGUE is prioritized first

        CheckIfEmpty(dialogueList.requirements, dialogueList.seenRequirements);

        // For every dialogue with a requirement—
        foreach (Dialogue dialogue in dialogueList.requirements) {

            // For every requirement that piece of dialogue has—
            for (int i = 0; i < dialogue.requirements.Count; i++) {

                // Check the requirements
                if (CheckRequirements(dialogue.requirements[i], player)) {

                    // Add it to the priority list if all requirements have been fulfilled
                    priority.Add(dialogue);
                    fulfilledReqs.Add(dialogue);
                    counter++;
                    Debug.Log("Added a fulfilled CALL requirement dialogue!");
                }
            }
        }

        // After requirement dialogues have been added to the priority list, remove them from the possible dialogues list
        foreach (Dialogue dialogue in fulfilledReqs) {
            DiscardDialogue(dialogue, dialogueList.requirements, dialogueList.seenRequirements);
        }

        fulfilledReqs.Clear();

        // If no call requirements dialogue is fulfilled, then roll for special call type / normal type

        if (counter == 0) {
            Debug.Log("No requirements dialogue were generated!");

            // Randomly generates what type of call to play upon energy bar filled
            float rand = UnityEngine.Random.value;

            // If a special type of call is activated, play that
            if (rand <= callDialogueChance) {
                SelectCallType(dialogueList, player);
            } 
            // Else, play a random Fallow call dialogue
            else {
                AddRandomNoReqDialogue(dialogueList.noRequirements, dialogueList.seenNoRequirements);
            }
        }
    }

    private void SelectCallType(DialogueList dialogueList, PlayerController player) {

        CheckIfEmpty(dialogueList.noRequirements, dialogueList.seenNoRequirements);

        float rand = Random.value;

        // Support calls
        if (rand <= callChances[0]) {
            //SelectCallDialogue(dialogueList, CallDialogueType.SUPPORT);
            Debug.Log("WOULD'VE PLAYED A SUPPORT CALL!");
        } 
        // Quest calls
        else if (rand <= callChances[1]) {
            //SelectCallDialogue(dialogueList, CallDialogueType.QUEST);
            Debug.Log("WOULD'VE PLAYED A QUEST CALL!");
        }
        // Shop calls 
        else if (rand <= callChances[2]) {
            //SelectCallDialogue(dialogueList, CallDialogueType.SHOP);
            Debug.Log("WOULD'VE PLAYED A SHOP CALL!");
        } 
        // Trade calls
        else if (rand <= callChances[3]) {
            //SelectCallDialogue(dialogueList, CallDialogueType.TRADE);
            Debug.Log("WOULD'VE PLAYED A TRADE CALL!");
        }
    }

    private void SelectCallDialogue(DialogueList dialogueList, CallDialogueType callType) {

        // Create new list only for this call type (e.g. only Support Call dialogue)
        List<Dialogue> sourceList = new()
        {
            dialogueList.noRequirements.Find(x => x.callDialogueType == callType)
        };
        
        // Randomly pick one dialogue of call type to play
        int rand = Random.Range(0, sourceList.Count - 1);

        // Add this to the priority list
        if (sourceList.Count != 0) {
            priority.Add(sourceList[rand]);
        } else {
            Debug.LogWarning("There are no dialogues of type: " + callType);
        }
    }

    public void PlayCallDialogue(DialogueList dialogueList) {

        // Create new priority list only for this dialogue source (e.g. only Fallow dialogue)
        List<Dialogue> sourceList = new()
        {
            priority.Find(x => x.callDialogueType != CallDialogueType.NONE)
        };
        sourceList.Sort();

        if (sourceList.Count != 0) {
            GameStateManager.dialogueManager.StartDialogue(sourceList[0]);
        } else {
            Debug.LogWarning("There is nothing in the priority dialogue queue!");
        }
    }

    public bool CheckRequirements(DialogueRequirement requirement, PlayerController player) {

        switch (requirement.reqType) {

            case DialogueRequirementType.IS_HOLDING:
                if (player.heldWeapons[PlayerController.CurrentWeaponIndex].TryGetComponent<Weapon>(out var weapon)) {
                    Debug.Log(requirement.targetObjectID);
                    Debug.Log(weapon.id);
                    return weapon.id == requirement.targetObjectID;
                } else {
                    Debug.LogError("Could not find component Weapon on held weapon " + player.heldWeapons[PlayerController.CurrentWeaponIndex]);
                    return false;
                }
            case DialogueRequirementType.IS_CARRYING:
                return true;
            case DialogueRequirementType.HAS_TALKED_TO:
                return true;
            case DialogueRequirementType.HAS_KILLED:
                return true;
            case DialogueRequirementType.STAGE_NUM:
                return true;
            case DialogueRequirementType.LEVEL_NUM:
                return true;
            case DialogueRequirementType.ENERGY_NUM:
                return true;
            case DialogueRequirementType.HEALTH_NUM:
                return true;
            case DialogueRequirementType.SPEED_NUM:
                return true;
            case DialogueRequirementType.HAS_DIED:
                return true;
            case DialogueRequirementType.HAS_NOT_OPENED_CHEST:
                return true;
            case DialogueRequirementType.IS_ON_FIRE:
                return true;
            default:
                return false;
        }
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
            uiElements.Clear();

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
        if (choicesBar == null) {
            choicesBar = GameObject.FindGameObjectWithTag("ChoicesBar");
            Debug.Log("Choices bar was null! Reassigned.");
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

        if (uiElements.Count > 0) {
            foreach (Animator element in uiElements) {
                element.SetBool("Hide", true);
            }
        }

        currentDialogue = dialogue;

        if (priority.Contains(dialogue)) {
            priority.Remove(dialogue);
        }

        animator.SetBool("IsOpen", true);

        // Save previous game state and set current game state to MENU (disables input and enemies)
        if (GameStateManager.GetState() != GAMESTATE.MENU) {
            previousGameState = GameStateManager.GetState();
        }
        GameStateManager.SetState(GAMESTATE.MENU);

        // Sets the character's sprite for this piece of dialogue
        dialogueSprite.sprite = dialogue.characterSprite;

        // Sets the character's name for this piece of dialogue
        nameText.text = dialogue.characterName;

        // Clears the sentences list (just in case lol)
        sentences.Clear();

        // Remove existing buttons
        if (choiceButtonsList.Count > 0) {
            foreach (GameObject button in choiceButtonsList) {
                Destroy(button);
            }
            choiceButtonsList.Clear();
        }

        foreach (string sentence in dialogue.sentences) {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence() {

        // Display choices
        if (sentences.Count == 0 && currentDialogue.choices.Length > 0) {

            playingChoices = true;

            ShowChoices();
            
            return;
        }
        // End dialogue if no choices and nothing else to say
        else if (sentences.Count == 0 && currentDialogue.choices.Length <= 0) {
            EndDialogue();
            return;
        }

        // Removes current sentence from the queue of sentences to be displayed
        string sentence = sentences.Dequeue();

        // Stops any currently displaying sentences if player clicks continue mid-way through
        StopAllCoroutines();

        // Types the currently queued sentence
        StartCoroutine(TypeSentence(sentence));
    }

    public void ShowChoices() {

        // Remove existing buttons
        if (choiceButtonsList.Count > 0) {
            foreach (GameObject button in choiceButtonsList) {
                Destroy(button);
            }
            choiceButtonsList.Clear();
        }

        //animator.SetBool("IsOpen", false);

        choicesBar.SetActive(true);

        // If there are choices to be displayed—
        if (currentDialogue) {

            foreach (DialogueChoice choice in currentDialogue.choices) {

                // Create button in UI
                TMP_Text choiceText = Instantiate(choicePrefab, choicesBar.transform).GetComponentInChildren<TMP_Text>();

                // Set UI button text to the choice text
                choiceText.text = choice.choiceText;

                // Add button object to a list for future destruction
                choiceButtonsList.Add(choiceText.transform.parent.gameObject);

                // Add on-click button action (triggers certain dialogue)
                if (choiceText.transform.parent.TryGetComponent<Button>(out var button)) {
                    button.onClick.AddListener(() => AssignChoiceFollowUp(choice));
                }
            }
        }
    }

    public void AssignChoiceFollowUp(DialogueChoice choice) {
        playingChoices = false;

        // If choice response exists, play it
        if (choice.nextDialogue != null) {
            StartDialogue(choice.nextDialogue);
        }
    }

    private IEnumerator TypeSentence (string sentence) {
        Debug.Log(sentence);
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray()) {

            dialogueText.text += letter;

            yield return new WaitForSeconds(textCPS);
        }
    }

    void EndDialogue() {

        if (uiElements.Count != 0) {
            foreach (Animator element in uiElements) {
                element.SetBool("Hide", false);
            }
        }

        // Disables the continue button
        if (continueButton.activeSelf) {
            continueButton.SetActive(false);
        }

        // Clears the priority list of any queued dialogue
        //priority.Clear();

        // Plays closing dialogue box animation
        animator.SetBool("IsOpen", false);

        // Resets game state to previous state
        GameStateManager.SetState(previousGameState);

        playingDialogue = false;

        Debug.Log("End of conversation.");
    }
}
