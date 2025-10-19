# Health Pickup Spawner - Setup Guide

## ✅ Quick Setup

### Step 1: Create Spawner
1. Create empty GameObject in your scene
2. Name it "HealthPickupSpawner"

### Step 2: Add Component
1. Select HealthPickupSpawner
2. Add Component → TopDown Engine → Items → **Health Pickup Spawner**

### Step 3: Assign Prefab
1. Find "Health Pickup Prefab" field
2. Drag your health pickup prefab here
3. Done!

---

## 🎯 How It Works

### Spawn Logic:
1. Every 10 seconds (configurable)
2. Check if < 3 pickups active
3. Find position outside camera view
4. Near player (5-10 units away)
5. Inside level bounds
6. On walkable tiles
7. **Spawn health pickup!**

### Max Limit:
- Maximum 3 health pickups active at once
- When player picks one up → can spawn another
- Prevents overflow of pickups

---

## ⚙️ Settings

### Health Pickup Settings:
- **Health Pickup Prefab**: Your health prefab
- **Max Active Pickups**: 3 (limit)
- **Spawn Interval**: 10 seconds (time between spawns)
- **Is Spawning**: Checked ✅

### Spawn Position:
- **Spawn Distance From Camera**: 2 units outside camera view
- **Min Distance From Player**: 5 units (not too close)
- **Max Distance From Player**: 10 units (not too far)

### Level Bounds:
- **Use Manual Bounds**: Unchecked (auto-detects from tilemap)
- **Walkable Tilemap**: Auto-finds "Ground", "Floor", etc.

### References:
- **Main Camera**: Auto-finds Camera.main
- **Player**: Auto-finds by "Player" tag

---

## 🎨 Common Configurations

### Frequent Health Drops:
```
Spawn Interval: 5 seconds
Max Active Pickups: 5
```

### Rare Health Drops:
```
Spawn Interval: 20 seconds
Max Active Pickups: 2
```

### Close to Player:
```
Min Distance From Player: 3
Max Distance From Player: 6
```

### Far from Player:
```
Min Distance From Player: 8
Max Distance From Player: 15
```

---

## 🔧 Manual Bounds Setup (Optional)

If you want specific spawn area:
1. Check "Use Manual Bounds"
2. Set coordinates:
   ```
   Bounds Min X: -15
   Bounds Max X: 15
   Bounds Min Y: -10
   Bounds Max Y: 10
   ```
3. Health only spawns in this rectangle

---

## 🧪 Testing

### Context Menu (while playing):
- **Spawn Health Pickup Now** - Force spawn immediately
- **Clear All Pickups** - Remove all active pickups

### Debug Mode:
1. Enable "Show Debug Info"
2. Check console for spawn messages
3. Enable "Show Spawn Zones" to see green zones in Scene view

---

## 🎮 How It Spawns

### Position Selection:
1. Pick random camera side (top/bottom/left/right)
2. Place 2 units outside camera edge
3. Check if 5-10 units from player ✅
4. Check if inside bounds ✅
5. Check if on walkable tile ✅
6. Spawn! 🏥

### Off-Screen Guarantee:
- Spawns outside camera view
- Player won't see it appear
- Appears naturally as they explore

---

## 💡 Pro Tips

1. **Balance**: 10-second interval with 3 max = good balance
2. **Tilemap**: Name your floor "Ground" or "Floor" for auto-detection
3. **Manual Bounds**: Use if your level has specific playable area
4. **Spawn Interval**: Adjust based on game difficulty
5. **Debug Gizmos**: Enable to see spawn zones in Scene view

---

## 📊 Spawn Rate Examples

### Easy Difficulty:
```
Spawn Interval: 8 seconds
Max Active: 4
```

### Normal Difficulty:
```
Spawn Interval: 12 seconds
Max Active: 3
```

### Hard Difficulty:
```
Spawn Interval: 20 seconds
Max Active: 2
```

---

## 🔍 Troubleshooting

| Problem | Solution |
|---------|----------|
| No pickups spawn | Assign health prefab |
| Spawns in view | Increase "Spawn Distance From Camera" |
| Spawns outside level | Enable "Use Manual Bounds" |
| Too many pickups | Lower "Max Active Pickups" |
| Spawns too fast | Increase "Spawn Interval" |
| Spawns on walls | Assign walkable tilemap |

---

Health pickups will now spawn intelligently around your player! 🏥✨

