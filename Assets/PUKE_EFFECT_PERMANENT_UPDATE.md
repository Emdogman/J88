# Puke Effect - Permanent Mode Update

## Changes Made

The `PukeEffect.cs` script has been updated to support permanent puke effects that don't disappear.

### New Feature: `disappearAfterDuration`

A new boolean field has been added to control whether the puke effect disappears or stays permanently.

```csharp
[Tooltip("Whether the effect should disappear after duration")]
public bool disappearAfterDuration = false;
```

## Behavior

### Default Behavior (disappearAfterDuration = false):
- ✅ Puke fades in and scales up
- ✅ Puke stays visible **permanently**
- ✅ Subtle rotation animation continues indefinitely
- ✅ Effect **never** fades out or destroys itself

### Optional Disappearing (disappearAfterDuration = true):
- Puke fades in and scales up
- Puke stays visible for `effectDuration` seconds
- Puke fades out over `fadeOutTime` seconds
- Effect destroys itself after completion

## Usage

### To Keep Puke Permanent (Default):
1. Select your puke effect prefab
2. Ensure `Disappear After Duration` is **unchecked** (false)
3. The puke will now stay on screen forever

### To Make Puke Disappear:
1. Select your puke effect prefab
2. Check the `Disappear After Duration` checkbox (true)
3. Configure `Effect Duration` and `Fade Out Time` as desired
4. The puke will fade out and destroy itself after the duration

## Inspector Fields

### Effect Settings:
- **Disappear After Duration** (bool, default: false)
  - Controls whether puke disappears or stays permanent
  
- **Effect Duration** (float, default: 4s)
  - Only used if `disappearAfterDuration` is true
  - Total time before fade out begins
  
- **Fade In Time** (float, default: 0.3s)
  - Time for puke to fade in and scale up
  
- **Fade Out Time** (float, default: 1s)
  - Only used if `disappearAfterDuration` is true
  - Time for puke to fade out before destruction
  
- **Final Scale** (float, default: 1.5)
  - Final size multiplier of the puke effect
  
- **Max Alpha** (float, default: 0.7)
  - Maximum opacity of the puke effect

### Animation:
- **Animate Scale** (bool, default: true)
  - Whether to animate the scale during fade in
  
- **Animate Alpha** (bool, default: true)
  - Whether to animate the transparency
  
- **Animate Rotation** (bool, default: true)
  - Whether to add subtle rotation (works in both modes)
  
- **Rotation Speed** (float, default: 10)
  - Speed of the subtle rotation animation

## Use Cases

### Permanent Puke (Default):
Perfect for:
- Showing player's "drunk trail" throughout the level
- Creating visual evidence of where the player has been
- Building up a messy environment over time
- Making the player see the consequences of getting too drunk

### Disappearing Puke:
Good for:
- Performance optimization (fewer objects on screen)
- Temporary visual feedback
- Cleaner visual presentation
- Limited "drunk evidence" that fades away

## Notes

- **Performance**: Permanent puke means more GameObjects stay in the scene. If performance becomes an issue, consider using `disappearAfterDuration = true`.
- **Memory**: Permanent puke accumulates over time. In long play sessions, this could add up.
- **Visual Design**: Permanent puke creates a "drunk trail" effect that shows where the player has been drinking.

## Example Configuration

### Classic "Drunk Trail" (Recommended):
```
Disappear After Duration: false (unchecked)
Fade In Time: 0.3s
Final Scale: 1.5
Max Alpha: 0.7
Animate Rotation: true
Rotation Speed: 10
```

### Temporary Visual Feedback:
```
Disappear After Duration: true (checked)
Effect Duration: 5s
Fade In Time: 0.3s
Fade Out Time: 1.5s
Final Scale: 1.5
Max Alpha: 0.7
```

## Migration

If you have existing puke effect prefabs:
- They will default to `disappearAfterDuration = false` (permanent)
- No action needed if you want permanent puke
- Check the box if you want the old disappearing behavior

## Summary

- ✅ **Default**: Puke stays permanently on screen
- ✅ **Optional**: Can enable disappearing mode
- ✅ **Flexible**: Easy to switch between modes in Inspector
- ✅ **Backward Compatible**: Old behavior available via checkbox
