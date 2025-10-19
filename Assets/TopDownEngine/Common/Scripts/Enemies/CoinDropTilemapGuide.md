# Coin Drop Tilemap Integration Guide

## Overview
The ChaserEnemy now validates coin drop positions against your Tilemap to ensure coins only spawn on walkable tiles within the level boundaries.

## How It Works

### Automatic Tilemap Detection
The system automatically searches for your walkable tilemap using these names (in order):
1. "Ground"
2. "Floor"
3. "Walkable"
4. "Base"
5. "Tilemap"
6. **Fallback:** Any Tilemap component in the scene

### Walkability Check
When an enemy dies and drops coins:
1. System generates a random offset position around the enemy
2. Converts world position to tilemap cell coordinates
3. Checks if a tile exists at that cell using `HasTile()`
4. If tile exists = walkable ✅
5. If no tile = outside level boundaries ❌

### Retry Logic
- Tries up to **8 random positions** to find a valid drop location
- If all attempts fail, spawns coin at enemy's exact position (safe fallback)

## Setup Options

### Option 1: Automatic (Recommended)
**No setup needed!** Just ensure your walkable tilemap has one of these names:
- Ground
- Floor  
- Walkable
- Base
- Tilemap

### Option 2: Manual Assignment
1. Select your enemy GameObject in the hierarchy
2. Find the `ChaserEnemy` component in the Inspector
3. Expand the **References** section
4. Drag your walkable Tilemap into the `Walkable Tilemap` field

## Debug Mode

Enable debug logging to see:
- Which tilemap was auto-detected
- When positions fail validation
- When fallback position is used

**To Enable:**
1. Select enemy in hierarchy
2. Find `ChaserEnemy` component
3. Check `Show Debug Info` under the Debug section

## Troubleshooting

### Coins still spawning outside the level
**Cause:** Tilemap not detected correctly

**Solutions:**
1. Check console for "Auto-found tilemap" message
2. Manually assign the correct Tilemap in Inspector
3. Ensure your walkable tiles actually have tiles placed on them

### Coins always spawning at enemy position
**Cause:** No valid positions found in 8 attempts

**Solutions:**
1. Increase `Drop Offset` value (currently 0.5)
2. Ensure tiles exist around enemy death locations
3. Check that enemy dies on walkable tiles

### Multiple tilemaps in scene
**Cause:** System finds wrong tilemap

**Solutions:**
1. Rename your walkable tilemap to "Ground" or "Floor"
2. Manually assign the correct tilemap in Inspector

## Technical Details

### Methods Added
- `GetValidCoinDropPosition()` - Finds valid spawn position with retries
- `IsPositionWalkable(Vector3)` - Validates if position has a walkable tile
- `FindWalkableTilemap()` - Auto-detects tilemap in scene

### Parameters
- `maxAttempts = 8` - Number of random positions to try
- `dropOffset` - Range of random offset (configurable in Inspector)

## Performance
- Minimal overhead: Only runs when enemy dies
- Efficient: Uses Unity's built-in `HasTile()` method
- Cached: Tilemap reference cached after first lookup

## Example Scenarios

### Scenario 1: Enemy dies on walkable tile
✅ Coin spawns within dropOffset range on nearby walkable tiles

### Scenario 2: Enemy dies near wall
✅ Coin spawns on walkable tile, avoids wall cells

### Scenario 3: Enemy dies on isolated tile
✅ If no nearby valid positions, coin spawns at enemy position

### Scenario 4: No tilemap in scene
⚠️ System allows all positions (fail-safe behavior)

