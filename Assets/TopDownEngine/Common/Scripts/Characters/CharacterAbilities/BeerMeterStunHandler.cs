using UnityEngine;
using System.Collections;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Handles beer meter stun system - stuns player when beer meter reaches 100
    /// Prevents movement and attacks for 1.5 seconds and spawns puke effect
    /// </summary>
    [AddComponentMenu("TopDown Engine/Character/Abilities/Beer Meter Stun Handler")]
    public class BeerMeterStunHandler : CharacterAbility
    {
        [Header("Stun Settings")]
        [Tooltip("Duration of the stun in seconds")]
        [Range(0.5f, 5f)]
        public float stunDuration = 1.5f;
        
        [Tooltip("Beer meter threshold to trigger stun")]
        [Range(90f, 100f)]
        public float beerMeterThreshold = 100f;
        
        [Tooltip("Beer meter value to reset to after stun")]
        [Range(0f, 50f)]
        public float beerMeterResetValue = 15f;

        [Header("Puke Effect Settings")]
        [Tooltip("Prefab for the puke effect (green circle)")]
        public GameObject pukeEffectPrefab;
        
        [Tooltip("Distance from player to spawn puke effect")]
        [Range(0.5f, 3f)]
        public float pukeSpawnDistance = 1f;
        
        [Tooltip("Random offset for puke spawn position")]
        [Range(0f, 1f)]
        public float pukeSpawnRandomOffset = 0.3f;
        
        [Tooltip("How long the puke effect stays visible")]
        [Range(1f, 10f)]
        public float pukeEffectDuration = 4f;

        [Header("Audio")]
        [Tooltip("Sound effect when stun triggers")]
        public AudioClip stunSoundEffect;
        
        [Tooltip("Sound effect when puke spawns")]
        public AudioClip pukeSoundEffect;
        
        [Tooltip("Volume for sound effects")]
        [Range(0f, 1f)]
        public float soundVolume = 0.7f;

        [Header("Debug")]
        [Tooltip("Show debug information in console")]
        public bool showDebugInfo = false;
        
        [Tooltip("Show debug line to puke spawn position")]
        public bool showDebugLine = false;

        // Private variables
        private BeerManager _beerManager;
        private CharacterHandleWeapon _characterHandleWeapon;
        private bool _isStunned = false;
        private bool _originalMovementInputAuthorized;
        private bool _originalInputDetectionActive;
        private AudioSource _audioSource;
        private Vector3 _pukeSpawnPosition;

        protected override void Initialization()
        {
            base.Initialization();
            
            // Get BeerManager reference
            _beerManager = BeerManager.Instance;
            if (_beerManager == null)
            {
                Debug.LogError("BeerMeterStunHandler: BeerManager not found! Make sure BeerManager is in the scene.");
                return;
            }
            
            // Get CharacterHandleWeapon reference
            _characterHandleWeapon = _character.FindAbility<CharacterHandleWeapon>();
            if (_characterHandleWeapon == null)
            {
                Debug.LogWarning("BeerMeterStunHandler: CharacterHandleWeapon not found! Weapon abilities will not be disabled during stun.");
            }
            
            // Setup audio source
            SetupAudioSource();
            
            if (showDebugInfo)
            {
                Debug.Log($"BeerMeterStunHandler: Initialized - Stun Duration: {stunDuration}s, Threshold: {beerMeterThreshold}, Reset Value: {beerMeterResetValue}");
            }
        }

        protected override void HandleInput()
        {
            if (!AbilityAuthorized || _isStunned) return;
            
            // Check if beer meter has reached threshold
            if (_beerManager != null && _beerManager.CurrentBeer >= beerMeterThreshold)
            {
                if (showDebugInfo)
                {
                    Debug.Log($"BeerMeterStunHandler: Beer level {_beerManager.CurrentBeer} >= threshold {beerMeterThreshold}, triggering stun!");
                }
                TriggerStun();
            }
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();
            
            // Also check in ProcessAbility for more reliable detection
            if (!AbilityAuthorized || _isStunned) return;
            
            // Check if beer meter has reached threshold
            if (_beerManager != null)
            {
                if (showDebugInfo && _beerManager.CurrentBeer > 90f)
                {
                    Debug.Log($"BeerMeterStunHandler: Beer level {_beerManager.CurrentBeer:F1} (threshold: {beerMeterThreshold})");
                }
                
                // Check if beer level has reached or exceeded threshold
                if (_beerManager.CurrentBeer >= beerMeterThreshold)
                {
                    if (showDebugInfo)
                    {
                        Debug.Log($"BeerMeterStunHandler: Beer level {_beerManager.CurrentBeer} >= threshold {beerMeterThreshold}, triggering stun!");
                    }
                    TriggerStun();
                }
                // Also check if beer level is very close to threshold (within 0.5 units)
                else if (_beerManager.CurrentBeer >= (beerMeterThreshold - 0.5f))
                {
                    if (showDebugInfo)
                    {
                        Debug.Log($"BeerMeterStunHandler: Beer level {_beerManager.CurrentBeer} is very close to threshold {beerMeterThreshold}, triggering stun!");
                    }
                    TriggerStun();
                }
            }
        }

        /// <summary>
        /// Setup audio source for sound effects
        /// </summary>
        private void SetupAudioSource()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            _audioSource.playOnAwake = false;
            _audioSource.volume = soundVolume;
        }

        /// <summary>
        /// Trigger the stun sequence
        /// </summary>
        private void TriggerStun()
        {
            if (_isStunned) return;
            
            if (showDebugInfo)
            {
                Debug.Log($"BeerMeterStunHandler: Triggering stun - Beer Level: {_beerManager.CurrentBeer}");
            }
            
            StartCoroutine(StunSequence());
        }

        /// <summary>
        /// Complete stun sequence
        /// </summary>
        private IEnumerator StunSequence()
        {
            _isStunned = true;
            
            // 1. Disable character abilities
            DisableCharacterAbilities();
            
            // 2. Set stun state
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);
            
            // 3. Play stun sound
            PlayStunSound();
            
            // 4. Spawn puke effect
            SpawnPukeEffect();
            
            // 5. Wait for stun duration
            yield return new WaitForSeconds(stunDuration);
            
            // 6. Re-enable character abilities
            EnableCharacterAbilities();
            
            // 7. Reset beer meter
            ResetBeerMeter();
            
            // 8. Return to normal state
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
            
            _isStunned = false;
            
            if (showDebugInfo)
            {
                Debug.Log("BeerMeterStunHandler: Stun sequence completed");
            }
        }

        /// <summary>
        /// Disable character abilities during stun
        /// </summary>
        private void DisableCharacterAbilities()
        {
            // Store original states
            _originalMovementInputAuthorized = _characterMovement != null ? _characterMovement.InputAuthorized : true;
            _originalInputDetectionActive = _character.LinkedInputManager != null && _character.LinkedInputManager.InputDetectionActive;
            
            // Disable movement
            if (_characterMovement != null)
            {
                _characterMovement.InputAuthorized = false;
            }
            
            // Block input (this will prevent weapon use as well)
            if (_character.LinkedInputManager != null)
            {
                _character.LinkedInputManager.InputDetectionActive = false;
            }
            
            if (showDebugInfo)
            {
                Debug.Log("BeerMeterStunHandler: Character abilities disabled");
            }
        }

        /// <summary>
        /// Re-enable character abilities after stun
        /// </summary>
        private void EnableCharacterAbilities()
        {
            // Re-enable movement
            if (_characterMovement != null)
            {
                _characterMovement.InputAuthorized = _originalMovementInputAuthorized;
            }
            
            // Re-enable input (this will restore weapon use as well)
            if (_character.LinkedInputManager != null)
            {
                _character.LinkedInputManager.InputDetectionActive = _originalInputDetectionActive;
            }
            
            if (showDebugInfo)
            {
                Debug.Log("BeerMeterStunHandler: Character abilities re-enabled");
            }
        }

        /// <summary>
        /// Spawn puke effect near player
        /// </summary>
        private void SpawnPukeEffect()
        {
            // Calculate spawn position with random offset
            Vector2 randomOffset = Random.insideUnitCircle * pukeSpawnRandomOffset;
            _pukeSpawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0) + transform.forward * pukeSpawnDistance;
            
            GameObject pukeEffect;
            
            if (pukeEffectPrefab == null)
            {
                // Create a simple puke effect if no prefab is assigned
                pukeEffect = CreateSimplePukeEffect();
                if (showDebugInfo)
                {
                    Debug.LogWarning("BeerMeterStunHandler: No puke effect prefab assigned! Creating simple effect.");
                }
            }
            else
            {
                // Spawn puke effect from prefab
                pukeEffect = Instantiate(pukeEffectPrefab, _pukeSpawnPosition, Quaternion.identity);
            }
            
            // Note: The PukeEffect script handles its own destruction based on disappearAfterDuration setting
            // No manual destruction needed here
            
            // Play puke sound
            PlayPukeSound();
            
            if (showDebugInfo)
            {
                Debug.Log($"BeerMeterStunHandler: Puke effect spawned at {_pukeSpawnPosition}");
            }
        }

        /// <summary>
        /// Create a simple puke effect if no prefab is assigned
        /// </summary>
        private GameObject CreateSimplePukeEffect()
        {
            GameObject pukeEffect = new GameObject("PukeEffect");
            pukeEffect.transform.position = _pukeSpawnPosition;
            
            // Add SpriteRenderer
            SpriteRenderer spriteRenderer = pukeEffect.AddComponent<SpriteRenderer>();
            
            // Create a simple green circle texture
            Texture2D circleTexture = CreateCircleTexture(64, Color.green);
            Sprite circleSprite = Sprite.Create(circleTexture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
            spriteRenderer.sprite = circleSprite;
            spriteRenderer.color = new Color(0, 1, 0, 0.7f); // Semi-transparent green
            
            // Add PukeEffect script
            PukeEffect pukeScript = pukeEffect.AddComponent<PukeEffect>();
            pukeScript.disappearAfterDuration = false; // Puke stays permanently
            pukeScript.effectDuration = pukeEffectDuration;
            pukeScript.fadeInTime = 0.3f;
            pukeScript.fadeOutTime = 0.5f;
            pukeScript.finalScale = 1.5f;
            
            // Set up the effect
            pukeEffect.transform.localScale = Vector3.zero; // Start small
            
            return pukeEffect;
        }

        /// <summary>
        /// Creates a simple circle texture
        /// </summary>
        private Texture2D CreateCircleTexture(int size, Color color)
        {
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];
            
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;
            
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    if (distance <= radius)
                    {
                        pixels[y * size + x] = color;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Reset beer meter after stun
        /// </summary>
        private void ResetBeerMeter()
        {
            if (_beerManager != null)
            {
                float oldBeer = _beerManager.CurrentBeer;
                _beerManager.CurrentBeer = beerMeterResetValue;
                
                if (showDebugInfo)
                {
                    Debug.Log($"BeerMeterStunHandler: Beer meter reset from {oldBeer} to {beerMeterResetValue}");
                }
            }
            else
            {
                Debug.LogError("BeerMeterStunHandler: BeerManager is null! Cannot reset beer meter.");
            }
        }

        /// <summary>
        /// Play stun sound effect
        /// </summary>
        private void PlayStunSound()
        {
            if (stunSoundEffect != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(stunSoundEffect, soundVolume);
            }
        }

        /// <summary>
        /// Play puke sound effect
        /// </summary>
        private void PlayPukeSound()
        {
            if (pukeSoundEffect != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(pukeSoundEffect, soundVolume);
            }
        }

        /// <summary>
        /// Test stun sequence (for debugging)
        /// </summary>
        [ContextMenu("Test Stun Sequence")]
        public void TestStunSequence()
        {
            if (_isStunned)
            {
                Debug.Log("BeerMeterStunHandler: Already stunned, cannot test");
                return;
            }
            
            Debug.Log("BeerMeterStunHandler: Testing stun sequence");
            TriggerStun();
        }

        /// <summary>
        /// Force trigger stun (for debugging)
        /// </summary>
        [ContextMenu("Force Trigger Stun")]
        public void ForceTriggerStun()
        {
            if (_beerManager != null)
            {
                _beerManager.CurrentBeer = beerMeterThreshold;
                Debug.Log($"BeerMeterStunHandler: Force set beer to {beerMeterThreshold}, triggering stun!");
                TriggerStun();
            }
            else
            {
                Debug.LogError("BeerMeterStunHandler: BeerManager is null! Cannot force trigger stun.");
            }
        }

        /// <summary>
        /// Set beer meter to 100 for testing
        /// </summary>
        [ContextMenu("Set Beer to 100")]
        public void SetBeerTo100()
        {
            if (_beerManager != null)
            {
                _beerManager.CurrentBeer = 100f;
                Debug.Log($"BeerMeterStunHandler: Set beer to 100. Current beer: {_beerManager.CurrentBeer}");
            }
            else
            {
                Debug.LogError("BeerMeterStunHandler: BeerManager is null! Cannot set beer level.");
            }
        }

        /// <summary>
        /// Check current beer level
        /// </summary>
        [ContextMenu("Check Current Beer Level")]
        public void CheckCurrentBeerLevel()
        {
            if (_beerManager != null)
            {
                Debug.Log($"BeerMeterStunHandler: Current beer level: {_beerManager.CurrentBeer} (Threshold: {beerMeterThreshold})");
            }
            else
            {
                Debug.LogError("BeerMeterStunHandler: BeerManager is null!");
            }
        }

        /// <summary>
        /// Monitor beer level changes (for debugging)
        /// </summary>
        [ContextMenu("Start Beer Level Monitoring")]
        public void StartBeerLevelMonitoring()
        {
            if (_beerManager != null)
            {
                StartCoroutine(MonitorBeerLevel());
                Debug.Log("BeerMeterStunHandler: Started beer level monitoring");
            }
            else
            {
                Debug.LogError("BeerMeterStunHandler: BeerManager is null! Cannot start monitoring.");
            }
        }

        /// <summary>
        /// Monitor beer level changes for debugging
        /// </summary>
        private IEnumerator MonitorBeerLevel()
        {
            float lastBeerLevel = _beerManager.CurrentBeer;
            
            while (true)
            {
                if (_beerManager != null)
                {
                    float currentBeerLevel = _beerManager.CurrentBeer;
                    
                    if (Mathf.Abs(currentBeerLevel - lastBeerLevel) > 0.1f)
                    {
                        Debug.Log($"BeerMeterStunHandler: Beer level changed from {lastBeerLevel:F1} to {currentBeerLevel:F1}");
                        lastBeerLevel = currentBeerLevel;
                    }
                    
                    if (currentBeerLevel >= beerMeterThreshold)
                    {
                        Debug.Log($"BeerMeterStunHandler: Beer level {currentBeerLevel} reached threshold {beerMeterThreshold}!");
                        break;
                    }
                }
                
                yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
            }
        }

        /// <summary>
        /// Show current status (for debugging)
        /// </summary>
        [ContextMenu("Show Status")]
        public void ShowStatus()
        {
            Debug.Log($"BeerMeterStunHandler Status:");
            Debug.Log($"  - Is Stunned: {_isStunned}");
            Debug.Log($"  - Ability Authorized: {AbilityAuthorized}");
            Debug.Log($"  - Beer Level: {(_beerManager != null ? _beerManager.CurrentBeer.ToString() : "BeerManager not found")}");
            Debug.Log($"  - Threshold: {beerMeterThreshold}");
            Debug.Log($"  - Stun Duration: {stunDuration}s");
            Debug.Log($"  - Puke Prefab: {(pukeEffectPrefab != null ? "Assigned" : "Not assigned")}");
            Debug.Log($"  - Character Movement: {(_characterMovement != null ? "Found" : "Not found")}");
            Debug.Log($"  - Character Handle Weapon: {(_characterHandleWeapon != null ? "Found" : "Not found")}");
            Debug.Log($"  - Input Manager: {(_character.LinkedInputManager != null ? "Found" : "Not found")}");
        }

        /// <summary>
        /// Check if system is properly set up
        /// </summary>
        [ContextMenu("Check System Setup")]
        public void CheckSystemSetup()
        {
            Debug.Log("=== Beer Meter Stun System Setup Check ===");
            
            // Check BeerManager
            if (_beerManager == null)
            {
                Debug.LogError("❌ BeerManager not found! Make sure BeerManager is in the scene.");
            }
            else
            {
                Debug.Log($"✅ BeerManager found - Current beer: {_beerManager.CurrentBeer}");
            }
            
            // Check Character components
            if (_character == null)
            {
                Debug.LogError("❌ Character component not found!");
            }
            else
            {
                Debug.Log($"✅ Character found - Type: {_character.CharacterType}");
            }
            
            if (_characterMovement == null)
            {
                Debug.LogError("❌ CharacterMovement not found!");
            }
            else
            {
                Debug.Log($"✅ CharacterMovement found - InputAuthorized: {_characterMovement.InputAuthorized}");
            }
            
            if (_characterHandleWeapon == null)
            {
                Debug.LogWarning("⚠️ CharacterHandleWeapon not found - weapon abilities won't be disabled");
            }
            else
            {
                Debug.Log($"✅ CharacterHandleWeapon found - AbilityAuthorized: {_characterHandleWeapon.AbilityAuthorized}");
            }
            
            if (_character.LinkedInputManager == null)
            {
                Debug.LogError("❌ InputManager not found!");
            }
            else
            {
                Debug.Log($"✅ InputManager found - InputDetectionActive: {_character.LinkedInputManager.InputDetectionActive}");
            }
            
            // Check puke effect prefab
            if (pukeEffectPrefab == null)
            {
                Debug.LogWarning("⚠️ Puke Effect Prefab not assigned!");
            }
            else
            {
                Debug.Log($"✅ Puke Effect Prefab assigned: {pukeEffectPrefab.name}");
            }
            
            Debug.Log("=== Setup Check Complete ===");
        }

        private void OnDrawGizmos()
        {
            if (!showDebugLine || !Application.isPlaying) return;
            
            if (_isStunned && _pukeSpawnPosition != Vector3.zero)
            {
                // Draw line to puke spawn position
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _pukeSpawnPosition);
                
                // Draw puke spawn position
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_pukeSpawnPosition, 0.2f);
            }
        }

    }
}
