# Beer Damage Multiplier - Quick Setup

## ✅ Implementation Complete!

I've recreated the Beer Damage Scaling system. More drunk = More damage!

---

## 🎯 Damage Scaling

| Beer Level | Stage | Damage Multiplier |
|------------|-------|-------------------|
| 67-100% | Stage 1 (Sober) | **1.0x** - Normal |
| 34-66% | Stage 2 (Tipsy) | **1.5x** - +50% damage |
| 0-33% | Stage 3 (Drunk) | **2.0x** - +100% damage |

---

## 🎮 Quick Setup (2 Steps)

### Step 1: Find Your Player's Weapon
1. Play your scene
2. In Hierarchy, expand your player character
3. Find the weapon GameObject (usually under WeaponAttachment or similar)
4. Stop playing

### Step 2: Add Component to Weapon
1. Select the weapon GameObject
2. Add Component → TopDown Engine → Weapons → **Beer Damage Multiplier**
3. Done!

---

## ⚙️ Settings (Already Configured)

Default multipliers:
- **Stage 1 Multiplier**: 1.0 (sober, normal damage)
- **Stage 2 Multiplier**: 1.5 (tipsy, 50% more damage)
- **Stage 3 Multiplier**: 2.0 (drunk, 100% more damage)

---

## 🧪 Testing

### In-Game Testing:
1. Play your scene
2. Check your beer level
3. Attack an enemy
4. Pick up beer to change stages
5. Attack again - damage should be different!

### Debug Testing:
1. Select the weapon
2. Enable "Show Debug Info"
3. Right-click component → **Show Status**
4. Check console for current multiplier

### Context Menu Options:
- **Show Status** - Shows current damage and multiplier
- **Reset Damage** - Resets to original damage

---

## 💡 How It Works

The script:
1. Stores your weapon's original damage values
2. Listens to BeerManager for zone changes
3. When zone changes, applies the multiplier
4. Updates weapon damage in real-time

**Example:**
- Original damage: 10-20
- Beer at 20% (Zone 1, very drunk)
- Multiplier: 2.0x
- **New damage: 20-40!**

---

## 🔧 Customization

### More Damage When Drunk:
```
Stage 3 Multiplier: 3.0 (triple damage!)
```

### Less Damage Scaling:
```
Stage 2 Multiplier: 1.2
Stage 3 Multiplier: 1.5
```

### Extreme Drunk Power:
```
Stage 1 Multiplier: 1.0
Stage 2 Multiplier: 2.0
Stage 3 Multiplier: 4.0 (4x damage when wasted!)
```

---

## ⚠️ Important Notes

- **Beer Manager Required**: Make sure BeerManager exists in your scene
- **Per Weapon**: Add this to EACH weapon that should scale
- **Auto-Updates**: Damage changes automatically with beer level
- **Original Values**: Stored on Start(), so changes in Inspector after Start() won't affect scaling

---

## 🎮 Gameplay Impact

**Risk/Reward:**
- Get drunk → Deal more damage ✅
- But → Movement becomes harder ❌
- Strategic choice: Power vs Control!

---

Quick and simple - just add to your weapons! 🍺⚔️

