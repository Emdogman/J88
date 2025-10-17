# Drunk Movement System Testing Guide 🍺🥴

## Overview
The drunk movement system makes the character move like they're drunk, with effects that scale with the beer meter level. Higher beer levels = more drunk = more intense movement effects.

## How It Works

### **Beer Level to Drunk Intensity:**
- **0-33% Beer (Zone 1):** Sober - No drunk effects
- **34-66% Beer (Zone 2):** Tipsy - Moderate drunk effects (0.3-0.6 intensity)
- **67-100% Beer (Zone 3):** Drunk - Heavy drunk effects (0.7-1.0 intensity)

### **Drunk Movement Effects:**

#### **1. Enhanced Wobble Effect:**
- **Character wobbles** in complex multi-frequency patterns
- **Multiple wobble frequencies** for more chaotic movement
- **Intensity scales** with beer level
- **Speed:** 7 oscillations per second (INCREASED)

#### **2. Enhanced Sway Effect:**
- **Character sways** side to side with multiple frequencies
- **Intensity scales** with beer level  
- **Speed:** 4 oscillations per second (INCREASED)

#### **3. Enhanced Random Direction Changes:**
- **More intense random input variations** added to movement
- **Additional chaos effects** for unpredictable movement
- **Intensity scales** with beer level
- **Creates very chaotic movement**

## Testing the Drunk Movement System

### **Quick Setup:**
1. **Create empty GameObject** → Add **BeerSystemDebugger** component
2. **Run the scene** - everything creates automatically!
3. **Enable "Show Debug Info"** on CharacterMovementStaged component

### **Manual Testing:**
1. **Select your character** in the scene
2. **Find CharacterMovementStaged component**
3. **Check "Show Debug Info"** ✅
4. **Check "UseBeerSystem"** ✅
5. **Use BeerSystemDebugger** to test different beer levels

### **Expected Behavior:**

#### **Low Beer Level (0-33%):**
- **No drunk effects** - Character moves normally
- **Precise control** - No wobble or sway
- **Drunk Intensity: 0.0**

#### **Medium Beer Level (34-66%):**
- **Moderate drunk effects** - Character wobbles slightly
- **Some sway** - Harder to control precisely
- **Drunk Intensity: 0.3-0.6**

#### **High Beer Level (67-100%):**
- **Heavy drunk effects** - Character wobbles significantly
- **Strong sway** - Very hard to control
- **Random movements** - Unpredictable direction changes
- **Drunk Intensity: 0.7-1.0**

## Debug Information

### **On-Screen Display:**
- **Movement Stage:** Current stage (1, 2, or 3)
- **Drunk Intensity:** How drunk the character is (0-1)
- **Beer Level:** Current beer percentage
- **Beer Zone:** Current beer zone (1, 2, or 3)

### **Console Messages:**
- **Drunk Effects** are logged when applied
- **Intensity and offset** values shown
- **Beer level changes** are logged

## Configuration

### **Drunk Movement Settings (Inspector):**
- **DrunkWobbleAmount:** 0.8 (how much character wobbles - INCREASED)
- **DrunkWobbleSpeed:** 7 (how fast wobble oscillates - INCREASED)
- **DrunkRandomness:** 0.4 (how much random direction changes - INCREASED)
- **DrunkSwayAmount:** 0.6 (how much character sways - INCREASED)
- **DrunkSwaySpeed:** 4 (how fast sway oscillates - INCREASED)

### **Adjusting Drunk Effects:**
- **Higher values** = More intense drunk effects
- **Lower values** = Subtle drunk effects
- **Speed values** = How fast effects oscillate

## Testing Scenarios

### **Scenario 1: Sober Character**
1. **Set beer level to 25%** (Zone 1)
2. **Move character** - Should move normally
3. **No wobble or sway** - Precise control

### **Scenario 2: Tipsy Character**
1. **Set beer level to 50%** (Zone 2)
2. **Move character** - Should wobble slightly
3. **Some sway** - Harder to control precisely

### **Scenario 3: Drunk Character**
1. **Set beer level to 80%** (Zone 3)
2. **Move character** - Should wobble significantly
3. **Strong sway** - Very hard to control
4. **Random movements** - Unpredictable

### **Scenario 4: Beer Depletion Over Time**
1. **Start with high beer level** (90%)
2. **Let beer deplete naturally** over time
3. **Watch drunk effects** gradually decrease
4. **Character becomes more stable** as beer level drops

## Advanced Testing

### **Beer System Integration:**
1. **Use BeerSystemDebugger** → Press 1,2,3,0,9 to test different levels
2. **Watch drunk intensity** change automatically with beer zones
3. **Test with beer depletion** over time
4. **Test with beer pickup** - effects should increase

### **Movement Combination:**
1. **Test drunk effects** with momentum system
2. **Drunk + momentum** = Very hard to control
3. **High beer + high speed** = Maximum difficulty

---

**🍺 Ready to test!** The drunk movement system should make the character feel authentically drunk based on beer level!
