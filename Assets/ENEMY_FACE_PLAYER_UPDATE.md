# Enemy Always Face Player - Update

## Overview
Enemies now **always face the player** regardless of their movement state, making them more intimidating and visually consistent.

## What Changed

### Before:
- Enemies only rotated when moving
- Idle enemies would face random directions
- Rotation was based on movement direction

### After:
- Enemies **always face the player** at all times
- Rotation is based on direction to player, not movement
- More consistent and threatening visual behavior

## Technical Details

### Method Updated:
- **`RotateTowardsMovement()`** → **`RotateTowardsPlayer()`**
- **Logic**: Now calculates direction to player instead of movement direction
- **Behavior**: Continuous rotation regardless of movement state

### Key Changes:
```csharp
// OLD: Only rotated when moving
if (movementDirection.magnitude < movementDeadZone) return;

// NEW: Always faces player
Vector2 directionToPlayer = (player.position - transform.position).normalized;
```

### Rotation Speed:
- **Configurable**: `rotationSpeed` field in inspector (default: 180°/second)
- **Smooth**: Uses `Quaternion.RotateTowards()` for smooth rotation
- **Consistent**: Same rotation speed regardless of distance to player

## Visual Impact

### Benefits:
- **More Intimidating**: Enemies always "looking" at the player
- **Visual Consistency**: All enemies face the same direction
- **Better Combat Feel**: Enemies appear more focused and threatening
- **Clear Intent**: Player can see where enemies are "aiming"

### Behavior:
- **Orbiting**: Enemy faces player while orbiting
- **Charging**: Enemy faces player during charge attacks
- **Idle**: Enemy faces player even when not moving
- **Interrupted**: Enemy faces player during attack interruption

## Configuration

### Inspector Settings:
- **Rotation Speed**: How fast enemy rotates to face player (degrees/second)
  - **Default**: 180°/second (half rotation per second)
  - **Faster**: 360°/second (full rotation per second)
  - **Slower**: 90°/second (quarter rotation per second)

### Recommended Values:
- **Fast-paced combat**: 270-360°/second
- **Standard combat**: 180-270°/second
- **Slow, methodical**: 90-180°/second

## Testing

### How to Test:
1. **Spawn enemies** in your game scene
2. **Move around** as the player
3. **Observe** - All enemies should face you at all times
4. **Test different scenarios**:
   - Enemy orbiting around you
   - Enemy charging at you
   - Enemy idle/not moving
   - Enemy interrupted by your attack

### Expected Behavior:
- Enemies should smoothly rotate to face the player
- Rotation should be consistent regardless of enemy state
- No enemies should face away from the player
- Rotation should feel natural and not jittery

## Notes

- **Performance**: Minimal impact - only calculates direction and rotates
- **Compatibility**: Works with all existing enemy behaviors
- **Customization**: Rotation speed can be adjusted per enemy
- **Sprite Orientation**: Uses 90-degree offset for typical Unity sprite orientation
