# 🍺 Beer Meter Stun System - Quick Setup Guide

## 🚨 **The Problem**
Your beer meter stun system isn't working because the **BeerMeterStunHandler component is not attached to your player character**.

## ⚡ **Quick Fix (2 minutes)**

### **Step 1: Add Setup Helper**
1. **Select your player character** in the scene
2. **Add Component** → Search "Beer Meter Stun Setup Helper"
3. **Right-click on the component** → Select "Setup Beer Meter Stun System"

### **Step 2: Test the System**
1. **Right-click on BeerMeterStunHandler** → Select "Test Beer Level 100"
2. **Watch the console** for debug messages
3. **Player should get stunned** and puke effect should appear

## 🔧 **Manual Setup (if needed)**

### **Step 1: Create BeerManager**
1. **Create empty GameObject** named "BeerManager"
2. **Add Component** → "Beer Manager"
3. **Set Current Beer**: 50
4. **Set Depletion Rate**: 1

### **Step 2: Add BeerMeterStunHandler to Player**
1. **Select your player character**
2. **Add Component** → "Beer Meter Stun Handler"
3. **Configure settings**:
   - Beer Meter Threshold: 100
   - Beer Meter Reset Value: 15
   - Stun Duration: 1.5
   - Show Debug Info: ✅

### **Step 3: Create Puke Effect Prefab**
1. **Create empty GameObject** named "PukeEffect"
2. **Add SpriteRenderer** with green circle sprite
3. **Add PukeEffect script**
4. **Save as prefab**
5. **Assign to BeerMeterStunHandler**

## 🧪 **Testing Commands**

Use these context menu options (right-click on components):

**BeerMeterStunHandler:**
- "Check System Setup" - Diagnose all components
- "Show Status" - Show current system status  
- "Set Beer to 100" - Set beer level to 100
- "Force Trigger Stun" - Force trigger stun sequence

**BeerMeterStunSetupHelper:**
- "Setup Beer Meter Stun System" - Complete setup
- "Test Beer Level 100" - Test with beer level 100
- "Reset Beer Level" - Reset to normal level

## 📋 **Expected Console Output**

When working correctly, you should see:
```
BeerMeterStunHandler: Initialized - Stun Duration: 1.5s, Threshold: 100, Reset Value: 15
BeerMeterStunHandler: Beer level 100 >= threshold 100, triggering stun!
BeerMeterStunHandler: Character abilities disabled
BeerMeterStunHandler: Puke effect spawned at (x, y, z)
BeerMeterStunHandler: Beer meter reset to 15
BeerMeterStunHandler: Stun sequence completed
```

## ❌ **Common Issues**

1. **No debug messages**: BeerMeterStunHandler not attached
2. **"BeerManager not found"**: Create BeerManager GameObject
3. **"Puke Effect Prefab not assigned"**: Create and assign puke prefab
4. **Player still moves**: Check if other scripts override movement

## ✅ **Success Checklist**

- [ ] BeerManager exists in scene
- [ ] BeerMeterStunHandler attached to player
- [ ] Show Debug Info enabled
- [ ] Puke Effect Prefab assigned
- [ ] Console shows debug messages
- [ ] Stun triggers at beer level 100
- [ ] Player freezes during stun
- [ ] Green puke effect appears
- [ ] Beer meter resets to 15 after stun

## 🎯 **Quick Test**

1. **Run the game**
2. **Right-click BeerMeterStunHandler** → "Set Beer to 100"
3. **Player should immediately get stunned** for 1.5 seconds
4. **Green puke effect should appear**
5. **Beer meter should reset to 15**

**If this works, your system is properly set up!** 🍺✨
