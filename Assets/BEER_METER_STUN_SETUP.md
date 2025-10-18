# Beer Meter Stun System Setup Guide

## Overview
This guide will help you set up the beer meter stun system where the player gets stunned for 1.5 seconds when the beer meter reaches 100, preventing movement and attacks, and spawning a green puke effect nearby.

## Quick Setup

### Step 1: Create Puke Effect Prefab
1. **Create empty GameObject** named "PukeEffect"
2. **Add SpriteRenderer** component
3. **Create green circle sprite** (or use existing sprite)
4. **Set color to green** (#00FF00 or similar)
5. **Add PukeEffect.cs script**
6. **Configure settings**:
   - Effect Duration: 4
   - Fade In Time: 0.3
   - Fade Out Time: 1
   - Final Scale: 1.5
   - Max Alpha: 0.7
6. **Save as prefab** in Assets folder

### Step 2: Add BeerMeterStunHandler to Player
1. **Select your Player character** in the scene hierarchy
2. **Add Component** → Search "Beer Meter Stun Handler"
3. **Configure settings**:
   - Stun Duration: 1.5
   - Beer Meter Threshold: 100
   - Beer Meter Reset Value: 15
   - Puke Effect Prefab: (assign the prefab from Step 1)
   - Puke Spawn Distance: 1
   - Puke Spawn Random Offset: 0.3
   - Puke Effect Duration: 4
4. **Enable "Show Debug Info"** for testing

### Step 3: Test the System
1. **Play the game**
2. **Increase beer meter to 100** (use BeerManager or debug tools)
3. **Verify stun triggers** - player should freeze
4. **Check puke effect spawns** - green circle should appear
5. **Wait 1.5 seconds** - player should unfreeze
6. **Check beer meter resets** to 15

## Configuration Options

### Stun Settings

**Stun Duration**: 1.5 seconds (as requested)
- Controls how long the player is frozen
- Range: 0.5-5 seconds

**Beer Meter Threshold**: 100 (full meter)
- When stun triggers
- Range: 90-100

**Beer Meter Reset Value**: 15
- What the meter resets to after stun
- Range: 0-50

### Puke Effect Settings

**Spawn Distance**: 1 unit from player
- How far from player to spawn puke
- Range: 0.5-3 units

**Random Offset**: 0.3 units
- Random variation in spawn position
- Range: 0-1 units

**Effect Duration**: 4 seconds
- How long puke stays visible
- Range: 1-10 seconds

### Visual Design

**Puke Effect Prefab**:
- **Color**: Green (#00FF00)
- **Shape**: Circle sprite
- **Size**: Medium (1.5x scale)
- **Alpha**: Fades in/out smoothly
- **Animation**: Subtle rotation during display

## Integration with BeerManager

The system automatically integrates with the existing `BeerManager` singleton:

```csharp
// The system monitors BeerManager.CurrentBeer
// When it reaches the threshold, stun triggers
// After stun, the meter resets to the specified value
```

**BeerManager Events**:
- `OnBeerLevelChanged` - Monitored for threshold detection
- `CurrentBeer` - Direct access to beer level
- `AddBeer()` - Can be used to test the system

## Testing the System

### Manual Testing
1. **Use BeerManager debug tools** to set beer level to 100
2. **Or use BeerMeterStunHandler context menu**:
   - Right-click component → "Force Trigger Stun"
   - Right-click component → "Test Stun Sequence"

### Debug Information
Enable "Show Debug Info" to see:
- Beer level monitoring
- Stun trigger events
- Character ability states
- Puke effect spawning

### Expected Behavior

**When Beer Meter Reaches 100**:
1. ✅ **Stun triggers immediately**
2. ✅ **Player cannot move** (MovementAuthorized = false)
3. ✅ **Player cannot attack** (WeaponAbility = false)
4. ✅ **Input is blocked** (InputDetectionActive = false)
5. ✅ **Green puke effect spawns** near player
6. ✅ **Stun lasts exactly 1.5 seconds**
7. ✅ **Beer meter resets to 0**
8. ✅ **Player returns to normal** after stun

## Troubleshooting

### Stun Doesn't Trigger
1. **Check BeerManager**: Ensure it exists in scene
2. **Check threshold**: Verify beer level reaches 100
3. **Check component**: Ensure BeerMeterStunHandler is enabled
4. **Check debug**: Enable "Show Debug Info" and check console

### Player Still Moves During Stun
1. **Check InputAuthorized**: Should be false during stun
2. **Check ConditionState**: Should be set to Stunned
3. **Check InputManager**: InputDetectionActive should be false
4. **Check movement**: CharacterMovement.InputAuthorized should be false

### Puke Effect Not Spawning
1. **Check prefab**: Ensure pukeEffectPrefab is assigned
2. **Check spawn position**: Verify calculation is correct
3. **Check visibility**: Ensure sprite is visible and green
4. **Check duration**: Effect should auto-destroy after duration

### Beer Meter Doesn't Reset
1. **Check BeerManager**: Ensure it's accessible
2. **Check reset value**: Verify beerMeterResetValue is set
3. **Check timing**: Reset happens after stun completes

## Advanced Configuration

### Custom Puke Effects
Create different puke effect prefabs:
- **Small puke**: Smaller scale, shorter duration
- **Large puke**: Larger scale, longer duration
- **Animated puke**: Add particle effects or animations
- **Damaging puke**: Add damage to enemies in area

### Multiple Stun Triggers
The system prevents multiple stuns:
- Only one stun can be active at a time
- Additional triggers are ignored while stunned
- System automatically resets after stun completes

### Integration with Other Systems
The stun system works with:
- **CharacterStates**: Uses Stunned condition
- **CharacterMovement**: Disables movement
- **CharacterHandleWeapon**: Disables weapon use
- **InputManager**: Blocks input
- **BeerManager**: Monitors and resets beer level

## Context Menu Options

Right-click on BeerMeterStunHandler component for:

- **Test Stun Sequence** - Test the complete stun sequence
- **Force Trigger Stun** - Force trigger stun (for testing)
- **Show Status** - Display current system status

## Expected Result

After setup:
- ✅ **Beer meter monitored** continuously
- ✅ **Stun triggers at 100** automatically
- ✅ **Player frozen** - no movement or attacks for 1.5s
- ✅ **Green puke effect** spawns near player
- ✅ **Beer meter resets to 15** after stun completes
- ✅ **Player returns to normal** after stun
- ✅ **Visual feedback** clear and satisfying
- ✅ **Integrates with TopDownEngine** character state system

This creates a fun drunk mechanic with clear visual feedback and gameplay consequences! 🍺🥴✨

## Notes

- The system uses the existing BeerManager singleton
- No conflicts with other character abilities
- Easy to configure via Inspector
- Debug tools help with setup and testing
- Works with both 2D and 3D setups
- Can be easily disabled or modified
