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
     *      Standardize use of "inventory" vs "inv"
     *      Add ability to remove journal entries
     *      Add ability to remove inventory items
     *      Add plot point of missing spells? Have only limited spells at the beginning, add more?
     *      Add Save/Load functionality
     *      Clean up blurry text
     */

    /*
     * BUGS
     *      YAY! No current Bugs!
     */

    /*
     * COMPLETED TASKS
     *      Reorganize variable and method declaration, lay things out in an order that makes sense and is easy to read
     *      Create fade in/out for text
     */

    [SerializeField] Text storyField;
    [SerializeField] Text optionsField;
    [SerializeField] TextMeshProUGUI chapterField;

    [SerializeField] Text overlayStoryField;
    [SerializeField] Text overlayOptionsField;

    [SerializeField] Player p;
    State currentState;

    [SerializeField] State startingState;
    State[] childStates;
    List<State> availableStates = new List<State>();

    // These AudioSource lists only exist until a better solution can be found
    // Ideally I'd like to assign AudioSource components to the State, but I can't figure out how to do that
    // So right now the clunky solution is the one we've got
    [SerializeField] List<AudioSource> soundEffectFiles;
    [SerializeField] List<State> soundEffectTriggerStates;

    string[] codes;

    bool previousStateWasBasic = false;
    State invState;
    State journalState;

    int page = 0;


    // Start is called before the first frame update
    void Start() {

        overlayStoryField.CrossFadeAlpha(0, 0.0f, false);
        overlayOptionsField.CrossFadeAlpha(0, 0.0f, false);
        storyField.text = "";
        optionsField.text = "";

        invState = ScriptableObject.CreateInstance<State>();
        journalState = ScriptableObject.CreateInstance<State>();

        codes = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "q", "i", "j" };

        currentState = startingState;
        if(currentState.initializesPlayer()) {
            currentState.initPlayer(p);
        }
        p.setCurrentState(currentState);
        manageState();
    }

    // Update is called once per frame
    void Update() {
        listen();
    }

    IEnumerator textFadeHandler(string storyText, string optionsText) {
        overlayStoryField.text = storyText;
        storyField.CrossFadeAlpha(0, 0.2f, false);
        overlayStoryField.CrossFadeAlpha(1, 0.2f, false);
        if(optionsText != optionsField.text) {
            overlayOptionsField.text = optionsText;
            optionsField.CrossFadeAlpha(0, 0.2f, false);
            overlayOptionsField.CrossFadeAlpha(1, 0.2f, false);
        }
        yield return new WaitForSeconds(0.2f);
        storyField.text = storyText;
        optionsField.text = optionsText;
        storyField.CrossFadeAlpha(1, 0.0f, false);
        optionsField.CrossFadeAlpha(1, 0.0f, false);
        overlayStoryField.CrossFadeAlpha(0, 0.0f, false);
        overlayOptionsField.CrossFadeAlpha(0, 0.0f, false);
    }

    private void listen() {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            if(currentState != invState && currentState != journalState) {
                manageKeystroke(0);
                return;
            } else {
                manageBasicKeystroke(0);
                return;
            }
        }

        string input = Input.inputString;
        if(input.Length == 0) {
            return;
        }
        if(currentState != invState && currentState != journalState) {
            manageKeystroke(getIndex(input));
            return;
        } else {
            manageBasicKeystroke(getIndex(input));
            return;
        }
    }

    private void manageKeystroke(int code) {
        if(code == 10) {
            // Q - Quit
            UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        } else if(code == 11) {
            // I - Inventory
            currentState = invState;
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
        if((currentState == journalState && p.getJournalEntries().Count <= page * 3 + 3) ||
            (currentState == invState && p.getInventoryEntries().Count <= page * 3 + 3)) {
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
                } else if(currentState == invState) {
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
        } else if(!currentState.getChildStates()[0].getOptText().Equals("(Continue)") && !currentState.getChildStates()[0].getOptText().Equals("(Press Enter to Continue)")) {
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
        string invText = "";
        string optionsText = "";
        if(page == 0) {
            invText += "INVENTORY\n---------\n\n";
        }
        for(int i = page * 3; i < inventoryEntries.Count; i++) {
            if(i != page * 3) {
                invText += "\n\n";
            }
            invText += inventoryEntries[i];
            if(i == inventoryEntries.Count - 1 || i == page * 3 + 2) {
                break;
            }
        }
        if(page * 3 + 3 >= inventoryEntries.Count) {
            optionsText = "(Return)";
        } else {
            optionsText = "1 - Next Page\n2 - Return";
        }
        StartCoroutine(textFadeHandler(invText, optionsText));
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
        if(page * 3 + 3 >= journalEntries.Count) {
            optionsText = "(Return)";
        } else {
            optionsText = "1 - Next Page\n2 - Return";
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

}
