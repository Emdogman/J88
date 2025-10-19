# UI Image Zoom Effect - Setup Guide

## ✅ Quick Setup

### Step 1: Select Your UI Image
1. Find your UI Image in the hierarchy (under Canvas)
2. Click to select it

### Step 2: Add Component
1. Click **Add Component**
2. Search for: **UI Image Zoom Effect**
3. Click to add

### Step 3: Configure (Optional)
The default settings work great, but you can customize!

---

## 🎨 What It Does

The UI Image will **gradually zoom from center** when activated:
- Starts at scale 0 (invisible/tiny)
- Smoothly grows to scale 1 (normal size)
- Animates over 1 second
- Perfectly centered zoom effect

---

## ⚙️ Settings

### Zoom Settings:
- **Start Scale**: 0 (starts invisible)
- **Target Scale**: 1 (normal size)
- **Zoom Duration**: 1 second
- **Zoom Curve**: EaseInOut (smooth acceleration/deceleration)

### Auto Start:
- **Auto Start On Enable**: Checked ✅ (zooms when GameObject enables)
- **Auto Start Delay**: 0 seconds (starts immediately)

### Optional Fade:
- **Fade While Zooming**: Unchecked (no fade by default)
- **Start Alpha**: 0 (invisible)
- **Target Alpha**: 1 (visible)

### Loop Settings:
- **Loop Animation**: Unchecked (plays once)
- **Reverse On Loop**: Unchecked

---

## 🎮 Usage Examples

### Example 1: Splash Screen Logo
```
Start Scale: 0
Target Scale: 1.2 (slightly bigger than normal)
Zoom Duration: 1.5
Fade While Zooming: Checked
Auto Start On Enable: Checked
```

### Example 2: Warning Overlay
```
Start Scale: 0.5
Target Scale: 1.5 (grows larger)
Zoom Duration: 0.5
Zoom Curve: Linear (constant speed)
```

### Example 3: Pulsing Effect
```
Start Scale: 0.8
Target Scale: 1.2
Zoom Duration: 1.0
Loop Animation: Checked
Reverse On Loop: Checked
```

### Example 4: Fade In With Zoom
```
Start Scale: 0
Target Scale: 1
Fade While Zooming: Checked
Start Alpha: 0
Target Alpha: 1
Zoom Duration: 2.0
```

---

## 🎯 Common Use Cases

### When UI Panel Opens:
- Image zooms in smoothly
- Creates polished transition

### Game Over Screen:
- Logo/text zooms from center
- Professional game over effect

### Level Complete:
- Victory image zooms in
- Celebratory effect

### Warning/Alert:
- Alert image pops in
- Gets player attention

---

## 🔧 Advanced Features

### Manual Control:
Use these methods in your code or Context Menu:
- **Start Zoom** - Begin the animation
- **Stop Zoom** - Halt the animation
- **Reset** - Return to original scale
- **Set Start Scale** - Jump to start size
- **Set Target Scale** - Jump to target size

### Animation Curves:
Customize the zoom feel:
- **Linear**: Constant speed
- **EaseIn**: Starts slow, speeds up
- **EaseOut**: Starts fast, slows down
- **EaseInOut**: Smooth acceleration and deceleration (default)
- **Bounce**: Bouncy spring effect
- **Custom**: Create your own curve!

---

## 💡 Pro Tips

1. **Center Pivot**: Make sure your UI Image's pivot is centered (0.5, 0.5)
2. **Start Small**: Use 0 or 0.1 for dramatic zoom-in
3. **Overshoot**: Use target scale > 1 then settle to 1 for impact
4. **Combine with Fade**: Enable fade for extra polish
5. **Loop for Pulsing**: Great for attention-getting UI elements

---

## 🔧 Troubleshooting

| Problem | Solution |
|---------|----------|
| Zoom from corner instead of center | Set Image pivot to (0.5, 0.5) |
| Doesn't start automatically | Check "Auto Start On Enable" |
| Too fast/slow | Adjust "Zoom Duration" |
| Jerky animation | Use EaseInOut curve |
| Image disappears | Check Start Alpha if fade enabled |

---

## 📝 Technical Notes

- **Requirement**: Must have Image component
- **Performance**: Very lightweight (just scale interpolation)
- **Works with**: Any UI Image, RawImage works too
- **Canvas Mode**: Works with all canvas render modes

---

Enjoy your smooth zoom effect! 🎯

