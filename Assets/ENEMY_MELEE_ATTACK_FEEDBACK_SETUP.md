# Enemy Melee Attack Feedback Setup Guide

## Overview
The `ChaserEnemy` script now supports **MMFeedbacks** for melee attacks. This allows you to add visual and audio feedback when the enemy performs a melee attack.

## Changes Made

### 1. Added MMFeedbacks Support
**File**: `Assets/TopDownEngine/Common/Scripts/Enemies/ChaserEnemy.cs`

#### New Import
```csharp
using MoreMountains.Feedbacks;
```

#### New Field
```csharp
[Header("Feedbacks")]
[Tooltip("Feedbacks to play when the enemy starts a melee attack")]
public MMFeedbacks MeleeAttackStartFeedbacks;
```

#### Updated Method
In the `PerformMeleeAttack()` method:
```csharp
// Play melee attack start feedbacks
MeleeAttackStartFeedbacks?.PlayFeedbacks(transform.position);
```

## How to Setup

### Step 1: Add MMFeedbacks Component to Enemy
1. **Select your Enemy prefab** in the scene or prefab editor
2. **Add Component** → Search for "**MM Feedbacks**"
3. Add the **MMFeedbacks** component

### Step 2: Configure Feedbacks
In the **MMFeedbacks** component you just added:

1. **Add Feedbacks** - Click the "+" button to add feedback types:
   - **MMF_AudioSource** - For attack sounds
   - **MMF_Flash** - For visual flash effect
   - **MMF_Scale** - For scale punch effect
   - **MMF_Particles** - For particle effects
   - **MMF_CameraShake** - For camera shake (if desired)
   - **MMF_Sound** - For sound effects via MMSoundManager

2. **Configure Each Feedback**:
   - Set the **Timing** (when each feedback plays)
   - Set the **Intensity** for visual effects
   - Assign **Audio Clips** for sound feedbacks
   - Assign **Particle Prefabs** for particle effects

### Step 3: Link MMFeedbacks to ChaserEnemy
1. **Select your Enemy prefab**
2. Find the **ChaserEnemy** component in the Inspector
3. Look for the **"Feedbacks"** section
4. Drag the **MMFeedbacks** component into the **"Melee Attack Start Feedbacks"** field

### Step 4: Test
1. **Enter Play Mode**
2. Wait for the enemy to get close and perform a melee attack
3. You should see/hear the feedbacks you configured!

## Example Feedback Configurations

### Example 1: Simple Attack Sound
**Best for**: Basic audio feedback

1. Add **MMF_Sound** feedback
2. Assign your attack sound clip (e.g., "sword_swing.wav")
3. Set volume to 0.5-1.0

### Example 2: Attack with Visual Flash
**Best for**: More impactful attacks

1. Add **MMF_Sound** feedback (attack sound)
2. Add **MMF_Flash** feedback
   - Target: Enemy's SpriteRenderer
   - Flash Color: Red or White
   - Duration: 0.1-0.2 seconds
3. Add **MMF_Scale** feedback
   - Target: Enemy transform
   - Scale: 1.2 (20% larger)
   - Duration: 0.15 seconds
   - Curve: Animation curve with punch effect

### Example 3: Full Combat Feedback
**Best for**: Polished, game-ready enemies

1. **MMF_Sound** - Attack grunt/yell sound
2. **MMF_Flash** - White flash on enemy sprite (0.1s)
3. **MMF_Scale** - Scale punch (1.15x for 0.2s)
4. **MMF_Particles** - Slash effect particles at attack position
5. **MMF_CameraShake** - Subtle camera shake (intensity 0.1-0.3)

### Example 4: Enemy-Specific Sounds
Different enemies can have different feedback:

**Fast Enemy**:
- Quick, sharp attack sound
- Fast flash (0.05s)
- Small scale punch (1.1x)

**Heavy Enemy**:
- Deep, heavy attack sound
- Slower flash (0.2s)
- Larger scale punch (1.3x)
- Stronger camera shake

## Tips and Best Practices

### Performance
- **Keep feedback count reasonable** - 3-5 feedbacks per attack is usually enough
- **Avoid expensive effects** - Don't spawn too many particles
- **Use object pooling** - If using particle feedbacks, enable pooling

### Feel and Polish
- **Layer multiple feedbacks** - Combine sound, visual, and haptic feedback
- **Use animation curves** - Makes scale/position changes feel more natural
- **Time feedbacks properly** - Make sure they sync with the animation
- **Test in-game** - What looks good in inspector might feel different in gameplay

### Audio
- **Volume control** - Keep attack sounds at 0.5-0.8 volume (not too loud)
- **Variation** - Use MMF_Sound with multiple clips for variety
- **Pitch variation** - Add slight random pitch (0.9-1.1) for organic feel

### Visual
- **Flash duration** - Keep flashes short (0.05-0.2s) to avoid seizure risk
- **Color choice** - White for generic, Red for heavy attacks, Blue for magic
- **Scale amount** - Subtle is better (1.1-1.3x max)

## Troubleshooting

### Issue: Feedback doesn't play
**Solutions**:
- Check that the MMFeedbacks component is assigned in the **Melee Attack Start Feedbacks** field
- Make sure the MMFeedbacks component is on the same GameObject (or a child)
- Verify that feedbacks are not disabled in the MMFeedbacks inspector
- Enable **Show Debug Info** in ChaserEnemy to see if melee attacks are triggering

### Issue: Feedback plays but nothing happens
**Solutions**:
- Check that each feedback is properly configured (targets assigned, values set)
- Make sure audio clips are assigned to audio feedbacks
- Verify sprite renderers are assigned to visual feedbacks
- Test each feedback individually using the "Play" button in inspector

### Issue: Feedback plays too often/not often enough
**Solutions**:
- Adjust the **Attack Cooldown** in ChaserEnemy (default 1 second)
- Check the **enemy_Melee_Radius** - smaller radius = less frequent attacks
- Modify **attackWhileMoving** - disabling makes attacks more deliberate

### Issue: Sound doesn't play
**Solutions**:
- Make sure **MMSoundManager** exists in your scene
- Check that audio source/listener are present
- Verify audio clip is not muted or too quiet
- Use MMF_AudioSource instead of MMF_Sound if MMSoundManager isn't available

## Integration with Existing Systems

### Works With:
- ✅ TopDownEngine CharacterHandleWeapon system
- ✅ MeleeWeapon component
- ✅ Attack animations
- ✅ MMSoundManager
- ✅ Camera shake systems
- ✅ Particle systems

### Compatible With:
- Attack interruption system (feedback respects interruptions)
- Beer meter stun system
- Enemy spawning system
- Loot drop system
- Score system

## Advanced: Multiple Feedback Types

You can create **different feedback sets** for different situations:

### Setup Multiple Feedbacks
1. Add **multiple MMFeedbacks components** to the enemy
2. Name them clearly: "Melee_Light_Feedback", "Melee_Heavy_Feedback"
3. Assign the appropriate one to **MeleeAttackStartFeedbacks**

### Use Cases
- **Different feedback per enemy type** (fast vs slow enemies)
- **Different feedback per stage** (normal vs enraged)
- **Context-specific feedback** (attack in water vs on land)

## Summary

**Quick Setup Checklist**:
1. ✅ Add **MMFeedbacks** component to enemy GameObject
2. ✅ Configure feedbacks (sound, flash, scale, particles, etc.)
3. ✅ Assign MMFeedbacks to **Melee Attack Start Feedbacks** field in ChaserEnemy
4. ✅ Test in Play Mode
5. ✅ Fine-tune timing and intensity
6. ✅ Done!

**Recommended Minimum Setup**:
- **MMF_Sound** with attack sound effect
- **MMF_Flash** with brief white flash (0.1s)

**Recommended Full Setup**:
- **MMF_Sound** with attack sound
- **MMF_Flash** with brief flash
- **MMF_Scale** with punch effect
- **MMF_Particles** with slash/impact particles

The feedback system makes combat feel more impactful and gives players better visual/audio cues for enemy attacks!
