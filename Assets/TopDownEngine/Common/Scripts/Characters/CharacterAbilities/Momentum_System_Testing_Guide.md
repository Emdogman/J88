# Momentum System Testing Guide 🏃‍♂️🍺

## Overview
The momentum system makes direction changes IMPOSSIBLE when the character has built up speed, especially in stages 2 and 3 (drunk states). This simulates the difficulty of controlling a drunk character. The system completely blocks direction changes until momentum is canceled by opposite input.

## How It Works

### **Stage 1 (Normal - Sober):**
- **No momentum resistance** - Character responds immediately to input
- **Instant direction changes** - No momentum buildup affects movement

### **Stage 2 (Drift - Tipsy):**
- **Momentum blocking** - Cannot change direction at all while moving
- **Opposite input required** - Must press opposite direction to cancel momentum first
- **Complete blocking** - Perpendicular movement is impossible until momentum is canceled

### **Stage 3 (High Drift - Drunk):**
- **Strong momentum blocking** - Cannot change direction at all while moving
- **Must cancel momentum** - Must use opposite input to reduce momentum before any direction change
- **Complete blocking** - All direction changes blocked until momentum is zeroed

## Testing the Momentum System

### **Quick Setup:**
1. **Create empty GameObject** → Add **BeerSystemDebugger** component
2. **Run the scene** - everything creates automatically!
3. **Enable "Show Debug Info"** on CharacterMovementStaged component

### **Manual Testing:**
1. **Select your character** in the scene
2. **Find CharacterMovementStaged component**
3. **Check "Show Debug Info"** ✅
4. **Set "UseBeerSystem" to false** (for manual testing)
5. **Press 1, 2, 3** to switch between stages

### **Expected Behavior:**

#### **Stage 1 Testing:**
- **Walk in any direction** → **Release input** → **Immediately press opposite direction**
- **Result:** Character should turn instantly with no resistance

#### **Stage 2 Testing:**
- **Run horizontally** (build up speed) → **Try to move vertically**
- **Result:** Should work - pure perpendicular movement is allowed
- **Try diagonal movement:** Should be BLOCKED - must cancel momentum first
- **Try opposite direction:** Should work to cancel momentum

#### **Stage 3 Testing:**
- **Run fast horizontally** (build up speed) → **Try to move vertically**
- **Result:** Should work - pure perpendicular movement is allowed
- **Try diagonal movement:** Should be BLOCKED - must cancel momentum first
- **Must use opposite input:** Press opposite direction to cancel momentum first

## Debug Information

### **On-Screen Display:**
- **Movement Stage:** Current stage (1, 2, or 3)
- **Current Deceleration:** Deceleration value for current stage
- **Momentum Resistance:** How much momentum affects direction changes (0-1)
- **Drift Influence:** How much drift affects turning (0-1)
- **Current Speed:** How fast the character is moving
- **Momentum Strength:** How much momentum is built up (0-1)
- **Beer Level/Zone:** If using beer system

### **Console Messages:**
- **Stage changes** are logged when switching
- **Momentum calculations** are logged (if debug enabled)

## Configuration

### **Momentum Settings (Inspector):**
- **Stage2MomentumResistance:** 0.5 (50% resistance)
- **Stage3MomentumResistance:** 0.8 (80% resistance)  
- **DirectionChangeDamping:** 0.3 (how quickly turns are resisted)

### **Adjusting Difficulty:**
- **Lower values** = Easier to turn (less drunk feeling)
- **Higher values** = Harder to turn (more drunk feeling)
- **DirectionChangeDamping** = How quickly the resistance kicks in

## Common Issues

### **Issue: No momentum resistance in any stage**
**Solution:** Check that "UseBeerSystem" is false for manual testing, or that beer system is working

### **Issue: Too much resistance, character can't turn at all**
**Solution:** Lower the momentum resistance values (0.3-0.5 for stage 2, 0.6-0.7 for stage 3)

### **Issue: Not enough resistance, feels like stage 1**
**Solution:** Increase momentum resistance values (0.7-0.8 for stage 2, 0.9+ for stage 3)

## Advanced Testing

### **Speed-Based Testing:**
1. **Walk slowly** → Try to turn (should be easy)
2. **Run fast** → Try to turn (should be hard)
3. **Stop completely** → Try to turn (should be easy again)

### **Direction-Based Testing:**
1. **Move forward** → Try to go backward (should be hardest)
2. **Move forward** → Try to go left/right (should be medium difficulty)
3. **Move forward** → Try to go diagonal (should be easier)

### **Beer System Integration:**
1. **Use BeerSystemDebugger** → Press 1,2,3,0,9 to test different beer levels
2. **Watch momentum resistance** change automatically with beer zones
3. **Test with beer depletion** over time

---

**🎮 Ready to test!** The momentum system should make drunk character control feel realistic and challenging!
