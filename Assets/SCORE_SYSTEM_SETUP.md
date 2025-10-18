# Score System Setup Guide

## 🎯 Quick Setup

### Step 1: Create Score UI Text

1. **In your Canvas**, right-click and select:
   - **UI → Text - TextMeshPro** (Recommended)
   - OR **UI → Text** (Legacy, if TextMeshPro not available)

2. **Name it** "ScoreText"

3. **Position it in top right**:
   - Select the ScoreText
   - In Rect Transform:
     - **Anchor Preset**: Top-Right (hold Shift+Alt and click top-right)
     - **Pos X**: -20 (or adjust to preference)
     - **Pos Y**: -20 (or adjust to preference)
     - **Width**: 200
     - **Height**: 50

4. **Style the text**:
   - Font Size: 24-32 (or larger)
   - Color: White (or your preference)
   - Alignment: Right (for right-aligned text)
   - Text: "Score: 0" (initial text)

### Step 2: Add Score Manager

1. **Create an empty GameObject** in your scene:
   - Right-click in Hierarchy → Create Empty
   - Name it "ScoreManager"

2. **Add the ScoreManager component**:
   - Select ScoreManager
   - Add Component → Search for "Score Manager"
   - Add it

3. **Configure the component**:
   - Drag your **ScoreText** into the appropriate field:
     - If using TextMeshPro: Drag to "Score Text TMP"
     - If using Legacy Text: Drag to "Score Text Legacy"
   - **Points Per Kill**: 100 (default, adjust as needed)
   - **Score Prefix**: "Score: " (default)

### Step 3: Test

1. **Play the scene**
2. **Kill an enemy**
3. **Score should increase** by 100 points
4. **Test manually**: Right-click ScoreManager component → "Add 100 Points"

## 🎨 Styling Recommendations

### **Top-Right Position**:
```
Anchor: Top-Right
Pivot: (1, 1)
Pos X: -20
Pos Y: -20
```

### **Font Styling**:
- **Size**: 28-36 for good visibility
- **Color**: White with subtle shadow/outline
- **Alignment**: Right-aligned
- **Font**: Bold for better readability

### **Shadow/Outline** (Optional but recommended):
- Add Component → Shadow (or Outline)
- Makes text more readable on any background

## ⚙️ Configuration Options

### **ScoreManager Settings**:
- `pointsPerKill`: How many points per enemy (default: 100)
- `scorePrefix`: Text before number (default: "Score: ")
- `debugMode`: Enable console logging for debugging

### **Different Point Values**:
You can modify `pointsPerKill` in the inspector or call:
```csharp
ScoreManager.Instance.AddScore(customAmount);
```

## 🎮 Advanced Usage

### **From Code**:
```csharp
// Add points
ScoreManager.Instance.AddScore(100);

// Get current score
int currentScore = ScoreManager.Instance.CurrentScore;

// Reset score
ScoreManager.Instance.ResetScore();

// Set specific score
ScoreManager.Instance.SetScore(500);
```

### **Events** (if you want to add them):
You can extend the ScoreManager to fire events when score changes for things like:
- Achievement unlocks
- High score tracking
- UI animations

## 🔧 Troubleshooting

### **Score doesn't increase when killing enemies**:
1. Check if ScoreManager is in the scene
2. Verify ScoreText is assigned in inspector
3. Enable Debug Mode and check console
4. Make sure enemies have Health components

### **Text doesn't appear**:
1. Check if Canvas is present
2. Verify text is on correct layer
3. Check if text color contrasts with background
4. Ensure Canvas Scaler is configured

### **Score resets on scene reload**:
- This is intentional
- If you want persistent score, you need to add:
  - PlayerPrefs saving
  - Or DontDestroyOnLoad logic

## ✅ Complete Checklist

- [ ] Created ScoreText in Canvas (top-right)
- [ ] Created ScoreManager GameObject
- [ ] Added ScoreManager component
- [ ] Assigned ScoreText to ScoreManager
- [ ] Configured points per kill
- [ ] Tested by killing an enemy
- [ ] Styled text for good visibility
- [ ] Added shadow/outline for readability

The score system is now ready! Every enemy kill will add 100 points to the score displayed in the top right corner. 🍺⚔️✨

