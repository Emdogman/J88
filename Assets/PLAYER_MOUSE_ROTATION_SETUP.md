# Player Mouse Rotation Setup Guide

## Overview
This guide will help you set up mouse-based character rotation where the player's sprite/body continuously faces the mouse cursor position. This creates a modern twin-stick shooter feel.

## Quick Setup

### Step 1: Add PlayerMouseRotation to Player
1. **Select your Player character** in the scene hierarchy
2. **Add Component** → Search "Player Mouse Rotation"
3. **Configure settings**:
   - Rotate Entire Character: ✅ (recommended)
   - Rotation Speed: 0 (instant) or 5-10 (smooth)
   - Rotation Offset: 0 (adjust if sprite faces wrong direction)
   - Use Flipping: ❌ (unless you want simple left/right flip)
   - Show Debug Line: ✅ (for testing)
   - Show Debug Info: ✅ (for testing)

### Step 2: Disable Conflicting Components
1. **Find CharacterOrientation2D** component on player
2. **Set Facing Mode to "None"** or **disable the component**
3. **Check for CharacterOrientation3D** and disable if present

### Step 3: Test the Setup
1. **Play the game**
2. **Move mouse around** → Character should rotate to face mouse
3. **Check debug line** → Red line should point from character to mouse
4. **Adjust settings** if needed

## Configuration Options

### Rotation Settings

**Rotate Entire Character (Recommended)**:
- ✅ Rotate Entire Character: true
- Rotates the entire GameObject
- Weapons and children follow automatically
- Best for most setups

**Sprite Only Rotation**:
- ❌ Rotate Entire Character: false
- Only rotates the sprite renderer
- Movement direction independent from facing
- Good for separating movement from aim

**Sprite Flipping (Simple)**:
- ✅ Use Flipping: true
- Only flips horizontally (left/right)
- Simpler than full rotation
- Good for 2D platformer-style games

### Rotation Speed

**Instant Rotation (Snappy)**:
- Rotation Speed: 0
- Character instantly faces mouse
- Good for: Fast-paced shooters
- Feels: Very responsive

**Smooth Rotation (Natural)**:
- Rotation Speed: 5-10
- Character smoothly rotates to mouse
- Good for: Character-focused games
- Feels: More natural, weighted

### Rotation Offset

Adjust if your sprite doesn't face the right direction:

- **0°** - Sprite faces right (default)
- **90°** - Sprite faces up
- **-90°** - Sprite faces down  
- **180°** - Sprite faces left

## Integration with TopDownEngine

### Weapon Aim System
The mouse rotation works alongside TopDownEngine's existing weapon aim system:
- **WeaponAim** components handle weapon rotation
- **PlayerMouseRotation** handles character body rotation
- Both can work together for complete mouse control

### Character Orientation
You may need to disable conflicting orientation components:
- **CharacterOrientation2D** - Set Facing Mode to "None"
- **CharacterOrientation3D** - Disable or set to "None"
- These handle movement-based rotation which conflicts with mouse rotation

## Testing Checklist

- [ ] Character rotates to face mouse cursor
- [ ] Rotation feels smooth/responsive
- [ ] Weapons rotate with character (if using full character rotation)
- [ ] No conflicts with movement
- [ ] Debug line shows mouse direction
- [ ] Works at different camera angles/distances
- [ ] Animation system works correctly

## Troubleshooting

### Character Doesn't Rotate
1. **Check component**: Ensure PlayerMouseRotation is enabled
2. **Check camera**: Make sure there's a camera tagged as "MainCamera"
3. **Check debug**: Enable "Show Debug Info" and check console
4. **Check conflicts**: Disable CharacterOrientation2D/3D components

### Character Rotates Wrong Direction
1. **Adjust rotation offset**: Try 0, 90, -90, 180
2. **Check sprite orientation**: Ensure sprite faces right (0°) by default
3. **Try flipping**: Enable "Use Flipping" for simple left/right

### Rotation Jitters/Glitches
1. **Use smooth rotation**: Set Rotation Speed > 0
2. **Check update timing**: Should be in HandleInput()
3. **Verify camera distance**: Mouse world position calculation needs correct Z

### Weapons Don't Follow
1. **Use full character rotation**: Set "Rotate Entire Character" to true
2. **Check weapon parent**: Weapons should be children of character
3. **Check weapon aim**: Ensure WeaponAim components are configured

### Conflicts with Movement
1. **Disable CharacterOrientation**: Turn off movement-based rotation
2. **Check animation**: Some animations may override rotation
3. **Priority order**: Mouse rotation should happen after movement

## Advanced Configuration

### Different Rotation for Different Characters

**Player Character (Full Control)**:
- Rotate Entire Character: true
- Rotation Speed: 0-5
- Rotation Offset: 0

**NPC Characters (Limited)**:
- Rotate Entire Character: false
- Use Flipping: true
- Rotation Speed: 0

### Integration with Animation

If using animation system:
1. **Check animation clips** - ensure they don't override rotation
2. **Use sprite rotation** - set "Rotate Entire Character" to false
3. **Animation priority** - ensure mouse rotation happens after animation

## Context Menu Options

Right-click on PlayerMouseRotation component for:

- **Test Rotation 0°** - Test rotation to right
- **Test Rotation 90°** - Test rotation to up
- **Test Rotation 180°** - Test rotation to left
- **Test Rotation -90°** - Test rotation to down
- **Reset Rotation** - Return to default rotation
- **Show Debug Info** - Display current settings and mouse position

## Expected Result

After setup:
- ✅ **Character sprite faces mouse cursor** at all times
- ✅ **Smooth or instant rotation** (configurable)
- ✅ **Weapons follow aim direction** automatically
- ✅ **Movement independent from facing** (can strafe)
- ✅ **Works with existing TopDownEngine systems**
- ✅ **Debug visualization** for testing
- ✅ **Easy to configure** via Inspector

This creates a modern twin-stick shooter feel where the character always faces where you're aiming! 🎯✨

## Notes

- The system works alongside TopDownEngine's existing weapon aim system
- No conflicts with movement - character can move in any direction while facing mouse
- Supports both 2D and 3D setups
- Easy to disable or modify for different character types
- Debug tools help with setup and troubleshooting
