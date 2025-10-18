# AIActionMoveTowardsPlayer Setup Guide

## Overview
This guide explains how to convert existing TopDownEngine enemies (like KoalaNinjaSwordMaster) to use simple player-chasing behavior instead of complex AI systems.

## Quick Setup for KoalaNinjaSwordMaster

### Step 1: Remove Complex AI Components
1. **Select KoalaNinjaSwordMaster** in the scene
2. **Remove/Disable these components:**
   - All `AIDecision` components (AIDecisionDistance, AIDecisionTime, etc.)
   - Complex `AIAction` components (AIActionPatrol, AIActionShoot, etc.)
   - Any custom AI scripts
   - `AIBrain` states that aren't needed

### Step 2: Add Simple Chaser AI
1. **Add AIActionMoveTowardsPlayer** component
2. **Configure settings:**
   ```
   Target Settings:
   - Player Tag: "Player"
   - Player Search Interval: 1.0
   
   Movement Settings:
   - Use Avoidance: True (recommended)
   - Avoidance Radius: 1.0
   - Avoidance Strength: 0.3
   - Enemy Layer Mask: Enemy layer
   
   Debug:
   - Show Debug Info: True (for testing)
   ```

### Step 3: Configure AIBrain
1. **Open AIBrain** component
2. **Create new state** called "Chase Player"
3. **Add AIActionMoveTowardsPlayer** to this state
4. **Remove all other states** or make this the default
5. **No transitions needed** - this state runs continuously

### Step 4: Test
1. **Play the scene**
2. **Check console** for "Found player" messages
3. **Verify enemy chases player**
4. **Test with multiple enemies** to see avoidance

## Detailed Component Setup

### Required Components (Keep These)
```
KoalaNinjaSwordMaster:
├── Character (required)
├── CharacterMovement (or CharacterMovementStaged)
├── Health (if you want damage)
├── AIBrain (simplified)
├── Colliders (for physics)
├── SpriteRenderer (visual)
└── Animator (if using animations)
```

### Components to Remove/Disable
```
Remove These:
├── AIDecisionDistance
├── AIDecisionTime
├── AIDecisionHealth
├── AIActionPatrol
├── AIActionShoot
├── AIActionAttack
├── AIActionWait
└── Any custom AI scripts
```

### AIBrain Configuration
```
States:
└── Chase Player
    ├── Actions: AIActionMoveTowardsPlayer
    └── Transitions: None (always active)
```

## Configuration Options

### Basic Setup (Minimal)
```
AIActionMoveTowardsPlayer:
- Player Tag: "Player"
- Use Avoidance: False
- Show Debug Info: False
```

### Advanced Setup (Recommended)
```
AIActionMoveTowardsPlayer:
- Player Tag: "Player"
- Player Search Interval: 1.0
- Use Avoidance: True
- Avoidance Radius: 1.0
- Avoidance Strength: 0.3
- Enemy Layer Mask: Enemy
- Show Debug Info: True
```

## Layer Setup

### Create Enemy Layer
1. **Edit > Project Settings > Tags and Layers**
2. **Create "Enemy" layer**
3. **Assign KoalaNinjaSwordMaster to Enemy layer**
4. **Set Enemy Layer Mask** to include Enemy layer

### Player Tag Setup
1. **Ensure player has "Player" tag**
2. **If not, select player GameObject**
3. **Change Tag to "Player"**

## Troubleshooting

### Enemy Not Moving
- **Check AIBrain** has the action in a state
- **Verify CharacterMovement** component exists
- **Check console** for error messages
- **Enable Show Debug Info** for diagnostics

### Enemy Not Finding Player
- **Verify player has "Player" tag**
- **Check Player Tag** setting in AIActionMoveTowardsPlayer
- **Enable Show Debug Info** to see search messages

### Enemies Clustering
- **Enable Use Avoidance**
- **Increase Avoidance Radius**
- **Increase Avoidance Strength**
- **Check Enemy Layer Mask** includes enemy layer

### Performance Issues
- **Increase Player Search Interval**
- **Reduce Avoidance Radius**
- **Limit number of enemies**

## Advanced Configuration

### Multiple Enemy Types
```
Fast Enemy:
- Avoidance Radius: 0.8
- Avoidance Strength: 0.2

Slow Enemy:
- Avoidance Radius: 1.2
- Avoidance Strength: 0.4
```

### Custom Player Detection
```
Manual Player Assignment:
- Use SetPlayer() method
- Override automatic tag search
- Useful for specific scenarios
```

## Testing Checklist

- [ ] Enemy finds player automatically
- [ ] Enemy moves towards player
- [ ] Enemy avoids other enemies (if enabled)
- [ ] No console errors
- [ ] Performance is acceptable
- [ ] Multiple enemies work together

## Example Complete Setup

```
GameObject: "KoalaNinjaSwordMaster"
├── Character
├── CharacterMovement
├── Health
├── AIBrain
│   └── State: "Chase Player"
│       └── Actions: AIActionMoveTowardsPlayer
├── CircleCollider2D
├── SpriteRenderer
└── AIActionMoveTowardsPlayer
    ├── Player Tag: "Player"
    ├── Use Avoidance: True
    ├── Avoidance Radius: 1.0
    └── Show Debug Info: True
```

## Benefits of This Approach

1. **Simplicity**: One component replaces complex AI
2. **Compatibility**: Works with existing TopDownEngine systems
3. **Performance**: Lightweight and efficient
4. **Flexibility**: Easy to configure and modify
5. **Reusability**: Can apply to any enemy character

## Next Steps

After basic setup:
1. **Test with multiple enemies**
2. **Adjust avoidance settings**
3. **Add custom behaviors** if needed
4. **Optimize for performance**
5. **Create enemy prefabs** for reuse
