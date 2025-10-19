# Enemy AI - Aggressive Behavior Update

## Changes Made
Updated enemy AI to be more aggressive and use attacks contextually based on distance.

## New Behavior

### Movement (Always Aggressive):
- ✅ **Always moves toward player** - No retreat
- ✅ **Stops only in melee range** - To attack
- ✅ **Never backs away** - Pure aggression
- ✅ **Chases relentlessly** - Constant pressure

### Attack Logic (Context-Based):

```
┌─────────────────────────────────────────┐
│ FAR (2.5+ to 5 units):                  │
│ → Use CHARGE attack (close the gap)    │
├─────────────────────────────────────────┤
│ CLOSE (0 to 1.5 units):                │
│ → Use MELEE attack (in range)          │
├─────────────────────────────────────────┤
│ MEDIUM (1.5 to 2.5 units):             │
│ → CHASE player (approach for melee)    │
└─────────────────────────────────────────┘
```

### Priority System:

**Priority 1: Melee Attack**
- Condition: Distance ≤ 1.5 units
- Action: Perform melee attack
- Why: Enemy is already close, use close-range weapon

**Priority 2: Charge Attack**
- Condition: Distance > 2.5 units AND ≤ 5 units
- Action: Telegraph then charge
- Why: Enemy is far, use charge to close distance quickly

**Priority 3: Chase**
- Condition: All other cases
- Action: Move toward player
- Why: Get into attack range

## Key Differences

### Old Behavior:
```
Enemy at 3 units from player:
→ Tried to maintain orbit distance
→ Would back away if too close
→ Would stop and wait at ideal distance
→ Passive, defensive

Player approaches enemy:
→ Enemy backs away
→ Tries to maintain distance
→ Can create chase scenarios
```

### New Behavior:
```
Enemy at 3 units from player:
→ Charges to close gap
→ No backing away
→ Aggressive approach
→ Active, offensive

Player approaches enemy:
→ Enemy stays put or keeps approaching
→ Switches to melee when player gets close
→ No retreat, pure aggression
```

## Attack Usage

### Charge Attack:
**When**: Enemy is already **far from player** (2.5-5 units)
**Purpose**: Close the distance quickly
**Behavior**: 
- Telegraph for 1.5 seconds
- Dash toward player's position
- Deal 20 damage on hit

### Melee Attack:
**When**: Enemy is **close to player** (0-1.5 units)
**Purpose**: Primary damage when in range
**Behavior**:
- Immediate attack
- Deal 10 damage
- Can attack every 1 second

### Chase (No Attack):
**When**: Enemy is in **medium range** (1.5-2.5 units)
**Purpose**: Get into melee range
**Behavior**:
- Move directly toward player
- Close gap to trigger melee
- No backing away

## Distances Explained

```
Player Position (0)
     │
     ├─ 0-1.5 units:  MELEE RANGE
     │                → Stop and attack
     │
     ├─ 1.5-2.5 units: CHASE ZONE  
     │                 → Approach to melee
     │
     ├─ 2.5-5 units:  CHARGE RANGE
     │                → Telegraph and charge
     │
     └─ 5+ units:     FAR CHASE
                      → Run toward player
```

## Tactical Behavior

### Enemy's Strategy:
1. **Spot player** → Move toward them
2. **If far away** (2.5-5 units) → Charge to close gap
3. **If medium distance** (1.5-2.5 units) → Chase to melee range  
4. **If close** (0-1.5 units) → Melee attack repeatedly
5. **Never retreat** → Always pressure forward

### Player's Experience:
- Enemies feel aggressive and threatening
- Must kite enemies to avoid melee
- Charge attacks punish staying at mid-range
- Close combat is constant melee barrage
- No "safe zone" where enemies just orbit

## Comparison: Retreat vs No Retreat

### With Retreat (Old):
```
Player at 1 unit from enemy:
→ Enemy backs away to 3 units
→ Enemy uses charge
→ Player can follow and pressure
→ Enemy keeps backing away
→ Chase scenarios, less threatening
```

### Without Retreat (New):
```
Player at 1 unit from enemy:
→ Enemy melees immediately
→ Enemy keeps attacking
→ Player must back away or take damage
→ Enemy feels dangerous up close
→ More intense combat
```

## Configuration

### Key Settings:
```
enemy_Melee_Radius: 1.5      - Melee attack trigger distance
enemy_Charge_Radius: 5       - Charge attack trigger distance  
moveSpeed: 2.5               - Chase speed
chargeCooldown: 4            - Time between charges
attackCooldown: 1            - Time between melees
```

### Tuning Suggestions:

**More aggressive melee**:
- Increase `enemy_Melee_Radius` to 2.0
- Decrease `attackCooldown` to 0.7

**More charges**:
- Increase `enemy_Charge_Radius` to 6
- Decrease `chargeCooldown` to 3

**Faster enemies**:
- Increase `moveSpeed` to 3.0

## Expected Results

### Enemy Behavior:
- ✅ Constantly moving toward player
- ✅ Uses charge when starting far away
- ✅ Switches to melee when player gets close
- ✅ Never retreats or backs away
- ✅ Feels threatening and aggressive

### Combat Feel:
- ✅ Intense melee combat when close
- ✅ Charge attacks from distance
- ✅ Player must actively kite/dodge
- ✅ No passive enemies just orbiting
- ✅ Clear attack patterns

## Summary

### Removed:
- ❌ Retreat/back-away behavior
- ❌ Ideal distance maintenance
- ❌ Passive orbital movement
- ❌ Distance-based retreat

### Added:
- ✅ Always aggressive forward movement
- ✅ Context-aware attack selection
- ✅ Charge only when already far
- ✅ Melee when close or player approaches
- ✅ Relentless pursuit

**Enemies are now aggressive, never retreat, and use attacks contextually based on their starting distance!** 🎮⚔️🔥

