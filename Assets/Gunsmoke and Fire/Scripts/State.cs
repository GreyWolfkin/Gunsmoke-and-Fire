using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject {

    [SerializeField] string stateName;

    [SerializeField] bool basic = false;
    [SerializeField] bool initializePlayer = false;
    [TextArea(10, 14)] [SerializeField] string storyText;
    [TextArea(2, 5)] [SerializeField] string optText;
    [SerializeField] string chapterTitle;
    [SerializeField] bool locked = false;
    [SerializeField] State[] childStates;

    [TextArea(10, 14)] [SerializeField] string parameters;
    [TextArea(10, 14)] [SerializeField] string setFunctions;
    [TextArea(10, 14)] [SerializeField] string initialSettings;

    [SerializeField] AudioClip audioClip;

    public string getStateName() {
        return stateName;
    }

    public bool isBasic() {
        return basic;
    }

    public bool initializesPlayer() {
        return initializePlayer;
    }

    public string getStoryText(Player p) {
        return storyText;
    }
    public void setStoryText(string set) {
        storyText = set;
    }

    public string getOptText() {
        return optText;
    }
    public void setOptText(string set) {
        optText = set;
    }

    public string getChapterTitle() {
        return chapterTitle;
    }
    public bool hasChapterTitle() {
        if(chapterTitle.Length > 0) {
            return true;
        } else {
            return false;
        }
    }

    public bool isLocked() {
        return locked;
    }
    public void setLocked(bool set) {
        locked = set;
    }

    public State[] getChildStates() {
        return childStates;
    }

    public AudioClip getAudioClip() {
        return audioClip;
    }

    public bool isAvailable(Player p) {

        if(parameters.Length == 0) {
            return true;
        }

        string[] parameterArr = parameters.Split('|');
        foreach(string parameter in parameterArr) {
            string[] set = parameter.Split('%');
            switch(set[1]) {
                case "VAR_REQ":
                    if(!varCheck(p, set)) {
                        return false;
                    }
                    break;
                case "FLAG_REQ":
                    if(!p.hasFlag(set[0])) {
                        return false;
                    }
                    break;
                case "ITEM_REQ":
                    if(!p.hasItem(set[0])) {
                        return false;
                    }
                    break;
                case "VAR_LOCK":
                    if(varCheck(p, set)) {
                        return false;
                    }
                    break;
                case "FLAG_LOCK":
                    if(p.hasFlag(set[0])) {
                        return false;
                    }
                    break;
                case "ITEM_LOCK":
                    if(p.hasItem(set[0])) {
                        return false;
                    }
                    break;
                default:
                    Debug.Log("Bad set in parameters.\nReceived " + set[1]);
                    return false;
            }
        }
        return true;
    }

    private bool varCheck(Player p, string[] set) {
        try {
            int statVal = p.getVarVal(set[0]);
            int goal = Int32.Parse(set[3]);
            if(set[2].Equals("==")) {
                if(statVal == goal) {
                    return true;
                } else {
                    return false;
                }
            } else if(set[2].Equals("!=")) {
                if(statVal != goal) {
                    return true;
                } else {
                    return false;
                }
            } else if(set[2].Equals(">")) {
                if(statVal > goal) {
                    return true;
                } else {
                    return false;
                }
            } else if(set[2].Equals(">=")) {
                if(statVal >= goal) {
                    return true;
                } else {
                    return false;
                }
            } else if(set[2].Equals("<")) {
                if(statVal < goal) {
                    return true;
                } else {
                    return false;
                }
            } else if(set[2].Equals("<=")) {
                if(statVal <= goal) {
                    return true;
                } else {
                    return false;
                }
            } else {
                Debug.Log("Bad operator passed to varCheck.\nReceived " + set[2]);
                return false;
            }
        } catch(FormatException) {
            Debug.Log("Bad value passed to varCheck.\nReceived " + set[3]);
            return false;
        } catch(IndexOutOfRangeException) {
            Debug.Log("Bad set passed to varCheck.\nThrew IndexOutOfRangeException");
            return false;
        }
    }

    public void managePlayer(Player p) {
        if(setFunctions.Length == 0) {
            return;
        }

        string[] setFunctionArrs = setFunctions.Split('|');
        foreach(string function in setFunctionArrs) {
            string[] set = function.Split('%');
            switch(set[1]) {
                case "VAR_SET":
                    manageVars(p, set);
                    break;
                case "FLAG_SET":
                    p.setFlag(set[0]);
                    break;
                case "ITEM_SET":
                    p.setItem(set[0]);
                    break;
                case "FLAG_KILL":
                    p.killFlag(set[0]);
                    break;
                case "ITEM_KILL":
                    p.killItem(set[0]);
                    break;
                default:
                    Debug.Log("Bad set in setFunctions.\nReceived " + set[1]);
                    return;
            }
        }
    }

    private void manageVars(Player p, string[] set) {
        try {
            int val = Int32.Parse(set[3]);
            if(set[2].Equals("=")) {
                p.setVarVal(set[0], val);
                return;
            } else if(set[2].Equals("+")) {
                p.adjVarVal(set[0], val);
                return;
            } else if(set[2].Equals("-")) {
                p.adjVarVal(set[0], -1 * val);
                return;
            } else {
                Debug.Log("Bad operator passed to manageVars.\nReceived " + set[2]);
                return;
            }
        } catch(FormatException) {
            Debug.Log("Bad value passed to manageVars.\nReceived " + set[3]);
            return;
        } catch(IndexOutOfRangeException) {
            Debug.Log("Bad set passed to manageVars.\nThrew IndexOutOfRangeException");
            return;
        }
    }

    public void initPlayer(Player p) {
        if(initialSettings.Length == 0) {
            return;
        }

        List<string> varNames = new List<string>();
        List<int> varVals = new List<int>();
        List<string> flags = new List<string>();
        List<string> items = new List<string>();

        bool clear = false;

        string[] settings = initialSettings.Split('|');
        string[] set = new string[3];
        try {
            foreach(string setting in settings) {
                set = setting.Split('%');
                switch(set[1]) {
                    case "CLEAR":
                        if(set[0].Equals("TRUE")) {
                            clear = true;
                        } else {
                            clear = false;
                        }
                        break;
                    case "VAR":
                        varNames.Add(set[0]);
                        varVals.Add(Int32.Parse(set[2]));
                        break;
                    case "FLAG":
                        flags.Add(set[0]);
                        break;
                    case "ITEM":
                        items.Add(set[0]);
                        break;
                    default:
                        Debug.Log("Bad value passed to initPlayer.\nReceived " + set[1]);
                        break;
                }
            }

            if(clear) {
                p.clearVars();
                p.clearFlags();
                p.clearItems();
            }
            foreach(string varName in varNames) {
                p.addVarName(varName);
            }
            foreach(int varVal in varVals) {
                p.addVarVal(varVal);
            }
            foreach(string flag in flags) {
                p.setFlag(flag);
            }
            foreach(string item in items) {
                p.setItem(item);
            }
        } catch(FormatException) {
            Debug.Log("Bad value passed to initPlayer.\nReceived " + set[2]);
        } catch(IndexOutOfRangeException) {
            Debug.Log("Bad set passed to initPlayer.\nThrew IndexOutOfRangeException");
        }
    }
}
