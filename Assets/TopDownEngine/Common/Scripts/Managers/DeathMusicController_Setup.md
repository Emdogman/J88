# Death Music Controller - Setup Guide

## ✅ Quick Setup

### Step 1: Create Music Manager
1. Right-click in Hierarchy → Create Empty
2. Name it "DeathMusicManager"

### Step 2: Add Component
1. Select DeathMusicManager
2. Add Component → TopDown Engine → Managers → **Death Music Controller**

### Step 3: Assign Music
1. In Inspector, find **"Death Music"** field
2. Drag your death music AudioClip here
3. Done!

---

## 🎵 How It Works

### Player Dies:
1. Death event triggers
2. **Background music fades out** (optional)
3. **Death music starts playing** (with fade in)
4. Music continues until player presses Space

### Scene Reloads:
1. Player presses Space (from SceneReloadOnDeath)
2. Scene reloads
3. **Death music automatically stops**
4. Normal game music resumes

---

## ⚙️ Settings

### Audio Settings:
- **Death Music**: Your AudioClip here
- **Music Volume**: 0.7 (70% volume)
- **Loop Music**: Unchecked (play once)

### Fade Settings:
- **Fade In**: Checked ✅ (smooth music entry)
- **Fade In Duration**: 1 second
- **Fade Out Previous Music**: Checked ✅ (stops game music smoothly)
- **Fade Out Duration**: 0.5 seconds

### Player Detection:
- **Player Tag**: "Player"

### Audio Source:
- **Audio Source**: Auto-creates if not assigned
- **Create Persistent Audio Source**: Checked ✅

---

## 🎨 Common Configurations

### Dramatic Death Music:
```
Music Volume: 0.8
Fade In: Checked
Fade In Duration: 2.0 (slow dramatic fade)
Loop Music: Checked (keeps playing)
```

### Quick Death Jingle:
```
Music Volume: 0.6
Fade In: Unchecked (instant start)
Loop Music: Unchecked (plays once)
```

### Silent Previous Music:
```
Fade Out Previous Music: Checked
Fade Out Duration: 1.5 (long fade)
```

---

## 💡 Complete Death System Setup

**Three Components Working Together:**

1. **SceneReloadOnDeath**
   - Wait For Space Key: ✅ Checked
   
2. **UIImageOnPlayerDeath**
   - Target UI: Your death screen
   
3. **DeathMusicController** (this script)
   - Death Music: Your AudioClip

**Result:**
- Player dies
- Background music fades out
- Death music fades in
- Death screen appears and zooms
- Player presses Space
- Scene reloads
- Death music stops
- Game music resumes

---

## 🧪 Testing

### In-Editor Testing:
1. While playing, select DeathMusicManager
2. Right-click component → **Test Death Music**
3. Music should start playing
4. Right-click → **Stop Death Music** to stop

### Full Test:
1. Play your game
2. Let player die
3. Death music should start
4. Press Space
5. Scene reloads
6. Death music stops

---

## 🔧 Troubleshooting

| Problem | Solution |
|---------|----------|
| No music plays | Check AudioClip is assigned |
| Music too loud | Lower "Music Volume" |
| Fade too slow | Decrease "Fade In Duration" |
| Music doesn't stop on reload | It stops automatically via OnDestroy |
| Previous music doesn't fade | Check "Fade Out Previous Music" |
| Music keeps playing after reload | Check that AudioSource isn't DontDestroyOnLoad |

---

## 📝 Technical Notes

- **Automatic AudioSource**: Creates one if needed
- **Fade System**: Smooth volume interpolation
- **Auto-Stop**: Uses OnDestroy to stop on scene reload
- **Previous Music Detection**: Finds and fades other AudioSources
- **Performance**: Lightweight, only active during death

---

## 🎵 Music Recommendations

**Death Music Types:**
- Sad/somber melody
- Game over jingle
- Dramatic orchestral hit
- Dark ambient music
- Retro game over sound

**Duration:**
- Short: 3-5 seconds (quick jingle)
- Medium: 10-15 seconds (if looped)
- Long: 30+ seconds (cinematic)

---

Your death sequence will now have proper music! 🎵💀

