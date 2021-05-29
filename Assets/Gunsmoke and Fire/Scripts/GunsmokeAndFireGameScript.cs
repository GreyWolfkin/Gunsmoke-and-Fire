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
     *      Add ability to remove journal entries
     *      Add ability to remove inventory items
     *      Fix naming convention on methods (even though I don't want to)
     *      Figure out a way to load only scenes for each chapter at a time?
     *      Cleanup Start method, abstract to new methods
     *      Set up tabs in Journal?
     *          Contacts, Clues, Key Events?
     *      
     */

    /*
     * TODO - STORY
     *      Add plot point of missing spells? Have only limited spells at the beginning, add more?    
     *      Get Playtester Opinion: Is repeating narrative text immersion breaking?
     *          e.g. Harald's Bar. You get the same introduction whether you've been there before or not. Do we need a different state for returning to the place? Or will the user accept repeated text?
     *      Flashback Chapter - 4 years ago, murder investigation, tutorial on puzzle solving mechanics, interrogation scene, then receive Atellena case
     *      Do we really need the instructional screens? Consider removing them? If the mechanics are well-designed, you shouldn't need a tutorial
     * 
     */

    /*
     * BUGS
     *    
     */

    /*
     * CURRENT DEVLOG
     *      Ability to remove items and flags, removing their entries as well
     *      Ability to count qty of items in inventory, and display that qty in Inventory screen
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

        codes = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "i", "j", "o" };

        currentState = startingState;
        SetStatesList();

        // p = ScriptableObject.CreateInstance<Player>();
        p = new Player();

        // If starting state is set to a different state for debugging purposes, or if loading a saved game, if the starting state does not have the initializesPlayer bool, do not reinitialize player variables
        if(currentState.initializesPlayer()) {
            currentState.initPlayer(p);
        }
        p.setCurrentState(currentState.getStateName());
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

        if(currentState == optionsState) {
            optionsListener();
        } else if(
            currentState == saveState ||
            currentState == loadState ||
            currentState == newGameState ||
            currentState == quitState 
        ) {
            internalOptionsListener();
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
            ConfirmSave();
            return;
        } else if(Input.GetKeyDown(KeyCode.L)) {
            currentState = loadState;
            ConfirmLoad();
            return;
        } else if(Input.GetKeyDown(KeyCode.N)) {
            currentState = newGameState;
            ConfirmNew();
            return;
        } else if(Input.GetKeyDown(KeyCode.Q)) {
            currentState = quitState;
            ConfirmQuit();
            return;
        } else {
            return;
        }
    }

    private void internalOptionsListener() {
        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
            if(currentState == saveState) {
                SaveGame();
                return;
            } else if(currentState == loadState) {
                LoadGame();
                return;
            } else if(currentState == newGameState) {
                currentState = startingState;
                SetStatesList();
                p = new Player();
                if(currentState.initializesPlayer()) {
                    currentState.initPlayer(p);
                }
                p.setCurrentState(currentState.getStateName());
                manageState();
                returnState();
            } else if(currentState == quitState) {
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

    private void manageKeystroke(int code) {
        if(code == 10) {
            // I - Inventory
            currentState = inventoryState;
            previousStateWasBasic = true;
            readInventory();
        } else if(code == 11) {
            // J - Journal
            currentState = journalState;
            previousStateWasBasic = true;
            readJournal();
        } else if(code == 12) {
            // O - Options
            currentState = optionsState;
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
                if(currentState == journalState) {
                    readJournal();
                } else if(currentState == inventoryState) {
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
        manageState();
        previousStateWasBasic = false;
    }


    private void changeState(int opt) {
        for(int i = 0; i < availableStates.Count; i++) {
            if(opt == i) {
                currentState = availableStates[opt];
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
            optionsText += "I - Inventory\nJ - Journal\nO - Options";
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

}
