using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Extended CharacterMovement with 3-stage movement system
    /// Stage 1: Normal movement with standard deceleration
    /// Stage 2: Slower deceleration with slight drift
    /// Stage 3: Very slow deceleration with high drift and hard control
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Abilities/Character Movement Staged")]
    public class CharacterMovementStaged : CharacterMovement
    {
        /// <summary>
        /// The different movement stages available
        /// </summary>
        public enum MovementStage
        {
            Stage1_Normal = 1,
            Stage2_Drift = 2,
            Stage3_HighDrift = 3
        }

        [Header("Staged Movement Settings")]
        
        /// <summary>
        /// The current movement stage
        /// </summary>
        [Tooltip("The current movement stage")]
        public MovementStage CurrentStage = MovementStage.Stage1_Normal;

        /// <summary>
        /// Deceleration value for Stage 1 (Normal)
        /// </summary>
        [Tooltip("Deceleration value for Stage 1 (Normal)")]
        public float Stage1Deceleration = 10f;

        /// <summary>
        /// Deceleration value for Stage 2 (Drift)
        /// </summary>
        [Tooltip("Deceleration value for Stage 2 (Drift)")]
        public float Stage2Deceleration = 4f;

        /// <summary>
        /// Deceleration value for Stage 3 (High Drift)
        /// </summary>
        [Tooltip("Deceleration value for Stage 3 (High Drift)")]
        public float Stage3Deceleration = 1.5f;

        /// <summary>
        /// Whether to show debug information about current stage
        /// </summary>
        [Tooltip("Whether to show debug information about current stage")]
        public bool ShowDebugInfo = false;

        /// <summary>
        /// Whether to use the beer system for automatic stage switching
        /// </summary>
        [Tooltip("Whether to use the beer system for automatic stage switching")]
        public bool UseBeerSystem = true;

        [Header("Momentum Settings")]

        /// <summary>
        /// Momentum resistance for Stage 2 (0 = no resistance, 1 = full resistance)
        /// </summary>
        [Tooltip("Momentum resistance for Stage 2 (0 = no resistance, 1 = full resistance)")]
        public float Stage2MomentumResistance = 0.5f;

        /// <summary>
        /// Momentum resistance for Stage 3 (0 = no resistance, 1 = full resistance)
        /// </summary>
        [Tooltip("Momentum resistance for Stage 3 (0 = no resistance, 1 = full resistance)")]
        public float Stage3MomentumResistance = 0.8f;

        /// <summary>
        /// How quickly the character can turn when momentum is high
        /// </summary>
        [Tooltip("How quickly the character can turn when momentum is high")]
        public float DirectionChangeDamping = 0.3f;

        /// <summary>
        /// How much drift affects direction changes while moving
        /// </summary>
        [Tooltip("How much drift affects direction changes while moving")]
        public float DriftInfluence = 0.5f;


        protected Vector2 _previousMovementDirection = Vector2.zero;
        protected Vector2 _currentMomentum = Vector2.zero;
        protected float _momentumStrength = 0f;
        protected float _horizontalMomentum = 0f;
        protected float _verticalMomentum = 0f;
        
        [Header("Car-Like Momentum Settings")]
        
        /// <summary>
        /// How much momentum blocks direction changes in Stage 2 (like a car)
        /// </summary>
        [Tooltip("How much momentum blocks direction changes in Stage 2 (like a car)")]
        public float Stage2CarMomentum = 0.8f;
        
        /// <summary>
        /// How much momentum blocks direction changes in Stage 3 (like a car)
        /// </summary>
        [Tooltip("How much momentum blocks direction changes in Stage 3 (like a car)")]
        public float Stage3CarMomentum = 0.95f;
        
        /// <summary>
        /// Minimum speed threshold before momentum blocking kicks in
        /// </summary>
        [Tooltip("Minimum speed threshold before momentum blocking kicks in")]
        public float MomentumThreshold = 0.2f;
        
        /// <summary>
        /// How fast momentum builds up when moving
        /// </summary>
        [Tooltip("How fast momentum builds up when moving")]
        public float MomentumBuildRate = 3f;
        
        /// <summary>
        /// How fast momentum decays when not moving
        /// </summary>
        [Tooltip("How fast momentum decays when not moving")]
        public float MomentumDecayRate = 200f;
        
        protected Vector2 _carMomentum = Vector2.zero;
        protected Vector2 _lastVelocity = Vector2.zero;
        protected Vector2 _currentMomentumDirection = Vector2.zero;

        [Header("Drunk Wobble Settings")]

        /// <summary>
        /// Enable drunk wobble effects in Stage 3
        /// </summary>
        [Tooltip("Enable drunk wobble effects in Stage 3")]
        public bool EnableDrunkWobble = true;

        /// <summary>
        /// How much the input wobbles when drunk (0-1)
        /// </summary>
        [Tooltip("How much the input wobbles when drunk (0-1)")]
        public float DrunkWobbleAmount = 0.15f;

        /// <summary>
        /// Primary wobble frequency (Hz)
        /// </summary>
        [Tooltip("Primary wobble frequency (Hz)")]
        public float DrunkWobbleSpeed = 3f;

        /// <summary>
        /// Secondary wobble frequency for complexity (Hz)
        /// </summary>
        [Tooltip("Secondary wobble frequency for complexity (Hz)")]
        public float DrunkWobbleSpeed2 = 5f;

        protected override void Initialization()
        {
            base.Initialization();
            
            // Subscribe to beer zone change events if beer system is enabled
            if (UseBeerSystem)
            {
                BeerManager.OnBeerZoneChanged += OnBeerZoneChanged;
                
                // Set initial stage based on current beer zone if BeerManager exists
                if (BeerManager.HasInstance)
                {
                    int currentZone = BeerManager.Instance.CurrentZone;
                    MovementStage initialStage = GetMovementStageFromBeerZone(currentZone);
                    SetMovementStage(initialStage);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            // Unsubscribe from events
            BeerManager.OnBeerZoneChanged -= OnBeerZoneChanged;
        }

        /// <summary>
        /// Handle input for stage switching and movement
        /// </summary>
        protected override void HandleInput()
        {
            base.HandleInput();

            // Only allow manual stage switching if beer system is disabled
            if (!UseBeerSystem)
            {
                // Handle stage switching with number keys
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    SetMovementStage(MovementStage.Stage1_Normal);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    SetMovementStage(MovementStage.Stage2_Drift);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    SetMovementStage(MovementStage.Stage3_HighDrift);
                }
            }
        }

        /// <summary>
        /// Sets the movement stage and updates deceleration accordingly
        /// </summary>
        /// <param name="newStage">The new movement stage</param>
        public virtual void SetMovementStage(MovementStage newStage)
        {
            CurrentStage = newStage;
            
            // Update the deceleration value based on the current stage
            switch (CurrentStage)
            {
                case MovementStage.Stage1_Normal:
                    Deceleration = Stage1Deceleration;
                    break;
                case MovementStage.Stage2_Drift:
                    Deceleration = Stage2Deceleration;
                    break;
                case MovementStage.Stage3_HighDrift:
                    Deceleration = Stage3Deceleration;
                    break;
            }

            if (ShowDebugInfo)
            {
                Debug.Log($"Movement Stage changed to: {CurrentStage} (Deceleration: {Deceleration})");
            }
        }

        /// <summary>
        /// Override SetMovement to apply stage-specific deceleration
        /// </summary>
        protected override void SetMovement()
        {
            _movementVector = Vector3.zero;
            _currentInput = Vector2.zero;

            _currentInput.x = _horizontalMovement;
            _currentInput.y = _verticalMovement;
            
            _normalizedInput = _currentInput.normalized;


            // Apply car-like momentum system for stages 2 and 3
            if (CurrentStage != MovementStage.Stage1_Normal)
            {
                ApplyCarLikeMomentum();
            }

            // Apply drunk wobble effects in Stage 3 only (after momentum system)
            if (EnableDrunkWobble && CurrentStage == MovementStage.Stage3_HighDrift)
            {
                ApplyDrunkWobble();
            }

            float interpolationSpeed = 1f;
            
            if ((Acceleration == 0) || (Deceleration == 0))
            {
                _lerpedInput = AnalogInput ? _currentInput : _normalizedInput;
            }
            else
            {
                if (_normalizedInput.magnitude == 0)
                {
                    // Apply stage-specific deceleration
                    float currentDeceleration = GetCurrentStageDeceleration();
                    _acceleration = Mathf.Lerp(_acceleration, 0f, currentDeceleration * Time.deltaTime);
                    _lerpedInput = Vector2.Lerp(_lerpedInput, _lerpedInput * _acceleration, Time.deltaTime * currentDeceleration);
                    interpolationSpeed = currentDeceleration;
                }
                else
                {
                    _acceleration = Mathf.Lerp(_acceleration, 1f, Acceleration * Time.deltaTime);
                    _lerpedInput = AnalogInput ? Vector2.ClampMagnitude (_currentInput, _acceleration) : Vector2.ClampMagnitude(_normalizedInput, _acceleration);
                    interpolationSpeed = Acceleration;
                }
            }		
            
            _movementVector.x = _lerpedInput.x;
            _movementVector.y = 0f;
            _movementVector.z = _lerpedInput.y;

            if (InterpolateMovementSpeed)
            {
                _movementSpeed = Mathf.Lerp(_movementSpeed, MovementSpeed * ContextSpeedMultiplier * MovementSpeedMultiplier, interpolationSpeed * Time.deltaTime);
            }
            else
            {
                _movementSpeed = MovementSpeed * MovementSpeedMultiplier * ContextSpeedMultiplier;
            }

            _movementVector *= _movementSpeed;

            if (_movementVector.magnitude > MovementSpeed * ContextSpeedMultiplier * MovementSpeedMultiplier)
            {
                _movementVector = Vector3.ClampMagnitude(_movementVector, MovementSpeed);
            }

            if ((_currentInput.magnitude <= IdleThreshold) && (_controller.CurrentMovement.magnitude < IdleThreshold))
            {
                _controller.SetMovement(Vector3.zero);
            }
            else
            {
                _controller.SetMovement(_movementVector);
            }
        }

        /// <summary>
        /// Gets the deceleration value for the current stage
        /// </summary>
        /// <returns>The deceleration value for the current stage</returns>
        protected virtual float GetCurrentStageDeceleration()
        {
            switch (CurrentStage)
            {
                case MovementStage.Stage1_Normal:
                    return Stage1Deceleration;
                case MovementStage.Stage2_Drift:
                    return Stage2Deceleration;
                case MovementStage.Stage3_HighDrift:
                    return Stage3Deceleration;
                default:
                    return Stage1Deceleration;
            }
        }

        /// <summary>
        /// Gets the momentum resistance value for the current stage
        /// </summary>
        /// <returns>The momentum resistance value for the current stage</returns>
        protected virtual float GetMomentumResistance()
        {
            switch (CurrentStage)
            {
                case MovementStage.Stage1_Normal:
                    return 0f; // No momentum resistance in stage 1
                case MovementStage.Stage2_Drift:
                    return Stage2MomentumResistance;
                case MovementStage.Stage3_HighDrift:
                    return Stage3MomentumResistance;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Applies car-like momentum system - character must stop before changing direction
        /// Works for all 8 directions (including diagonals)
        /// </summary>
        protected virtual void ApplyCarLikeMomentum()
        {
            // Get current velocity as a 2D vector (2D top-down game uses Y for vertical)
            Vector2 currentVelocity = new Vector2(_controller.CurrentMovement.x, _controller.CurrentMovement.y);
            float currentSpeed = currentVelocity.magnitude;
            
            // Check if player is giving opposite input (braking)
            bool isBraking = _normalizedInput.magnitude > 0.1f && 
                _currentMomentumDirection.magnitude > 0.1f && 
                Vector2.Dot(_normalizedInput.normalized, _currentMomentumDirection.normalized) < -0.3f;
            
            if (isBraking)
            {
                // Player is braking - decay momentum immediately and continuously
                // Stage 3 (very drunk) gets faster control return
                float decayMultiplier = (CurrentStage == MovementStage.Stage3_HighDrift) ? 3f : 1f;
                _currentMomentumDirection = Vector2.Lerp(_currentMomentumDirection, Vector2.zero, 
                    MomentumDecayRate * decayMultiplier * Time.deltaTime);
                
                if (ShowDebugInfo && CurrentStage == MovementStage.Stage3_HighDrift)
                {
                    Debug.Log($"Stage 3 Fast Decay: Multiplier {decayMultiplier}x, Effective Rate: {MomentumDecayRate * decayMultiplier}");
                }
            }
            else if (currentSpeed > MomentumThreshold)
            {
                Vector2 currentDirection = currentVelocity.normalized;
                
                // Build up momentum in the current direction
                _currentMomentumDirection = Vector2.Lerp(_currentMomentumDirection, currentDirection, 
                    MomentumBuildRate * Time.deltaTime);
                
                _lastVelocity = currentVelocity;
            }
            else if (currentSpeed < 0.05f)
            {
                // Only decay when completely stopped and not braking
                _currentMomentumDirection = Vector2.Lerp(_currentMomentumDirection, Vector2.zero, 
                    MomentumDecayRate * Time.deltaTime);
            }
            
            // Apply car-like momentum blocking for all directions
            if (_currentMomentumDirection.magnitude > 0.1f && _normalizedInput.magnitude > 0.1f)
            {
                Vector2 inputDirection = _normalizedInput.normalized;
                Vector2 momentumDirection = _currentMomentumDirection.normalized;
                
                float directionSimilarity = Vector2.Dot(inputDirection, momentumDirection);
                
                if (directionSimilarity > 0.7f)
                {
                    // Same direction - allow it to continue
                    if (ShowDebugInfo)
                    {
                        Debug.Log($"Car Momentum: Continuing in same direction. Similarity: {directionSimilarity:F2}");
                    }
                }
                else if (directionSimilarity < -0.3f)
                {
                     // Opposite direction - apply strong resistance but allow some movement
                     float momentumStrength = _currentMomentumDirection.magnitude;
                     float resistance = Mathf.Lerp(0.7f, 0.9f, momentumStrength);
                     _normalizedInput *= (1f - resistance);
                     
                     // Decay momentum when braking
                     _currentMomentumDirection = Vector2.Lerp(_currentMomentumDirection, Vector2.zero, 
                         MomentumDecayRate * Time.deltaTime);
                     
                     if (ShowDebugInfo)
                     {
                         Debug.Log($"Car Momentum: Braking with resistance {(1f - resistance):P0}. Similarity: {directionSimilarity:F2}");
                     }
                }
                else
                {
                     // Different direction - apply sliding resistance
                     float momentumStrength = _currentMomentumDirection.magnitude;
                     float resistance = Mathf.Lerp(0.5f, 0.8f, momentumStrength);
                     _normalizedInput *= (1f - resistance);
                     
                     // Slow momentum decay for sliding
                     _currentMomentumDirection = Vector2.Lerp(_currentMomentumDirection, Vector2.zero, 
                         MomentumDecayRate * 0.3f * Time.deltaTime);
                     
                     if (ShowDebugInfo)
                     {
                         Debug.Log($"Car Momentum: Sliding with resistance {(1f - resistance):P0}. Similarity: {directionSimilarity:F2}");
                     }
                }
            }
            else if (_currentMomentumDirection.magnitude > 0.1f)
            {
                // No input but momentum exists - let it decay naturally
                _currentMomentumDirection *= 0.95f; // 5% decay per frame
                
                if (_currentMomentumDirection.magnitude < 0.1f)
                {
                    _currentMomentumDirection = Vector2.zero;
                    if (ShowDebugInfo)
                    {
                        Debug.Log("Car Momentum: Natural decay completed - momentum zeroed");
                    }
                }
            }
        }

        /// <summary>
        /// Applies drunk wobble effects to movement input (Stage 3 only)
        /// Uses pure sinusoidal oscillation for smooth, predictable wobble
        /// </summary>
        protected virtual void ApplyDrunkWobble()
        {
            if (_normalizedInput.magnitude < 0.1f)
            {
                // No wobble when not moving
                return;
            }
            
            // Pure horizontal sinusoidal wobble only (no vertical movement)
            float wobbleX = Mathf.Sin(Time.time * DrunkWobbleSpeed) * DrunkWobbleAmount;
            wobbleX += Mathf.Sin(Time.time * DrunkWobbleSpeed2 * 0.7f) * DrunkWobbleAmount * 0.5f;
            
            // No vertical wobble - only horizontal sway
            float wobbleY = 0f;
            
            Vector2 wobbleOffset = new Vector2(wobbleX, wobbleY);
            
            // Apply wobble to normalized input
            _normalizedInput += wobbleOffset;
            _normalizedInput = Vector2.ClampMagnitude(_normalizedInput, 1f);
            
            if (ShowDebugInfo)
            {
                Debug.Log($"Drunk Wobble Applied: Offset {wobbleOffset}, Final Input: {_normalizedInput}");
            }
        }

        /// <summary>
        /// Gets the car momentum strength for the current stage
        /// </summary>
        /// <returns>The car momentum strength for the current stage</returns>
        protected virtual float GetCarMomentumStrength()
        {
            switch (CurrentStage)
            {
                case MovementStage.Stage1_Normal:
                    return 0f; // No car momentum in stage 1
                case MovementStage.Stage2_Drift:
                    return Stage2CarMomentum;
                case MovementStage.Stage3_HighDrift:
                    return Stage3CarMomentum;
                default:
                    return 0f;
            }
        }



        /// <summary>
        /// Handles beer zone change events from the BeerManager
        /// </summary>
        /// <param name="zoneChangeEvent">The beer zone change event</param>
        protected virtual void OnBeerZoneChanged(BeerZoneChangeEvent zoneChangeEvent)
        {
            if (!UseBeerSystem)
            {
                return;
            }

            // Convert beer zone to movement stage
            MovementStage newStage = GetMovementStageFromBeerZone(zoneChangeEvent.NewZone);
            SetMovementStage(newStage);

            if (ShowDebugInfo)
            {
                Debug.Log($"Beer zone changed to {zoneChangeEvent.NewZone}, switching to movement stage {newStage}");
            }
        }

        /// <summary>
        /// Converts beer zone to movement stage
        /// </summary>
        /// <param name="beerZone">The beer zone (1, 2, or 3)</param>
        /// <returns>The corresponding movement stage</returns>
        protected virtual MovementStage GetMovementStageFromBeerZone(int beerZone)
        {
            switch (beerZone)
            {
                case 1:
                    return MovementStage.Stage1_Normal;    // Zone 1 (0-33% drunk) → Stage 1 (Normal - precise control)
                case 2:
                    return MovementStage.Stage2_Drift;     // Zone 2 (34-66% tipsy) → Stage 2 (Drift - slight drift)
                case 3:
                    return MovementStage.Stage3_HighDrift; // Zone 3 (67-100% sober) → Stage 3 (High Drift - hard to control)
                default:
                    return MovementStage.Stage1_Normal;
            }
        }

        /// <summary>
        /// Context menu method to test movement stage 1
        /// </summary>
        [ContextMenu("Test Stage 1 (Normal)")]
        protected virtual void TestStage1()
        {
            SetMovementStage(MovementStage.Stage1_Normal);
        }

        /// <summary>
        /// Context menu method to test movement stage 2
        /// </summary>
        [ContextMenu("Test Stage 2 (Drift)")]
        protected virtual void TestStage2()
        {
            SetMovementStage(MovementStage.Stage2_Drift);
        }

        /// <summary>
        /// Context menu method to test movement stage 3
        /// </summary>
        [ContextMenu("Test Stage 3 (High Drift)")]
        protected virtual void TestStage3()
        {
            SetMovementStage(MovementStage.Stage3_HighDrift);
        }

        /// <summary>
        /// Context menu method to refresh from BeerManager
        /// </summary>
        [ContextMenu("Refresh from BeerManager")]
        public virtual void RefreshFromBeerManager()
        {
            if (BeerManager.HasInstance)
            {
                int currentZone = BeerManager.Instance.CurrentZone;
                MovementStage newStage = GetMovementStageFromBeerZone(currentZone);
                SetMovementStage(newStage);
                Debug.Log($"Movement stage refreshed from BeerManager: Zone {currentZone} -> Stage {newStage}");
            }
            else
            {
                Debug.LogWarning("BeerManager not found! Cannot refresh movement stage.");
            }
        }

        /// <summary>
        /// OnGUI for debug information display
        /// </summary>
        protected virtual void OnGUI()
        {
            if (ShowDebugInfo)
            {
                GUI.Label(new Rect(10, 10, 300, 20), $"Movement Stage: {CurrentStage}");
                GUI.Label(new Rect(10, 30, 300, 20), $"Current Deceleration: {GetCurrentStageDeceleration()}");
                GUI.Label(new Rect(10, 50, 300, 20), $"Car Momentum Strength: {GetCarMomentumStrength():F2}");
                GUI.Label(new Rect(10, 70, 300, 20), $"Momentum Direction: {_currentMomentumDirection}");
                GUI.Label(new Rect(10, 90, 300, 20), $"Momentum Magnitude: {_currentMomentumDirection.magnitude:F2}");
                GUI.Label(new Rect(10, 110, 300, 20), $"Current Speed: {_controller.CurrentMovement.magnitude:F2}");
                GUI.Label(new Rect(10, 130, 300, 20), $"Momentum Threshold: {MomentumThreshold:F2}");
                
                if (EnableDrunkWobble && CurrentStage == MovementStage.Stage3_HighDrift)
                {
                    GUI.Label(new Rect(10, 150, 300, 20), $"Drunk Wobble: ACTIVE");
                }
                
                GUI.Label(new Rect(10, 170, 300, 20), "Press 1, 2, or 3 to switch stages");
                
                if (BeerManager.HasInstance)
                {
                    GUI.Label(new Rect(10, 190, 300, 20), $"Beer Level: {BeerManager.Instance.CurrentBeer:F1}%");
                    GUI.Label(new Rect(10, 210, 300, 20), $"Beer Zone: {BeerManager.Instance.CurrentZone}");
                }
            }
        }
    }
}
