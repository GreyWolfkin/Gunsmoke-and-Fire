using System.Collections;
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

        string entry = Writer.getJournalEntry(flag);
        if(entry.Length != 0) {
            journalEntries.Insert(0, entry);
        }

        string killedEntry = Writer.killEntry(flag);
        if(killedEntry.Length != 0 && flags.Contains(killedEntry)) {
            journalEntries.Remove(Writer.getJournalEntry(killedEntry));
        }
    }
    public void addFlags(List<string> flags) {
        flags.AddRange(flags);
    }
    public void killFlag(string flag) {
        if(flags.Contains(flag)) {
            flags.Remove(flag);

            string entry = Writer.getJournalEntry(flag);
            if(entry.Length != 0) {
                journalEntries.Remove(entry);
            }
        } else {
            return;
        }
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

        string entry = Writer.getInventoryEntry(item);

        inventoryEntries.Insert(0, entry);
    }
    public void addItems(List<string> items) {
        items.AddRange(items);
    }
    public void killItem(string item) {
        items.Remove(item);

        string entry = Writer.getInventoryEntry(item);

        inventoryEntries.Remove(entry);
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

    public List<string> getInventoryEntries() {
        return inventoryEntries;
    }

    public List<string> getDeductions() {
        return deductions;
    }

    public List<string> getJournalEntries() {
        return journalEntries;
    }
}
