using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunsmokeAndFireGameScript : MonoBehaviour {

    /*
     * TODO
     *      Finish commenting Engine code
     *      Cleanup Player code
     *      Comment Player code
     *      Cleanup State code
     *      Comment state code
     *      Find a cleaner way to store audio files and state triggers
     *      Build framework for puzzle-solving mechanic
     *      Add ability to remove journal entries
     *      Add ability to remove inventory items
     *      Add plot point of missing spells? Have only limited spells at the beginning, add more?
     *      Add Save/Load functionality
     */

    /*
     * BUGS
     *          
     */

    /*
     * COMPLETED TASKS
     *      Reorganize variable and method declaration, lay things out in an order that makes sense and is easy to read
     *      Create fade in/out for text
     *      Standardize use of "inventory" vs "inv"
     *      Clean up "Continue" option checks
     *      Clean up blurry text
     *      Pick a better text font
     */

    // Text Fields for displaying story text and options text.
    // chapterField uses TextMeshProUGUI for fancy "noir" font
    [SerializeField] Text storyField;
    [SerializeField] Text optionsField;
    [SerializeField] TextMeshProUGUI chapterField;

    // Fields which store temporary story and options text to fade in/out over existing text
    [SerializeField] Text overlayStoryField;
    [SerializeField] Text overlayOptionsField;

    [SerializeField] Player p;

    // currentState stores temporary values that can modify Player.currentState
    State currentState;

    [SerializeField] State startingState;

    // childStates uses an array and is populated by State.getChildStates
    // availableStates uses a List because it needs to add states one at a time from childStates if they meet necessary criteria
    State[] childStates;
    List<State> availableStates = new List<State>();

    // These AudioSource lists only exist until a better solution can be found
    // Ideally I'd like to assign AudioSource components to the State, but I can't figure out how to do that
    // So right now the clunky solution is the one we've got
    [SerializeField] List<AudioSource> soundEffectFiles;
    [SerializeField] List<State> soundEffectTriggerStates;

    // Populated with accepted input values. Uses index of input value for navigation
    string[] codes;

    // If the previous state was Inventory or Journal (i.e. "basic states"), then navigating back to the primary state should not reinitialize variables or replay SFX
    // previousStateWasBasic is set <true> when navigating to Inventory or Journal, and set <false> when navigating back to primary state or to another non-basic state
    bool previousStateWasBasic = false;

    // inventoryState and journalState need to be created within Start so that currentState can be set to these for navigation and read purposes
    State inventoryState;
    State journalState;

    // What "page" of inventory or journal the user is looking at. Resets to 0 when leaving the inventory or journal state
    int page = 0;


    // Start is called before the first frame update
    void Start() {

        // Set Alpha to 0 for overlayStoryField and overlayOptionsField
        overlayStoryField.CrossFadeAlpha(0, 0.0f, false);
        overlayOptionsField.CrossFadeAlpha(0, 0.0f, false);

        // Set story and options to empty, so that starting state can fade in
        storyField.text = "";
        optionsField.text = "";

        // Create instance of State for inventoryState and journalState
        inventoryState = ScriptableObject.CreateInstance<State>();
        journalState = ScriptableObject.CreateInstance<State>();

        codes = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "q", "i", "j" };

        currentState = startingState;

        // If starting state is set to a different state for debugging purposes, or if loading a saved game, if the starting state does not have the initializesPlayer bool, do not reinitialize player variables
        if(currentState.initializesPlayer()) {
            currentState.initPlayer(p);
        }
        p.setCurrentState(currentState);
        manageState();
    }

    // Update is called once per frame
    void Update() {
        // listen is used to abstract input handling and clean up Update method
        // A clean Update method makes it easier to add additional code in the future, if necessary
        listen();
    }

    // textFadeHandler fades old text out and new text in for scene transitions
    // Chapter heading does not fade in/out, because I'm not sure how CrossFadeAlpha will work with TextMeshProUGUI
    IEnumerator textFadeHandler(string storyText, string optionsText) {
        // Sets overlayStoryField to the new story text, then cross-fades the old text with the overlay
        overlayStoryField.text = storyText;
        storyField.CrossFadeAlpha(0, 0.2f, false);
        overlayStoryField.CrossFadeAlpha(1, 0.2f, false);

        // If the new options text is different than the old options text, do a cross-fade
        // Otherwise, do not cross-fade
        // This prevents the annoying "fade-in-fade-out" effect if the options text does not change
        //      e.g. when both options are "(Continue)"
        if(optionsText != optionsField.text) {
            overlayOptionsField.text = optionsText;
            optionsField.CrossFadeAlpha(0, 0.2f, false);
            overlayOptionsField.CrossFadeAlpha(1, 0.2f, false);
        }

        // <yeild return new WaitForSeconds(seconds)> forces the Coroutine to wait
        // In this case, wait the length of time necessary for the texts to cross-fade
        yield return new WaitForSeconds(0.2f);

        // Set the story and options fields to the new values, then fade out the overlay and fade in the default fields instantly
        // This resets alpha for the fields to the correct values, so that cross-fade can be used again next time the scene transitions
        storyField.text = storyText;
        optionsField.text = optionsText;
        storyField.CrossFadeAlpha(1, 0.0f, false);
        optionsField.CrossFadeAlpha(1, 0.0f, false);
        overlayStoryField.CrossFadeAlpha(0, 0.0f, false);
        overlayOptionsField.CrossFadeAlpha(0, 0.0f, false);
    }

    // listen abstracts input handling out of Update
    private void listen() {

        if(!previousStateWasBasic) {
            if(
                (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) &&
                currentStateHasContinueOption()
            ) {
                manageKeystroke(0);
                return;
            }
        } else {
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                if(
                    (currentState == inventoryState && !hasAdditionalPages("inventory")) ||
                    (currentState == journalState && !hasAdditionalPages("journal"))
                ) {
                    manageBasicKeystroke(0);
                    return;
                }
            }

        }

        string input = Input.inputString;
        if(input.Length == 0) {
            return;
        }
        if(!previousStateWasBasic) {
            manageKeystroke(getIndex(input));
        } else {
            manageBasicKeystroke(getIndex(input));
        }
        return;
    }

    private void manageKeystroke(int code) {
        if(code == 10) {
            // Q - Quit
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        } else if(code == 11) {
            // I - Inventory
            currentState = inventoryState;
            previousStateWasBasic = true;
            readInventory();
        } else if(code == 12) {
            // J - Journal
            currentState = journalState;
            previousStateWasBasic = true;
            readJournal();
        } else {
            // 1 thru 0
            changeState(code);
        }
    }

    private void manageBasicKeystroke(int code) {
        if(
            (currentState == journalState && !hasAdditionalPages("journal")) ||
            (currentState == inventoryState && !hasAdditionalPages("inventory"))
        ) {
            if(code == 0) {
                page = 0;
                currentState = p.getCurrentState();
                manageState();
                previousStateWasBasic = false;
            }
        } else {
            if(code == 0) {
                page++;
                if(currentState == journalState) {
                    readJournal();
                } else if(currentState == inventoryState) {
                    readInventory();
                }
            } else if(code == 1) {
                page = 0;
                currentState = p.getCurrentState();
                manageState();
                previousStateWasBasic = false;
            }
        }
    }

    private void changeState(int opt) {
        for(int i = 0; i < availableStates.Count; i++) {
            if(opt == i) {
                currentState = availableStates[opt];
                p.setCurrentState(currentState);
                previousStateWasBasic = false;
                manageState();
            }
        }
    }

    private void manageState() {
        if(currentState.hasChapterTitle()) {
            chapterField.text = currentState.getChapterTitle();
        }
        if(!previousStateWasBasic) {
            currentState.managePlayer(p);
        }
        setAvailableStates();
        string optionsText = "";
        if(availableStates.Count == 0) {
            optionsText = "Q - Quit";
        } else if(!currentStateHasContinueOption()) { 
            for(int i = 0; i < availableStates.Count; i++) {
                optionsText += (i + 1) + " - " + availableStates[i].getOptText() + "\n";
            }
            optionsText += "I - Inventory\nJ - Journal\nQ - Quit";
        } else {
            optionsText += availableStates[0].getOptText();
        }
        StartCoroutine(textFadeHandler(currentState.getStoryText(p), optionsText));
        playSoundEffects();
    }

    private void setAvailableStates() {
        availableStates.Clear();
        childStates = currentState.getChildStates();
        availableStates.AddRange(childStates);
        foreach(State state in childStates) {
            if(!state.isAvailable(p)) {
                availableStates.Remove(state);
            }
        }
        if(currentState.isBasic()) {
            availableStates.Add(p.getCurrentState());
        }
    }

    private void readInventory() {
        List<string> inventoryEntries = p.getInventoryEntries();
        string inventoryText = "";
        string optionsText = "";
        if(page == 0) {
            inventoryText += "INVENTORY\n---------\n\n";
        }
        for(int i = page * 3; i < inventoryEntries.Count; i++) {
            if(i != page * 3) {
                inventoryText += "\n\n";
            }
            inventoryText += inventoryEntries[i];
            if(i == inventoryEntries.Count - 1 || i == page * 3 + 2) {
                break;
            }
        }
        if(hasAdditionalPages("inventory")) {
            optionsText = "1 - Next Page\n2 - Return";
        } else {
            optionsText = "(Return)";
        }
        StartCoroutine(textFadeHandler(inventoryText, optionsText));
    }

    private void readJournal() {
        List<string> journalEntries = p.getJournalEntries();
        string journalText = "";
        string optionsText = "";
        if(page == 0) {
            journalText += "JOURNAL\n-------\n\n";
        }
        for(int i = page * 3; i < journalEntries.Count; i++) {
            if(i != page * 3) {
                journalText += "\n\n";
            }
            journalText += journalEntries[i];
            if(i == journalEntries.Count - 1 || i == page * 3 + 2) {
                break;
            }
        }
        if(hasAdditionalPages("journal")) {
            optionsText = "1 - Next Page\n2 - Return";
        } else {
            optionsText = "(Return)";
        }
        StartCoroutine(textFadeHandler(journalText, optionsText));
    }

    private void playSoundEffects() {
        foreach(AudioSource audio in soundEffectFiles) {
            audio.Stop();
        }
        for(int i = 0; i < soundEffectTriggerStates.Count; i++) {
            if(soundEffectTriggerStates[i] == currentState && !previousStateWasBasic) {
                soundEffectFiles[i].Play();
            }
        }
    }
    private int getIndex(string code) {
        for(int i = 0; i < codes.Length; i++) {
            if(code == codes[i]) {
                return i;
            }
        }
        return -1;
    }

    private bool currentStateHasContinueOption() {
        if(availableStates[0].getOptText().Equals("(Continue)") || availableStates[0].getOptText().Equals("(Press Enter to Continue)")) {
            return true;
        } else {
            return false;
        }
    }

    private bool hasAdditionalPages(string screenType) {
        List<string> entries = new List<string>();
        if(screenType.Equals("inventory")) {
            entries = p.getInventoryEntries();
        } else if(screenType.Equals("journal")) {
            entries = p.getJournalEntries();
        } else {
            Debug.Log("Bad screenType passed to hasAdditionalPages()\nReceived " + screenType);
        }
        if(page * 3 + 3 < entries.Count) {
            return true;
        } else {
            return false;
        }
    }

}
