# ChaserEnemy Setup Guide

## Overview
This guide explains how to set up a ChaserEnemy that automatically finds and chases the player using tag-based detection.

## Required Components

### 1. GameObject Setup
Create a new GameObject and name it "ChaserEnemy" (or any name you prefer).

### 2. Required Components
Add these components to your enemy GameObject:

#### **Rigidbody2D**
- **Body Type:** Dynamic
- **Material:** Create a Physics Material 2D with:
  - **Friction:** 0.5
  - **Bounciness:** 0.1
- **Linear Drag:** 1.0
- **Angular Drag:** 1.0
- **Gravity Scale:** 0 (for top-down games)

#### **Collider2D** (CircleCollider2D recommended)
- **Is Trigger:** False
- **Radius:** 0.5
- **Material:** Same as Rigidbody2D

#### **ChaserEnemy Script**
- **Move Speed:** 3.0
- **Rotation Speed:** 180
- **Avoidance Radius:** 0.5
- **Avoidance Strength:** 0.3
- **Enemy Layer Mask:** Set to enemy layer
- **Player Tag:** "Player"
- **Player Search Interval:** 1.0
- **Show Debug Info:** True (for testing)

### 3. Layer Setup

#### **Create Enemy Layer**
1. Go to **Edit > Project Settings > Tags and Layers**
2. Create a new layer called "Enemy"
3. Assign your enemy GameObject to the "Enemy" layer
4. Set the **Enemy Layer Mask** in ChaserEnemy to include the "Enemy" layer

#### **Player Tag Setup**
1. Make sure your player GameObject has the "Player" tag
2. If not, select your player GameObject
3. In the Inspector, change the Tag to "Player"

## Configuration Steps

### Step 1: Basic Setup
1. **Create Enemy GameObject**
2. **Add Rigidbody2D** with settings above
3. **Add Collider2D** (CircleCollider2D)
4. **Add ChaserEnemy Script**

### Step 2: Configure ChaserEnemy Script
```
Movement Settings:
- Move Speed: 3.0 (adjust for game balance)
- Rotation Speed: 180 (degrees per second)
- Avoidance Radius: 0.5 (how close enemies stay apart)
- Avoidance Strength: 0.3 (how strong avoidance is)
- Enemy Layer Mask: Enemy layer

Player Detection:
- Player Tag: "Player"
- Player Search Interval: 1.0 (seconds)

Debug:
- Show Debug Info: True (for testing)
```

### Step 3: Layer and Tag Setup
1. **Create "Enemy" layer** in Project Settings
2. **Assign enemy to "Enemy" layer**
3. **Ensure player has "Player" tag**
4. **Set Enemy Layer Mask** to include Enemy layer

### Step 4: Testing
1. **Play the game**
2. **Check console** for "Found player" messages
3. **Enable Show Debug Info** to see enemy state
4. **Adjust movement speed** if needed

## Advanced Configuration

### Multiple Enemies
- Each enemy will automatically avoid others
- Set different **Avoidance Radius** for different enemy types
- Use different **Move Speed** for variety

### Custom Player Detection
- Change **Player Tag** if you use a different tag
- Adjust **Player Search Interval** for performance
- Use **SetPlayer()** method for manual assignment

### Performance Optimization
- **Player Search Interval:** Higher values = better performance
- **Avoidance Radius:** Smaller values = better performance
- **Enemy Layer Mask:** Only include necessary layers

## Troubleshooting

### Enemy Not Moving
- Check if **Rigidbody2D** is set to Dynamic
- Verify **Move Speed** > 0
- Ensure **player** is found (check console)

### Enemy Not Finding Player
- Verify player has **"Player" tag**
- Check **Player Tag** setting in ChaserEnemy
- Enable **Show Debug Info** for messages

### Enemies Clustering
- Increase **Avoidance Radius**
- Increase **Avoidance Strength**
- Check **Enemy Layer Mask** includes enemy layer

### Performance Issues
- Increase **Player Search Interval**
- Reduce **Avoidance Radius**
- Limit number of enemies

## Example Setup

```
GameObject: "ChaserEnemy"
├── Transform
├── Rigidbody2D (Dynamic, Linear Drag: 1, Angular Drag: 1)
├── CircleCollider2D (Radius: 0.5)
├── ChaserEnemy Script
│   ├── Move Speed: 3.0
│   ├── Rotation Speed: 180
│   ├── Avoidance Radius: 0.5
│   ├── Avoidance Strength: 0.3
│   ├── Enemy Layer Mask: Enemy
│   ├── Player Tag: "Player"
│   └── Show Debug Info: True
└── Sprite Renderer (for visual)
```

## Script Features

- **Automatic Player Detection:** Finds player by tag
- **Enemy Avoidance:** Prevents clustering
- **Smooth Rotation:** Faces movement direction
- **Debug Information:** Shows enemy state
- **Manual Player Assignment:** SetPlayer() method
- **Performance Optimized:** Configurable search intervals
