using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleHelper {

    public string getClue(string chapter, string flag) {
        if(chapter.Equals("PT")) {
            return getCluePT(flag);
        } else {
            return "";
        }
    }

    public List<string> getPossibleConclusionsFlags(string chapter, List<string> playerConclusions, List<string> cluesToCombine) {
        if(chapter.Equals("PT")) {
            return getPossibleConclusionsFlagsPT(playerConclusions, cluesToCombine);
        } else {
            return new List<string>();
        }
    }

    public string getConclusionText(string chapter, string conclusion) {
        if(chapter.Equals("PT")) {
            return getConclusionTextPT(conclusion);
        } else {
            return "";
        }
    }

    private string getCluePT(string flag) {
        if(flag.Equals("flag_a")) {
            return "clue_a";
        } else if(flag.Equals("flag_b")) {
            return "clue_b";
        } else if(flag.Equals("flag_c")) {
            return "clue_c";
        } else if(flag.Equals("flag_e")) {
            return "clue_e";
        } else if(flag.Equals("flag_i")) {
            return "clue_i";
        } else if(flag.Equals("flag_j")) {
            return "clue_j";
        } else if(flag.Equals("flag_k")) {
            return "clue_k";
        } else {
            return "";
        }
    }

    private List<string> getPossibleConclusionsFlagsPT(List<string> playerConclusions, List<string> cluesToCombine) {
        List<string> possibleConclusions = new List<string>();
        if(cluesToCombine.Contains("clue_a")) {
            if(cluesToCombine.Contains("clue_c")) {
                possibleConclusions.AddRange(new string[] { "ded_b", "ded_c", "ded_d" });
            } else if(cluesToCombine.Contains("clue_e")) {
                possibleConclusions.AddRange(new string[] { "ded_a", "ded_d", "ded_e" });
            } else if(cluesToCombine.Contains("clue_i")) {
                possibleConclusions.AddRange(new string[] { "ded_a", "ded_b", "ded_f" });
            }
        } else if(cluesToCombine.Contains("clue_b")) {
             if(cluesToCombine.Contains("clue_c")) {
                possibleConclusions.AddRange(new string[] { "ded_d", "ded_g", "ded_h" });
             } else if(cluesToCombine.Contains("clue_k")) {
                possibleConclusions.AddRange(new string[] { "ded_c", "ded_f", "ded_i" });
             }
        } else if(cluesToCombine.Contains("clue_c")) {
             if(cluesToCombine.Contains("clue_e")) {
                possibleConclusions.AddRange(new string[] { "ded_g", "ded_h", "ded_i" });
             }
        } else if(cluesToCombine.Contains("clue_e")) {
            if(cluesToCombine.Contains("clue_i")) {
                possibleConclusions.AddRange(new string[] { "ded_b", "ded_j", "ded_k" });
            } else if(cluesToCombine.Contains("clue_j")) {
                possibleConclusions.AddRange(new string[] { "ded_l", "ded_m", "ded_n" });
            }
        } else if(cluesToCombine.Contains("clue_i")) {
            if(cluesToCombine.Contains("clue_j")) {
                possibleConclusions.AddRange(new string[] { "ded_j", "ded_o", "ded_p" });
            }
        } else if(cluesToCombine.Contains("clue_j") && cluesToCombine.Contains("clue_k")) {
            possibleConclusions.AddRange(new string[] { "ded_a", "ded_e", "ded_n" });
        }

        for(int i = possibleConclusions.Count - 1; i >= 0; i--) {
            if(playerConclusions.Contains(possibleConclusions[i])) {
                possibleConclusions.RemoveAt(i);
            }
        }

        return possibleConclusions;
    }

    private string getConclusionTextPT(string conclusion) {
        if(conclusion.Equals("ded_a")) {
            return "Deduction A";
        } else if(conclusion.Equals("ded_b")) {
            return "Deduction B";
        } else if(conclusion.Equals("ded_c")) {
            return "Deduction C";
        } else if(conclusion.Equals("ded_d")) {
            return "Deduction D";
        } else if(conclusion.Equals("ded_e")) {
            return "Deduction E";
        } else if(conclusion.Equals("ded_f")) {
            return "Deduction F";
        } else if(conclusion.Equals("ded_g")) {
            return "Deduction G";
        } else if(conclusion.Equals("ded_h")) {
            return "Deduction H";
        } else if(conclusion.Equals("ded_i")) {
            return "Deduction I";
        } else if(conclusion.Equals("ded_j")) {
            return "Deduction J";
        } else if(conclusion.Equals("ded_k")) {
            return "Deduction K";
        } else if(conclusion.Equals("ded_l")) {
            return "Deduction L";
        } else if(conclusion.Equals("ded_m")) {
            return "Deduction M";
        } else if(conclusion.Equals("ded_n")) {
            return "Deduction N";
        } else if(conclusion.Equals("ded_o")) {
            return "Deduction O";
        } else if(conclusion.Equals("ded_p")) {
            return "Deduction P";
        } else {
            return "";
        }
    }
}
