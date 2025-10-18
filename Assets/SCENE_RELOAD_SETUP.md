# Scene Reload on Death Setup Guide

## 🎯 **Quick Setup**

### **Step 1: Add SceneReloadOnDeath to a GameObject**
1. **Create an empty GameObject** in your scene:
   - Right-click in Hierarchy → Create Empty
   - Name it "SceneReloadManager"

2. **Add the SceneReloadOnDeath component**:
   - Select the SceneReloadManager
   - Add Component → Search for "Scene Reload On Death"
   - Add it

### **Step 2: Configure Settings (Optional)**
The default settings should work well, but you can adjust:
- **Reload Delay**: 2.0 seconds (how long to wait before reloading)
- **Player Tag**: "Player" (should be correct by default)
- **Debug Mode**: true (shows debug messages in console)

### **Step 3: Test the Setup**
1. **Right-click** on the SceneReloadOnDeath component
2. **Select "Test Reload"** from the context menu
3. **The scene should reload** after 2 seconds

### **Step 4: Test in Game**
1. **Play the scene**
2. **Let the player die** (take damage until health reaches 0)
3. **Scene should reload** after 2 seconds

## 🔧 **How It Works**

1. **Finds the player** by tag and subscribes to their Health.OnDeath event
2. **When player dies** - starts a countdown timer
3. **After the delay** - reloads the current scene from scratch
4. **Prevents multiple reloads** - only reloads once per death

## ✅ **Expected Result**

When the player dies:
1. **Player health reaches 0**
2. **2-second delay** (configurable)
3. **Scene reloads** completely from the beginning
4. **Player respawns** at the starting position
5. **All enemies respawn** and the game restarts

## 🎮 **Features**

- ✅ **Automatic scene reload** when player dies
- ✅ **Configurable delay** before reload
- ✅ **Debug logging** to see what's happening
- ✅ **Prevents multiple reloads** (only reloads once)
- ✅ **Easy to configure** in inspector
- ✅ **Test function** for debugging

## 🚀 **Ready to Use**

The scene reload system is now ready! When the player dies, the scene will automatically reload after 2 seconds, giving the player a fresh start. 🍺⚔️✨
