# Enemy Attack Interruption Setup Guide

## Overview
Enemies now get interrupted when the player attacks them, making combat more dynamic and giving players a tactical advantage.

## How It Works

### When Interruption Occurs:
- **Player hits enemy** → Enemy's current attack is immediately interrupted
- **Interrupted attacks**: Melee attacks, telegraphing charge attacks, and charging attacks
- **Interruption duration**: Configurable (default: 0.5 seconds)

### What Happens During Interruption:
1. **Attack stops immediately** - Any ongoing attack is cancelled
2. **Movement stops** - Enemy stops moving briefly
3. **State resets** - Enemy returns to orbiting state
4. **Cooldown period** - Enemy cannot start new attacks during interruption
5. **Resume normal behavior** - After interruption period, enemy resumes normal AI

## Configuration

### Inspector Settings:
- **Attack Interrupt Duration**: How long the enemy stays interrupted (default: 0.5 seconds)
  - Shorter duration = More aggressive enemies
  - Longer duration = More tactical advantage for player

### Recommended Values:
- **Fast-paced combat**: 0.3-0.5 seconds
- **Tactical combat**: 0.7-1.0 seconds
- **Very challenging**: 0.2-0.3 seconds

## Technical Details

### Events Used:
- **Health.OnHit** - Triggered when enemy takes damage
- **Automatic subscription** - No manual setup required

### Attack States Interrupted:
- `AttackState.MeleeAttack` - Close-range melee attacks
- `AttackState.TelegraphingCharge` - Charge attack preparation
- `AttackState.Charging` - Active charge attacks

### Behavior During Interruption:
- Enemy stops all movement
- Returns to `AttackState.Orbiting`
- Cannot start new attacks until interruption ends
- Resumes normal AI behavior after interruption period

## Benefits

### For Players:
- **Tactical advantage** - Can interrupt enemy attacks by being aggressive
- **Risk vs reward** - Must get close to interrupt, but risk taking damage
- **More dynamic combat** - Combat feels more interactive and responsive

### For Gameplay:
- **Balanced difficulty** - Players can counter enemy attacks
- **Skill-based combat** - Timing attacks becomes important
- **Reduced frustration** - Players have more control over combat situations

## Testing

### How to Test:
1. **Spawn enemies** in your game scene
2. **Let enemy start attacking** (melee or charge)
3. **Attack the enemy** while it's attacking
4. **Observe** - Enemy should stop attacking and move away briefly
5. **Wait** - Enemy should resume normal behavior after interruption period

### Debug Information:
- Enable `ShowDebugInfo` in the enemy inspector
- Console will show when attacks are interrupted
- Console will show when interruption period ends

## Integration

### Automatic Setup:
- **No manual configuration required** - Works out of the box
- **Uses existing Health component** - Leverages TopDownEngine's damage system
- **Compatible with all enemy types** - Works with any enemy using ChaserEnemy script

### Customization:
- **Adjust interruption duration** in the inspector
- **Modify interruption behavior** by editing the `OnEnemyHit()` method
- **Add visual/audio feedback** in the interruption logic

## Notes

- **Only interrupts attacks** - Does not prevent movement or other behaviors
- **Temporary effect** - Enemy resumes normal behavior after interruption
- **Player advantage** - Gives players a way to counter enemy attacks
- **Balanced design** - Interruption duration can be tuned for difficulty
