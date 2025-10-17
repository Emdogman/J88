# Car-Like Momentum System Testing Guide

## Overview

The car-like momentum system makes the character behave like a car - it must come to a complete stop before changing direction. This creates realistic momentum physics where the character can't immediately reverse or turn while moving.

## How It Works

### **Car-Like Physics:**
- **Character builds momentum** in both horizontal and vertical directions (like a car accelerating)
- **Momentum decays when player gives opposite input (braking)** - like pressing brake pedal
- **Only when braking is complete (momentum = 0)** can player change direction
- **Only same direction or opposite direction allowed** while having momentum
- **Any other direction is COMPLETELY BLOCKED** - input is zeroed
- **Separate momentum tracking** for horizontal and vertical movement

### **Stage-Based Behavior:**
- **Stage 1 (Normal):** No car momentum - character responds immediately
- **Stage 2 (Drift):** Moderate car momentum - some direction change resistance
- **Stage 3 (High Drift):** Heavy car momentum - very difficult to change direction

## Configuration

### **Car-Like Momentum Settings (Inspector):**
- **Stage2CarMomentum:** 0.8 (how much momentum blocks direction changes in Stage 2)
- **Stage3CarMomentum:** 0.95 (how much momentum blocks direction changes in Stage 3)
- **MomentumThreshold:** 0.2 (minimum speed before momentum blocking kicks in)
- **MomentumBuildRate:** 3 (how fast momentum builds up when moving)
- **MomentumDecayRate:** 2 (how fast momentum decays when not moving)

### **Adjusting Car Momentum:**
- **Higher values** = More car-like behavior (harder to change direction)
- **Lower values** = Less car-like behavior (easier to change direction)
- **Threshold** = How fast you need to be moving before momentum kicks in

## Testing the Car-Like Momentum System

### **Quick Setup:**
1. **Create empty GameObject** → Add **BeerSystemDebugger** component
2. **Press 9** to set high beer level (90%) for Stage 3 testing
3. **Enable ShowDebugInfo** on CharacterMovementStaged component
4. **Move character** and observe car-like behavior

### **Expected Behavior:**

#### **Stage 1 (Normal Movement):**
- **No car momentum** - Character responds immediately to input
- **Can change direction** instantly
- **No momentum blocking** - Normal 2D movement

#### **Stage 2 (Drift Movement):**
- **Moderate car momentum** - Character builds momentum when moving
- **Must brake to stop** before changing direction
- **Some direction change resistance** - Like a car with moderate speed

#### **Stage 3 (High Drift Movement):**
- **Heavy car momentum** - Character builds strong momentum when moving
- **Must brake to stop** before changing direction
- **High direction change resistance** - Like a car moving fast

### **Testing Scenarios:**

#### **1. Basic Car Movement (Horizontal):**
1. **Start moving** horizontally (e.g., right)
2. **Try to change direction** immediately (e.g., left)
3. **Expected:** Horizontal input COMPLETELY BLOCKED - character won't move left at all
4. **Press opposite direction** to brake - momentum starts decaying
5. **Keep pressing opposite direction** until momentum reaches zero
6. **Then change direction** - should work only after momentum is zero

#### **1b. Basic Car Movement (Vertical):**
1. **Start moving** vertically (e.g., up)
2. **Try to change direction** immediately (e.g., down)
3. **Expected:** Vertical input COMPLETELY BLOCKED - character won't move down at all
4. **Press opposite direction** to brake - momentum starts decaying
5. **Keep pressing opposite direction** until momentum reaches zero
6. **Then change direction** - should work only after momentum is zero

#### **2. Braking System:**
1. **Start moving** in one direction
2. **Keep moving** in same direction
3. **Expected:** Momentum builds up (watch debug display)
4. **Try to change direction** - should be blocked
5. **Press opposite direction** to brake - momentum starts decaying
6. **Keep pressing opposite direction** until momentum reaches zero
7. **Then change direction** - should work only after momentum is zero

#### **3. Stage Comparison:**
1. **Test Stage 1:** Should have no car momentum
2. **Test Stage 2:** Should have moderate car momentum
3. **Test Stage 3:** Should have heavy car momentum
4. **Compare behavior** between stages

### **Debug Information:**

#### **Debug Display Shows:**
- **Movement Stage:** Current movement stage
- **Car Momentum Strength:** How strong momentum blocking is
- **Horizontal Momentum:** Current horizontal momentum (-1 to 1)
- **Vertical Momentum:** Current vertical momentum (-1 to 1)
- **Current Speed:** How fast character is moving
- **Momentum Threshold:** Speed threshold for momentum

#### **Console Logs:**
- **"Car Momentum: Horizontal/Vertical braking"** - When opposite input is detected (allows braking)
- **"Car Momentum: Horizontal/Vertical continuing"** - When same direction input is detected (allows continuing)
- **"Car Momentum: BLOCKED horizontal/vertical direction change"** - When direction change is completely blocked
- **Input and momentum direction values** - Shows which direction input vs momentum

### **Troubleshooting:**

#### **Character Can't Move At All:**
- **Check MomentumThreshold** - might be too low
- **Check MomentumBuildRate** - might be too high
- **Try Stage 1** - should have no car momentum

#### **Character Changes Direction Too Easily:**
- **Increase CarMomentum values** for stages 2 and 3
- **Decrease MomentumDecayRate** to make momentum last longer
- **Increase MomentumThreshold** to require more speed

#### **Character Never Stops:**
- **Check MomentumDecayRate** - might be too low
- **Check if opposite input** is being detected
- **Try pressing opposite direction** to brake

### **Advanced Testing:**

#### **1. Momentum Decay Test:**
1. **Build up momentum** by moving in one direction
2. **Stop input** completely
3. **Wait for momentum to decay** (watch debug display)
4. **Try to change direction** - should work when momentum is low

#### **2. Braking Test:**
1. **Build up momentum** by moving in one direction
2. **Press opposite direction** to brake
3. **Expected:** Character should slow down and stop
4. **Then change direction** - should work

#### **3. Speed Threshold Test:**
1. **Move very slowly** (below MomentumThreshold)
2. **Try to change direction** - should work
3. **Move faster** (above MomentumThreshold)
4. **Try to change direction** - should be blocked

## Expected Results

### **Successful Implementation:**
- **Character behaves like a car** - must stop before changing direction
- **Momentum builds up** when moving in one direction
- **Direction changes COMPLETELY BLOCKED** while moving (except opposite input)
- **Opposite input brakes** the character to a stop
- **Input is zeroed** when trying to change direction while having momentum
- **Different behavior** for each movement stage

### **Stage-Specific Behavior:**
- **Stage 1:** No car momentum - immediate response
- **Stage 2:** Moderate car momentum - some resistance
- **Stage 3:** Heavy car momentum - very difficult to change direction

The character should now move like a car, requiring the player to brake (opposite input) before changing direction, creating realistic momentum physics! 🚗✨
