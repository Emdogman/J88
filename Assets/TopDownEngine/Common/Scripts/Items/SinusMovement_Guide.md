# Sinus Movement for KoalaCoinPicker (Beer) - Guide

## ✅ Implementation Complete!

The beer items (KoalaCoinPicker) now have beautiful sinusoidal wave movement after they land!

---

## 🎨 What It Does

When an enemy dies and drops beer:
1. **Drop Animation**: Beer flies from enemy to landing spot (existing)
2. **Lands**: Beer reaches its final position
3. **Sinus Movement Starts**: Beer begins moving in a wave pattern around the landing spot
4. **Gentle Bobbing**: Beer also bobs up and down gently

This makes the beer items **visually appealing** and **easier to notice**!

---

## ⚙️ Default Settings (Already Configured)

The system is **already set up automatically** when enemies drop beer:

```
Start Delay: 0.5s (waits for drop animation to finish)
Horizontal Amplitude: 0.2 (moves ±0.2 units left/right)
Vertical Amplitude: 0.15 (moves ±0.15 units up/down)
Horizontal Speed: 2.0 (wave speed)
Vertical Speed: 2.5 (different speed creates organic movement)
Bobbing: Enabled
Bobbing Amplitude: 0.08 (subtle up/down)
```

---

## 🎯 Movement Patterns

### Pattern 1: Simple Wave (Default)
- Beer moves in a smooth elliptical orbit
- Predictable and pleasant
- **Currently enabled**

### Pattern 2: Figure-8 Pattern
- Beer moves in a figure-8 / infinity symbol shape
- More dynamic and eye-catching
- **Enable in Inspector**: Check "Use Figure 8 Pattern"

---

## 🎮 No Setup Needed!

The system is **fully automatic**:
- ✅ Added to all beer drops automatically
- ✅ Waits for drop animation to complete
- ✅ Starts sinus movement at landed position
- ✅ Continues until beer is picked up

---

## 🔧 Customization (Optional)

If you want to adjust the movement, select a dropped beer item while playing and modify:

### Make Movement Bigger:
```
Horizontal Amplitude: 0.4
Vertical Amplitude: 0.3
```

### Make Movement Faster:
```
Horizontal Speed: 4.0
Vertical Speed: 5.0
```

### Make Movement Slower/Calmer:
```
Horizontal Speed: 1.0
Vertical Speed: 1.2
```

### Disable Bobbing:
```
Enable Bobbing: Unchecked
```

### Enable Figure-8:
```
Use Figure 8 Pattern: Checked
```

---

## 🎨 Visual Examples

### Simple Wave (Default):
```
○ → ○ → ○ → ○
↓         ↑
○ ← ○ ← ○ ← ○
```
Beer moves in ellipse/circle

### Figure-8 Pattern:
```
  ○ → ○
 ↙     ↘
○   X   ○
 ↖     ↗
  ○ ← ○
```
Beer traces infinity symbol

---

## 💡 Benefits

✅ **Visual Appeal**: Beer items are animated and eye-catching
✅ **Easy to Spot**: Moving objects attract player attention
✅ **Professional Look**: Adds polish to your game
✅ **Automatic**: No manual setup needed
✅ **Performance**: Lightweight sine calculations
✅ **Customizable**: All parameters exposed

---

## 🔧 Advanced: Manual Setup

If you want to add SinusMovement to other objects:

1. Select the GameObject
2. Add Component → TopDown Engine → Items → **Sinus Movement**
3. Configure settings
4. Call `SetLandedPosition()` when ready to start

---

## 📝 Technical Details

- **Movement Type**: Sinusoidal (sine wave)
- **Dimensions**: 2D (X and Y)
- **Start Trigger**: Automatically after CoinDropAnimation completes
- **Performance**: Very lightweight (just sine calculations)
- **Cleanup**: Component stays active until object is destroyed/picked up

---

The beer will now float around in a mesmerizing wave pattern after landing! 🍺✨

