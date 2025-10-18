# Drunken Animation Setup Guide

This guide explains how to set up and use the drunken animation system for the koala character.

## Overview

The drunken animation system consists of several components that work together to provide smooth animation transitions between idle and drunken run states.

## Components

### 1. DrunkenAnimationController.cs
A basic animation controller that handles simple animation state transitions.

**Features:**
- Automatic movement detection
- Idle and drunken run state management
- Manual animation triggering
- Animation speed control

**Usage:**
```csharp
// Get the controller
DrunkenAnimationController controller = GetComponent<DrunkenAnimationController>();

// Trigger animations manually
controller.TriggerDrunkenRun();
controller.TriggerIdle();

// Set animation speed
controller.SetAnimationSpeed(1.5f);
```

### 2. DrunkenCharacterAnimation.cs
An advanced animation controller that integrates with the TopDown Engine.

**Features:**
- Full TopDown Engine integration
- Drunken mode toggling
- Advanced animation state management
- Multiple animator controller support

**Usage:**
```csharp
// Get the controller
DrunkenCharacterAnimation animation = GetComponent<DrunkenCharacterAnimation>();

// Toggle drunken mode
animation.ToggleDrunkenMode();

// Check current state
bool isDrunken = animation.IsDrunkenMode();
bool isMoving = animation.IsMoving();
```

### 3. DrunkenAnimationSetup.cs
A setup utility for configuring animations in the inspector.

**Features:**
- Automatic component setup
- Animation validation
- Inspector-based configuration
- One-click setup

**Usage:**
1. Add the component to your koala prefab
2. Assign the drunken animator controller
3. Click "Setup Drunken Animations" in the context menu

### 4. DrunkenAnimationExample.cs
An example script showing how to use the animation system.

**Features:**
- Keyboard input handling
- Animation state display
- Example usage patterns

## Setup Instructions

### Step 1: Prepare the Koala Prefab
1. Open the koala prefab in the scene
2. Ensure the "KoalaModel" child has an Animator component
3. Assign the "Drunken anim.controller" to the animator

### Step 2: Add Animation Components
1. Add `DrunkenAnimationSetup` to the koala prefab
2. Assign the drunken animator controller in the inspector
3. Click "Setup Drunken Animations" in the context menu

### Step 3: Configure Animation Parameters
The system uses these animator parameters:
- `Move` (Bool) - Controls transition between idle and run states
- `Drunken` (Bool) - Controls drunken mode (optional)
- `Speed` (Float) - Animation speed multiplier (optional)

### Step 4: Test the Setup
1. Add `DrunkenAnimationExample` to the koala prefab
2. Use the keyboard controls to test animations:
   - `D` - Toggle drunken mode
   - `R` - Trigger drunken run
   - `I` - Trigger idle
   - `T` - Reset animation

## Animation Controller Structure

The "Drunken anim.controller" contains:
- **Idle State**: Default state when not moving
- **Drunken run State**: Active when moving
- **Transitions**: Based on the "Move" parameter

## Animation Clips

The system uses these animation clips:
- `Idle.anim` - Idle animation
- `Drunken run.anim` - Drunken running animation
- `Drunken feet.anim` - Feet animation (optional)

## Troubleshooting

### Common Issues

1. **No Animator Found**
   - Ensure the KoalaModel child has an Animator component
   - Check that the animator is properly assigned

2. **Animations Not Playing**
   - Verify the animator controller is assigned
   - Check that animation parameters are set correctly
   - Ensure the character is moving above the threshold

3. **Animation Transitions Not Working**
   - Verify the "Move" parameter exists in the animator
   - Check transition conditions in the animator controller

### Debug Information

The system provides debug logs for:
- Animation state changes
- Parameter updates
- Component initialization
- Error conditions

## Advanced Usage

### Custom Animation States
You can extend the system by:
1. Adding new animation states to the controller
2. Creating new parameters in the animator
3. Modifying the animation scripts to handle new states

### Performance Optimization
- Use animation parameter hashes for better performance
- Cache animator references to avoid repeated lookups
- Use the built-in sanity checks to prevent errors

## Integration with TopDown Engine

The system integrates seamlessly with TopDown Engine by:
- Using the Character component for movement detection
- Following TopDown Engine animation patterns
- Supporting the engine's animation parameter system

## Example Code

```csharp
// Get the animation controller
DrunkenCharacterAnimation animation = GetComponent<DrunkenCharacterAnimation>();

// Enable drunken mode
animation.EnableDrunkenMode();

// Check if character is moving
if (animation.IsMoving())
{
    Debug.Log("Character is in drunken run state");
}

// Set custom animation speed
animation.SetAnimationSpeed(1.5f);
```

## Support

For issues or questions:
1. Check the debug logs for error messages
2. Verify all components are properly assigned
3. Ensure the animator controller is correctly configured
