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

        [Header("Drunk Movement Settings")]

        /// <summary>
        /// How much the character wobbles when drunk
        /// </summary>
        [Tooltip("How much the character wobbles when drunk")]
        public float DrunkWobbleAmount = 0.8f;

        /// <summary>
        /// How fast the wobble oscillates
        /// </summary>
        [Tooltip("How fast the wobble oscillates")]
        public float DrunkWobbleSpeed = 7f;

        /// <summary>
        /// How much random direction changes occur when drunk
        /// </summary>
        [Tooltip("How much random direction changes occur when drunk")]
        public float DrunkRandomness = 0.4f;

        /// <summary>
        /// How much the character sways side to side when drunk
        /// </summary>
        [Tooltip("How much the character sways side to side when drunk")]
        public float DrunkSwayAmount = 0.6f;

        /// <summary>
        /// How fast the sway oscillates
        /// </summary>
        [Tooltip("How fast the sway oscillates")]
        public float DrunkSwaySpeed = 4f;

        protected Vector2 _previousMovementDirection = Vector2.zero;
        protected Vector2 _currentMomentum = Vector2.zero;
        protected float _momentumStrength = 0f;
        protected float _horizontalMomentum = 0f;
        protected float _verticalMomentum = 0f;

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

            // Apply momentum system for stages 2 and 3
            if (CurrentStage != MovementStage.Stage1_Normal)
            {
                // Get current velocity components
                float currentHorizontalVelocity = _controller.CurrentMovement.x;
                float currentVerticalVelocity = _controller.CurrentMovement.z;
                
                // Update horizontal momentum
                if (Mathf.Abs(currentHorizontalVelocity) > 0.1f)
                {
                    _horizontalMomentum = Mathf.Clamp01(Mathf.Abs(currentHorizontalVelocity) / MovementSpeed);
                }
                else
                {
                    _horizontalMomentum = Mathf.Lerp(_horizontalMomentum, 0f, GetCurrentStageDeceleration() * Time.deltaTime);
                }
                
                // Update vertical momentum
                if (Mathf.Abs(currentVerticalVelocity) > 0.1f)
                {
                    _verticalMomentum = Mathf.Clamp01(Mathf.Abs(currentVerticalVelocity) / MovementSpeed);
                }
                else
                {
                    _verticalMomentum = Mathf.Lerp(_verticalMomentum, 0f, GetCurrentStageDeceleration() * Time.deltaTime);
                }
                
                // Apply momentum blocking - IMPOSSIBLE to change direction without canceling momentum
                if (_normalizedInput.magnitude > 0.1f)
                {
                    Vector2 currentVelocity = new Vector2(currentHorizontalVelocity, currentVerticalVelocity);
                    float currentSpeed = currentVelocity.magnitude;
                    
                    // Update overall momentum strength
                    _momentumStrength = Mathf.Clamp01(currentSpeed / MovementSpeed);
                    
                    // Only apply momentum blocking if there's significant movement
                    if (currentSpeed > 0.1f)
                    {
                        Vector2 normalizedVelocity = currentVelocity.normalized;
                        float directionDifference = Vector2.Dot(normalizedVelocity, _normalizedInput);
                        
                        // Check if input is opposite to current velocity (canceling momentum)
                        bool isOppositeDirection = directionDifference < -0.3f;
                        
                        // Check if input is same direction as current velocity (continuing momentum)
                        bool isSameDirection = directionDifference > 0.7f;
                        
                        // Check if input is pure vertical or pure horizontal (allow these)
                        bool isPureVertical = Mathf.Abs(_normalizedInput.x) < 0.1f && Mathf.Abs(_normalizedInput.y) > 0.1f;
                        bool isPureHorizontal = Mathf.Abs(_normalizedInput.x) > 0.1f && Mathf.Abs(_normalizedInput.y) < 0.1f;
                        
                        // Check if current movement is pure vertical or pure horizontal
                        bool isCurrentPureVertical = Mathf.Abs(currentVelocity.x) < 0.1f && Mathf.Abs(currentVelocity.y) > 0.1f;
                        bool isCurrentPureHorizontal = Mathf.Abs(currentVelocity.x) > 0.1f && Mathf.Abs(currentVelocity.y) < 0.1f;
                        
                        if (isOppositeDirection)
                        {
                            // Opposite input - allow it to cancel momentum
                            if (ShowDebugInfo)
                            {
                                Debug.Log($"Momentum: Canceling momentum with opposite input. Direction diff: {directionDifference:F2}");
                            }
                        }
                        else if (isSameDirection)
                        {
                            // Same direction - allow it to continue building momentum
                            if (ShowDebugInfo)
                            {
                                Debug.Log($"Momentum: Continuing momentum in same direction. Direction diff: {directionDifference:F2}");
                            }
                        }
                        else if ((isPureVertical && isCurrentPureHorizontal) || (isPureHorizontal && isCurrentPureVertical))
                        {
                            // Pure perpendicular movement (vertical to horizontal or vice versa) - ALLOW
                            if (ShowDebugInfo)
                            {
                                Debug.Log($"Momentum: Allowing pure perpendicular movement. Direction diff: {directionDifference:F2}");
                            }
                        }
                        else
                        {
                            // Diagonal or similar direction - COMPLETELY BLOCK input
                            _normalizedInput = Vector2.zero;
                            
                            if (ShowDebugInfo)
                            {
                                Debug.Log($"Momentum: BLOCKED diagonal direction change. Must cancel momentum first. Direction diff: {directionDifference:F2}");
                            }
                        }
                    }
                }
            }

            // Apply drunk movement effects based on beer level
            if (UseBeerSystem && BeerManager.HasInstance)
            {
                float beerLevel = BeerManager.Instance.CurrentBeer;
                float drunkIntensity = GetDrunkIntensity(beerLevel);
                
                if (drunkIntensity > 0.1f)
                {
                    ApplyDrunkEffects(drunkIntensity);
                }
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
        /// Gets the drunk intensity based on beer level
        /// </summary>
        /// <param name="beerLevel">Current beer level (0-100)</param>
        /// <returns>Drunk intensity (0-1)</returns>
        protected virtual float GetDrunkIntensity(float beerLevel)
        {
            // Higher beer level = more drunk = more intense effects
            // Beer level 0-33% = Zone 1 (sober) = 0 intensity
            // Beer level 34-66% = Zone 2 (tipsy) = 0.3-0.6 intensity  
            // Beer level 67-100% = Zone 3 (drunk) = 0.7-1.0 intensity
            if (beerLevel <= 33f)
            {
                return 0f; // Sober
            }
            else if (beerLevel <= 66f)
            {
                return Mathf.Lerp(0.3f, 0.6f, (beerLevel - 33f) / 33f); // Tipsy
            }
            else
            {
                return Mathf.Lerp(0.7f, 1.0f, (beerLevel - 66f) / 34f); // Drunk
            }
        }

        /// <summary>
        /// Applies drunk movement effects to the input
        /// </summary>
        /// <param name="drunkIntensity">How drunk the character is (0-1)</param>
        protected virtual void ApplyDrunkEffects(float drunkIntensity)
        {
            if (_normalizedInput.magnitude > 0.1f)
            {
                // Add complex wobble effect with multiple frequencies
                float wobbleX = Mathf.Sin(Time.time * DrunkWobbleSpeed) * DrunkWobbleAmount * drunkIntensity;
                wobbleX += Mathf.Sin(Time.time * DrunkWobbleSpeed * 0.7f) * DrunkWobbleAmount * 0.5f * drunkIntensity;
                
                float wobbleY = Mathf.Cos(Time.time * DrunkWobbleSpeed * 1.3f) * DrunkWobbleAmount * drunkIntensity;
                wobbleY += Mathf.Cos(Time.time * DrunkWobbleSpeed * 0.9f) * DrunkWobbleAmount * 0.6f * drunkIntensity;
                
                // Add sway effect with multiple frequencies
                float swayX = Mathf.Sin(Time.time * DrunkSwaySpeed) * DrunkSwayAmount * drunkIntensity;
                swayX += Mathf.Sin(Time.time * DrunkSwaySpeed * 1.5f) * DrunkSwayAmount * 0.4f * drunkIntensity;
                
                float swayY = Mathf.Cos(Time.time * DrunkSwaySpeed * 0.8f) * DrunkSwayAmount * 0.3f * drunkIntensity;
                
                // Add more intense random direction changes
                float randomX = (Random.value - 0.5f) * 2f * DrunkRandomness * drunkIntensity;
                float randomY = (Random.value - 0.5f) * 2f * DrunkRandomness * drunkIntensity;
                
                // Add additional random wobble for more chaotic movement
                float chaosX = (Random.value - 0.5f) * 2f * DrunkRandomness * 0.5f * drunkIntensity;
                float chaosY = (Random.value - 0.5f) * 2f * DrunkRandomness * 0.5f * drunkIntensity;
                
                // Apply all effects to the input
                Vector2 drunkOffset = new Vector2(
                    wobbleX + swayX + randomX + chaosX,
                    wobbleY + swayY + randomY + chaosY
                );
                
                _normalizedInput += drunkOffset;
                _normalizedInput = Vector2.ClampMagnitude(_normalizedInput, 1f);
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"Drunk Effects: Intensity {drunkIntensity:F2}, Offset {drunkOffset}");
                }
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
                GUI.Label(new Rect(10, 50, 300, 20), $"Momentum Resistance: {GetMomentumResistance():F2}");
                GUI.Label(new Rect(10, 70, 300, 20), $"Drift Influence: {GetMomentumResistance() * DriftInfluence:F2}");
                GUI.Label(new Rect(10, 90, 300, 20), $"Current Speed: {_controller.CurrentMovement.magnitude:F2}");
                GUI.Label(new Rect(10, 110, 300, 20), $"Momentum Strength: {_momentumStrength:F2}");
                
                if (BeerManager.HasInstance)
                {
                    float drunkIntensity = GetDrunkIntensity(BeerManager.Instance.CurrentBeer);
                    GUI.Label(new Rect(10, 130, 300, 20), $"Drunk Intensity: {drunkIntensity:F2}");
                }
                
                GUI.Label(new Rect(10, 150, 300, 20), "Press 1, 2, or 3 to switch stages");
                
                if (BeerManager.HasInstance)
                {
                    GUI.Label(new Rect(10, 170, 300, 20), $"Beer Level: {BeerManager.Instance.CurrentBeer:F1}%");
                    GUI.Label(new Rect(10, 190, 300, 20), $"Beer Zone: {BeerManager.Instance.CurrentZone}");
                }
            }
        }
    }
}
