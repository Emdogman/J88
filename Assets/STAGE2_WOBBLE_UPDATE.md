# Stage 2 Wobble Update - Complete

## Changes Made
Added drunk wobble effect to Stage 2 (Tipsy) with a moderate amount that's less than Stage 3 (Very Drunk).

### File: `Assets/TopDownEngine/Common/Scripts/Characters/CharacterAbilities/CharacterMovementStaged.cs`

## New Wobble System

### Before:
- **Stage 1 (Sober)**: No wobble ✓
- **Stage 2 (Tipsy)**: No wobble ❌
- **Stage 3 (Very Drunk)**: Strong wobble ✓

### After:
- **Stage 1 (Sober)**: No wobble ✓
- **Stage 2 (Tipsy)**: **Moderate wobble** ✓ (NEW!)
- **Stage 3 (Very Drunk)**: Strong wobble ✓

## Wobble Intensity Progression

```
Stage 1: 0.00 wobble (no effect)
         ↓
Stage 2: 0.08 wobble (subtle sway) ← NEW!
         ↓
Stage 3: 0.15 wobble (strong sway)
```

**Stage 2 is ~53% of Stage 3 intensity** - noticeable but not overwhelming.

## New Inspector Fields

### Replaced Single Field:
- ❌ `DrunkWobbleAmount` (0.15)

### With Two Separate Fields:
- ✅ `Stage2WobbleAmount` (0.08) - Tipsy wobble
- ✅ `Stage3WobbleAmount` (0.15) - Very drunk wobble

## How It Works

The wobble system now automatically adjusts based on the current stage:

```csharp
// Stage 2 (Tipsy):
wobbleAmount = 0.08
→ Moderate sway
→ Player feels slightly tipsy
→ Movement still mostly controllable

// Stage 3 (Very Drunk):
wobbleAmount = 0.15
→ Strong sway
→ Player feels very drunk
→ Movement more challenging
```

### Wobble Calculation:
```csharp
// Primary wobble wave
wobbleX = sin(time * 3Hz) * wobbleAmount

// Secondary complexity wave  
wobbleX += sin(time * 3.5Hz) * wobbleAmount * 0.5

// Applied to horizontal movement only (side-to-side sway)
```

## Visual Feel

### Stage 1 (0-33% Beer - Sober):
```
Movement: ━━━━━━━━━━━→
Feel: Precise, straight, fully controlled
```

### Stage 2 (34-66% Beer - Tipsy):
```
Movement: ～～～～～～→
Feel: Slight sway, tipsy feeling, mostly controlled
Wobble: Subtle but noticeable
```

### Stage 3 (67-100% Beer - Very Drunk):
```
Movement: ～～～～～～～～～→
Feel: Strong sway, drunk feeling, hard to control
Wobble: Obvious and challenging
```

## Progression Example

As player drinks more:
```
0% Beer:   No wobble          ━━━━━→
20% Beer:  No wobble          ━━━━━→
35% Beer:  Moderate wobble    ～～～→  (Stage 2 starts)
50% Beer:  Moderate wobble    ～～～→
68% Beer:  Strong wobble      ～～～～→ (Stage 3 starts)
85% Beer:  Strong wobble      ～～～～→
100% Beer: STUN! 💫
```

## Customization

### To Increase Stage 2 Wobble:
```
Stage2WobbleAmount: 0.10 (instead of 0.08)
```

### To Decrease Stage 2 Wobble:
```
Stage2WobbleAmount: 0.05 (instead of 0.08)
```

### To Make Stage 3 Even More Intense:
```
Stage3WobbleAmount: 0.20 (instead of 0.15)
```

### Recommended Values:

**Subtle Progression**:
- Stage 2: 0.05
- Stage 3: 0.10

**Moderate Progression** (Default):
- Stage 2: 0.08
- Stage 3: 0.15

**Extreme Progression**:
- Stage 2: 0.10
- Stage 3: 0.25

## Testing

### Test Stage 2 Wobble:
1. Get beer level to 34-66% (Stage 2 range)
2. Move your character
3. You should see **subtle side-to-side sway**
4. Less than Stage 3 but more than Stage 1

### Test Stage 3 Wobble:
1. Get beer level to 67-99% (Stage 3 range)
2. Move your character
3. You should see **strong side-to-side sway**
4. Noticeably more than Stage 2

### Compare Stages:
```
Test: Move in a straight line at each stage
→ Stage 1: Perfect straight line
→ Stage 2: Slight wavy line
→ Stage 3: Very wavy line
```

## Technical Details

### Wobble Frequency:
Both stages use the same frequencies:
- Primary: 3 Hz (3 cycles per second)
- Secondary: 3.5 Hz (for complexity)

### Only Amplitude Changes:
```
Stage 2: Amplitude = 0.08 (smaller waves)
Stage 3: Amplitude = 0.15 (larger waves)
```

### Why This Works:
- Same wobble pattern/rhythm
- Different intensity
- Creates natural progression
- Feels like "getting more drunk" not "different drunk"

## Balance Notes

### Stage 2 Balance:
- Should feel **noticeable** but not frustrating
- Player can still aim and navigate
- Adds challenge without being punishing
- Hints at what Stage 3 will be like

### Stage 3 Balance:
- Should feel **challenging** but not impossible
- Movement requires skill and compensation
- Significant impact on gameplay
- Feels appropriately "very drunk"

## Summary

### Changes:
- ✅ Added wobble to Stage 2 (was missing)
- ✅ Made wobble amount configurable per stage
- ✅ Stage 2: 0.08 wobble (moderate)
- ✅ Stage 3: 0.15 wobble (strong)
- ✅ Smooth progression from sober → tipsy → drunk

### Result:
The drunk progression now feels more gradual and natural:
```
Sober → Tipsy → Very Drunk
  0%  →  53%  →  100%
       (wobble intensity)
```

**Players will now feel progressively drunker as the beer meter increases!** 🍺～～～→
