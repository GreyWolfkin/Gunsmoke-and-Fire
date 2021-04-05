using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player")]
public class Player : ScriptableObject {

    State currentState;

    string[] varNames;
    int[] varVals;
    List<string> flags;
    List<string> items;

    public State getCurrentState() {
        return currentState;
    }
    public void setCurrentState(State set) {
        currentState = set;
    }

    public string[] getVarNames() {
        return varNames;
    }
    public void setVarNames(string[] set) {
        varNames = set;
    }

    public int[] getVarVals() {
        return varVals;
    }
    public void setVarVals(int[] set) {
        varVals = set;
    }
    public void setVarVal(string name, int set) {
        int index = getVarIndex(name);
        varVals[index] = set;
    }
    public void adjVarVal(string name, int adj) {
        int index = getVarIndex(name);
        varVals[index] += adj;
    }
    public int getVarVal(string name) {
        int index = getVarIndex(name);
        return varVals[index];
    }
    private int getVarIndex(string name) {
        for(int i = 0; i < varNames.Length; i++) {
            if(varNames[i].Equals(name)) {
                return i;
            }
        }
        return -1;
    }

    public List<string> getFlags() {
        return flags;
    }
    public void setFlags(List<string> set) {
        flags = set;
    }
    public void setFlag(string flag) {
        flags.Add(flag);
    }
    public void killFlag(string flag) {
        flags.Remove(flag);
    }
    public bool hasFlag(string flag) {
        if(flags.Contains(flag)) {
            return true;
        } else {
            return false;
        }
    }
    private int getFlagIndex(string name) {
        for(int i = 0; i < flags.Count; i++) {
            if(flags[i].Equals(name)) {
                return i;
            }
        }
        return -1;
    }

    public List<string> getItems() {
        return items;
    }
    public void setItems(List<string> set) {
        items = set;
    }
    public bool hasItem(string item) {
        if(items.Contains(item)) {
            return true;
        } else {
            return false;
        }
    }
    public void setItem(string item) {
        items.Add(item);
    }
    public void killItem(string item) {
        items.Remove(item);
    }
    private int getItemIndex(string name) {
        for(int i = 0; i < items.Count; i++) {
            if(items[i].Equals(name)) {
                return i;
            }
        }
        return -1;
    }

    public bool meetsRequirements(List<string> reqFlags, List<string> lockFlags, List<string> reqItems, List<string> lockItems) {
        foreach(string flag in reqFlags) {
            if(!hasFlag(flag)) {
                return false;
            }
        }
        foreach(string flag in lockFlags) {
            if(hasFlag(flag)) {
                return false;
            }
        }
        foreach(string item in reqItems) {
            if(!hasItem(item)) {
                return false;
            }
        }
        foreach(string item in lockItems) {
            if(hasItem(item)) {
                return false;
            }
        }
        return true;
    }

    public string getInv() {
        string invString = "";
        foreach(string item in items) {
            // Add an entry for each item the player can carry
        }
        return invString;
    }
}
