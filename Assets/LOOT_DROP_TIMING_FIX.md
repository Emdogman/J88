# Loot Drop Timing Fix

## Problem Identified
Enemies were not dropping coins because they get deactivated immediately when they die, preventing the `HandleEnemyDeath()` method from running in the Update loop.

## Root Cause
- `HandleEnemyDeath()` was called in `Update()`
- When enemies die, they get deactivated immediately
- Deactivated GameObjects stop running Update loops
- Loot drop logic never executed

## Solution Implemented

### 1. Health Change Detection
Added health tracking variables:
```csharp
private float _lastHealth;
private bool _hasDroppedLoot;
```

### 2. Proactive Death Detection
Created `CheckForDeathAndDropLoot()` method that:
- Tracks health changes between frames
- Detects when health drops from >0 to ≤0
- Drops loot immediately when death is detected
- Prevents multiple drops with `_hasDroppedLoot` flag

### 3. Early Execution
Moved death checking to the beginning of `Update()`:
```csharp
private void Update()
{
    // Check for death and loot dropping (before other updates)
    CheckForDeathAndDropLoot();
    
    // ... rest of update logic
}
```

### 4. Health Initialization
Initialize `_lastHealth` in `Start()` method to track health changes from the beginning.

## How It Works Now

1. **Health Tracking**: Each frame, compare current health with previous frame's health
2. **Death Detection**: When `_lastHealth > 0` and `currentHealth ≤ 0`, enemy just died
3. **Immediate Drop**: Call `DropLoot()` immediately when death is detected
4. **Prevent Duplicates**: Use `_hasDroppedLoot` flag to prevent multiple drops
5. **Debug Info**: Enhanced debug display shows health tracking status

## Testing

### Debug Information
Enable "Show Debug Info" to see:
- Current Health
- Last Health (previous frame)
- Has Dropped Loot (boolean)
- Drop Rate and Amount

### Expected Behavior
1. Enemy takes damage
2. Health drops to 0 or below
3. Loot drops immediately (before deactivation)
4. Enemy gets deactivated
5. KoalaCoinPicker items appear at enemy position

## Files Modified

- `Assets/TopDownEngine/Common/Scripts/Enemies/ChaserEnemy.cs`
  - Added health tracking variables
  - Added `CheckForDeathAndDropLoot()` method
  - Modified `Update()` to check death early
  - Enhanced debug display
  - Initialize health tracking in `Start()`

## Result

Enemies now drop KoalaCoinPicker items immediately when they die, before getting deactivated. The loot drop system works reliably with the existing TopDownEngine health and deactivation systems.
