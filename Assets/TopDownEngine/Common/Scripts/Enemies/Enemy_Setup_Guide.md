# Enemy Setup Guide - ChaserEnemy

## Overview
This guide explains how to create a player-following enemy using the `ChaserEnemy` script. The enemy will chase the player while avoiding other enemies.

## Required Components

### 1. GameObject Setup
Create a new GameObject and name it "ChaserEnemy" (or any name you prefer).

### 2. Essential Components

#### **Rigidbody2D**
- **Purpose**: Physics-based movement
- **Settings**:
  - `Body Type`: Dynamic
  - `Gravity Scale`: 0 (for top-down games)
  - `Linear Drag`: 0-2 (optional, for smoother movement)
  - `Angular Drag`: 0-2 (optional, for smoother rotation)

#### **Collider2D**
- **Purpose**: Collision detection and enemy avoidance
- **Options**:
  - `CircleCollider2D` (recommended for simple enemies)
  - `BoxCollider2D` (for rectangular enemies)
  - `CapsuleCollider2D` (for humanoid enemies)
- **Settings**:
  - `Is Trigger`: False (for solid collision)
  - `Size`: Adjust to match enemy sprite

#### **SpriteRenderer**
- **Purpose**: Visual representation
- **Settings**:
  - `Sprite`: Assign your enemy sprite
  - `Sorting Layer`: Set appropriate layer
  - `Order in Layer`: Set appropriate order

### 3. Script Configuration

#### **ChaserEnemy Script Settings**:

**Movement Settings:**
- `Move Speed`: 3f (how fast the enemy moves)
- `Rotation Speed`: 180f (how fast it rotates to face movement direction)
- `Avoidance Radius`: 0.5f (radius for avoiding other enemies)
- `Avoidance Strength`: 0.3f (how strong the avoidance force is)
- `Enemy Layer Mask`: Set to enemy layer

**References:**
- `Player`: Will be auto-assigned via FindGameObjectWithTag("Player")
- `Base Enemy Transform`: Assign the main transform (usually this GameObject's transform)
- `Rb`: Assign the Rigidbody2D component

## Step-by-Step Setup

### Step 1: Create the Enemy GameObject
1. Right-click in Hierarchy → Create Empty
2. Name it "ChaserEnemy"
3. Set position to desired spawn location

### Step 2: Add Required Components
1. **Add Rigidbody2D**:
   - Select the GameObject
   - Add Component → Physics 2D → Rigidbody 2D
   - Set Body Type to Dynamic
   - Set Gravity Scale to 0

2. **Add Collider2D**:
   - Add Component → Physics 2D → Circle Collider 2D
   - Adjust radius to match enemy size
   - Ensure Is Trigger is False

3. **Add SpriteRenderer**:
   - Add Component → Rendering → Sprite Renderer
   - Assign your enemy sprite
   - Set appropriate sorting layer

### Step 3: Add the ChaserEnemy Script
1. Add Component → Scripts → Enemy → ChaserEnemy
2. Configure the settings:

**Movement Settings:**
- Move Speed: 3f
- Rotation Speed: 180f
- Avoidance Radius: 0.5f
- Avoidance Strength: 0.3f
- Enemy Layer Mask: Create a new layer called "Enemy" and assign it

**References:**
- Base Enemy Transform: Drag the GameObject's transform here
- Rb: Drag the Rigidbody2D component here
- Player: Will be auto-assigned (make sure your player has "Player" tag)

### Step 4: Set Up Layers and Tags

#### **Create Enemy Layer:**
1. Go to Edit → Project Settings → Tags and Layers
2. Create a new layer called "Enemy"
3. Assign this layer to your enemy GameObject

#### **Ensure Player Has "Player" Tag:**
1. Select your player GameObject
2. In the Inspector, set Tag to "Player"
3. If "Player" tag doesn't exist, create it in Tags and Layers

### Step 5: Optional Enhancements

#### **Add Visual Feedback:**
- Add a child GameObject for the enemy sprite
- Use the child's transform for rotation (baseEnemyTransform)
- This allows the sprite to rotate while keeping collision centered

#### **Add Health System:**
- Create a Health component
- Add it to the enemy GameObject
- Connect it to the ChaserEnemy script

#### **Add Attack System:**
- Create an Attack component
- Add it to the enemy GameObject
- Trigger attacks when close to player

## Advanced Configuration

### **Avoidance System:**
The enemy uses a sophisticated avoidance system:
- Detects other enemies within `avoidanceRadius`
- Calculates avoidance force based on distance
- Combines player direction with avoidance force
- Prevents enemies from clustering together

### **Movement System:**
- Uses `Rigidbody2D.linearVelocity` for smooth movement
- Rotates to face movement direction
- Combines player-seeking with enemy avoidance

### **Performance Optimization:**
- Uses object pooling for enemy hits array
- Efficient collision detection with ContactFilter2D
- Minimal Update() calls with cached references

## Troubleshooting

### **Enemy Not Moving:**
- Check if player has "Player" tag
- Verify Rigidbody2D is set to Dynamic
- Check if moveSpeed is greater than 0
- Ensure player reference is assigned

### **Enemy Not Rotating:**
- Verify baseEnemyTransform is assigned
- Check if rotationSpeed is greater than 0
- Ensure movement is being calculated

### **Enemy Clustering:**
- Increase avoidanceRadius
- Increase avoidanceStrength
- Check if enemy layer mask is set correctly

### **Performance Issues:**
- Reduce avoidanceRadius
- Limit number of enemies
- Use object pooling for enemy spawning

## Example Usage

```csharp
// Spawn enemy at runtime
GameObject enemyPrefab = Resources.Load<GameObject>("ChaserEnemy");
GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

// Configure enemy settings
ChaserEnemy chaser = newEnemy.GetComponent<ChaserEnemy>();
// Settings are configured in the Inspector
```

## Best Practices

1. **Use Prefabs**: Create a prefab of your configured enemy for easy spawning
2. **Layer Management**: Keep enemies on their own layer for collision detection
3. **Performance**: Limit the number of active enemies to maintain good performance
4. **Balancing**: Adjust moveSpeed and avoidanceStrength for desired difficulty
5. **Visual Polish**: Add particle effects, animations, or sound effects for better gameplay

## Integration with TopDown Engine

This enemy script is designed to work with the TopDown Engine:
- Uses 2D physics for movement
- Compatible with the engine's collision system
- Can be extended with engine-specific features
- Works with the engine's layer and tag system
