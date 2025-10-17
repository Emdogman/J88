# Beer Meter Auto-Setup System 🍺

The Beer Meter UI now features an **automatic setup system** that creates all necessary components without manual configuration!

## Quick Setup Methods

### Method 1: Using BeerMeterSetup Component
1. **Create an empty GameObject** in your scene
2. **Add the `BeerMeterSetup` component** to it
3. **Run the scene** - the system will automatically create everything!
4. The setup component will destroy itself after completion

### Method 2: Using BeerMeterUI Directly
1. **Create an empty GameObject** in your scene
2. **Add the `BeerMeterUI` component** to it
3. **Run the scene** - it will auto-setup all UI components!

### Method 3: Code Setup
```csharp
// Quick setup from code
BeerMeterUI beerMeter = BeerMeterSetup.QuickSetup();
```

## What Gets Auto-Created

### 🎯 **BeerMeterUI Auto-Setup Creates:**
- **Background Image** - Dark semi-transparent background
- **Fill Image** - Color-coded beer level indicator
- **Zone Dividers** - Vertical lines at 33% and 66%
- **Percentage Text** - Shows current beer level
- **Canvas Setup** - Creates canvas if none exists
- **Positioning** - Places meter at top center (thin and long)

### 🎯 **BeerMeterSetup Creates:**
- **BeerManager** - Central beer system manager
- **BeerMeterUI** - Complete UI system
- **Proper positioning** - Top center placement (thin and long)

## Auto-Setup Features

### ✅ **Smart Component Detection**
- Automatically finds existing UI components
- Creates missing components as needed
- Maintains existing configurations when possible

### ✅ **Automatic Canvas Setup**
- Creates canvas if none exists in the scene
- Sets up proper canvas scaler and raycaster
- Handles screen space overlay rendering

### ✅ **Intelligent Positioning**
- Defaults to top center (20%-80% width, 90%-95% height)
- Proper anchor and offset setup
- Responsive to different screen sizes

### ✅ **Zone Divider Creation**
- Creates vertical lines at 33% and 66% positions
- Semi-transparent white dividers
- Proper anchoring and sizing

## Context Menu Options

### 🛠️ **BeerMeterUI Context Menu:**
- **"Force Auto Setup"** - Re-run auto-setup
- **"Test Beer Level 25%"** - Test zone 1 (red)
- **"Test Beer Level 50%"** - Test zone 2 (orange)  
- **"Test Beer Level 75%"** - Test zone 3 (green)

### 🛠️ **BeerMeterSetup Context Menu:**
- **"Setup Beer Meter System"** - Manual setup trigger

## Configuration Options

### 🎛️ **BeerMeterSetup Settings:**
- `CreateBeerManager` - Auto-create BeerManager
- `ShowDebugInfo` - Display setup messages
- `DestroyAfterSetup` - Remove setup component when done

### 🎛️ **BeerMeterUI Settings:**
- `ShowDebugInfo` - Display debug information
- `Zone1Color` - Red color for high drift zone
- `Zone2Color` - Orange color for drift zone
- `Zone3Color` - Green color for normal zone

## Usage Examples

### 🚀 **Quick Start (Recommended):**
```csharp
// Just add this to any GameObject and run the scene!
gameObject.AddComponent<BeerMeterSetup>();
```

### 🚀 **Manual Setup:**
```csharp
// Create the UI manually
GameObject beerMeterGO = new GameObject("BeerMeterUI");
BeerMeterUI beerMeter = beerMeterGO.AddComponent<BeerMeterUI>();
// Auto-setup will run automatically!
```

### 🚀 **Code Setup:**
```csharp
// One-line setup
BeerMeterUI beerMeter = BeerMeterSetup.QuickSetup();
```

## Troubleshooting

### ❓ **No UI Appearing?**
- Check if you have a Canvas in your scene
- Ensure the BeerMeterUI is positioned correctly
- Try the "Force Auto Setup" context menu option

### ❓ **Beer Level Not Updating?**
- Make sure BeerManager exists in your scene
- Check that BeerMeterUI is subscribed to events
- Verify the character has CharacterMovementStaged component

### ❓ **Zone Colors Not Changing?**
- Test with context menu options (25%, 50%, 75%)
- Check that FillImage is properly assigned
- Verify zone color settings in inspector

## Integration with Existing Systems

The auto-setup system works seamlessly with:
- ✅ **TopDown Engine** - Full integration
- ✅ **Existing UI systems** - Finds and uses existing canvases
- ✅ **Character movement** - Automatic stage switching
- ✅ **Beer pickup system** - Event-driven updates

---

**🎮 Ready to use!** Just add the `BeerMeterSetup` component to any GameObject and run your scene!
