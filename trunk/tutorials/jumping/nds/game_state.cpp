// game_state.cpp
// Routines to manage the current game state
// This file was automatically generated by Spritely

#include <nds.h>

#include "game_state.h"
#include "game_utils.h"
#include "object_utils.h"

// Constructor
// This is called once at the beginning of the game to
// set up the initial game state.
GameState::GameState() {
	// Setup the stage for level 1.
	SetupStage(kLevel_1);
}

// SetupStage
// This sets up the stage for each level.
void GameState::SetupStage(int level) {
	// Record which level we're in.
	_level = level;

	SetupStage_Level1();
}

// SetupStage_Level1
// Set up the stage (sprites and backgrounds) for level 1.
void GameState::SetupStage_Level1() {
	// Set the default sprite video mode.
	SetSpriteVideoMode();

	// Setup the foreground sprites.
	// The sprite data is not active until we copy it from our data
	// tables (in sprites.cpp) into the real Palette and Graphics memory.
	// So, let's copy the default spriteset there now.
	ClearSprites();
	LoadSpriteset(0);

	// Setup the background tiles and map.
	// Just like sprites, the data is not active until we copy it from
	// our data tables (in background_maps.cpp) into real memory.
	ClearBackgrounds();
	LoadBgTileset(0);
	LoadBgMap(0);

	// Initialize the objects for the first level.
	InitObject(kObj_Player, 0);

	// Calculate the ground level.
	_yPlayerHeight = GetObjectHeight(kObj_Player);
	_yGroundLevel = 128;

	// Set the initial location of each object.
	_xPlayer = 0;
	_yPlayer = _yGroundLevel - _yPlayerHeight;
	MoveObjectTo(kObj_Player, _xPlayer, _yPlayer);

	_isjumping = false;
	_yVelocity = 0;

	// TODO: Add more initialization for level 1 here.
}

// Update
// This is called repeatedly, 60 times per second.
// You should check the buttons and update the game state here.
void GameState::Update() {
	// Get the current state of all of the buttons.
	GetKeyState();

	// Handle input and updates for level 1.
	Update_Level1();
}

// Update_Level1
// Handle buttons and update game state for level 1.
void GameState::Update_Level1() {
	// The arrow keys are used to move the current object.
	// We use CheckKeyHeld() because we want the action to repeat as long
	// as the player is holding down the button.
	int dx = 0;
	int dy = 0;
	if (CheckKeyHeld(KEY_LEFT))
		dx = -1;
	if (CheckKeyHeld(KEY_RIGHT))
		dx = 1;

	// Handle the player pressing the 'A' button.
	// We use CheckKeyPress() because we *don't* want the action to repeat
	// unless the player presses the 'A' button multiple times.
	if (CheckKeyPress(KEY_A)) {
		// ToDo: Add code to respond to 'A' button press here.
	}

	// Handle the player jump.
	if (CheckKeyPress(KEY_UP) && !_isjumping) {
		_yVelocity = 10;
		_isjumping = true;
	}
	dy = -_yVelocity;

	// If we need to move the player.
	if (dx != 0 || dy != 0) {
		// Record the player's new location.
		_xPlayer += dx;
		_yPlayer += dy;

		// Move the player to the new location.
		MoveObjectTo(kObj_Player, _xPlayer, _yPlayer);
	}

	// If the player is above ground, apply gravity.
	if (_yPlayer + _yPlayerHeight < _yGroundLevel) {
		// Apply gravity by reducing upward velocity.
		_yVelocity--;
	} else {
		// Player is on the ground, so stop jumping.
		_yVelocity = 0;
		_isjumping = false;
		// Force player to be exactly at ground level.
		_yPlayer = _yGroundLevel - _yPlayerHeight;
		MoveObjectTo(kObj_Player, _xPlayer, _yPlayer);
	}

	// TODO: Add additional game state updates for level 1 here.
}