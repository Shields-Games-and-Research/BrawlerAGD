# BrawlerAGD

An Automated Game Designer targeted at the "Brawler" game genre. 

# Current Status

This repository is the initial release of an Automated Game Designer (AGD) that focuses on a specific genre of game. A pilot study was conducted in the spring of 2022 investigating whether or not an AGD approach could be leveraged in generating a feeling of a "balanced" game for players. This prototype was then iterated on with the help of several students, building an interface that allows users to manage characteristics of the genetic algorithm used in the system, enabling a GUI-approach to performing experiments with the system.

# About the System

This system aims to create 2D "Brawler" fighting video games by randomly generating games from a template, repeatedly evaluating them with behavior-tree agents and fitness functions, and repeating until a relatively consistent fitness emerges. The system edits games by changing parameters of discrete game objects in the game environemnt: characters, moves, levels.

The system has two main ways of being used by the public: for more technical users, the Unity Interface and direct editing of game scripts provides the most flexibility and depth. For less technical users, or users looking for a quick start, a user-interface is provided in a video-game fashion for trying to "grow" your very own Brawler games.

The system is compatible with Windows 10 and above.

# Using the Graphical User Interface (GUI):

Running "BrawlerAGD.exe" in the root of the repository runs the game as a Unity application. On loading, the main menu should appear. You will have four options: Start Evolution, Load Game, Load Pilot, and Credits.

## Start Evolution (Generating Games)

Starting an evolution shows you a screen with a description of key variables in a genetic algorithm. An information icon at the top left of the screen provides details on how the parameters affect the system.

Output data are saved in an Assets folder relative to the where the BrawlerAGD.exe was run. Evolutionary runs can be stopped at any time, as games and results are saved to disk on a realtime basis. 

## Load Game

This menu is used to load generated game from disk to play them.

## Pilot Study

This menu loads the game menu used for the original pilot study of the game. Games shown in the pilot study paper are presented here.

# Gameplay

Games are played between two players, where each player attempts to knock the other player off of the screen boundaries a number of times. Every time a player is knocked off the screen, a player loses a "stock" and is able to fight fresh. Whenever a character is hit, that character takes a certain number of damage, which in turn impacts how far they are flung when hit. Characters' movement and ability to stay on screen depends on their attacking ability, movement capabilities, timing parameters, etc.

## Controls

XBOX One controllers can be used in place of keyboard bindings. The joystick controls left and right movement; the face buttons control jumps and attacks.

Player 1:

A: Left
D: Right
S: Attack
W: Jump

Player 2:

J: Left
L: Right
K: Attack
I: Jump

# Key Technical Information

For those looking to dig deeper, two critical portions of the system for improvement are agent behaviors and the system's fitness function.

The fitness functions can be found and adjusted in the file "GameResult.cs".
The agent behaviors are detailed in the file "Controller.cs".

These initial implementations are on the simpler side, and are the next targets of iteration for future studies.

## Known Issues

Please report any found bugs! We are a very nascent team and still trying to improve - any suggestions are helpful.

1. The Pause menu was not implemented at the time of the pilot study, and thus is not fully functional in that context.
2. The Tutorial is currently a dead-end, and needs a restart to start the other games.
3. Exiting the evolution system and attempting to perform other actions can sometimes corrupt the game state and require a restart.
4. The "Announcer" UI does not always correctly announce the correct winner/loser (Though the correct loser is saved to disk)
