# Main Menu Button Manager Setup Guide

## Overview
The `MainMenuButtonManager` script provides simple functionality for main menu buttons - Play Game and Quit Game.

## Features
- **Play Game**: Loads a specified scene using TopDownEngine's scene loading system
- **Quit Game**: Properly quits the application (works in both editor and build)
- **Auto-Detection**: Automatically finds buttons named "PlayGameButton" and "QuitGameButton"
- **Manual Assignment**: Can manually assign button references in the inspector
- **Debug Support**: Optional debug logging for troubleshooting

## Setup Instructions

### Method 1: Auto-Detection (Easiest)
1. Create your main menu UI with buttons
2. Name your buttons exactly:
   - `PlayGameButton` for the play button
   - `QuitGameButton` for the quit button
3. Add the `MainMenuButtonManager` script to any GameObject in your main menu scene
4. Set the `GameSceneName` field to your game scene name (e.g., "GameScene")
5. Enable `ShowDebugInfo` if you want to see debug messages

### Method 2: Manual Assignment
1. Create your main menu UI with buttons
2. Add the `MainMenuButtonManager` script to any GameObject in your main menu scene
3. Drag your buttons into the `PlayGameButton` and `QuitGameButton` fields in the inspector
4. Set the `GameSceneName` field to your game scene name
5. Enable `ShowDebugInfo` if you want to see debug messages

## Inspector Fields

### Scene Settings
- **Game Scene Name**: The name of the scene to load when Play Game is pressed

### Button References
- **Play Game Button**: Reference to the Play Game button (optional - auto-detected if named "PlayGameButton")
- **Quit Game Button**: Reference to the Quit Game button (optional - auto-detected if named "QuitGameButton")

### Debug
- **Show Debug Info**: Enable to see debug messages in the console

## Usage

### For Play Game Button:
1. Assign the `PlayGame()` method to your button's OnClick event
2. Or use auto-detection by naming your button "PlayGameButton"

### For Quit Game Button:
1. Assign the `QuitGame()` method to your button's OnClick event
2. Or use auto-detection by naming your button "QuitGameButton"

## Example Setup

1. **Create UI Canvas**:
   - Create a Canvas in your main menu scene
   - Add two buttons: "PlayGameButton" and "QuitGameButton"

2. **Add Button Manager**:
   - Create an empty GameObject named "MainMenuManager"
   - Add the `MainMenuButtonManager` component
   - Set `GameSceneName` to your game scene name (e.g., "GameScene")

3. **Test**:
   - Play the scene
   - Click Play Game to load the game scene
   - Click Quit Game to exit the application

## Debug Features

- **Test Play Game**: Right-click the component in inspector → "Test Play Game"
- **Test Quit Game**: Right-click the component in inspector → "Test Quit Game"
- **Debug Logging**: Enable `ShowDebugInfo` to see button connection status and actions

## Notes

- The script uses TopDownEngine's `MMSceneLoadingManager` for scene loading
- Quit functionality works properly in both Unity Editor and built applications
- Button listeners are automatically cleaned up when the object is destroyed
- The script inherits from `TopDownMonoBehaviour` for TopDownEngine integration
