# UI Image On Player Death - Setup Guide

## ✅ Quick Setup

### Step 1: Create a Manager GameObject
1. Right-click in Hierarchy → Create Empty
2. Name it "DeathUIManager" (or similar)

### Step 2: Add the Script
1. Select the DeathUIManager
2. Add Component → TopDown Engine → GUI → **UI Image On Player Death**

### Step 3: Assign Your UI
1. In the Inspector, find **"Target UI"** field
2. Drag your death UI Image/Panel here
3. Done!

---

## 🎮 How It Works

When the player dies:
1. Script detects player death via Health.OnDeath event
2. Waits for delay (if set)
3. **Enables your UI Image/GameObject**
4. Perfect for Game Over screens, death overlays, etc.

---

## ⚙️ Settings

### UI Reference:
- **Target UI**: Drag your UI Image/Panel/GameObject here
- **Auto Disable On Start**: Checked ✅ (hides UI at game start)

### Player Detection:
- **Player Tag**: "Player" (tag to find player)

### Timing:
- **Show Delay**: 0 seconds (instant)
  - Increase for delayed effect (e.g., 1.5 seconds)

---

## 🎨 Common Use Cases

### Game Over Screen:
```
Target UI: GameOverPanel
Auto Disable On Start: Checked
Show Delay: 1.0 (wait 1 second before showing)
```

### Death Overlay:
```
Target UI: DeathOverlayImage
Auto Disable On Start: Checked
Show Delay: 0 (instant)
```

### You Died Text:
```
Target UI: YouDiedText
Auto Disable On Start: Checked
Show Delay: 0.5
```

---

## 💡 Combine With Zoom Effect

For awesome results, combine with UIImageZoomEffect!

**Setup:**
1. Add **UIImageOnPlayerDeath** to a manager object
2. Assign your death UI
3. On the death UI itself, add **UIImageZoomEffect**
4. Configure zoom settings

**Result:**
- Player dies
- UI enables
- UI zooms in from center
- Professional death screen! 🎯

---

## 🧪 Testing

### Context Menu Testing (while playing):
1. Select the manager object
2. Right-click component → **Test Trigger**
3. UI should enable (and zoom if you added zoom effect)

### Reset Testing:
1. Right-click component → **Reset**
2. UI hides and can be triggered again

---

## 🔧 Advanced Usage

### Multiple UI Elements:
Create multiple instances:
- One for death overlay
- One for "You Died" text
- One for restart button
- Each with different delays

### Custom Delay Pattern:
```
Death Overlay: 0s delay
You Died Text: 0.5s delay
Restart Button: 1.5s delay
```

### Disable Auto-Hide:
If your UI is already hidden:
- Uncheck "Auto Disable On Start"
- Manually control visibility

---

## 📝 Requirements

- Player must have **Health** component
- Player must have tag **"Player"** (or your custom tag)
- Target UI must be assigned

---

## 🎯 Example Complete Setup

**Manager GameObject:**
- Component: UIImageOnPlayerDeath
- Target UI: DeathScreenPanel

**DeathScreenPanel:**
- Component: Image (with your death image)
- Component: UIImageZoomEffect
  - Start Scale: 0
  - Target Scale: 1
  - Zoom Duration: 1.5

**Result:** Smooth zooming death screen on player death! 💀

---

Simple, effective, and works perfectly with the TopDown Engine death system! 🎮

