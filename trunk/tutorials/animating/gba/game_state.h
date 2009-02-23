// game_state.h
// Global structure to record all game state.
// This file was automatically generated by Spritely

#ifndef _GAME_STATE_H_
#define _GAME_STATE_H_

#include "animation.h"
#include "sprites.h"
#include "background_maps.h"

// The levels in our game.
const int kLevel_1 = 1;

// The objects in our game.
// Only 128 objects (0-127) can be defined at any time.
const int kObj_Player = 0;

// The GameState class holds all of the global game state information.
class GameState {
public:
	// Constructor
	// This method initializes the game state.
	GameState();

	// SetupStage
	// Set up the stage (backgrounds, sprites) for the each level.
	void SetupStage(int level);
	void SetupStage_Level1();

	// Update
	// This is called continuously to update the game to the next state.
	void Update();
	void Update_Level1();

private:
	// The current level of the game.
	int _level;

	// The (x,y) location of the object representing the player.
	int _xPlayer, _yPlayer;

	// Keep track of the current animation state of the player.
	AnimationInfo animatePlayer;
};

#endif	// _GAME_STATE_H_