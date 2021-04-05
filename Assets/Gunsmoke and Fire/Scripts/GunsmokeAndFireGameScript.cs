using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunsmokeAndFireGameScript : MonoBehaviour {

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

    // Start is called before the first frame update
    void Start() {
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
                manageKeystroke(code, codes);
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
                storyField.text = currentState.getStoryText(p);
                previousStateWasBasic = true;
                ManageState(-1);
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
            Debug.Log(currentState.getChildStates()[0].getStoryText(p));
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
