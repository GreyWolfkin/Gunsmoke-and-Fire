using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunsmokeAndFireGameScript : MonoBehaviour {

    /*
     * TODO - CODE
     *      Finish commenting Engine code
     *      Cleanup Player code
     *      Comment Player code
     *      Cleanup State code
     *      Comment state code
     *      Build framework for puzzle-solving mechanic
     *          Player can access puzzle-solving at any time (D for Deductions?) AFTER the mechanic has been introduced. Way to prevent player from accessing it before that
     *          Boolean that gets tripped on specific screen?
     *      Fix naming convention on methods (even though I don't want to)
     *      Cleanup Start method, abstract to new methods
     *      Set up tabs in Journal?
     *          Contacts, Clues, Key Events?
     *          Ability to sort Journal Entries?
     *      Ability to temporarily blank out entries (for Chapter 1 and possibly subsequent chapters)
     *          Add boolean to states to use secondary, temporary journal? MAYBE
     *      Difficulty Settings (Maybe in Chapter 1?) that determine whether puzzle-solving is required or automatic
     *      Way to remove Journal Entries without removing flags (e.g. don't need Prologue entries after Prologue, but still need flags for what the player did)
     *      Create variable that can contain various option states? So that multiple instances of scriptable object don't need to be created
     *      
     */

    /*
     * TODO - STORY
     *      Add plot point of missing spells? Have only limited spells at the beginning, add more?    
     *      Get Playtester Opinion: Is repeating narrative text immersion breaking?
     *          e.g. Harald's Bar. You get the same introduction whether you've been there before or not. Do we need a different state for returning to the place? Or will the user accept repeated text?
     *      Flashback Chapter - 4 years ago, murder investigation, tutorial on puzzle solving mechanics, interrogation scene, then receive Atellena case
     *      Do we really need the instructional screens? Consider removing them? If the mechanics are well-designed, you shouldn't need a tutorial
     *          Feedback from Katie - Information is important, but maybe there's a better way to relay it
     * 
     */

    /*
     * BUGS
     *    When starting a new game, previously selected deductions are still held, cannot be chosen again
     */

    /*
     * CURRENT DEVLOG
     *      
     */

    /*
     * COMPLETED TASKS
     *      Reorganize variable and method declaration, lay things out in an order that makes sense and is easy to read
     *      Create fade in/out for text
     *      Standardize use of "inventory" vs "inv"
     *      Clean up "Continue" option checks
     *      Clean up blurry text
     *      Pick a better text font
     *      Add Save/Load functionality
     *      Finish Options screens
     *      Abstract some code from Listener back to Update
     *      Find a cleaner way to store audio files and state triggers
     *      Ability to remove items and flags, removing their entries as well
     *      Ability to count qty of items in inventory, and display that qty in Inventory screen
     *      Add ability to remove journal entries
     *      Add ability to remove inventory items
     *      Figure out a way to load only scenes for each chapter at a time?
     */

    // Text Fields for displaying story text and options text.
    // chapterField uses TextMeshProUGUI for fancy "noir" font
    [SerializeField] Text storyField;
    [SerializeField] Text optionsField;
    [SerializeField] TextMeshProUGUI chapterField;

    // Fields which store temporary story and options text to fade in/out over existing text
    [SerializeField] Text overlayStoryField;
    [SerializeField] Text overlayOptionsField;

    [SerializeField] State[] chapterStates;

    Player p;
    List<State> statesList;
    State[] states;
    string[] stateNames;

    // currentState stores temporary values that can modify Player.currentState
    State currentState;

    [SerializeField] State startingState;

    // childStates uses an array and is populated by State.getChildStates
    // availableStates uses a List because it needs to add states one at a time from childStates if they meet necessary criteria
    State[] childStates;
    List<State> availableStates = new List<State>();

    // Populated with accepted input values. Uses index of input value for navigation
    string[] codes;

    // If the previous state was Inventory or Journal (i.e. "basic states"), then navigating back to the primary state should not reinitialize variables or replay SFX
    // previousStateWasBasic is set <true> when navigating to Inventory or Journal, and set <false> when navigating back to primary state or to another non-basic state
    bool previousStateWasBasic = false;

    // inventoryState and journalState need to be created within Start so that currentState can be set to these for navigation and read purposes
    string stateType;
    State inventoryState;
    State journalState;
    State optionsState;
    State saveState;
    State loadState;
    State quitState;
    State newGameState;

    // What "page" of inventory or journal the user is looking at. Resets to 0 when leaving the inventory or journal state
    int page = 0;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start() {


        /*

        statesList = new List<State>();

        AddChildStatesToStatesList(startingState);
        states = statesList.ToArray();
        stateNames = new string[states.Length];

        for(int i = 0; i < states.Length; i++) {
            stateNames[i] = states[i].getStateName();
        }
        */

        // Set Alpha to 0 for overlayStoryField and overlayOptionsField
        overlayStoryField.CrossFadeAlpha(0, 0.0f, false);
        overlayOptionsField.CrossFadeAlpha(0, 0.0f, false);

        // Set story and options to empty, so that starting state can fade in
        storyField.text = "";
        optionsField.text = "";

        // Create instance of State for inventoryState and journalState
        inventoryState = ScriptableObject.CreateInstance<State>();
        journalState = ScriptableObject.CreateInstance<State>();
        optionsState = ScriptableObject.CreateInstance<State>();
        saveState = ScriptableObject.CreateInstance<State>();
        loadState = ScriptableObject.CreateInstance<State>();
        quitState = ScriptableObject.CreateInstance<State>();
        newGameState = ScriptableObject.CreateInstance<State>();

        codes = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "i", "j", "c", "o" };

        currentState = startingState;
        SetStatesList();

        stateType = currentState.getStateName();

        // p = ScriptableObject.CreateInstance<Player>();
        p = new Player();

        // If starting state is set to a different state for debugging purposes, or if loading a saved game, if the starting state does not have the initializesPlayer bool, do not reinitialize player variables
        p.setCurrentState(currentState.getStateName());
        if(currentState.initializesPlayer()) {
            currentState.initPlayer(p);
        }
        manageState();
    }

    void SetStatesList() {
        if(statesList != null) {
            statesList.Clear();
        } else {
            statesList = new List<State>();
        }
        AddChildStatesToStatesList(currentState);
        states = statesList.ToArray();
        stateNames = new string[states.Length];

        for(int i = 0; i < states.Length; i++) {
            stateNames[i] = states[i].getStateName();
        }
    }

    void AddChildStatesToStatesList(State state) {
        if(!statesList.Contains(state)) {
            statesList.Add(state);
            foreach(State childState in state.getChildStates()) {
                if(SameChapter(state, childState)) {
                    AddChildStatesToStatesList(childState);
                }
            }
        } else {
            return;
        }
    }

    bool SameChapter(State parentState, State childState) {
        string parentChapter = GetChapter(parentState.getStateName());
        string childChapter = GetChapter(childState.getStateName());
        if(childChapter.Equals(parentChapter)) {
            return true;
        } else {
            return false;
        }
    }

    string GetChapter(string stateName) {
        string chapter = "";
        for(int i = 0; i < stateName.Length; i++) {
            if(stateName[i] != '-') {
                chapter += stateName[i];
            } else {
                return chapter;
            }
        }
        return chapter;
    }

    // Update is called once per frame
    void Update() {

        // if(currentState == optionsState) {
        if(stateType.Equals("options")) {
            optionsListener();
            // } else if(
        } else if(
            stateType.Equals("save") ||
            stateType.Equals("load") ||
            stateType.Equals("new") ||
            stateType.Equals("quit")
            ) {
            // currentState == saveState ||
            //currentState == loadState ||
            //currentState == newGameState ||
            //currentState == quitState
            // ) {
            internalOptionsListener();
            // } else if(currentState == puzzleState) {
        } else {
            genericListener();
        }
    }

    private void genericListener() {
        if(!previousStateWasBasic) {
            if(
                (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) &&
                currentStateHasContinueOption()
            ) {
                manageKeystroke(0);
                return;
            }
        } else {
            if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                if(
                    (currentState == inventoryState && !hasAdditionalPages("inventory")) ||
                    (currentState == journalState && !hasAdditionalPages("journal"))
                ) {
                    manageBasicKeystroke(0);
                    return;
                }
            }

        }

        string input = Input.inputString.ToLower();
        if(input.Length == 0) {
            return;
        }
        if(!previousStateWasBasic) {
            manageKeystroke(getCodeIndex(input));
        } else if(stateType.Equals("puzzle")) {
            puzzleListener(getCodeIndex(input));
        } else {
            manageBasicKeystroke(getCodeIndex(input));
        }
        return;
    }

    private void optionsListener() {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            returnState();
            return;
        } else if(Input.GetKeyDown(KeyCode.S)) {
            currentState = saveState;
            stateType = "save";
            ConfirmSave();
            return;
        } else if(Input.GetKeyDown(KeyCode.L)) {
            currentState = loadState;
            stateType = "load";
            ConfirmLoad();
            return;
        } else if(Input.GetKeyDown(KeyCode.N)) {
            currentState = newGameState;
            stateType = "new";
            ConfirmNew();
            return;
        } else if(Input.GetKeyDown(KeyCode.Q)) {
            currentState = quitState;
            stateType = "quit";
            ConfirmQuit();
            return;
        } else {
            return;
        }
    }

    private void internalOptionsListener() {
        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
            // if(currentState == saveState) {
            if(stateType.Equals("save")) {
                SaveGame();
                return;
                // } else if(currentState == loadState) {
            } else if(stateType.Equals("load")) {
                LoadGame();
                return;
                // } else if(currentState == newGameState) {
            } else if(stateType.Equals("new")) {
                currentState = startingState;
                SetStatesList();
                p = new Player();
                p.setCurrentState(currentState.getStateName());
                if(currentState.initializesPlayer()) {
                    currentState.initPlayer(p);
                }
                manageState();
                returnState();
                // } else if(currentState == quitState) {
            } else if(stateType.Equals("quit")) { 
                // UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
            }
        } else if(Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
            returnState();
            return;
        } else {
            return;
        }
    }

    private void puzzleListener(int code) {
        if(cluesToCombine.Count == 2) {
            if(code == possibleConclusionsFlags.Count) {
                puzzlePage = 0;
                playerClues.Clear();
                cluesToCombine.Clear();
                possibleConclusionsFlags.Clear();
                possibleConclusionsText.Clear();
                foreach(string flag in p.getFlags()) {
                    string clue = helper.getClue(GetChapter(p.getCurrentState()), flag);
                    if(clue.Length != 0) {
                        playerClues.Add(clue);
                    }
                }
                runPuzzle();
            } else if(code == possibleConclusionsFlags.Count + 1) {
                returnState();
            } else if(code < possibleConclusionsFlags.Count) {
                playerConclusions.Add(possibleConclusionsFlags[code]);
                // p.setDeduction(possibleConclusionsFlags[code], possibleConclusionsText[code]);
                p.setFlag(possibleConclusionsFlags[code]);
                StartCoroutine(displayConclusion(possibleConclusionsText[code]));
            }
        } else {
            if(playerClues.Count <= 5) {
                // Only one page
                if(code == playerClues.Count) {
                    // Example: 3 clues (input 1, 2, 3)
                    // code 3 == input 4
                    returnState();
                } else if(code < playerClues.Count) {
                    // code < 3 == input 1, 2, 3
                    cluesToCombine.Add(playerClues[code]);
                    runPuzzle();
                }
            } else {
                // More than one page
                if(puzzlePage == 0) {
                    // On first page
                    // 1 - 5 (codes 0 - 4) select clues
                    // 6 (code 5) next page
                    // 7 (code 6) close journal
                    if(code == 5) {
                        // code 5 == input 6
                        puzzlePage++;
                        runPuzzle();
                    } else if(code == 6) {
                        // code 6 == input 7
                        returnState();
                    } else if (code < 5) {
                        // code < 5 == input 1, 2, 3, 4, 5
                        cluesToCombine.Add(playerClues[code]);
                        puzzlePage = 0;
                        runPuzzle();
                    }
                } else if(playerClues.Count <= (puzzlePage + 1) * 5) {
                    // On last page
                    //  Example: 13 clues, 3 pages (0, 1, 2)
                    //  On page 2
                    //  playerClues.Count = 13; puzzlePage + 1 = 3; 3 * 5 = 15; 13 <= 15; On last page
                    if(code == (playerClues.Count - (puzzlePage * 5))) {
                        // Example:
                        //  13 clues
                        //  On page 2
                        //  Showing clues 11, 12, 13 for input 1, 2, 3 (code 0, 1, 2)
                        //  input 4 (code 3) = page back
                        //  playerClues.Count = 13; puzzlePage * 5 = 10; 13 - 10 = 3; code == 3
                        puzzlePage--;
                        runPuzzle();
                    } else if(code == (playerClues.Count - (puzzlePage * 5)) + 1) {
                        // playerClues.Count = 13; puzzlePage * 5 = 10; 13 - 10 = 3; 3 + 1 = 4; code == 4
                        returnState();
                    } else if(code < (playerClues.Count - (puzzlePage * 5))) {
                        // playerClues.Count = 13; puzzlePage * 5 = 10; 13 - 10 = 3; code < 3
                        cluesToCombine.Add(playerClues[(puzzlePage * 5) + code]);
                        puzzlePage = 0;
                        runPuzzle();
                    }
                } else {
                    // On middle page
                    if(code == 5) {
                        // code 5 == input 6
                        puzzlePage++;
                        runPuzzle();
                    } else if(code == 6) {
                        // code 6 == input 7
                        puzzlePage--;
                        runPuzzle();
                    } else if(code == 7) {
                        // code 7 == input 8
                        returnState();
                    } else if(code < 5) {
                        // code < 5 == input 1, 2, 3, 4, 5
                        cluesToCombine.Add(playerClues[(puzzlePage * 5) + code]);
                        puzzlePage = 0;
                        runPuzzle();
                    }
                }
            }
        }
    }

    private void manageKeystroke(int code) {
        if(code == 10) {
            // I - Inventory
            currentState = inventoryState;
            stateType = "inventory";
            previousStateWasBasic = true;
            readInventory();
        } else if(code == 11) {
            // J - Journal
            currentState = journalState;
            stateType = "journal";
            previousStateWasBasic = true;
            readJournal();
        } else if(code == 12) {
            stateType = "puzzle";
            previousStateWasBasic = true;
            puzzlePage = 0;
            playerClues.Clear();
            cluesToCombine.Clear();
            possibleConclusionsFlags.Clear();
            possibleConclusionsText.Clear();
            foreach(string flag in p.getFlags()) {
                string clue = helper.getClue(GetChapter(p.getCurrentState()), flag);
                if(clue.Length != 0) {
                    playerClues.Add(clue);
                }
            }
            runPuzzle();
        } else if(code == 13) {
            // O - Options
            currentState = optionsState;
            stateType = "options";
            previousStateWasBasic = true;
            readOptions();
        } else {
            // 1 thru 0
            changeState(code);
        }
    }


    private void manageBasicKeystroke(int code) {
        if(
            (currentState == journalState && !hasAdditionalPages("journal")) ||
            (currentState == inventoryState && !hasAdditionalPages("inventory"))
        ) {
            if(code == 0) {
                page = 0;
                returnState();
            }
        } else {
            if(code == 0) {
                page++;
                // if(currentState == journalState) {
                if(stateType.Equals("journal")) {
                    readJournal();
                    // } else if(currentState == inventoryState) {
                } else if(stateType.Equals("inventory")) { 
                    readInventory();
                }
            } else if(code == 1) {
                page = 0;
                returnState();
            }
        }
    }

    // textFadeHandler fades old text out and new text in for scene transitions
    // Chapter heading does not fade in/out, because I'm not sure how CrossFadeAlpha will work with TextMeshProUGUI
    IEnumerator textFadeHandler(string storyText, string optionsText) {
        // Sets overlayStoryField to the new story text, then cross-fades the old text with the overlay
        overlayStoryField.text = storyText;
        storyField.CrossFadeAlpha(0, 0.2f, false);
        overlayStoryField.CrossFadeAlpha(1, 0.2f, false);

        // If the new options text is different than the old options text, do a cross-fade
        // Otherwise, do not cross-fade
        // This prevents the annoying "fade-in-fade-out" effect if the options text does not change
        //      e.g. when both options are "(Continue)"
        if(optionsText != optionsField.text) {
            overlayOptionsField.text = optionsText;
            optionsField.CrossFadeAlpha(0, 0.2f, false);
            overlayOptionsField.CrossFadeAlpha(1, 0.2f, false);
        }

        // <yeild return new WaitForSeconds(seconds)> forces the Coroutine to wait
        // In this case, wait the length of time necessary for the texts to cross-fade
        yield return new WaitForSeconds(0.2f);

        // Set the story and options fields to the new values, then fade out the overlay and fade in the default fields instantly
        // This resets alpha for the fields to the correct values, so that cross-fade can be used again next time the scene transitions
        storyField.text = storyText;
        optionsField.text = optionsText;
        storyField.CrossFadeAlpha(1, 0.0f, false);
        optionsField.CrossFadeAlpha(1, 0.0f, false);
        overlayStoryField.CrossFadeAlpha(0, 0.0f, false);
        overlayOptionsField.CrossFadeAlpha(0, 0.0f, false);
    }

    State getPlayerState() {
        foreach(State state in states) {
            if(p.getCurrentState().Equals(state.getStateName())) {
                return state;
            }
        }

        Debug.Log("Cannot find Player.CurrentState\n" + p.getCurrentState());
        return null;
    }

    private void returnState() {
        currentState = getPlayerState();
        stateType = getPlayerState().getStateName();
        manageState();
        previousStateWasBasic = false;
    }


    private void changeState(int opt) {
        for(int i = 0; i < availableStates.Count; i++) {
            if(opt == i) {
                currentState = availableStates[opt];
                stateType = currentState.getStateName();
                if(Array.Exists(chapterStates, element => element == currentState)) {
                    SetStatesList();
                }
                if(currentState.initializesPlayer()) {
                    currentState.initPlayer(p);
                }
                p.setCurrentState(currentState.getStateName());
                previousStateWasBasic = false;
                manageState();
            }
        }
    }


    private void manageState() {
        if(currentState.hasChapterTitle()) {
            chapterField.text = currentState.getChapterTitle();
        }
        if(!previousStateWasBasic) {
            currentState.managePlayer(p);
        }
        setAvailableStates();
        string optionsText = "";
        if(availableStates.Count == 0) {
            optionsText = "O - Options";
        } else if(!currentStateHasContinueOption()) { 
            for(int i = 0; i < availableStates.Count; i++) {
                optionsText += (i + 1) + " - " + availableStates[i].getOptText() + "\n";
            }
            optionsText += "I - Inventory\nJ - Journal\nC - Draw Conclusions\nO - Options";
        } else {
            optionsText += availableStates[0].getOptText();
            optionsText += "\n\nO - Options";
        }
        StartCoroutine(textFadeHandler(currentState.getStoryText(p), optionsText));
        playSoundEffects();
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
    }


    private void readInventory() {
        List<string> inventoryEntries = p.getInventoryEntries();
        string inventoryText = "";
        string optionsText = "";
        if(page == 0) {
            inventoryText += "INVENTORY\n---------\n\n";
        }
        for(int i = page * 3; i < inventoryEntries.Count; i++) {
            if(i != page * 3) {
                inventoryText += "\n\n";
            }
            inventoryText += inventoryEntries[i];
            if(i == inventoryEntries.Count - 1 || i == page * 3 + 2) {
                break;
            }
        }
        if(hasAdditionalPages("inventory")) {
            optionsText = "1 - Next Page\n2 - Return";
        } else {
            optionsText = "(Return)";
        }
        StartCoroutine(textFadeHandler(inventoryText, optionsText));
    }


    private void readJournal() {
        List<string> journalEntries = p.getJournalEntries();
        string journalText = "";
        string optionsText = "";
        if(page == 0) {
            journalText += "JOURNAL\n-------\n\n";
        }
        for(int i = page * 3; i < journalEntries.Count; i++) {
            if(i != page * 3) {
                journalText += "\n\n";
            }
            journalText += journalEntries[i];
            if(i == journalEntries.Count - 1 || i == page * 3 + 2) {
                break;
            }
        }
        if(hasAdditionalPages("journal")) {
            optionsText = "1 - Next Page\n2 - Return";
        } else {
            optionsText = "(Return)";
        }
        StartCoroutine(textFadeHandler(journalText, optionsText));
    }

    private void readOptions() {
        string display = "";
        display += "OPTIONS\n----------\n";
        display += "RETURN - Return to the game.\n";
        display += "SAVE - Save your game.\n";
        display += "LOAD - Load a saved game.\n";
        display += "NEW - Start a new game.\n";
        display += "QUIT - Quit the game. All unsaved progress will be lost.";

        string options = "";
        options += "ENTER - RETURN\n";
        options += "S - SAVE\n";
        options += "L - LOAD\n";
        options += "N - NEW\n";
        options += "Q - QUIT";

        StartCoroutine(textFadeHandler(display, options));
    }


    private Save CreateSaveGameObject() {
        Save save = new Save();
        save.setPlayer(p);
        return save;
    }


    public void SaveGame() {
        if(!Directory.Exists(Directory.GetCurrentDirectory() + "\\savedata")) {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\savedata");
        }

        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Directory.GetCurrentDirectory() + "\\savedata\\savegame.gsaf");
        bf.Serialize(file, save);
        file.Close();

        StartCoroutine(displayGameSaved());
    }

    public void LoadGame() {
        if(File.Exists(Directory.GetCurrentDirectory() + "\\savedata\\savegame.gsaf")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Directory.GetCurrentDirectory() + "\\savedata\\savegame.gsaf", FileMode.Open);
            Save save = (Save) bf.Deserialize(file);
            file.Close();

            p = save.getPlayer();
            string playerChapter = GetChapter(p.getCurrentState());
            foreach(State chapterState in chapterStates) {
                if(GetChapter(chapterState.getStateName()).Equals(playerChapter)) {
                    currentState = chapterState;
                    break;
                }
            }
            SetStatesList();
            currentState = getPlayerState();
            manageState();
            returnState();
        } else {
            StartCoroutine(displayLoadError());
        }
    }

    IEnumerator displayConclusion(string conclusion) {
        string text = "";
        text += conclusion;
        text += "\n\nYou write it down in your Journal.";
        overlayStoryField.text = text;
        storyField.CrossFadeAlpha(0, 0.2f, false);
        overlayStoryField.CrossFadeAlpha(1, 0.2f, false);

        optionsField.CrossFadeAlpha(0, 0.2f, false);

        yield return new WaitForSeconds(0.2f);

        storyField.text = overlayStoryField.text;
        storyField.CrossFadeAlpha(1, 0, false);
        overlayStoryField.CrossFadeAlpha(0, 0, false);

        yield return new WaitForSeconds(0.8f);

        returnState();
    } 


    IEnumerator displayGameSaved() {
        overlayStoryField.text = "GAME SAVED";
        storyField.CrossFadeAlpha(0, 0.2f, false);
        overlayStoryField.CrossFadeAlpha(1, 0.2f, false);

        optionsField.CrossFadeAlpha(0, 0.2f, false);

        yield return new WaitForSeconds(0.2f);

        storyField.text = overlayStoryField.text;
        storyField.CrossFadeAlpha(1, 0, false);
        overlayStoryField.CrossFadeAlpha(0, 0, false);

        yield return new WaitForSeconds(0.8f);

        returnState();
    }

    IEnumerator displayLoadError() {
        overlayStoryField.text = "COULD NOT LOAD SAVED GAME";
        storyField.CrossFadeAlpha(0, 0.2f, false);
        overlayStoryField.CrossFadeAlpha(1, 0.2f, false);

        optionsField.CrossFadeAlpha(0, 0.2f, false);

        yield return new WaitForSeconds(1);

        returnState();
    }


    private void ConfirmSave() {
        string display = "Are you sure you wish to save?\nThis will overwrite your existing save file.";
        string options = "1 - YES\n2 - NO";

        StartCoroutine(textFadeHandler(display, options));
    }

    private void ConfirmLoad() {
        string display = "Are you sure you wish to load a saved game?\nAll progress since your last save will be lost.";
        string options = "1 - YES\n2 - NO";

        StartCoroutine(textFadeHandler(display, options));
    }


    private void ConfirmNew() {
        string display = "Are you sure you wish to start a new game?";
        string options = "1 - YES\n2 - NO";

        StartCoroutine(textFadeHandler(display, options));
    }


    private void ConfirmQuit() {
        string display = "Are you sure you wish to quit?";
        string options = "1 - YES\n2 - NO";

        StartCoroutine(textFadeHandler(display, options));
    }


    private void playSoundEffects() {

        if(audioSource != null) {
            audioSource.Stop();
        }

        if(!previousStateWasBasic && currentState.getAudioClip() != null) {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = currentState.getAudioClip();
            audioSource.Play();
        }
    }


    private int getCodeIndex(string code) {
        for(int i = 0; i < codes.Length; i++) {
            if(code == codes[i]) {
                return i;
            }
        }
        return -1;
    }


    private bool currentStateHasContinueOption() {
        if(availableStates[0].getOptText().Equals("(Continue)") || availableStates[0].getOptText().Equals("(Press Enter to Continue)")) {
            return true;
        } else {
            return false;
        }
    }


    private bool hasAdditionalPages(string screenType) {
        List<string> entries = new List<string>();
        if(screenType.Equals("inventory")) {
            entries = p.getInventoryEntries();
        } else if(screenType.Equals("journal")) {
            entries = p.getJournalEntries();
        } else {
            Debug.Log("Bad screenType passed to hasAdditionalPages()\nReceived " + screenType);
        }
        if(page * 3 + 3 < entries.Count) {
            return true;
        } else {
            return false;
        }
    }



    /**********
     * TEST CODE
     * PUZZLE MECHANICS
     **********/

    List<string> playerConclusions = new List<string>();
    int puzzlePage;
    PuzzleHelper helper = new PuzzleHelper();
    List<string> playerClues = new List<string>();
    List<string> cluesToCombine = new List<string>();
    List<string> possibleConclusionsText = new List<string>();
    List<string> possibleConclusionsFlags = new List<string>();

    void runPuzzle() {
        if(cluesToCombine.Count < 2) {
            printPuzzlePage();
        } else {
            printPossibleConclusions();
        }
    }

    void printPuzzlePage() {
        string puzzleText = "";
        string optionsText = "";
        if(playerClues.Count > 1) {
            if(cluesToCombine.Count != 0) {
                playerClues.Remove(cluesToCombine[0]);
            }
            int choice = 1;
            if(cluesToCombine.Count == 0) {
                puzzleText += "Select first clue";
            } else {
                puzzleText += "Select second clue";
            }
            for(int i = puzzlePage * 5; i < playerClues.Count; i++) {
                puzzleText += "\n\n";
                puzzleText += choice + " - " + playerClues[i];
                choice++;
                if(i == playerClues.Count - 1 || i == puzzlePage * 5 + 4) {
                    break;
                }
            }
            if(playerClues.Count > ((puzzlePage + 1) * 5)) {
                optionsText += choice + " - Next Page\n";
                choice++;
            }
            if(puzzlePage > 0) {
                optionsText += choice + " - Previous Page\n";
                choice++;
            }
            optionsText += choice + " - Close Journal";
        } else {
            puzzleText += "Not enough clues to put together yet.";
            optionsText += "1 - Close Journal";
        }
        StartCoroutine(textFadeHandler(puzzleText, optionsText));
    }

    void printPossibleConclusions() {
        possibleConclusionsFlags = helper.getPossibleConclusionsFlags(GetChapter(p.getCurrentState()), playerConclusions, cluesToCombine);
        // possibleConclusionsText = helper.getConclusionsText(GetChapter(p.getCurrentState()), possibleConclusionsFlags);
        for(int i = 0; i < possibleConclusionsFlags.Count; i++) {
            string text = helper.getConclusionText(GetChapter(p.getCurrentState()), possibleConclusionsFlags[i]);
            if(text.Length != 0) {
                possibleConclusionsText.Add(text);
            }
        }
        string deductionText = "";
        string optionsText = "";
        deductionText += "POSSIBLE CONCLUSIONS\n--------------------";
        if(possibleConclusionsFlags.Count != 0) {
            for(int i = 0; i < possibleConclusionsFlags.Count; i++) {
                deductionText += "\n\n";
                deductionText += (i + 1) + " - " + possibleConclusionsText[i];
            }
            optionsText += possibleConclusionsFlags.Count + " - Return\n";
        } else {
            deductionText += "\n\nThese clues don't seem to go together.";
        }
        optionsText += (possibleConclusionsFlags.Count + 1) + " - Close Journal";

        StartCoroutine(textFadeHandler(deductionText, optionsText));
    }

}
