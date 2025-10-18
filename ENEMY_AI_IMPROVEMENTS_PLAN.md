# Enemy AI Improvements Plan

## Overview
Make enemy combat more dynamic, less predictable, and more accurate with their attacks.

## Key Improvements

### 1. Dynamic Movement Patterns
**Current**: Enemies always orbit at same distance
**Improved**: 
- Vary orbit distance dynamically (closer when aggressive, farther when cautious)
- Add strafing patterns (move perpendicular to player occasionally)
- Random orbit direction changes
- Feint movements (fake charges to bait player)

### 2. Attack Prediction & Accuracy
**Current**: Enemies charge/attack at player's current position
**Improved**:
- Predict player movement direction and speed
- Lead the target (aim where player will be, not where they are)
- Higher accuracy on charge attacks
- Adjust attack timing based on player movement patterns

### 3. Unpredictable Attack Timing
**Current**: Fixed cooldowns make attacks predictable
**Improved**:
- Variable charge cooldown (randomize within a range)
- Mix up attack patterns (quick attack, delayed attack, fake-out)
- Don't telegraph every charge (some surprise attacks)
- Vary telegraph duration

### 4. Smart Positioning
**Current**: Enemies orbit randomly around player
**Improved**:
- Try to attack from player's blind spots
- Position behind player when possible
- Spread out better (don't stack on same side)
- Cut off escape routes

## Implementation Details

### File: `Assets/TopDownEngine/Common/Scripts/Enemies/ChaserEnemy.cs`

#### New Fields to Add:
```csharp
[Header("AI Intelligence Settings")]
[Tooltip("Enable movement prediction for more accurate attacks")]
[SerializeField] private bool usePredictiveAttacks = true;

[Tooltip("How far ahead to predict player position (seconds)")]
[SerializeField] private float predictionTime = 0.3f;

[Tooltip("Randomize charge cooldown for unpredictability")]
[SerializeField] private Vector2 chargeCooldownRange = new Vector2(4f, 6f);

[Tooltip("Chance to do a surprise attack (no telegraph)")]
[SerializeField] private float surpriseAttackChance = 0.2f;

[Tooltip("Orbit distance variation for dynamic movement")]
[SerializeField] private float orbitDistanceVariation = 1f;

[Tooltip("Chance to change orbit direction")]
[SerializeField] private float directionChangeChance = 0.3f;

[Tooltip("Enable strafing movement patterns")]
[SerializeField] private bool enableStrafing = true;

[Tooltip("Strafing frequency")]
[SerializeField] private float strafingInterval = 2f;
```

#### Methods to Add/Modify:

1. **PredictPlayerPosition()** - Predict where player will be
2. **CalculateDynamicOrbitDistance()** - Vary orbit distance
3. **ShouldChangeOrbitDirection()** - Random direction changes
4. **CalculateStrafeMovement()** - Add perpendicular movement
5. **GetRandomizedChargeCooldown()** - Variable attack timing
6. **ShouldDoSurpriseAttack()** - Occasional no-telegraph charges
7. **GetBestAttackAngle()** - Attack from behind when possible

#### Specific Changes:

**Change 1: Predictive Charge Targeting**
```csharp
// In StartChargeTelegraph()
if (usePredictiveAttacks && player != null)
{
    // Get player velocity
    Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
    if (playerRb != null)
    {
        Vector2 playerVelocity = playerRb.linearVelocity;
        Vector2 predictedPosition = (Vector2)player.position + (playerVelocity * predictionTime);
        _chargeTargetPosition = predictedPosition;
    }
    else
    {
        _chargeTargetPosition = player.position;
    }
}
```

**Change 2: Variable Charge Cooldown**
```csharp
// In UpdateAttackState()
float dynamicChargeCooldown = Random.Range(chargeCooldownRange.x, chargeCooldownRange.y);
bool canCharge = Time.time - _lastChargeTime > dynamicChargeCooldown;
```

**Change 3: Surprise Attacks**
```csharp
// In UpdateAttackState()
if (distanceToPlayer <= enemy_Charge_Radius && canCharge)
{
    bool doSurpriseAttack = Random.value < surpriseAttackChance;
    if (doSurpriseAttack)
    {
        // Skip telegraph, charge immediately
        _isCharging = true;
        _chargeStartTime = Time.time;
    }
    else
    {
        // Normal telegraph
        _currentAttackState = AttackState.TelegraphingCharge;
        StartChargeTelegraph();
    }
}
```

**Change 4: Dynamic Orbit Distance**
```csharp
// In CalculateOrbitPosition()
float dynamicOrbitDistance = flank_Distance + Random.Range(-orbitDistanceVariation, orbitDistanceVariation);
_targetOrbitPosition = currentPlayerPosition + orbitDirection * dynamicOrbitDistance + randomOffset;
```

**Change 5: Strafing Movement**
```csharp
// In CalculateMovement()
if (enableStrafing && Time.time - _lastStrafeTime > strafingInterval)
{
    if (Random.value < 0.5f)
    {
        // Add perpendicular movement
        Vector2 perpendicular = Vector2.Perpendicular(directionToOrbit);
        _movement += perpendicular * 0.3f;
        _lastStrafeTime = Time.time;
    }
}
```

**Change 6: Attack from Behind**
```csharp
// In CalculateOrbitPosition()
// Try to position behind player
Vector2 playerFacing = GetPlayerFacingDirection();
if (playerFacing != Vector2.zero)
{
    // Bias orbit angle to be behind player
    Vector2 behindPlayer = -playerFacing;
    float angleToPrefer = Mathf.Atan2(behindPlayer.y, behindPlayer.x);
    _orbitAngle = Mathf.LerpAngle(_orbitAngle, angleToPrefer, 0.3f);
}
```

### File: `Assets/Enemy.prefab`

**New Values**:
```yaml
usePredictiveAttacks: 1
predictionTime: 0.3
chargeCooldownRange: {x: 3.5, y: 6}
surpriseAttackChance: 0.15
orbitDistanceVariation: 0.8
directionChangeChance: 0.25
enableStrafing: 1
strafingInterval: 2.5
```

## Expected Results

1. **More Dynamic Combat**:
   - Enemies move unpredictably with varying patterns
   - Combat feels fresh each encounter
   - Less "robotic" behavior

2. **Higher Accuracy**:
   - Charges land more often due to prediction
   - Enemies lead moving targets
   - Better hit rate on attacks

3. **Less Predictable**:
   - Variable cooldowns mean player can't time attacks
   - Surprise charges keep player on edge
   - Dynamic positioning prevents camping

4. **Smarter Enemies**:
   - Attack from behind when possible
   - Better spacing between enemies
   - More tactical movement patterns

## Testing Checklist

- [ ] Enemies predict player movement accurately
- [ ] Charge attacks have higher success rate
- [ ] Attack timing feels less predictable
- [ ] Enemies vary orbit distance dynamically
- [ ] Strafing movement looks natural
- [ ] Surprise attacks happen occasionally
- [ ] Enemies position themselves better
- [ ] Combat feels more challenging and engaging

