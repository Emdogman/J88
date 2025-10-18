# Beer Meter Stun System Troubleshooting Guide

## Issue: Character Doesn't Get Stunned or Puke at 100 Beer Meter

### Quick Diagnosis Steps

1. **Check if BeerMeterStunHandler is attached to player**
   - Select your player character
   - Look for "Beer Meter Stun Handler" component
   - If missing, add it via "Add Component" → "Beer Meter Stun Handler"

2. **Enable Debug Information**
   - Select BeerMeterStunHandler component
   - Check "Show Debug Info" ✅
   - Play the game and watch console for messages

3. **Test System Setup**
   - Right-click on BeerMeterStunHandler component
   - Select "Check System Setup"
   - Check console for any ❌ errors

4. **Test Beer Level**
   - Right-click on BeerMeterStunHandler component
   - Select "Set Beer to 100"
   - Check if stun triggers immediately

### Common Issues and Solutions

#### Issue 1: BeerManager Not Found
**Symptoms**: Console shows "❌ BeerManager not found!"
**Solution**:
1. Check if BeerManager exists in scene
2. Look for "Beer Manager" GameObject in hierarchy
3. If missing, create empty GameObject → Add "Beer Manager" component

#### Issue 2: BeerMeterStunHandler Not Attached
**Symptoms**: No "Beer Meter Stun Handler" component on player
**Solution**:
1. Select player character
2. Add Component → Search "Beer Meter Stun Handler"
3. Configure settings (threshold: 100, reset: 15)

#### Issue 3: Beer Level Not Reaching 100
**Symptoms**: Beer level stays below 100
**Solution**:
1. Use BeerManager debug tools to increase beer level
2. Or use "Set Beer to 100" context menu option
3. Check if beer depletion rate is too high

#### Issue 4: Puke Effect Not Spawning
**Symptoms**: Stun works but no green circle appears
**Solution**:
1. Check "Puke Effect Prefab" is assigned in BeerMeterStunHandler
2. Create puke effect prefab:
   - Create empty GameObject
   - Add SpriteRenderer with green circle
   - Add PukeEffect script
   - Save as prefab
   - Assign to BeerMeterStunHandler

#### Issue 5: Character Still Moves During Stun
**Symptoms**: Player can still move/attack when stunned
**Solution**:
1. Check "Show Debug Info" is enabled
2. Look for "Character abilities disabled" message
3. Verify CharacterMovement and InputManager are found
4. Check if other scripts are overriding movement

### Testing Commands

Use these context menu options (right-click on BeerMeterStunHandler):

- **"Check System Setup"** - Diagnose all components
- **"Show Status"** - Show current system status
- **"Set Beer to 100"** - Set beer level to 100
- **"Force Trigger Stun"** - Force trigger stun sequence
- **"Test Stun Sequence"** - Test complete stun sequence

### Expected Console Output

When working correctly, you should see:
```
BeerMeterStunHandler: Initialized - Stun Duration: 1.5s, Threshold: 100, Reset Value: 15
BeerMeterStunHandler: Beer level 100 >= threshold 100, triggering stun!
BeerMeterStunHandler: Character abilities disabled
BeerMeterStunHandler: Puke effect spawned at (x, y, z)
BeerMeterStunHandler: Beer meter reset to 15
BeerMeterStunHandler: Character abilities re-enabled
BeerMeterStunHandler: Stun sequence completed
```

### Manual Testing Steps

1. **Setup Check**:
   - Right-click BeerMeterStunHandler → "Check System Setup"
   - Fix any ❌ errors shown

2. **Beer Level Test**:
   - Right-click BeerMeterStunHandler → "Set Beer to 100"
   - Should trigger stun immediately

3. **Force Stun Test**:
   - Right-click BeerMeterStunHandler → "Force Trigger Stun"
   - Should trigger stun regardless of beer level

4. **Status Check**:
   - Right-click BeerMeterStunHandler → "Show Status"
   - Check all values are correct

### Configuration Checklist

- [ ] BeerMeterStunHandler attached to player
- [ ] BeerManager exists in scene
- [ ] Show Debug Info enabled
- [ ] Beer Meter Threshold: 100
- [ ] Beer Meter Reset Value: 15
- [ ] Stun Duration: 1.5
- [ ] Puke Effect Prefab assigned
- [ ] No compilation errors

### Still Not Working?

If the system still doesn't work after following these steps:

1. **Check Console Errors**: Look for any red error messages
2. **Verify BeerManager**: Ensure BeerManager.CurrentBeer is actually reaching 100
3. **Test Manual Trigger**: Use "Force Trigger Stun" to bypass beer level check
4. **Check Component Order**: Ensure BeerMeterStunHandler is enabled and not overridden
5. **Verify Scene Setup**: Make sure you're testing in the correct scene with proper character setup

The system should work once all components are properly set up and configured! 🍺🥴✨
