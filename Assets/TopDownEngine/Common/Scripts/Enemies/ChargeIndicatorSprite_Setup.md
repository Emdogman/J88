# Charge Indicator Sprite - Quick Setup

## ✅ Implementation Complete!

A warning sprite now appears above the enemy's head when they're preparing to charge!

---

## 🎮 How to Set It Up

### Step 1: Prepare Your Sprite
Create or find a warning sprite. Good options:
- ❗ Exclamation mark
- ⚡ Lightning bolt
- ⚠️ Warning symbol
- 💢 Anger mark
- 🎯 Targeting reticle

### Step 2: Add Sprite to Enemy
1. **Select your enemy** in the hierarchy
2. **Find the ChaserEnemy component** in Inspector
3. **Scroll to "Charge Telegraph Visual"** section
4. **Drag your sprite** into the **"Charge Indicator Sprite"** field

### Step 3: Adjust Settings (Optional)
- **Indicator Height Offset**: 1.5 (how high above enemy)
  - Increase for higher placement
  - Decrease to bring closer
  
- **Indicator Size**: (0.5, 0.5) (width x height)
  - Increase for bigger sprite
  - Decrease for smaller sprite

### Step 4: Done!
Test it! The sprite will now appear when enemies prepare to charge.

---

## 🎯 How It Works

### When Sprite Appears:
1. Enemy detects player within charge range
2. Enemy enters "Telegraphing Charge" state
3. **⚠️ WARNING SPRITE APPEARS** above enemy's head
4. Sprite stays visible during telegraph windup
5. Player has time to dodge!

### When Sprite Disappears:
1. Telegraph completes → charge begins → sprite hides
2. Enemy gets hit → charge interrupted → sprite hides

---

## ⚙️ Default Settings

```
Height Offset: 1.5 units above enemy
Size: 0.5 x 0.5 units
Sorting Order: 100 (renders on top)
```

---

## 💡 Tips

### Visual Design:
- Use **bright colors** (red, yellow, white) for visibility
- **Simple shapes** work best (exclamation mark, lightning)
- **High contrast** against game background

### Size Adjustment:
- Too small? Increase to (0.8, 0.8)
- Too big? Decrease to (0.3, 0.3)
- Want it tall? Try (0.4, 0.8)

### Height Adjustment:
- Above head: 1.5 - 2.0
- Near head: 0.8 - 1.2
- Very high: 2.5 - 3.0

---

## 🧪 Testing

1. Play your scene
2. Let enemy get within charge range
3. Watch for the telegraph state
4. **Warning sprite should appear above enemy!**
5. Sprite disappears when charge executes

### Debug Mode:
- Enable "Show Debug Info" on ChaserEnemy
- Check console for messages:
  - "Charge indicator shown"
  - "Charge indicator hidden"

---

## 🎨 Example Setup

### Aggressive Warning:
```
Sprite: Red exclamation mark
Height: 2.0
Size: (0.7, 0.7)
```

### Subtle Indicator:
```
Sprite: Yellow glow
Height: 1.2
Size: (0.4, 0.4)
```

### Large Warning:
```
Sprite: White lightning bolt
Height: 1.8
Size: (1.0, 1.0)
```

---

The charge indicator is now ready! Just drag your sprite and test! ⚡

