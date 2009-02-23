// game_state.cpp
// Routines to manage the current game state
// This file was automatically generated by Spritely

#include <gba_video.h>

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

	// Set the initial location of each object.
	_xPlayer = 0;
	_yPlayer = 0;
	MoveObjectTo(kObj_Player, _xPlayer, _yPlayer);

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
	if (CheckKeyHeld(KEY_UP))
		dy = -1;
	if (CheckKeyHeld(KEY_DOWN))
		dy = 1;

	// Handle the player pressing the 'A' button.
	// We use CheckKeyPress() because we *don't* want the action to repeat
	// unless the player presses the 'A' button multiple times.
	if (CheckKeyPress(KEY_A)) {
		// ToDo: Add code to respond to 'A' button press here.
	}

	// If we need to move the player.
	if (dx != 0 || dy != 0) {
		// Calculate the player's new location.
		int x = _xPlayer + dx;
		int y = _yPlayer + dy;

		// Get the width/height of the player.
		int width, height;
		GetObjectSize(kObj_Player, &width, &height);

		// Don't let the player go outside the screen boundaries.
		if (x < 0 || x > SCREEN_WIDTH - width)
			dx = 0;
		if (y < 0 || y > SCREEN_HEIGHT - height)
			dy = 0;

		// Record the player's new location.
		_xPlayer += dx;
		_yPlayer += dy;

		// Move the player to the new location.
		MoveObjectTo(kObj_Player, _xPlayer, _yPlayer);
	}

	// TODO: Add additional game state updates for level 1 here.
}
