﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player {

    string currentState;

    List<string> varNames = new List<string>();
    List<int> varVals = new List<int>();
    List<string> flags = new List<string>();
    List<string> items = new List<string>();

    List<string> deductions = new List<string>();

    List<string> journalEntries = new List<string>();
    List<string> inventoryEntries = new List<string>();

    public string getCurrentState() {
        return currentState;
    }
    public void setCurrentState(string set) {
        currentState = set;
    }

    public List<string> getVarNames() {
        return varNames;
    }
    public void setVarNames(List<string> set) {
        varNames = set;
    }
    public void addVarName(string var) {
        varNames.Add(var);
    }
    public void addVarNames(List<string> vars) {
        varNames.AddRange(vars);
    }

    public List<int> getVarVals() {
        return varVals;
    }
    public void setVarVals(List<int> set) {
        varVals = set;
    }
    public void addVarVal(int val) {
        varVals.Add(val);
    }
    public void addVarVals(List<int> vals) {
        varVals.AddRange(vals);
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
        for(int i = 0; i < varNames.Count; i++) {
            if(varNames[i].Equals(name)) {
                return i;
            }
        }
        return -1;
    }

    public void clearVars() {
        varNames.Clear();
        varVals.Clear();
    }

    public List<string> getFlags() {
        return flags;
    }
    public void setFlags(List<string> set) {
        flags = set;
    }
    public void setFlag(string flag) {
        flags.Add(flag);

        // FOR FLAGS THAT ADD JOURNAL ENTRIES, ADD RELEVANT ENTRY HERE
        // default will catch any flag that does not add a journal entry, and return before anything is added

        string entry = "";

        switch(flag) {
            case "clue_a":
                entry = "I found clue A";
                break;
            case "clue_b":
                entry = "I found clue B";
                break;
            case "clue_c":
                entry = "I found clue C";
                break;
            case "clue_d":
                entry = "I found clue D";
                break;
            case "clue_e":
                entry = "I found clue E";
                break;
            case "clue_f":
                entry = "I found clue F";
                break;
            case "examined_desk":
                entry = "My desk had several bullet holes in the top. I must have taken cover behind it during the shoot-out.";
                break;
            case "examined_walls":
                entry = "Found bullet holes and scorch marks on the walls. Whoever I tussled with brought a gun and magic too.";
                break;
            case "examined_self":
                entry = "Got a nasty head wound. Just a hair to the left and I'd have a pretty new hole to whistle outta. They must have thought they did the job proper and left me there.";
                break;
            case "examined_filing_cabinets":
                entry = "They cleaned out the office of my files on the Atellena case. Someone doesn't want that little girl found.";
                break;
            default:
                return;
        }

        journalEntries.Insert(0, entry);
    }
    public void addFlags(List<string> flags) {
        flags.AddRange(flags);
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
    public void clearFlags() {
        flags.Clear();
        journalEntries.Clear();
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
        
        // FOR ITEMS THAT ADD INVENTORY ENTRIES, ADD RELEVANT ENTRY HERE
        // default will catch any flag that does not add an inventory entry, and return before anything is added

        string entry = "";

        switch(item) {
            case "bottle_of_bourbon":
                entry = "Bottle of Bourbon\n\tThe good stuff. Unopened.";
                break;
            default:
                return;
        }

        inventoryEntries.Insert(0, entry);
    }
    public void addItems(List<string> items) {
        items.AddRange(items);
    }
    public void killItem(string item) {
        items.Remove(item);
    }
    public void clearItems() {
        items.Clear();
        inventoryEntries.Clear();
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

    public string getInventory() {
        string inventoryString = "";
        foreach(string item in items) {
            // Add an entry for each item the player can carry
        }
        return inventoryString;
    }

    public List<string> getInventoryEntries() {
        return inventoryEntries;
    }

    public List<string> getDeductions() {
        return deductions;
    }

    public List<string> getJournalEntries() {
        return journalEntries;
    }

    public string getJournalText() {

        string journalText = "JOURNAL\n-------\n";
        for(int i = journalEntries.Count - 1; i >= 0; i--) {
            journalText += journalEntries[i];
            if(i > 0) {
                journalText += "\n\n";
            }
        }
        return journalText;
    }
}
