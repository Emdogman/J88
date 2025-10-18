# Manual Spawn Area Restriction - Setup Guide

## Overview
The `EnemySpawner` now has a manual spawn area restriction feature that lets you define exactly where enemies can spawn using simple X/Y min/max values.

## Quick Setup

### Step 1: Enable Manual Spawn Area
1. Select `EnemySpawner` in your scene
2. Find the **"Manual Spawn Area Restriction"** section
3. **Check** the `Use Manual Spawn Area` checkbox

### Step 2: Set Your Boundaries
Configure the min/max values to match your level:

```
Spawn Area Min X: -15  (left boundary)
Spawn Area Max X: 15   (right boundary)
Spawn Area Min Y: -15  (bottom boundary)
Spawn Area Max Y: 15   (top boundary)
```

**Example**: For a 30x30 level centered at (0,0):
- Min X: -15, Max X: 15
- Min Y: -15, Max Y: 15

### Step 3: Visualize in Scene View
1. Select the `EnemySpawner` GameObject
2. Look in Scene view for a **green wireframe box**
3. This shows your spawn restriction area
4. Adjust the min/max values until the green box matches your playable area

## How It Works

### The System Ensures TWO Things:

1. **Enemies spawn OUTSIDE camera view** (player can't see them spawn)
2. **Enemies spawn WITHIN your manual bounds** (stay in level)

### The Process:
```
Step 1: Calculate camera bounds
Step 2: Restrict camera bounds to fit within manual area
Step 3: Calculate spawn position outside restricted camera
Step 4: Clamp position to manual bounds (safety)
Step 5: Verify position is STILL outside camera view
Step 6: If yes → Spawn enemy
Step 7: If no → Retry (up to 30 times)
```

### Example:
```
Manual Bounds: X(-20 to 20), Y(-20 to 20)
Camera View: X(-5 to 5), Y(-3 to 3)

Spawn calculation:
→ Tries to spawn outside camera (e.g., X=7, Y=0)
→ Position is clamped to manual bounds (X=7, Y=0) ✓
→ Checks if outside camera view: YES ✓
→ Spawns at (7, 0) - outside camera, inside level!
```

### What This Prevents:
- ❌ Spawning beyond level boundaries
- ❌ Spawning in player's view
- ❌ Spawning outside the playable area

### What This Guarantees:
- ✅ Enemies always spawn outside camera
- ✅ Enemies always spawn within your defined area
- ✅ Player never sees enemies pop into existence
- ✅ Enemies never appear beyond level walls

## Finding Your Level Bounds

### Method 1: Look at Scene View
1. Select your level/tilemap in Scene view
2. Note the extents visually
3. Estimate the boundaries
4. Set values with some padding

### Method 2: Check Tilemap Transform
1. Select your tilemap
2. Look at Transform position
3. Note the size of your tilemap
4. Calculate bounds: center ± (size/2)

### Method 3: Use Grid Edges
1. Place your cursor at the corners of your grid in Scene view
2. Note the coordinates (shown in bottom left of Scene view)
3. Use these as min/max values

### Method 4: Measure in Play Mode
1. Enter Play Mode
2. Move player to level corners
3. Note player position in Inspector
4. Use these as approximate bounds

## Visual Guide

### In Scene View (with EnemySpawner selected):

```
┌─────────────────────────────┐  Green Box = Your manual spawn area
│                             │  
│   Yellow Box = Camera       │  Enemies spawn:
│   ┌─────────────┐          │  - Outside yellow (camera)
│   │             │          │  - Inside green (your bounds)
│   │   Player    │          │  - In red zones (spawn zones)
│   │             │          │  
│   └─────────────┘          │  Result: Enemies appear just
│                             │  off-screen but within level!
└─────────────────────────────┘
```

## Configuration Examples

### Example 1: Small Arena (20x20)
```
Use Manual Spawn Area: ✓
Spawn Area Min X: -10
Spawn Area Max X: 10
Spawn Area Min Y: -10
Spawn Area Max Y: 10
```

### Example 2: Rectangular Level (40x20)
```
Use Manual Spawn Area: ✓
Spawn Area Min X: -20
Spawn Area Max X: 20
Spawn Area Min Y: -10
Spawn Area Max Y: 10
```

### Example 3: Offset Level (not centered)
```
Use Manual Spawn Area: ✓
Spawn Area Min X: 0
Spawn Area Max X: 50
Spawn Area Min Y: 0
Spawn Area Max Y: 50
```

### Example 4: Very Large Level
```
Use Manual Spawn Area: ✓
Spawn Area Min X: -100
Spawn Area Max X: 100
Spawn Area Min Y: -100
Spawn Area Max Y: 100
```

## Tips for Setting Bounds

### Add Padding:
Set bounds slightly **inside** your actual walls for safety:
```
If your level goes from -20 to 20:
  Use: -18 to 18 (2 unit padding)
  
This prevents enemies spawning too close to walls.
```

### Test and Adjust:
1. Start with estimated values
2. Play the game
3. Watch where enemies spawn
4. Adjust min/max values if needed
5. Repeat until perfect

### Use Debug Mode:
```
1. Check "Show Debug Info"
2. Watch Console for spawn positions
3. If positions are being clamped, you'll see the effect
4. Adjust bounds accordingly
```

## Combining with Other Features

### Manual Spawn Area + Obstacle Check:
```
Use Manual Spawn Area: ✓     (restricts overall area)
Require Ground: ✗            (disable if causing issues)
Obstacle Layer Mask: None    (disable if causing issues)

Result: Simple bounds-based restriction only
```

### Manual Spawn Area Only (Simplest):
```
Use Manual Spawn Area: ✓
Spawn Area Min X: [your level min]
Spawn Area Max X: [your level max]
Spawn Area Min Y: [your level min]
Spawn Area Max Y: [your level max]

All other checks disabled.
Result: Enemies spawn only within your defined box.
```

## Troubleshooting

### Issue: Enemies still spawn outside bounds
**Check**:
- Is `Use Manual Spawn Area` checked?
- Are min/max values set correctly?
- Look for green wireframe in Scene view

**Fix**:
- Verify checkbox is enabled
- Double-check your min/max values
- Reduce the bounds size to be more restrictive

### Issue: No enemies spawning
**Cause**: Bounds too small or conflicting with camera

**Fix**:
- Increase bounds size
- Reduce `Spawn Distance From Camera`
- Make sure green box (manual area) overlaps with red boxes (spawn zones)

### Issue: Can't see green wireframe
**Check**:
- Is `Show Spawn Zones` checked?
- Is `Use Manual Spawn Area` checked?
- Is EnemySpawner selected in Hierarchy?

**Fix**: Enable both checkboxes

## Context Menu Tools

### Test Spawn Position
```
1. Enter Play Mode
2. Right-click EnemySpawner in Inspector
3. Select "Test Spawn Position"
4. Check Console for result
5. See if position is within your bounds
```

### Force Spawn Enemy Now
```
1. Enter Play Mode
2. Right-click EnemySpawner
3. Select "Force Spawn Enemy Now"
4. Enemy spawns immediately
5. Check if it's within your defined area
```

## Recommended Settings

### For Most Levels:
```
Use Manual Spawn Area: ✓
Spawn Area Min X: [measure your level]
Spawn Area Max X: [measure your level]  
Spawn Area Min Y: [measure your level]
Spawn Area Max Y: [measure your level]
Require Ground: ✗ (disable for simplicity)
Show Debug Info: ✓ (for testing)
Show Spawn Zones: ✓ (see visual bounds)
```

### Quick Test Values:
```
Use Manual Spawn Area: ✓
Spawn Area Min X: -25
Spawn Area Max X: 25
Spawn Area Min Y: -25
Spawn Area Max Y: 25
```

This creates a 50x50 spawn area - adjust to match your level!

## Summary

### What You Get:
- ✅ **Simple X/Y min/max values** you can set in Inspector
- ✅ **Green wireframe visualization** shows exact spawn area
- ✅ **Clamping system** forces all spawns within bounds
- ✅ **Easy to adjust** - just change 4 numbers
- ✅ **Visual feedback** - see the box in Scene view
- ✅ **Test tools** - context menu for testing

### How to Use:
1. Check `Use Manual Spawn Area`
2. Set min/max X and Y to match your level
3. Look for green box in Scene view
4. Adjust until it matches your playable area
5. Done!

**Enemies will now only spawn within your manually defined box!** 🎮📦✨
