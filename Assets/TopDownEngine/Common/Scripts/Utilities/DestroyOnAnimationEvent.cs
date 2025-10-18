using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Simple utility script that allows destroying a GameObject from an animation event.
    /// Attach this to any GameObject that needs to be destroyed via animation events.
    /// </summary>
    public class DestroyOnAnimationEvent : MonoBehaviour
    {
        [Header("Destroy Settings")]
        [Tooltip("The GameObject to destroy. If null, destroys this GameObject.")]
        public GameObject targetToDestroy;
        
        [Tooltip("Delay before destroying the object (in seconds)")]
        public float destroyDelay = 0f;
        
        [Header("Optional Effects")]
        [Tooltip("Optional particle effect to spawn on destruction")]
        public GameObject destructionEffect;
        
        [Tooltip("Optional sound to play on destruction")]
        public AudioClip destructionSound;
        
        [Header("Debug")]
        [Tooltip("Show debug logs when this component is used")]
        public bool showDebugInfo = false;

        /// <summary>
        /// Destroys the target GameObject immediately (or after delay).
        /// Call this method from an animation event.
        /// </summary>
        public void DestroyObject()
        {
            if (showDebugInfo)
            {
                Debug.Log($"DestroyOnAnimationEvent: DestroyObject() called on {gameObject.name}");
            }

            // Play optional effects
            PlayDestructionEffects();

            // Determine what to destroy
            GameObject objectToDestroy = targetToDestroy != null ? targetToDestroy : gameObject;

            if (destroyDelay > 0f)
            {
                Destroy(objectToDestroy, destroyDelay);
                
                if (showDebugInfo)
                {
                    Debug.Log($"DestroyOnAnimationEvent: Will destroy {objectToDestroy.name} in {destroyDelay} seconds");
                }
            }
            else
            {
                Destroy(objectToDestroy);
                
                if (showDebugInfo)
                {
                    Debug.Log($"DestroyOnAnimationEvent: Destroyed {objectToDestroy.name} immediately");
                }
            }
        }

        /// <summary>
        /// Destroys the target GameObject after a specific delay.
        /// Call this method from an animation event with a float parameter.
        /// </summary>
        /// <param name="delay">Delay in seconds before destroying</param>
        public void DestroyObjectWithDelay(float delay)
        {
            if (showDebugInfo)
            {
                Debug.Log($"DestroyOnAnimationEvent: DestroyObjectWithDelay({delay}) called on {gameObject.name}");
            }

            // Play optional effects
            PlayDestructionEffects();

            GameObject objectToDestroy = targetToDestroy != null ? targetToDestroy : gameObject;
            Destroy(objectToDestroy, delay);

            if (showDebugInfo)
            {
                Debug.Log($"DestroyOnAnimationEvent: Will destroy {objectToDestroy.name} in {delay} seconds");
            }
        }

        /// <summary>
        /// Destroys only this script component (keeps the GameObject).
        /// Useful if you only want to remove this behavior after animation.
        /// </summary>
        public void DestroySelf()
        {
            if (showDebugInfo)
            {
                Debug.Log($"DestroyOnAnimationEvent: DestroySelf() called, removing component from {gameObject.name}");
            }

            Destroy(this);
        }

        /// <summary>
        /// Deactivates the GameObject instead of destroying it (for object pooling).
        /// Call this method from an animation event if you're using object pooling.
        /// </summary>
        public void DeactivateObject()
        {
            if (showDebugInfo)
            {
                Debug.Log($"DestroyOnAnimationEvent: DeactivateObject() called on {gameObject.name}");
            }

            // Play optional effects
            PlayDestructionEffects();

            GameObject objectToDeactivate = targetToDestroy != null ? targetToDestroy : gameObject;
            objectToDeactivate.SetActive(false);
        }

        /// <summary>
        /// Plays optional particle effects and sounds
        /// </summary>
        private void PlayDestructionEffects()
        {
            // Spawn particle effect
            if (destructionEffect != null)
            {
                Instantiate(destructionEffect, transform.position, Quaternion.identity);
                
                if (showDebugInfo)
                {
                    Debug.Log($"DestroyOnAnimationEvent: Spawned destruction effect at {transform.position}");
                }
            }

            // Play sound
            if (destructionSound != null)
            {
                // Use MMSoundManager if available
                MMSoundManagerSoundPlayEvent.Trigger(destructionSound, MMSoundManager.MMSoundManagerTracks.Sfx, transform.position);
                
                if (showDebugInfo)
                {
                    Debug.Log($"DestroyOnAnimationEvent: Played destruction sound");
                }
            }
        }

        /// <summary>
        /// Context menu helper to test the destroy function in the editor
        /// </summary>
        [ContextMenu("Test Destroy Object")]
        private void TestDestroyObject()
        {
            if (Application.isPlaying)
            {
                DestroyObject();
            }
            else
            {
                Debug.LogWarning("DestroyOnAnimationEvent: Test can only be run in Play Mode!");
            }
        }

        /// <summary>
        /// Context menu helper to test the deactivate function in the editor
        /// </summary>
        [ContextMenu("Test Deactivate Object")]
        private void TestDeactivateObject()
        {
            if (Application.isPlaying)
            {
                DeactivateObject();
            }
            else
            {
                Debug.LogWarning("DestroyOnAnimationEvent: Test can only be run in Play Mode!");
            }
        }
    }
}

