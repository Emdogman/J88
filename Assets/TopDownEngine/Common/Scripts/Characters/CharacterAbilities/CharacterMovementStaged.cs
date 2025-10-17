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
                GUI.Label(new Rect(10, 50, 300, 20), "Press 1, 2, or 3 to switch stages");
                
                if (BeerManager.HasInstance)
                {
                    GUI.Label(new Rect(10, 70, 300, 20), $"Beer Level: {BeerManager.Instance.CurrentBeer:F1}%");
                    GUI.Label(new Rect(10, 90, 300, 20), $"Beer Zone: {BeerManager.Instance.CurrentZone}");
                }
            }
        }
    }
}
