# Auto Disable UI After Delay - Quick Guide

## ✅ Quick Setup

### Simple Setup (1 Step):
1. Select your UI Image/Panel in hierarchy
2. Add Component → TopDown Engine → GUI → **Auto Disable UI After Delay**
3. Done! Will disable after 4 seconds

---

## 🎯 What It Does

- UI element is visible
- Wait 4 seconds
- UI element automatically disables
- Perfect for notifications, splash screens, tutorial text

---

## ⚙️ Settings

### Disable Settings:
- **Disable Delay**: 4 seconds (time before disabling)
- **Disable Self**: Checked ✅ (disables this GameObject)
- **Additional Targets**: Empty (optional other objects to disable)

### Auto Start:
- **Auto Start On Enable**: Checked ✅ (starts when enabled)

### Fade Out (Optional):
- **Fade Before Disable**: Unchecked (no fade by default)
- **Fade Duration**: 1 second

---

## 🎨 Common Uses

### Tutorial Text:
```
Disable Delay: 5
Disable Self: Checked
Fade Before Disable: Checked
```

### Notification Pop-up:
```
Disable Delay: 3
Disable Self: Checked
Fade Before Disable: Unchecked
```

### Splash Screen:
```
Disable Delay: 4
Disable Self: Checked
Fade Before Disable: Checked
Fade Duration: 1.5
```

### Achievement Notification:
```
Disable Delay: 4
Disable Self: Checked
```

---

## 💡 Advanced Usage

### Disable Multiple UI Elements:
1. Add component to one UI element
2. Drag other UI elements into "Additional Targets"
3. All will disable together

### Keep Active But Hide Others:
1. Uncheck "Disable Self"
2. Add other UI to "Additional Targets"
3. This stays active, others disable

### With Fade Effect:
1. Check "Fade Before Disable"
2. Set "Fade Duration" to 1-2 seconds
3. UI fades out smoothly before hiding

---

## 🧪 Testing

### Context Menu (while playing):
- **Reset Timer** - Restart countdown
- **Disable Now** - Disable immediately
- **Cancel Timer** - Stop countdown

---

## 📝 Examples

### Example 1: "Level Start" Text
- Shows at level start
- Auto-disables after 4 seconds
- Clean!

### Example 2: "New Item Unlocked"
- Shows when item picked up
- Fades out after 3 seconds
- Professional!

### Example 3: Tutorial Tooltip
- Shows instructions
- Disappears after 5 seconds
- User-friendly!

---

Simple and effective! Just add to any UI element you want to auto-hide! 🎯

