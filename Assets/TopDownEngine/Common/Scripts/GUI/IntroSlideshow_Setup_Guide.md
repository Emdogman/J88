# Intro Slideshow Setup Guide

## Overview
The IntroSlideshow component displays a series of UI images when the scene loads, pauses the game, and allows the player to advance through slides by pressing Space. When all slides are viewed, the game unpauses and starts.

**Special Feature**: Slide 3 (the last slide) is displayed as a **background** from the very start. Slides 1 and 2 appear as overlays on top of it, then disappear, leaving only Slide 3 visible until you press Space to start the game.

## Quick Setup (6 Steps)

### Step 1: Prepare Your Images
1. Import your slide images into Unity (PNG or JPG)
2. Select each image in the Project window
3. In the Inspector, change **Texture Type** to **Sprite (2D and UI)**
4. Click **Apply**

### Step 2: Setup Canvas
1. In your scene, find or create a **Canvas**
2. Make sure Canvas has:
   - **Render Mode**: Screen Space - Overlay (or Camera)
   - **Canvas Scaler** component with UI Scale Mode set appropriately

### Step 3: Create Slide Images
1. Right-click the Canvas in Hierarchy
2. Select **UI > Image** (do this three times for 3 images)
3. Name them "Slide1", "Slide2", and "Slide3"
4. For each image:
   - Set **Anchor** to stretch (click the anchor preset and hold Alt+Shift, then click the bottom-right stretch option)
   - Set **Left, Right, Top, Bottom** to 0 (makes it fill the screen)
   - Drag your slide image sprite into the **Source Image** field
5. **Important**: Initially disable all three images (uncheck the checkbox at the top of Inspector)

**Tip**: Use Slide 3 for your game background or world view, and Slides 1-2 for tutorial/instruction overlays.

### Step 4: Create Slideshow Manager
1. Right-click the Canvas in Hierarchy
2. Select **Create Empty**
3. Name it "IntroSlideshow"
4. Click **Add Component**
5. Search for and add **IntroSlideshow** script

### Step 5: Configure IntroSlideshow
In the IntroSlideshow component:

**Slides Section:**
- Set **Size** to 3
- Drag "Slide1" into **Element 0**
- Drag "Slide2" into **Element 1**
- Drag "Slide3" into **Element 2**

**Settings Section:**
- **Advance Key**: Space (default)
- **Pause Game During Slides**: ✅ Checked
- **Fade Duration**: 0.3 (adjust for faster/slower transitions)

**Background Slide Section:**
- **Last Slide As Background**: ✅ Checked (default - Slide 3 shows from start)

**Optional Text Prompt:**
- Leave empty for now (or add a UI Text if you want a prompt)

### Step 6: Test It!
1. Press **Play**
2. You should see:
   - **Slide 3 visible in background** (always visible)
   - Slide 1 appears on top (game paused - Time.timeScale = 0)
   - Press Space → Slide 1 fades out, Slide 2 appears on top of Slide 3
   - Press Space → Slide 2 fades out, only Slide 3 remains visible
   - Press Space → Game starts (Time.timeScale = 1)

---

## Advanced Setup

### Adding a "Press Space to Continue" Prompt

1. Right-click Canvas → **UI > Text**
2. Name it "PromptText"
3. Configure:
   - Position it at the bottom center of the screen
   - Set text size to 24-32
   - Set alignment to Center
   - Set color to white (or your preference)
4. In IntroSlideshow component:
   - Drag "PromptText" into **Prompt Text** field
   - Set **Prompt Message** to "Press Space to Continue"

### Multiple Slides (More than 3)

1. Create as many UI Images as you need
2. In IntroSlideshow component:
   - Increase **Slides > Size** to match your slide count
   - Drag each slide into the array in order

### Custom Advance Key

In IntroSlideshow component:
- Change **Advance Key** from Space to any key you want (E, Enter, etc.)

### No Fade Effect

In IntroSlideshow component:
- Set **Fade Duration** to 0

---

## Hierarchy Structure

```
Canvas
├── IntroSlideshow (Empty GameObject with IntroSlideshow script)
├── Slide1 (UI Image - disabled by default)
├── Slide2 (UI Image - disabled by default)
├── Slide3 (UI Image - disabled by default)
└── PromptText (UI Text - optional)
```

---

## Troubleshooting

### Problem: Slides don't appear
**Solution**: 
- Make sure the slide Images are children of the Canvas
- Check that slide Images are initially disabled in the hierarchy
- Verify the IntroSlideshow script is on an active GameObject

### Problem: Game doesn't pause
**Solution**: 
- Ensure **Pause Game During Slides** is checked
- Check Console for "Game paused" message

### Problem: Can't advance slides
**Solution**: 
- Check Console for error messages
- Verify **Advance Key** is set to Space
- Make sure IntroSlideshow script is enabled

### Problem: Slides are the wrong size
**Solution**: 
- Select each slide Image
- Set Anchor to stretch (Alt+Shift + click bottom-right preset)
- Set Left, Right, Top, Bottom to 0

---

## How It Works

1. **On Scene Start**: 
   - Slide 3 (last slide) is immediately visible as background
   - Slide 3 is sent to the back (SetAsFirstSibling)
   - Slide 1 fades in on top
   - Game time is paused (`Time.timeScale = 0`)

2. **Player Presses Space** (First Time):
   - Slide 1 fades out
   - Slide 2 fades in on top of Slide 3 background
   - Slide 3 remains visible underneath

3. **Player Presses Space** (Second Time):
   - Slide 2 fades out
   - Only Slide 3 (background) remains visible
   - Game still paused, waiting for final input

4. **Player Presses Space** (Third Time):
   - Slide 3 stays visible (it's the game background/world)
   - Game time resumes (`Time.timeScale = 1`)
   - Slideshow component disables itself
   - Game starts!

---

## API Reference

### Public Methods

**`SkipSlideshow()`**
- Immediately skips all remaining slides and unpauses the game
- Usage: Call from another script or button

```csharp
// Example: Skip button
public IntroSlideshow slideshow;

void OnSkipButtonClicked()
{
    slideshow.SkipSlideshow();
}
```

### Inspector Fields

| Field | Description | Default |
|-------|-------------|---------|
| Slides | Array of UI Images to show | Empty |
| Advance Key | Key to press to continue | Space |
| Pause Game During Slides | Pause time during slideshow | True |
| Fade Duration | Fade transition time (seconds) | 0.3 |
| Prompt Text | Optional text element for prompt | None |
| Prompt Message | Message to display in prompt | "Press Space to Continue" |

---

## Tips

1. **Image Resolution**: Use images with 1920x1080 (Full HD) or 3840x2160 (4K) resolution for best quality
2. **Order Matters**: Slides are shown in the order they appear in the array
3. **Transparent Images**: You can use transparent PNGs to layer effects
4. **Skip Button**: Add a skip button by creating a UI Button that calls `SkipSlideshow()`
5. **Audio**: Add background music by attaching an AudioSource to the IntroSlideshow GameObject

