using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Writer
{

    public static string[] getItem(string chapter, string item) {
        switch(chapter) {
            case "PT":
                return getItemPT(item);
            case "0":
                return getItem0(item);
            default:
                return new string[2];
        }
    }

    private static string[] getItemPT(string item) {
        string name = "";
        string entry = "";

        return new string[] { name, entry };
    }

    private static string[] getItem0(string item) {
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

    public static string getJournalEntry(string chapter, string flag) {
        switch(chapter) {
            case "PT":
                return getJournalEntryPT(flag);
            case "0":
                return getJournalEntry0(flag);
            default:
                return "";
        }
    }

    private static string getJournalEntryPT(string flag) {
        switch(flag) {
            case "ded_a":
                return "Deduction A";
            case "ded_b":
                return "Deduction B";
            case "ded_c":
                return "Deduction C";
            case "ded_d":
                return "Deduction D";
            case "ded_e":
                return "Deduction E";
            case "ded_f":
                return "Deduction F";
            case "ded_g":
                return "Deduction G";
            case "ded_h":
                return "Deduction H";
            case "ded_i":
                return "Deduction I";
            case "ded_j":
                return "Deduction J";
            case "ded_k":
                return "Deduction K";
            case "ded_l":
                return "Deduction L";
            case "ded_m":
                return "Deduction M";
            case "ded_n":
                return "Deduction N";
            case "ded_o":
                return "Deduction O";
            case "ded_p":
                return "Deduction P";
            default:
                return "";
        }
    }

    private static string getJournalEntry0(string flag) {
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
