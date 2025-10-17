# Beer System Testing Guide 🍺

## Quick Setup for Testing

### Method 1: Automatic Setup (Recommended)
1. **Create empty GameObject** in your scene
2. **Add `BeerSystemDebugger` component** to it
3. **Run the scene** - everything will be created automatically!
4. **Press 1,2,3,0,9** to test different beer levels
5. **Press R** to refresh all components

### Method 2: Manual Setup
1. **Create empty GameObject** → Add `BeerManager` component
2. **Create empty GameObject** → Add `BeerMeterUI` component  
3. **Make sure your character has `CharacterMovementStaged`** component
4. **Enable `UseBeerSystem`** on the character
5. **Enable `ShowDebugInfo`** on all components

## Testing the Beer Meter Percentage

### ✅ **What Should Happen:**
- **Beer meter starts at 50%** (middle of the bar)
- **Gradually depletes** over time (1 beer per second)
- **Fill amount changes** as beer level changes
- **Color changes** based on zones (Red/Orange/Green)

### 🔧 **Debug Steps:**
1. **Right-click BeerManager** → "Test Beer Level 25%" (should show red zone)
2. **Right-click BeerManager** → "Test Beer Level 50%" (should show orange zone)
3. **Right-click BeerManager** → "Test Beer Level 75%" (should show green zone)
4. **Right-click BeerMeterUI** → "Refresh from BeerManager" (force update)

## Testing Movement Stage Changes

### ✅ **What Should Happen:**
- **Zone 1 (0-33%)** → **Stage 3 (High Drift)** - Hard to control, high drift
- **Zone 2 (34-66%)** → **Stage 2 (Drift)** - Slight drift, tipsy
- **Zone 3 (67-100%)** → **Stage 1 (Normal)** - Precise control, sober

### 🔧 **Debug Steps:**
1. **Right-click CharacterMovementStaged** → "Test Stage 1 (Normal)" (should have deceleration = 10)
2. **Right-click CharacterMovementStaged** → "Test Stage 2 (Drift)" (should have deceleration = 4)
3. **Right-click CharacterMovementStaged** → "Test Stage 3 (High Drift)" (should have deceleration = 1.5)
4. **Right-click CharacterMovementStaged** → "Refresh from BeerManager" (sync with beer level)

## Common Issues and Solutions

### ❌ **Beer Meter Always Full:**
- **Check**: BeerManager exists and is active
- **Check**: BeerMeterUI is subscribed to events
- **Solution**: Right-click BeerMeterUI → "Refresh from BeerManager"
- **Solution**: Enable `UsePeriodicRefresh` on BeerMeterUI

### ❌ **Movement Stage Not Changing:**
- **Check**: Character has CharacterMovementStaged component
- **Check**: `UseBeerSystem` is enabled on character
- **Check**: BeerManager is triggering events
- **Solution**: Right-click CharacterMovementStaged → "Refresh from BeerManager"

### ❌ **No Visual Updates:**
- **Check**: All components have `ShowDebugInfo` enabled
- **Check**: Console for error messages
- **Solution**: Use BeerSystemDebugger component for comprehensive testing

## Keyboard Testing

### 🎮 **Test Controls:**
- **Press 1** → Test 25% beer (Zone 1, Stage 1)
- **Press 2** → Test 50% beer (Zone 2, Stage 2)  
- **Press 3** → Test 75% beer (Zone 3, Stage 3)
- **Press 0** → Test 0% beer (Zone 1, Stage 1)
- **Press 9** → Test 100% beer (Zone 3, Stage 3)
- **Press R** → Refresh all components

## Expected Behavior

### 🍺 **Beer Meter:**
- **Starts at 50%** (middle of bar)
- **Depletes over time** (1 beer per second)
- **Picking up beers** increases the meter
- **Color changes** based on zones

### 🎮 **Movement:**
- **Low beer level** (0-33%) → **Normal movement** (precise control)
- **Medium beer level** (34-66%) → **Drift movement** (slight drift)
- **High beer level** (67-100%) → **High drift movement** (hard to control)

### 📊 **Debug Info:**
- **On-screen display** shows current beer level and movement stage
- **Console messages** show when events are triggered
- **Context menus** allow manual testing of all components

---

**🎯 Ready to test!** Use the BeerSystemDebugger component for the easiest testing experience!
