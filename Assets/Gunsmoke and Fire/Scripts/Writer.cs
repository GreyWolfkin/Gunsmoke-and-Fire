using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Writer
{

    public static string[] getItem(string item) {
        string name;
        string entry;

        switch(item) {
            case "bottle_of_bourbon":
                name = "Bottle of Bourbon";
                entry = "The good stuff. Unopened.";
                break;
            case "healing_potion":
                name = "Healing Potion";
                entry = "A vial of red, viscous liquid. Good for a quick pick-me-up.";
                break;
            default:
                name = "";
                entry = "";
                break;
        }

        return new string[] { name, entry };
    }
    
    public static string getInventoryEntry(string item) {
        string entry;

        switch(item) {
            case "bottle_of_bourbon":
                entry = "Bottle of Bourbon\n\tThe good stuff.Unopened.";
                break;
            case "healing_potion":
                entry = "Healing Potion\n\tA vial of red, viscous liquid. Good for a quick pick-me-up.";
                break;
            default:
                entry = "";
                break;
        }

        return entry;
    }

    public static string getJournalEntry(string flag) {
        string entry;

        switch(flag) {
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
                entry = "";
                break;
        }

        return entry;
    }

    // If a flag removes another journal entry, put the flag of the entry to be removed in case statement.
    public static string killEntry(string flag) {
        string entry;

        switch(flag) {
            case "test_clue_b":
                entry = "test_clue_b";
                break;
            default:
                entry = "";
                break;
        }

        return entry;
    }


}
