# Enemy Loot Drop System Setup Guide

## Overview
Enemies now drop KoalaCoinPicker items when they die, with configurable drop rates and amounts.

## Features Added

### 1. ChaserEnemy Loot System
- **Drop Rate**: Configurable chance (0-1) for enemies to drop loot
- **Drop Amount**: Number of items to drop per enemy
- **Drop Offset**: Random position variation for dropped items
- **Automatic Detection**: Checks enemy health and drops loot on death

### 2. EnemyLootSetupHelper
- **Easy Setup**: Context menu options for quick configuration
- **Bulk Configuration**: Set up all enemies in scene at once
- **Prefab Assignment**: Automatically assigns KoalaCoinPicker prefab

## Setup Instructions

### Step 1: Assign KoalaCoinPicker Prefab
1. Select any ChaserEnemy in your scene
2. Add the `EnemyLootSetupHelper` component
3. In the Inspector, assign the KoalaCoinPicker prefab:
   - **Path**: `Assets/TopDownEngine/Demos/Koala2D/Prefabs/ItemPickers/KoalaCoinPicker.prefab`
4. Configure default settings:
   - **Default Drop Rate**: 0.3 (30% chance)
   - **Default Drop Amount**: 1
   - **Default Drop Offset**: 0.5

### Step 2: Configure All Enemies
**Option A: Individual Setup**
1. Select each ChaserEnemy GameObject
2. Right-click the `EnemyLootSetupHelper` component
3. Choose "Setup Enemy Loot"

**Option B: Bulk Setup (Recommended)**
1. Select any GameObject with `EnemyLootSetupHelper`
2. Right-click the component
3. Choose "Setup All Enemies Loot"

### Step 3: Verify Configuration
1. Select a ChaserEnemy
2. Check the Inspector for these fields under "Loot Drop System":
   - **Drop Rate**: Should be 0.3 (30%)
   - **Drop Prefab**: Should show KoalaCoinPicker
   - **Drop Amount**: Should be 1
   - **Drop Offset**: Should be 0.5

## Configuration Options

### Drop Rate
- **0.0**: Never drops loot (0%)
- **0.3**: 30% chance to drop (recommended)
- **0.5**: 50% chance to drop
- **1.0**: Always drops loot (100%)

### Drop Amount
- **1**: Drop 1 item per enemy (recommended)
- **2-3**: Drop multiple items for higher value enemies
- **0**: Disable dropping (set drop rate to 0 instead)

### Drop Offset
- **0.5**: Small random spread around enemy position
- **1.0**: Larger spread for more natural distribution
- **0.0**: Items drop exactly at enemy position

## Testing the System

### In Play Mode
1. Enter Play Mode
2. Attack and kill enemies with melee attacks
3. Check for KoalaCoinPicker items dropping
4. Verify items can be picked up by player

### Debug Information
Enable "Show Debug Info" on ChaserEnemy to see:
- Drop Rate percentage
- Drop Amount
- Debug messages when items are dropped

## Troubleshooting

### No Items Dropping
1. **Check Drop Rate**: Ensure it's not 0
2. **Check Drop Prefab**: Verify KoalaCoinPicker is assigned
3. **Check Enemy Health**: Ensure enemies are actually dying
4. **Check Console**: Look for error messages

### Items Not Pickable
1. **Check KoalaCoinPicker**: Ensure it has proper picker components
2. **Check Player**: Ensure player has inventory system
3. **Check Layers**: Verify collision layers are correct

### Performance Issues
1. **Limit Drop Amount**: Keep drop amount reasonable (1-3)
2. **Check Drop Rate**: Lower rates reduce item spawning
3. **Clean Up Items**: Ensure dropped items are properly managed

## Advanced Configuration

### Custom Drop Rates per Enemy
1. Select individual ChaserEnemy
2. Manually adjust "Drop Rate" in Inspector
3. Higher value enemies can have higher drop rates

### Multiple Item Types
1. Create different drop prefabs
2. Modify ChaserEnemy script to support multiple drop types
3. Use weighted random selection for variety

## Files Modified

- `Assets/TopDownEngine/Common/Scripts/Enemies/ChaserEnemy.cs` - Added loot drop system
- `Assets/TopDownEngine/Common/Scripts/Enemies/EnemyLootSetupHelper.cs` - New helper script
- `Assets/LOOT_DROP_SETUP_GUIDE.md` - This guide

## Integration with TopDownEngine

The loot system integrates seamlessly with TopDownEngine's:
- **Inventory System**: KoalaCoinPicker works with existing inventory
- **Pickup System**: Uses standard TopDownEngine pickup mechanics
- **Feedback System**: Includes sound and visual effects
- **Animation System**: KoalaCoinPicker has built-in animations

## Example Usage

```csharp
// In ChaserEnemy Inspector:
// Drop Rate: 0.3 (30% chance)
// Drop Prefab: KoalaCoinPicker
// Drop Amount: 1
// Drop Offset: 0.5

// Result: 30% chance to drop 1 KoalaCoinPicker with random position offset
```
