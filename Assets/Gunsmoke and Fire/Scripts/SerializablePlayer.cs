using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializablePlayer {

    string currentStateName;

    List<string> varNames = new List<string>();
    List<int> varVals = new List<int>();
    List<string> flags = new List<string>();
    List<string> items = new List<string>();

    List<string> deductions = new List<string>();

    List<string> journalEntries = new List<string>();
    List<string> inventoryEntries = new List<string>();

    public SerializablePlayer(Player p) {
        currentStateName = p.getCurrentState().getStateName();

        varNames = p.getVarNames();
        varVals = p.getVarVals();
        flags = p.getFlags();
        items = p.getItems();

        deductions = p.getDeductions();

        journalEntries = p.getJournalEntries();
        inventoryEntries = p.getInventoryEntries();
    }

    public string getCurrentStateName() {
        return currentStateName;
    }

    public List<string> getVarNames() {
        return varNames;
    }
    
    public List<int> getVarVals() {
        return varVals;
    }

    public List<string> getFlags() {
        return flags;
    }

    public List<string> getItems() {
        return items;
    }

    public List<string> getDeductions() {
        return deductions;
    }

    public List<string> getJournalEntries() {
        return journalEntries;
    }

    public List<string> getInventoryEntries() {
        return inventoryEntries;
    }
}
