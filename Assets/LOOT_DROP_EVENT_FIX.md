# Loot Drop Event System Fix

## Problem Solved

**Issue**: Enemies were not dropping coins because the health polling approach in `Update()` couldn't detect death before the GameObject was destroyed/deactivated.

**Root Cause**: TopDownEngine's `Health.Kill()` method destroys the GameObject within the same frame as damage is dealt, preventing our `Update()` loop from detecting the health change.

## Solution Implemented

### Event-Driven Approach
Instead of polling health in `Update()`, we now use TopDownEngine's built-in `Health.OnDeath` event system.

### Key Changes Made

**1. Event Subscription**
```csharp
// In Start() method
if (_health != null)
{
    _health.OnDeath += HandleDeathAndDropLoot;
}
```

**2. Event Unsubscription**
```csharp
// In OnDestroy() method
private void OnDestroy()
{
    if (_health != null)
    {
        _health.OnDeath -= HandleDeathAndDropLoot;
    }
}
```

**3. Simplified Death Handler**
```csharp
private void HandleDeathAndDropLoot()
{
    if (!_hasDroppedLoot)
    {
        DropLoot();
        _hasDroppedLoot = true;
    }
}
```

**4. Removed Health Polling**
- Removed `CheckForDeathAndDropLoot()` call from `Update()`
- Removed `_lastHealth` variable
- Removed legacy death detection methods

## Why This Works

### Perfect Timing
- `Health.OnDeath` fires at line 813 in `Health.Kill()`
- This happens **after** health is set to 0 but **before** GameObject destruction (line 891)
- Guarantees loot drops before deactivation

### Event-Driven Benefits
1. **No Race Conditions**: Event fires exactly when needed
2. **Clean Architecture**: Uses TopDownEngine's existing event system
3. **Memory Safe**: Proper event subscription/unsubscription
4. **Reliable**: No dependency on Update() loop timing

## Testing Instructions

### Debug Information
Enable "Show Debug Info" on ChaserEnemy to see:
- Current Health
- Has Dropped Loot (boolean)
- Drop Rate and Amount

### Expected Behavior
1. Attack enemy with melee weapon
2. Enemy takes damage
3. When health reaches 0, `OnDeath` event fires
4. `HandleDeathAndDropLoot()` executes immediately
5. KoalaCoinPicker items spawn at enemy position
6. Enemy GameObject gets destroyed/deactivated
7. Items remain and can be picked up

### Console Output
Look for: `"ChaserEnemy: Dropped KoalaCoinPicker at [position]"`

## Files Modified

- `Assets/TopDownEngine/Common/Scripts/Enemies/ChaserEnemy.cs`
  - Added `OnDeath` event subscription in `Start()`
  - Added `OnDestroy()` for event cleanup
  - Simplified `HandleDeathAndDropLoot()` method
  - Removed health polling from `Update()`
  - Cleaned up unused variables and methods

## Result

The loot drop system now works reliably using TopDownEngine's event system. Enemies will drop KoalaCoinPicker items at the specified rate when they die, before getting destroyed/deactivated.
