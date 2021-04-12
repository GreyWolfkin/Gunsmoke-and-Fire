using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunsmokeAndFireGameScript : MonoBehaviour {

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

    int journalPage = 0;

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

        // codesM = new KeyCode[][] { codes1, codes2, codes3, codes4, codes5, codes6, codes7, codes8, codes9, codes0 };

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

    private void listen() {

        KeyCode[] codes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Return, KeyCode.I, KeyCode.J, KeyCode.Q };

        foreach(KeyCode code in codes) {
            if(Input.GetKeyDown(code)) {
                if(currentState != journalState) {
                    manageKeystroke(code, codes);
                } else {
                    journalKeystroke(code);
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
                storyField.text = currentState.getStoryText(p);
                previousStateWasBasic = true;
                ManageState(-1);
                break;
            case KeyCode.J:
                currentState = journalState;
                readJournal();

                break;
                /*
                currentState = journalState;
                storyField.text = currentState.getStoryText(p);
                previousStateWasBasic = true;
                ManageState(-1);
                break;
                */
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

    private void journalKeystroke(KeyCode code) {
        if(p.getJournalEntries().Count <= journalPage * 3 + 3) {
            if(codes1.Contains(code)) {
                journalPage = 0;
                currentState = p.getCurrentState();
                storyField.text = currentState.getStoryText(p);
                optionsField.text = "";
                for(int i = 0; i < availableStates.Count; i++) {
                    optionsField.text += (i + 1) + " - " + availableStates[i].getOptText() + "\n";
                }
                optionsField.text += "I - Inventory\nJ - Journal\nQ - Quit";
            }
        } else {
            if(codes1.Contains(code)) {
                journalPage++;
                readJournal();
            } else if(codes2.Contains(code)) {
                journalPage = 0;
                currentState = p.getCurrentState();
                storyField.text = currentState.getStoryText(p);
                optionsField.text = "";
                for(int i = 0; i < availableStates.Count; i++) {
                    optionsField.text += (i + 1) + " - " + availableStates[i].getOptText() + "\n";
                }
                optionsField.text += "I - Inventory\nJ - Journal\nQ - Quit";
            }
        }
    }

    private void readJournal() {
        Debug.Log("Journal Page = " + journalPage);
        List<string> journalEntries = p.getJournalEntries();
        string journalText = "";
        if(journalPage == 0) {
            journalText += "JOURNAL\n-------\n\n";
        }
        for(int i = journalPage * 3; i < journalEntries.Count; i++) {
            if(i != journalPage * 3) {
                journalText += "\n\n";
            }
            journalText += journalEntries[i];
            if(i == journalEntries.Count - 1 || i == journalPage * 3 + 2) {
                break;
            }
        }
        storyField.text = journalText;
        if(journalPage * 3 + 3 >= journalEntries.Count) {
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
}
