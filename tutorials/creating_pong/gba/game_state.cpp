// game_state.cpp
// Routines to manage the current game state
// This file was automatically generated by Spritely

#include <gba_video.h>

#include "collision.h"
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
	InitObject(kObj_Player, kSprites_Paddle);
	InitObject(kObj_Opponent, kSprites_Paddle);
	InitObject(kObj_Ball, kSprites_Ball);

	// Set the initial location of each object.
	_xPlayer = SCREEN_WIDTH - GetObjectWidth(kObj_Player) - 8;
	_yPlayer = 0;
	MoveObjectTo(kObj_Player, _xPlayer, _yPlayer);

	_xOpponent = 8;
	_yOpponent = 0;
	MoveObjectTo(kObj_Opponent, _xOpponent, _yOpponent);

	_xBall = SCREEN_WIDTH / 2;
	_yBall = 0;
	MoveObjectTo(kObj_Ball, _xBall, _yBall);

	// Initialize the ball direction.
	_dxBall = 1;
	_dyBall = 1;

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
		// Calculate the player's new y-location.
		int y = _yPlayer + dy;

		// Don't let the player go outside the screen boundaries.
		if (y < 0 || y > SCREEN_HEIGHT - GetObjectHeight(kObj_Player))
			dy = 0;

		// Record the player's new location.
		_xPlayer += dx;
		_yPlayer += dy;

		// Move the player to the new location.
		MoveObjectTo(kObj_Player, _xPlayer, _yPlayer);
	}

	// Check where the ball is moving.
	int x = _xBall + _dxBall;
	int y = _yBall + _dyBall;

	// Don't let the ball go off the left/right side of screen.
	if (x < 0 || x > SCREEN_WIDTH - GetObjectWidth(kObj_Ball))
		_dxBall *= -1;

	// Don't let the ball go off the top/bottom of screen.
	if (y < 0 || y > SCREEN_HEIGHT - GetObjectHeight(kObj_Ball))
		_dyBall *= -1;

	// Does the ball collide with a paddle?
	if (CollideBBox(kObj_Player, kObj_Ball))
		_dxBall = -1;
	if (CollideBBox(kObj_Opponent, kObj_Ball))
		_dxBall = 1;

	// Move the ball.
	_xBall += _dxBall;
	_yBall += _dyBall;
	MoveObjectTo(kObj_Ball, _xBall, _yBall);

	// Handle opponent AI.
	dy = 0;
	// If the ball is above the paddle.
	if (_yBall < _yOpponent) {
		// Move the paddle up.
		dy = -1;
		// Unless that would move the paddle above the top of the screen.
		if (_yOpponent < 0)
			dy = 0;
	} else {
		// Otherwise, move the paddle down.
		dy = 1;
		// Unless that would move the paddle below the bottom of the screen.
		if (_yOpponent > SCREEN_HEIGHT - GetObjectHeight(kObj_Opponent))
			dy = 0;
	}

	// Move the paddle.
	_yOpponent += dy;
	MoveObjectTo(kObj_Opponent, _xOpponent, _yOpponent);

	// TODO: Add additional game state updates for level 1 here.
}
