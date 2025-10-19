# Timed Destruction With Blink - Guide

## ✅ Implementation Complete!

KoalaCoinPicker (beer) items now automatically:
- Last for **6 seconds** after being dropped
- **Blink rapidly** in the last 3 seconds (warning!)
- Get **destroyed** after 6 seconds total

No setup needed - it's already integrated!

---

## 🎯 How It Works

### Timeline:

**0s - 3s: Normal**
- Beer drops from enemy
- Floats in sinus wave pattern
- Fully visible and opaque
- Can be picked up

**3s - 6s: Blinking Warning**
- Beer starts **blinking rapidly** (8 times per second)
- Flashes between transparent and opaque
- Warning that it's about to disappear!
- Can still be picked up

**6s: Destruction**
- Beer disappears
- GameObject is destroyed
- Player missed their chance!

---

## ⚙️ Settings (Already Configured)

The system is automatically added to every beer drop with:

```
Lifetime: 6 seconds (total time before destruction)
Blink Start Time: 3 seconds (starts blinking 3s before death)
Blink Speed: 8 (blinks 8 times per second - very fast)
Min Alpha: 0 (fully transparent during blink)
Max Alpha: 1 (fully opaque during blink)
Start Delay: 0.5s (waits for drop animation)
```

---

## 🎨 Visual Effect

### Phase 1 (0-3 seconds):
```
🍺 ← Solid, visible, floating
```

### Phase 2 (3-6 seconds):
```
🍺💫 ← Blinking rapidly, floating
(flashing: visible → invisible → visible → invisible)
```

### Phase 3 (6+ seconds):
```
💨 ← Destroyed, gone
```

---

## 🔧 Customization (Optional)

If you want to change the behavior, you can edit the values in `ChaserEnemy.cs` line 766-772:

### Make It Last Longer:
```csharp
timedDestruction.lifetime = 10f; // 10 seconds total
timedDestruction.blinkStartTime = 5f; // Blink in last 5 seconds
```

### Make Blinking Slower (More Visible):
```csharp
timedDestruction.blinkSpeed = 4f; // Slower blink
```

### Make Blinking Faster (More Urgent):
```csharp
timedDestruction.blinkSpeed = 12f; // Frantic blinking
```

### Semi-Transparent Blink (Not Fully Invisible):
```csharp
timedDestruction.minAlpha = 0.3f; // Never fully invisible
```

### Shorter Warning:
```csharp
timedDestruction.blinkStartTime = 1f; // Only blink in last 1 second
```

---

## 💡 Benefits

✅ **Visual Feedback**: Players know beer is disappearing soon
✅ **Creates Urgency**: Blinking makes players rush to grab it
✅ **Prevents Clutter**: Old beer doesn't pile up in level
✅ **Performance**: Auto-cleanup keeps scene clean
✅ **Automatic**: No manual setup needed

---

## 🎮 Gameplay Impact

**Strategic Element:**
- Players must prioritize beer pickups
- Can't hoard beer for later
- Creates risk/reward decisions
- "Should I grab this beer or fight enemies?"

**Difficulty Tuning:**
- **Easier**: Increase lifetime to 10s
- **Harder**: Decrease lifetime to 4s
- **Panic Mode**: Blink immediately (blinkStartTime = 6s)

---

## 📝 Technical Details

- **Method**: Sine wave alpha modulation
- **Performance**: Very lightweight (just alpha changes)
- **Cleanup**: GameObject.Destroy() after lifetime
- **Start Delay**: Waits for drop animation to complete
- **Works With**: All other beer systems (sinus movement, pickup delay, etc.)

---

## 🆚 Before vs After

**Before:**
- Beer drops stayed forever
- Cluttered the level
- No urgency to pick up

**After:**
- Beer lasts 6 seconds
- Blinks as warning
- Auto-cleanup
- Creates urgency and strategy!

---

The beer now has a limited lifespan with visual warning! 🍺⏰

