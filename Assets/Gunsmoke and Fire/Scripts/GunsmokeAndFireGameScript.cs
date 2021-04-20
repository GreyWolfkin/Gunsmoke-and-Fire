using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunsmokeAndFireGameScript : MonoBehaviour {

    // These AudioSource lists only exist until a better solution can be found
    // Ideally I'd like to assign AudioSource components to the State, but I can't figure out how to do that
    // So right now the clunky solution is the one we've got
    [SerializeField] List<AudioSource> soundEffectFiles;
    [SerializeField] List<State> soundEffectTriggerStates;

    //
    KeyCode[] codes1;
    KeyCode[] codes2;
    KeyCode[] codes3;
    KeyCode[] codes4;
    KeyCode[] codes5;
    KeyCode[] codes6;
    KeyCode[] codes7;
    KeyCode[] codes8;
    KeyCode[] codes9;
    KeyCode[] codes0;
    KeyCode[] codesBasic;
    KeyCode[][] codes;

    string[] stringCodes;


    [SerializeField] Text storyField;
    [SerializeField] Text optionsField;
    [SerializeField] TextMeshProUGUI chapterField;
    [SerializeField] State startingState;
    // [SerializeField] State invState;
    // [SerializeField] State journalState;

    State basicState;

    State invState;
    State journalState;

    State currentState;
    State[] childStates;
    List<State> availableStates = new List<State>();

    bool previousStateWasBasic = false;

    [SerializeField] Player p;

    World world;

    int page = 0;

    // Start is called before the first frame update
    void Start() {

        basicState = ScriptableObject.CreateInstance<State>();

        invState = ScriptableObject.CreateInstance<State>();
        journalState = ScriptableObject.CreateInstance<State>();

        codes1 = new KeyCode[] { KeyCode.Alpha1, KeyCode.Keypad1, KeyCode.Return, KeyCode.KeypadEnter };
        codes2 = new KeyCode[] { KeyCode.Alpha2, KeyCode.Keypad2 };
        codes3 = new KeyCode[] { KeyCode.Alpha3, KeyCode.Keypad3 };
        codes4 = new KeyCode[] { KeyCode.Alpha4, KeyCode.Keypad4 };
        codes5 = new KeyCode[] { KeyCode.Alpha5, KeyCode.Keypad5 };
        codes6 = new KeyCode[] { KeyCode.Alpha6, KeyCode.Keypad6 };
        codes7 = new KeyCode[] { KeyCode.Alpha7, KeyCode.Keypad7 };
        codes8 = new KeyCode[] { KeyCode.Alpha8, KeyCode.Keypad8 };
        codes9 = new KeyCode[] { KeyCode.Alpha9, KeyCode.Keypad9 };
        codes0 = new KeyCode[] { KeyCode.Alpha0, KeyCode.Keypad0 };
        codesBasic = new KeyCode[] { KeyCode.Q, KeyCode.J, KeyCode.I };
        codes = new KeyCode[][] { codes1, codes2, codes3, codes4, codes5, codes6, codes7, codes8, codes9, codes0, codesBasic };

        stringCodes = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "q", "i", "j" };

        world = new World();
        currentState = startingState;
        if(currentState.initializesPlayer()) {
            currentState.initPlayer(p);
        }
        p.setCurrentState(currentState);
        setAvailableStates();
        storyField.text = currentState.getStoryText(p);
        ManageState();
    }

    // Update is called once per frame
    void Update() {
        /*
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            Debug.Log("Enter");
        } else if(Input.inputString == "1") {  
            Debug.Log("1");
        }
        */
        listen();
    }

    private int getIndex(string code) {
        for(int i = 0; i < stringCodes.Length; i++) {
            if(code == stringCodes[i]) {
                return i;
            }
        }
        return -1;
    }

    private void listen() {

        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            if(currentState != invState && currentState != journalState) {
                manageKeystroke(0);
                return;
            } else {
                basicStateKeystroke(0);
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
            basicStateKeystroke(getIndex(input));
            return;
        }

        /*

        KeyCode[] codes = { KeyCode.Alpha1, KeyCode.Keypad1, KeyCode.Return, KeyCode.KeypadEnter, KeyCode.Alpha2, KeyCode.Keypad2, KeyCode.Alpha3, KeyCode.Keypad3, KeyCode.Alpha4, KeyCode.Keypad4, KeyCode.Alpha5, KeyCode.Keypad5, KeyCode.Alpha6, KeyCode.Keypad6, KeyCode.Alpha7, KeyCode.Keypad7, KeyCode.Alpha8, KeyCode.Keypad8, KeyCode.Alpha9, KeyCode.Keypad9, KeyCode.Alpha0, KeyCode.Keypad0, KeyCode.I, KeyCode.J, KeyCode.Q };

        foreach(KeyCode code in codes) {
            if(Input.GetKeyDown(code)) {
                if(currentState != journalState && currentState != invState) {
                    manageKeystroke(code);
                } else {
                    basicStateKeystroke(code);
                }
            }
        }
        */
    }

    private bool containsKey(KeyCode[] keyArr, KeyCode code) {
        foreach(KeyCode key in keyArr) {
            if(code == key) {
                return true;
            }
        }
        return false;
    }

    private bool containsKey(KeyCode code) {
        foreach(KeyCode[] codeArr in codes) {
            if(containsKey(codeArr, code)) {
                return true;
            }
        }
        return false;
    }

    private int indexOfKeystroke(KeyCode code) {
        for(int i = 0; i < codes.Length; i++) {
            if(containsKey(codes[i], code)) {
                return i;
            }
        }
        return -1;
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
            ManageState();
        }
    }

    private void manageKeystroke(KeyCode code) {

        if(code == KeyCode.Q) {
            // UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        } else if(code == KeyCode.I) {
            // currentState = invState;
            currentState = basicState;
            previousStateWasBasic = true;
            readInventory();
        } else if(code == KeyCode.J) {
            // currentState = journalState;
            currentState = basicState;
            previousStateWasBasic = true;
            readJournal();
        } else {
            changeState(indexOfKeystroke(code));
            ManageState();

            /*
            for(int i = 0; i < codesM.Length; i++) {
                if(codesM[i].Contains(code)) {
                    ManageState(i);
                }
            }
            */
        }
    }

    private void basicStateKeystroke(int code) {
        if((currentState == journalState && p.getJournalEntries().Count <= page * 3 + 3) ||
            (currentState == invState && p.getInventoryEntries().Count <= page * 3 + 3)) {
            // if(codes1.Contains(code)) {
            // if(containsKey(codes1, code)) {
            if(code == 0) { 
                page = 0;
                currentState = p.getCurrentState();
                storyField.text = currentState.getStoryText(p);
                optionsField.text = "";
                if(!currentState.getChildStates()[0].getOptText().Equals("(Continue)") && !currentState.getChildStates()[0].getOptText().Equals("(Press Enter to Continue)")) {
                    for(int i = 0; i < availableStates.Count; i++) {
                        optionsField.text += (i + 1) + " - " + availableStates[i].getOptText() + "\n";
                    }
                    optionsField.text += "I - Inventory\nJ - Journal\nQ - Quit";
                } else {
                    optionsField.text += availableStates[0].getOptText();
                }
                previousStateWasBasic = false;
            }
        } else {
            // if(codes1.Contains(code)) {
            // if(containsKey(codes1, code)) {
            if(code == 0) {
                page++;
                if(currentState == journalState) {
                    readJournal();
                } else if(currentState == invState) {
                    readInventory();
                }
            //} else if(codes2.Contains(code)) {
            // } else if(containsKey(codes2, code)) {
            } else if(code == 1) { 
                page = 0;
                currentState = p.getCurrentState();
                storyField.text = currentState.getStoryText(p);
                optionsField.text = "";
                if(!currentState.getChildStates()[0].getOptText().Equals("(Continue)") && !currentState.getChildStates()[0].getOptText().Equals("(Press Enter to Continue)")) {
                    for(int i = 0; i < availableStates.Count; i++) {
                        optionsField.text += (i + 1) + " - " + availableStates[i].getOptText() + "\n";
                    }
                    optionsField.text += "I - Inventory\nJ - Journal\nQ - Quit";
                } else {
                    optionsField.text += availableStates[0].getOptText();
                }
                previousStateWasBasic = false;
            }
        }
    }

    private void basicStateKeystroke(KeyCode code) {
        if((currentState == journalState && p.getJournalEntries().Count <= page * 3 + 3) ||
            (currentState == invState && p.getInventoryEntries().Count <= page * 3 + 3)) {
            // if(codes1.Contains(code)) {
            if(containsKey(codes1, code)) { 
                page = 0;
                currentState = p.getCurrentState();
                storyField.text = currentState.getStoryText(p);
                optionsField.text = "";
                if(!currentState.getChildStates()[0].getOptText().Equals("(Continue)") && !currentState.getChildStates()[0].getOptText().Equals("(Press Enter to Continue)")) {
                    for(int i = 0; i < availableStates.Count; i++) {
                        optionsField.text += (i + 1) + " - " + availableStates[i].getOptText() + "\n";
                    }
                    optionsField.text += "I - Inventory\nJ - Journal\nQ - Quit";
                } else {
                    optionsField.text += availableStates[0].getOptText();
                }
                previousStateWasBasic = false;
            }
        } else {
            // if(codes1.Contains(code)) {
            if(containsKey(codes1, code)) {
                page++;
                if(currentState == journalState) {
                    readJournal();
                } else if(currentState == invState) {
                    readInventory();
                }
            //} else if(codes2.Contains(code)) {
            } else if(containsKey(codes2, code)) { 
                page = 0;
                currentState = p.getCurrentState();
                storyField.text = currentState.getStoryText(p);
                optionsField.text = "";
                if(!currentState.getChildStates()[0].getOptText().Equals("(Continue)") && !currentState.getChildStates()[0].getOptText().Equals("(Press Enter to Continue)")) {
                    for(int i = 0; i < availableStates.Count; i++) {
                        optionsField.text += (i + 1) + " - " + availableStates[i].getOptText() + "\n";
                    }
                    optionsField.text += "I - Inventory\nJ - Journal\nQ - Quit";
                } else {
                    optionsField.text += availableStates[0].getOptText();
                }
                previousStateWasBasic = false;
            }
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

    private void changeState(int opt) {
        for(int i = 0; i < availableStates.Count; i++) {
            if(opt == i) {
                currentState = availableStates[opt];
                p.setCurrentState(currentState);
                storyField.text = currentState.getStoryText(p);
                if(!previousStateWasBasic) {
                    currentState.managePlayer(p);
                }
                previousStateWasBasic = false;
            }
        }
    }

    private void ManageState() {
        if(currentState.hasChapterTitle()) {
            chapterField.text = currentState.getChapterTitle();
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
}
