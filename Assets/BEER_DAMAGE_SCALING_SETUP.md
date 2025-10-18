# 🍺 Beer Damage Scaling System Setup Guide

## Overview
The Beer Damage Scaling System makes the player deal more damage as they get more drunk. Higher beer levels = higher damage multipliers!

## 🎯 **Damage Scaling Stages**

| Beer Level | Stage | Damage Multiplier | Description |
|------------|-------|-------------------|-------------|
| 0-33% | Stage 1 | 1.0x | Sober - Normal damage |
| 34-66% | Stage 2 | 1.5x | Tipsy - 50% more damage |
| 67-99% | Stage 3 | 2.0x | Drunk - 100% more damage |
| 100% | Stage 4 | 2.5x | Wasted - 150% more damage (before stun) |

## ⚡ **Quick Setup (2 minutes)**

### **Step 1: Add Setup Helper**
1. **Create empty GameObject** named "BeerDamageSetupHelper"
2. **Add Component** → "Beer Damage Multiplier Setup Helper"
3. **Right-click on component** → Select "Setup Beer Damage Multiplier System"

### **Step 2: Test the System**
1. **Right-click on component** → Select "Test Damage Scaling"
2. **Watch console** for damage multiplier changes
3. **Attack enemies** to see increased damage

## 🔧 **Manual Setup (if needed)**

### **Step 1: Create BeerManager**
1. **Create empty GameObject** named "BeerManager"
2. **Add Component** → "Beer Manager"
3. **Configure settings**:
   - Current Beer: 50
   - Depletion Rate: 1

### **Step 2: Add Damage Multipliers to Weapons**
1. **Select each weapon** in your scene
2. **Add Component** → "Beer Damage Multiplier"
3. **Configure damage stages**:
   - Stage 1 Multiplier: 1.0
   - Stage 2 Multiplier: 1.5
   - Stage 3 Multiplier: 2.0
   - Stage 4 Multiplier: 2.5

## 🧪 **Testing Commands**

Use these context menu options (right-click on components):

**BeerDamageMultiplierSetupHelper:**
- **"Setup Beer Damage Multiplier System"** - Complete setup
- **"Test Damage Scaling"** - Test with different beer levels
- **"Reset All Weapon Damage"** - Reset all weapons to original damage
- **"Show All Weapon Status"** - Show status of all weapons

**BeerDamageMultiplier (on individual weapons):**
- **"Test Beer Level 50"** - Set beer to 50% for testing
- **"Test Beer Level 80"** - Set beer to 80% for testing
- **"Test Beer Level 100"** - Set beer to 100% for testing
- **"Show Status"** - Show current weapon status
- **"Force Update Multiplier"** - Force update damage multiplier

## 📊 **Expected Console Output**

When working correctly, you should see:
```
BeerDamageMultiplier: Initialized - Base: 1, Max: 2.5
BeerDamageMultiplier: Beer level 50.0 -> Multiplier 1.50 (Min: 15.0, Max: 30.0)
BeerDamageMultiplier: Beer level 80.0 -> Multiplier 2.00 (Min: 20.0, Max: 40.0)
BeerDamageMultiplier: Beer level 100.0 -> Multiplier 2.50 (Min: 25.0, Max: 50.0)
```

## ⚙️ **Configuration Options**

### **Damage Scaling Settings**
- **Base Damage Multiplier**: 1.0 (sober damage)
- **Max Damage Multiplier**: 2.5 (maximum drunk damage)
- **Stage 1 Multiplier**: 1.0x (0-33% beer)
- **Stage 2 Multiplier**: 1.5x (34-66% beer)
- **Stage 3 Multiplier**: 2.0x (67-99% beer)
- **Stage 4 Multiplier**: 2.5x (100% beer)

### **Visual Feedback**
- **Show Damage Multiplier In UI**: Enable to show damage multiplier in UI
- **Damage Multiplier Text**: Assign UI text to display current multiplier

## 🎮 **How It Works**

1. **Beer Level Monitoring**: System continuously monitors beer meter level
2. **Stage Detection**: Determines current beer stage (1-4) based on level
3. **Damage Calculation**: Applies appropriate multiplier to weapon damage
4. **Real-time Updates**: Damage updates immediately when beer level changes
5. **Visual Feedback**: Optional UI display of current damage multiplier

## 🔍 **Debugging**

### **Check if System is Working**
1. **Enable "Show Debug Info"** on BeerDamageMultiplier components
2. **Watch console** for damage multiplier messages
3. **Test with different beer levels** using context menu options
4. **Attack enemies** and observe damage differences

### **Common Issues**
1. **No damage increase**: Check if BeerManager exists and is working
2. **Weapons not found**: Ensure weapons have Weapon component
3. **Multiplier not updating**: Check if beer level is actually changing
4. **UI not showing**: Assign DamageMultiplierText if using UI display

## 📋 **Setup Checklist**

- [ ] BeerManager exists in scene
- [ ] BeerDamageMultiplier added to all weapons
- [ ] Show Debug Info enabled
- [ ] Damage multipliers configured correctly
- [ ] Console shows debug messages
- [ ] Damage increases with beer level
- [ ] UI shows damage multiplier (if enabled)
- [ ] No compilation errors

## 🎯 **Expected Results**

After setup:
- ✅ **Sober (0-33%)**: Normal damage (1.0x multiplier)
- ✅ **Tipsy (34-66%)**: 50% more damage (1.5x multiplier)
- ✅ **Drunk (67-99%)**: 100% more damage (2.0x multiplier)
- ✅ **Wasted (100%)**: 150% more damage (2.5x multiplier)
- ✅ **Real-time scaling**: Damage updates immediately with beer level
- ✅ **Visual feedback**: Optional UI display of current multiplier

## 🚀 **Advanced Features**

### **Custom Damage Curves**
You can modify the `DamageScalingCurve` in the inspector to create custom damage scaling curves instead of linear stage-based scaling.

### **Per-Weapon Configuration**
Each weapon can have different damage scaling settings by configuring individual `BeerDamageMultiplier` components.

### **UI Integration**
Assign a UI Text component to `DamageMultiplierText` to display the current damage multiplier to the player.

## 🎉 **Result**

This creates a fun risk-reward mechanic where players can choose to get drunk for more damage, but risk getting stunned at 100% beer level! 🍺⚔️✨
