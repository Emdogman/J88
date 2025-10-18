# DestroyOnAnimationEvent - Setup Guide

## Overview
The `DestroyOnAnimationEvent` script allows you to destroy GameObjects from animation events. This is useful for objects like:
- Particle effects that need to self-destruct after animation
- Temporary visual effects (puke, explosions, etc.)
- Coin pickups with spawn animations
- Any GameObject that should be removed after an animation completes

## Quick Start

### Step 1: Attach the Script
1. Select the GameObject you want to destroy (e.g., your puke effect, coin, etc.)
2. Add Component → **DestroyOnAnimationEvent**

### Step 2: Configure Settings
In the Inspector, you'll see:

#### **Destroy Settings**
- **Target To Destroy**: Leave empty to destroy the current GameObject, or drag a specific GameObject to destroy something else
- **Destroy Delay**: Set to 0 for immediate destruction, or add seconds to wait before destroying

#### **Optional Effects**
- **Destruction Effect**: Drag a particle prefab to spawn when destroyed (optional)
- **Destruction Sound**: Drag an audio clip to play when destroyed (optional)

#### **Debug**
- **Show Debug Info**: Check this to see console logs for testing

### Step 3: Add Animation Event

1. **Open the Animation Window**
   - Select your GameObject
   - Window → Animation → Animation

2. **Select the Animation**
   - Choose the animation clip you want to add the event to

3. **Add the Event**
   - Move the timeline to where you want the object destroyed (usually at the end)
   - Click the "Add Event" button (small icon with a marker at the top of the timeline)

4. **Configure the Event**
   - In the Inspector, set **Function** to one of these methods:
     - `DestroyObject()` - Destroys immediately (uses the destroy delay you set)
     - `DestroyObjectWithDelay(float)` - Pass a custom delay in seconds
     - `DeactivateObject()` - Deactivates instead of destroying (for pooling)
     - `DestroySelf()` - Only removes the script component

## Available Methods

### `DestroyObject()`
**Best for**: Most common use case
- Destroys the target GameObject
- Uses the `destroyDelay` set in the Inspector
- Plays optional effects and sounds

**Animation Event Setup**:
```
Function: DestroyObject
```

### `DestroyObjectWithDelay(float delay)`
**Best for**: When you need different delays for different animations
- Destroys the target GameObject after a specific delay
- Delay is passed as a parameter from the animation event

**Animation Event Setup**:
```
Function: DestroyObjectWithDelay
Float: 1.5  (example: 1.5 seconds delay)
```

### `DeactivateObject()`
**Best for**: Object pooling systems
- Deactivates instead of destroying
- Better performance if objects are reused frequently
- Still plays optional effects

**Animation Event Setup**:
```
Function: DeactivateObject
```

### `DestroySelf()`
**Best for**: Removing just the script component
- Keeps the GameObject alive
- Only removes the DestroyOnAnimationEvent component
- Useful for one-time setup scripts

**Animation Event Setup**:
```
Function: DestroySelf
```

## Example Use Cases

### Example 1: Puke Effect
Your puke effect spawns, plays an animation, then destroys itself:

1. Attach `DestroyOnAnimationEvent` to the puke prefab
2. Leave `targetToDestroy` empty (will destroy itself)
3. Set `destroyDelay` to `0`
4. In the puke animation, add an event at the last frame
5. Set Function to `DestroyObject()`

### Example 2: Coin Drop Animation
Coin animates into position, then stays briefly before destroying:

1. Attach `DestroyOnAnimationEvent` to the coin prefab
2. Set `destroyDelay` to `2.0` (stays for 2 seconds after animation ends)
3. In the coin drop animation, add event at the last frame
4. Set Function to `DestroyObject()`

### Example 3: Explosion with Particle Effect
Explosion plays, spawns particles, then destroys itself:

1. Attach `DestroyOnAnimationEvent` to the explosion GameObject
2. Drag your particle prefab into `destructionEffect`
3. Drag your explosion sound into `destructionSound`
4. Set `destroyDelay` to `0`
5. In the explosion animation, add event at the last frame
6. Set Function to `DestroyObject()`

### Example 4: Pooled Enemy Death Effect
Enemy death effect that should be pooled, not destroyed:

1. Attach `DestroyOnAnimationEvent` to the death effect prefab
2. In the death animation, add event at the last frame
3. Set Function to `DeactivateObject()`
4. Your object pooling system can reuse this later

## Testing

### In-Editor Testing
1. Select the GameObject with the script
2. Enter **Play Mode**
3. Right-click the script in Inspector
4. Choose **"Test Destroy Object"** or **"Test Deactivate Object"**

### Debug Mode
- Enable **Show Debug Info** in the Inspector
- Console will show when methods are called and what happens
- Useful for verifying animation events are triggering correctly

## Common Issues

### Issue: Object doesn't destroy
**Solutions**:
- Check that the animation event is actually calling the method (enable Debug)
- Verify the animation plays to the frame with the event
- Make sure the GameObject has an Animator component

### Issue: Object destroys too early/late
**Solutions**:
- Adjust the animation event position on the timeline
- Use `DestroyObjectWithDelay(float)` for fine control
- Adjust the `destroyDelay` value

### Issue: Effects don't play
**Solutions**:
- Make sure effect prefabs and audio clips are assigned
- Check that MMSoundManager exists in your scene
- Enable Debug to see if effects are attempting to play

## Performance Notes

- **Destroy vs Deactivate**: 
  - Use `DestroyObject()` for one-time objects
  - Use `DeactivateObject()` for frequently spawned objects (better performance with pooling)

- **Destroy Delay**: 
  - Adding a small delay can make destruction feel smoother
  - But don't delay too long or you'll have inactive objects lingering

## Integration with Existing Systems

### Works With:
- ✅ TopDownEngine animation system
- ✅ Unity Animator and Animation Clips
- ✅ MMSoundManager for audio
- ✅ Object pooling systems (use `DeactivateObject()`)
- ✅ Particle systems

### Compatible With:
- PukeEffect system
- CoinDropAnimation system
- Enemy loot drops
- Any GameObject with animations

## Summary

**Quick Setup Checklist**:
1. ✅ Attach `DestroyOnAnimationEvent` to GameObject
2. ✅ Configure destroy settings in Inspector
3. ✅ Open Animation window
4. ✅ Add animation event at desired frame
5. ✅ Set Function to `DestroyObject()` (or your preferred method)
6. ✅ Test in Play Mode
7. ✅ Done!

**Most Common Usage**:
- Use `DestroyObject()` method
- Set destroy delay to 0 or a small value (0.5-2 seconds)
- Place event at the last frame of animation
- Enable Debug during setup, disable for production
