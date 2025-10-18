# Enemy Spawner System Setup Guide

## Overview

The Enemy Spawner system spawns ChaserEnemy instances outside the camera's view with increasing difficulty over time.

**Turkish**: Düşmanlar her zaman kamera görüş alanının dışında belirir. Spawn sıklığı zamanla artar.

## Features

- ✅ **Off-Screen Spawning**: Enemies always spawn outside camera view
- ✅ **Progressive Difficulty**: Spawn rate increases over time
- ✅ **Configurable Parameters**: Customizable spawn intervals and positions
- ✅ **Debug Visualization**: Gizmos and debug info for testing
- ✅ **Easy Setup**: Helper script for quick configuration

## Quick Setup

### Method 1: Automatic Setup (Recommended)

1. **Create Spawner GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "Enemy Spawner"
   - Add `EnemySpawner` component
   - Add `EnemySpawnerSetupHelper` component

2. **Auto Setup**:
   - Select the Enemy Spawner GameObject
   - In Inspector, find `EnemySpawnerSetupHelper` component
   - Right-click the component → "Setup Enemy Spawner"
   - Right-click the component → "Validate Setup"

3. **Manual Configuration** (if needed):
   - Assign ChaserEnemy prefab to "Enemy Prefab" field
   - Adjust spawn settings as desired

### Method 2: Complete Setup

1. **Create Complete Spawner**:
   - Right-click in Hierarchy → Create Empty
   - Add `EnemySpawnerSetupHelper` component
   - Right-click component → "Create Complete Spawner"

## Configuration Guide

### Spawn Settings

| Parameter | Description | Default | Recommended |
|-----------|-------------|---------|-------------|
| **Enemy Prefab** | ChaserEnemy prefab to spawn | None | Assign your ChaserEnemy prefab |
| **Initial Spawn Interval** | Starting time between spawns | 5s | 3-8 seconds |
| **Min Spawn Interval** | Fastest possible spawn rate | 1s | 0.5-2 seconds |
| **Spawn Rate Increase** | How fast difficulty scales | 0.1 (10%) | 0.05-0.2 |
| **Max Enemies** | Maximum concurrent enemies | 0 (unlimited) | 10-50 |

### Spawn Position Settings

| Parameter | Description | Default | Recommended |
|-----------|-------------|---------|-------------|
| **Spawn Distance From Camera** | Distance outside camera view | 2 units | 1-5 units |
| **Spawn Area Padding** | Extra padding around camera | (1, 1) | (0.5, 0.5) to (2, 2) |
| **Spawn On All Sides** | Spawn from all directions | True | True for balanced gameplay |
| **Allowed Spawn Sides** | Specific sides to spawn from | All | Customize for specific gameplay |

### Difficulty Scaling

| Parameter | Description | Default | Recommended |
|-----------|-------------|---------|-------------|
| **Enable Difficulty Scaling** | Enable/disable scaling | True | True |
| **Difficulty Increase Interval** | Time between increases | 30s | 20-60 seconds |
| **Spawn Rate Curve** | Custom difficulty curve | None | Optional for advanced control |

## Usage Examples

### Basic Setup (Easy)
```csharp
// Default values for casual gameplay
Initial Spawn Interval: 5 seconds
Min Spawn Interval: 1 second
Spawn Rate Increase: 0.1 (10% faster every 30 seconds)
Spawn Distance: 2 units
```

### Hardcore Setup (Difficult)
```csharp
// Aggressive values for hard gameplay
Initial Spawn Interval: 2 seconds
Min Spawn Interval: 0.5 seconds
Spawn Rate Increase: 0.2 (20% faster every 15 seconds)
Spawn Distance: 1 unit
```

### Wave-Based Setup
```csharp
// Disable continuous spawning, use manual control
Is Spawning: False (controlled by script)
Max Enemies: 10
// Spawn enemies in waves using SpawnEnemy() method
```

## Testing the Spawner

### Debug Information
1. **Enable Debug Info**: Check "Show Debug Info" in EnemySpawner
2. **View in Game**: Debug info appears in top-left corner
3. **Scene View**: Enable "Show Spawn Zones" to see spawn areas

### Expected Behavior
- **T=0s**: First enemy spawns after initial interval
- **T=30s**: Spawn rate increases (interval decreases)
- **T=60s**: Spawn rate increases again
- **T=120s**: Spawn rate at maximum (minimum interval)
- Enemies always appear outside camera bounds
- Move camera to see enemies spawn from edges

### Debug Output
Look for these console messages:
```
EnemySpawner initialized - Initial spawn interval: 5.00s
EnemySpawner: Spawned enemy at (10.5, 8.2). Active enemies: 1
EnemySpawner: Difficulty increased! Spawn interval: 4.50s -> 4.05s
```

## Advanced Configuration

### Custom Difficulty Curve
1. Enable "Spawn Rate Curve" in EnemySpawner
2. Create AnimationCurve in Inspector
3. X-axis: Time progress (0-1)
4. Y-axis: Spawn rate multiplier (0-1)
5. Curve controls how spawn rate changes over time

### Spawn Side Control
```csharp
// Spawn only from top and bottom
Spawn On All Sides: False
Allowed Spawn Sides: [Top, Bottom]

// Spawn only from left and right
Spawn On All Sides: False
Allowed Spawn Sides: [Left, Right]
```

### Script Control
```csharp
// Get spawner reference
EnemySpawner spawner = FindObjectOfType<EnemySpawner>();

// Stop spawning
spawner.StopSpawning();

// Start spawning
spawner.isSpawning = true;
spawner.StartSpawning();
```

## Troubleshooting

### Common Issues

**No enemies spawning**:
- Check if "Is Spawning" is enabled
- Verify "Enemy Prefab" is assigned
- Check console for error messages

**Enemies spawning on screen**:
- Increase "Spawn Distance From Camera"
- Check camera assignment
- Verify "Spawn Area Padding" values

**Too many/few enemies**:
- Adjust "Max Enemies" limit
- Modify spawn intervals
- Check "Spawn Rate Increase" value

**Performance issues**:
- Set "Max Enemies" limit
- Increase spawn intervals
- Check enemy cleanup in ChaserEnemy

### Debug Steps
1. Enable "Show Debug Info" and "Show Spawn Zones"
2. Check console for error messages
3. Verify camera and prefab assignments
4. Test with different spawn settings
5. Use "Validate Setup" in EnemySpawnerSetupHelper

## Integration with Existing Systems

### ChaserEnemy Integration
- Spawner works with existing ChaserEnemy prefabs
- No modifications needed to ChaserEnemy script
- Enemies automatically find and chase player

### Loot System Integration
- Spawned enemies drop KoalaCoinPicker items
- Loot drop rate configured in ChaserEnemy
- No additional setup required

### Camera System Integration
- Works with any camera (Camera.main, Cinemachine, etc.)
- Automatically detects camera bounds
- Updates spawn zones as camera moves

## Performance Considerations

### Optimization Tips
- Set reasonable "Max Enemies" limit (10-50)
- Use appropriate spawn intervals (not too fast)
- Monitor active enemy count in debug info
- Consider enemy cleanup and pooling

### Memory Management
- Spawned enemies are automatically tracked
- Destroyed enemies are removed from tracking
- Use spawn parent for organization
- Monitor memory usage with many enemies

## Files Created

- `Assets/TopDownEngine/Common/Scripts/Enemies/EnemySpawner.cs` - Main spawner component
- `Assets/TopDownEngine/Common/Scripts/Enemies/EnemySpawnerSetupHelper.cs` - Setup helper
- `Assets/ENEMY_SPAWNER_GUIDE.md` - This guide

## Example Timeline

**T=0s**: Spawner starts, first enemy spawns in 5 seconds
**T=5s**: First enemy spawns outside camera view
**T=30s**: Difficulty increases, spawn interval: 5s → 4.5s
**T=60s**: Difficulty increases, spawn interval: 4.5s → 4.05s
**T=90s**: Difficulty increases, spawn interval: 4.05s → 3.65s
**T=120s**: Difficulty increases, spawn interval: 3.65s → 3.28s
**T=300s**: Spawn interval reaches minimum (1s), maximum difficulty

The spawner creates a natural difficulty progression that keeps gameplay challenging and engaging!
