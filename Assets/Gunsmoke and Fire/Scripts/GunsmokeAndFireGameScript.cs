using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunsmokeAndFireGameScript : MonoBehaviour {

    /*
     * TODO
     *      DONE    Reorganize variable and method declaration, lay things out in an order that makes sense and is easy to read
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
     */

    [SerializeField] Text storyField;
    [SerializeField] Text optionsField;
    [SerializeField] TextMeshProUGUI chapterField;

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
            manageState();
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
            }
        }
    }

    private void manageState() {
        if(currentState.hasChapterTitle()) {
            chapterField.text = currentState.getChapterTitle();
        }
        storyField.text = currentState.getStoryText(p);
        if(!previousStateWasBasic) {
            currentState.managePlayer(p);
        }
        playSoundEffects();
        setAvailableStates();
        optionsField.text = "";
        if(availableStates.Count == 0) {
            optionsField.text = "Q - Quit";
        } else if(!currentState.getChildStates()[0].getOptText().Equals("(Continue)") && !currentState.getChildStates()[0].getOptText().Equals("(Press Enter to Continue)")) {
            for(int i = 0; i < availableStates.Count; i++) {
                optionsField.text += (i + 1) + " - " + availableStates[i].getOptText() + "\n";
            }
            optionsField.text += "I - Inventory\nJ - Journal\nQ - Quit";
        } else {
            optionsField.text += availableStates[0].getOptText();
        }
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
        storyField.text = invText;
        if(page * 3 + 3 >= inventoryEntries.Count) {
            optionsField.text = "(Return)";
        } else {
            optionsField.text = "1 - Next Page\n2 - Return";
        }
    }

    private void readJournal() {
        List<string> journalEntries = p.getJournalEntries();
        string journalText = "";
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
        storyField.text = journalText;
        if(page * 3 + 3 >= journalEntries.Count) {
            optionsField.text = "(Return)";
        } else {
            optionsField.text = "1 - Next Page\n2 - Return";
        }
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
