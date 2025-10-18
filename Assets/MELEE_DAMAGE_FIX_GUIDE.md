# Melee Damage Fix Guide

## Problem
Enemies are not taking damage when hit with melee attacks.

## Root Cause Analysis
After examining the KoalaNinjaSword.prefab, the issue is clear:

1. **KoalaNinjaSword TargetLayerMask**: 1280 (Layers 8 + 10)
2. **PlayerMeleeWeapon TargetLayerMask**: 512 (Layer 9 only) 
3. **Actual Enemy Layer**: Default Layer 0 (not Layer 9)

## Solution Applied

### 1. Updated PlayerMeleeWeapon.prefab
- **TargetLayerMask**: Changed from 512 to 1280
- **Layers Targeted**: 8 + 10 (matches TopDownEngine standard)
- **Damage**: 15-20 damage
- **Cooldown**: 0.8 seconds

### 2. Created EnemyHealthSetupHelper.cs
- Ensures enemies have proper Health and Character components
- Sets enemies to Layer 9 (Enemies layer)
- Configures non-trigger colliders for damage detection

## How to Apply the Fix

### Option A: Use the Helper Script (Recommended)
1. Select any ChaserEnemy in your scene
2. Add the `EnemyHealthSetupHelper` component
3. Right-click the component → "Setup All Enemies in Scene"
4. This will automatically configure all enemies

### Option B: Manual Setup
1. Select each ChaserEnemy GameObject
2. Set Layer to 9 (Enemies)
3. Ensure it has:
   - Health component
   - Character component  
   - CircleCollider2D (non-trigger)
   - Rigidbody2D

### Option C: Alternative Layer Configuration
If you prefer enemies on Layer 0 (Default):
1. Keep enemies on Layer 0
2. Change PlayerMeleeWeapon TargetLayerMask to 1 (Layer 0 only)

## Testing the Fix

1. Enter Play Mode
2. Left click to attack enemies
3. Enemies should now take damage
4. Check Console for any errors

## Layer Mask Reference

- Layer 0 (Default): 1
- Layer 8: 256  
- Layer 9 (Enemies): 512
- Layer 10 (Player): 1024
- Layers 8+10: 1280 ← **Current setting**
- Layers 8+9+10: 1792

## Verification Steps

1. **Check Enemy Layer**: Select enemy → Inspector → Layer should be 9
2. **Check Weapon Layer Mask**: Select PlayerMeleeWeapon → Inspector → TargetLayerMask should be 1280
3. **Check Enemy Components**: Enemy should have Health, Character, CircleCollider2D
4. **Test in Play Mode**: Left click should damage enemies

## Troubleshooting

If enemies still don't take damage:

1. **Check Console**: Look for error messages
2. **Verify Layers**: Ensure weapon targets correct layers
3. **Check Colliders**: Enemy collider must be non-trigger
4. **Check Components**: Enemy must have Health component
5. **Check Distance**: Ensure player is close enough to enemy

## Files Modified

- `Assets/PlayerMeleeWeapon.prefab` - Updated TargetLayerMask
- `Assets/TopDownEngine/Common/Scripts/Enemies/EnemyHealthSetupHelper.cs` - New helper script