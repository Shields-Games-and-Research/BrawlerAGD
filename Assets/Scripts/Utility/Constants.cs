using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants 
{
    //For File System Compatibility
    public static string PC_SLASH = "\\";
    public static string OSX_SLASH = "/";
    public static string JSON = ".json";
    
    //Top level of file structure
    public static string ASSETS = "Assets";

    //First level
    public static string OUTPUT = "Output";
    public static string EXAMPLES = "Examples";
    public static string RESEARCH = "Research";
    public static string SCRIPTS = "Scripts";

    //Second Level
    public static string EVO_GAME_POPULATION = "EvoPopulation";
    public static string EVO_GEN_RESULTS = "EvoResults";
    public static string VERBOSE_GEN_RESULTS = "VerboseResults";
    public static string TUTORIAL = "Tutorial";
    public static string GAME = "Game";
    public static string RESULTS = "Results";

    //Third Level
    public static string GEN_PREFIX = "Gen_";
    public static string GAME_PREFIX = "Game_";
    public static string ROUND_PREFIX = "Round_";

    //Fourth Level
    public static string ROUNDS_RESULTS = "Round_Results";

    //Scene Names
    public static string ARENA_SCENE = "Arena";
    public static string CREDITS_SCENE = "Credits";
    public static string EVOLUTION_SCENE = "EvolutionaryArenaManager";
    public static string EVOLUTION_MENU_SCENE = "EvolutionaryManagerStartScene";
    public static string PILOT_STUDY_SCENE = "LoadGame";
    public static string MAIN_MENU_SCENE = "MainMenuScene";
    public static string LOAD_GAME_DISK_SCENE = "LoadGameFromDisk";

    //Game File Names
    public static string LEVEL = "level";
    public static string PLAYER1 = "player1";
    public static string PLAYER2 = "player2";
    public static string PLAYER1MOVE1 = "p1move1";
    public static string PLAYER1MOVE2 = "p1move2";
    public static string PLAYER2MOVE1 = "p2move1";
    public static string PLAYER2MOVE2 = "p2move2";
    public static string GAME_RESULT = "gameresult";
    public static string RESULTS_FILE = "results";
    public static string ROUND_RESULTS_FOLDER = "round_results";

    //Game File Names with JSON
    public static string LEVEL_JSON = LEVEL + JSON;
    public static string PLAYER1_JSON = PLAYER1 + JSON;
    public static string PLAYER2_JSON = PLAYER2 + JSON;
    public static string PLAYER1MOVE1_JSON = PLAYER1MOVE1 + JSON;
    public static string PLAYER1MOVE2_JSON = PLAYER1MOVE2 + JSON;
    public static string PLAYER2MOVE1_JSON = PLAYER2MOVE1 + JSON;
    public static string PLAYER2MOVE2_JSON = PLAYER2MOVE2 + JSON;
    public static string GAME_RESULT_JSON = GAME_RESULT + JSON;
    public static string EVO_RESULT_JSON = RESULTS_FILE + JSON;

    //Paths - TODO: Mac paths
    public static string PILOT_GAME_LOAD_PATH = ASSETS + PC_SLASH + RESEARCH + PC_SLASH + GAME + PC_SLASH;
    public static string PILOT_GAME_RESULTS_PATH = ASSETS + PC_SLASH + RESEARCH + PC_SLASH + RESULTS + PC_SLASH;
    public static string TUTORIAL_GAME_PATH = ASSETS + PC_SLASH + EXAMPLES + PC_SLASH + TUTORIAL + PC_SLASH + GAME;
    public static string TUTORIAL_RESULTS_PATH = ASSETS + PC_SLASH + EXAMPLES + PC_SLASH + TUTORIAL + PC_SLASH + RESULTS;
    public static string EVO_POPULATION_PATH = ASSETS + PC_SLASH + OUTPUT + PC_SLASH + EVO_GAME_POPULATION + PC_SLASH + GAME;
    public static string EVO_RESULTS_PATH = ASSETS + PC_SLASH + OUTPUT + PC_SLASH + EVO_GEN_RESULTS;

    //Errors
    public static string FILE_NOT_FOUND = "Attempting to read JSON failed. Did you specify the right file location?";
    public static string LOAD_STUDY_BUTTON_ERROR = "A bad button name was passed to the load game function.";

    //System Logs
    public static string INITIALIZING_ARENA = "Initializing Arena with Game at path: ";
    public static string LOADING_PILOT_GAME = "Loading Pilot Game: ";
    public static string LOADING_TUTORIAL = "Loading Tutorial from: ";
    public static string GAME_TOO_LONG = "This game has exceeded the specified maximum game length. Ending now with fitness penalty.";
    public static string GENERATING_GAME = "Generating a new game from seed.";

    //Pilot Game Specific
    public static string GAME_A = "GameA";
    public static string GAME_B = "GameB";
    public static string GAME_C = "GameC";
    public static string GAME_D = "GameD";
    public static string GAME_E = "GameE";
    public static string GAME_F = "GameF";



}