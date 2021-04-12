using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunsmokeAndFireGameScript : MonoBehaviour {

    // TEST CODE
    [SerializeField] List<AudioSource> soundEffectFiles;
    [SerializeField] List<State> soundEffectTriggerStates;

    List<KeyCode> codes1;
    List<KeyCode> codes2;
    List<KeyCode> codes3;
    List<KeyCode> codes4;
    List<KeyCode> codes5;
    List<KeyCode> codes6;
    List<KeyCode> codes7;
    List<KeyCode> codes8;
    List<KeyCode> codes9;
    List<KeyCode> codes0;
    List<KeyCode> codesBasic;

    List<List<KeyCode>> codesM;

    // KeyCode[][] codesM;


    [SerializeField] Text storyField;
    [SerializeField] Text optionsField;
    [SerializeField] TextMeshProUGUI chapterField;
    [SerializeField] State startingState;
    [SerializeField] State invState;
    [SerializeField] State journalState;

    State currentState;
    State[] childStates;
    List<State> availableStates = new List<State>();
    State[] nextStates;
    List<State> tempStates;

    bool previousStateWasBasic = false;

    [SerializeField] Player p;

    World world;

    int page = 0;

    // Start is called before the first frame update
    void Start() {
        codes1 = new List<KeyCode>(new KeyCode[] { KeyCode.Alpha1, KeyCode.Keypad1, KeyCode.Return, KeyCode.KeypadEnter });
        codes2 = new List<KeyCode>(new KeyCode[] { KeyCode.Alpha2, KeyCode.Keypad2 });
        codes3 = new List<KeyCode>(new KeyCode[] { KeyCode.Alpha3, KeyCode.Keypad3 });
        codes4 = new List<KeyCode>(new KeyCode[] { KeyCode.Alpha4, KeyCode.Keypad4 });
        codes5 = new List<KeyCode>(new KeyCode[] { KeyCode.Alpha5, KeyCode.Keypad5 });
        codes6 = new List<KeyCode>(new KeyCode[] { KeyCode.Alpha6, KeyCode.Keypad6 });
        codes7 = new List<KeyCode>(new KeyCode[] { KeyCode.Alpha7, KeyCode.Keypad7 });
        codes8 = new List<KeyCode>(new KeyCode[] { KeyCode.Alpha8, KeyCode.Keypad8 });
        codes9 = new List<KeyCode>(new KeyCode[] { KeyCode.Alpha9, KeyCode.Keypad9 });
        codes0 = new List<KeyCode>(new KeyCode[] { KeyCode.Alpha0, KeyCode.Keypad0 });
        codesBasic = new List<KeyCode>(new KeyCode[] { KeyCode.I, KeyCode.J, KeyCode.Q });

        codesM = new List<List<KeyCode>>(new List<KeyCode>[] { codes1, codes2, codes3, codes4, codes5, codes6, codes7, codes8, codes9, codes0, codesBasic });

        world = new World();
        currentState = startingState;
        if(currentState.initializesPlayer()) {
            currentState.initPlayer(p);
        }
        p.setCurrentState(currentState);
        setAvailableStates();
        storyField.text = currentState.getStoryText(p);
        ManageState(-1);
    }

    // Update is called once per frame
    void Update() {
        listen();
    }

    // TEST CODE

    /*

    private void listen() {
        for(int i = 0; i < codesM.Count; i++) {
            for(int j = 0; i < codesM[i].Count; j++) {
                if(Input.GetKeyDown(codesM[i][j])) {
                    if(currentState != journalState && currentState != invState) {
                        manageKeystroke(i, j);
                    } else {
                        basicStateKeystroke(i);
                    }
                }
            }
        }
    }

    private void manageKeystroke(int code, int opt) {
        if(code == 10) {
            if(opt == 0) {
                currentState = invState;
                previousStateWasBasic = true;
                readInventory();
            } else if(opt == 1) {
                currentState = journalState;
                previousStateWasBasic = true;
                readJournal();
            } else if(opt == 2) {
                // UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
            }
        } else {
            ManageState(code);
        }
    }
    */

    private void listen() {

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
    }

    private void manageKeystroke(KeyCode code) {
        switch(code) {
            case KeyCode.Q:
                // UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
                break;
            case KeyCode.I:
                currentState = invState;
                previousStateWasBasic = true;
                readInventory();
                break;
            case KeyCode.J:
                currentState = journalState;
                previousStateWasBasic = true;
                readJournal();
                break;
            case KeyCode.Alpha1:
            case KeyCode.Keypad1:
            case KeyCode.Return:
            case KeyCode.KeypadEnter:
                ManageState(0);
                break;
            case KeyCode.Alpha2:
            case KeyCode.Keypad2:
                ManageState(1);
                break;
            case KeyCode.Alpha3:
            case KeyCode.Keypad3:
                ManageState(2);
                break;
            case KeyCode.Alpha4:
            case KeyCode.Keypad4:
                ManageState(3);
                break;
            case KeyCode.Alpha5:
            case KeyCode.Keypad5:
                ManageState(4);
                break;
            case KeyCode.Alpha6:
            case KeyCode.Keypad6:
                ManageState(5);
                break;
            case KeyCode.Alpha7:
            case KeyCode.Keypad7:
                ManageState(6);
                break;
            case KeyCode.Alpha8:
            case KeyCode.Keypad8:
                ManageState(7);
                break;
            case KeyCode.Alpha9:
            case KeyCode.Keypad9:
                ManageState(8);
                break;
            case KeyCode.Alpha0:
            case KeyCode.Keypad0:
                ManageState(9);
                break;
        }
    }

    // END OF TEST CODE

    /*
     *  GOOD CODE, UNCOMMENT IF BROKEN
     */

    /*
    private void listen() {

        KeyCode[] codes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Return, KeyCode.I, KeyCode.J, KeyCode.Q };

        foreach(KeyCode code in codes) {
            if(Input.GetKeyDown(code)) {
                if(currentState != journalState && currentState != invState) {
                    manageKeystroke(code, codes);
                } else {
                    basicStateKeystroke(code);
                }
            }
        }
    }

    private void manageKeystroke(KeyCode code, KeyCode[] codes) {
        switch(code) {
            case KeyCode.Q:
                // UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
                break;
            case KeyCode.I:
                currentState = invState;
                previousStateWasBasic = true;
                readInventory();
                break;
            case KeyCode.J:
                currentState = journalState;
                previousStateWasBasic = true;
                readJournal();
                break;
            case KeyCode.Return:
                ManageState(0);
                break;
            default:
                for(int i = 0; i < codes.Length; i++) {
                    if(codes[i] == code) {
                        ManageState(i);
                    }
                }
                break;
        }
    }

    */

    /*
     * END OF GOOD CODE
     */

    private void basicStateKeystroke(KeyCode code) {
        if((currentState == journalState && p.getJournalEntries().Count <= page * 3 + 3) ||
            (currentState == invState && p.getInventoryEntries().Count <= page * 3 + 3)) {
            if(codes1.Contains(code)) {
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
            if(codes1.Contains(code)) {
                page++;
                if(currentState == journalState) {
                    readJournal();
                } else if(currentState == invState) {
                    readInventory();
                }
            } else if(codes2.Contains(code)) {
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
    private void ManageState(int opt) {
        if(opt != -1) {
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
        if(currentState.hasChapterTitle()) {
            chapterField.text = currentState.getChapterTitle();
        }
        playSoundEffects();
        setAvailableStates();
        optionsField.text = "";
        if(!currentState.isBasic()) {
            if(!currentState.getChildStates()[0].getOptText().Equals("(Continue)") && !currentState.getChildStates()[0].getOptText().Equals("(Press Enter to Continue)")) {
                for(int i = 0; i < availableStates.Count; i++) {
                    optionsField.text += (i + 1) + " - " + availableStates[i].getOptText() + "\n";
                }
                optionsField.text += "I - Inventory\nJ - Journal\nQ - Quit";  
            } else {
                optionsField.text += availableStates[0].getOptText();
            }
        } else {
            optionsField.text += "(Return)";
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
        for(int i = 0; i < soundEffectTriggerStates.Count; i++) {
            if(soundEffectTriggerStates[i] == currentState && !previousStateWasBasic) {
                Debug.Log("Play SFX");
                soundEffectFiles[i].Play();
            }
        }
    }
}
