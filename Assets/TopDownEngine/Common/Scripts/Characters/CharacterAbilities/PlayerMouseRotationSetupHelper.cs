using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Helper script to easily set up PlayerMouseRotation on characters
    /// </summary>
    public class PlayerMouseRotationSetupHelper : MonoBehaviour
    {
        [Header("Setup Options")]
        [Tooltip("Whether to disable conflicting orientation components")]
        public bool disableConflictingComponents = true;
        
        [Tooltip("Whether to add PlayerMouseRotation to this character")]
        public bool addMouseRotation = true;
        
        [Tooltip("Whether to add PlayerMouseRotation to all characters in scene")]
        public bool addToAllCharacters = false;

        [Header("Mouse Rotation Settings")]
        [Tooltip("Rotate entire character or just sprite")]
        public bool rotateEntireCharacter = true;
        
        [Tooltip("Rotation speed (0 = instant)")]
        [Range(0f, 20f)]
        public float rotationSpeed = 0f;
        
        [Tooltip("Rotation offset to adjust sprite facing")]
        [Range(-180f, 180f)]
        public float rotationOffset = 0f;
        
        [Tooltip("Use sprite flipping instead of rotation")]
        public bool useFlipping = false;

        /// <summary>
        /// Set up mouse rotation for this character
        /// </summary>
        [ContextMenu("Setup Mouse Rotation for This Character")]
        public void SetupMouseRotationForThisCharacter()
        {
            if (addMouseRotation)
            {
                // Add PlayerMouseRotation component
                PlayerMouseRotation mouseRotation = GetComponent<PlayerMouseRotation>();
                if (mouseRotation == null)
                {
                    mouseRotation = gameObject.AddComponent<PlayerMouseRotation>();
                    Debug.Log($"Added PlayerMouseRotation to {gameObject.name}");
                }
                else
                {
                    Debug.Log($"PlayerMouseRotation already exists on {gameObject.name}");
                }
                
                // Configure settings
                mouseRotation.rotateEntireCharacter = rotateEntireCharacter;
                mouseRotation.rotationSpeed = rotationSpeed;
                mouseRotation.rotationOffset = rotationOffset;
                mouseRotation.useFlipping = useFlipping;
                mouseRotation.showDebugLine = true;
                mouseRotation.showDebugInfo = true;
            }
            
            if (disableConflictingComponents)
            {
                DisableConflictingComponents();
            }
        }

        /// <summary>
        /// Set up mouse rotation for all characters in scene
        /// </summary>
        [ContextMenu("Setup Mouse Rotation for All Characters")]
        public void SetupMouseRotationForAllCharacters()
        {
            Character[] allCharacters = FindObjectsOfType<Character>();
            
            foreach (Character character in allCharacters)
            {
                if (character.CharacterType == Character.CharacterTypes.Player)
                {
                    PlayerMouseRotation mouseRotation = character.GetComponent<PlayerMouseRotation>();
                    if (mouseRotation == null)
                    {
                        mouseRotation = character.gameObject.AddComponent<PlayerMouseRotation>();
                        Debug.Log($"Added PlayerMouseRotation to {character.name}");
                    }
                    
                    // Configure settings
                    mouseRotation.rotateEntireCharacter = rotateEntireCharacter;
                    mouseRotation.rotationSpeed = rotationSpeed;
                    mouseRotation.rotationOffset = rotationOffset;
                    mouseRotation.useFlipping = useFlipping;
                    mouseRotation.showDebugLine = true;
                    mouseRotation.showDebugInfo = true;
                }
            }
            
            if (disableConflictingComponents)
            {
                DisableConflictingComponentsOnAllCharacters();
            }
            
            Debug.Log($"Setup complete for {allCharacters.Length} characters");
        }

        /// <summary>
        /// Disable conflicting orientation components on this character
        /// </summary>
        [ContextMenu("Disable Conflicting Components")]
        public void DisableConflictingComponents()
        {
            // Disable CharacterOrientation2D
            CharacterOrientation2D orientation2D = GetComponent<CharacterOrientation2D>();
            if (orientation2D != null)
            {
                orientation2D.FacingMode = CharacterOrientation2D.FacingModes.None;
                Debug.Log($"Disabled CharacterOrientation2D on {gameObject.name}");
            }
            
            // Disable CharacterOrientation3D
            CharacterOrientation3D orientation3D = GetComponent<CharacterOrientation3D>();
            if (orientation3D != null)
            {
                orientation3D.RotationMode = CharacterOrientation3D.RotationModes.None;
                Debug.Log($"Disabled CharacterOrientation3D on {gameObject.name}");
            }
        }

        /// <summary>
        /// Disable conflicting orientation components on all characters
        /// </summary>
        [ContextMenu("Disable Conflicting Components on All Characters")]
        public void DisableConflictingComponentsOnAllCharacters()
        {
            Character[] allCharacters = FindObjectsOfType<Character>();
            
            foreach (Character character in allCharacters)
            {
                // Disable CharacterOrientation2D
                CharacterOrientation2D orientation2D = character.GetComponent<CharacterOrientation2D>();
                if (orientation2D != null)
                {
                    orientation2D.FacingMode = CharacterOrientation2D.FacingModes.None;
                }
                
                // Disable CharacterOrientation3D
                CharacterOrientation3D orientation3D = character.GetComponent<CharacterOrientation3D>();
                if (orientation3D != null)
                {
                    orientation3D.RotationMode = CharacterOrientation3D.RotationModes.None;
                }
            }
            
            Debug.Log($"Disabled conflicting components on {allCharacters.Length} characters");
        }

        /// <summary>
        /// Test mouse rotation on this character
        /// </summary>
        [ContextMenu("Test Mouse Rotation")]
        public void TestMouseRotation()
        {
            PlayerMouseRotation mouseRotation = GetComponent<PlayerMouseRotation>();
            if (mouseRotation != null)
            {
                mouseRotation.ShowDebugInfo();
                Debug.Log("Mouse rotation test - move mouse to see rotation");
            }
            else
            {
                Debug.LogWarning("No PlayerMouseRotation component found on this character");
            }
        }

        /// <summary>
        /// Remove mouse rotation from this character
        /// </summary>
        [ContextMenu("Remove Mouse Rotation")]
        public void RemoveMouseRotation()
        {
            PlayerMouseRotation mouseRotation = GetComponent<PlayerMouseRotation>();
            if (mouseRotation != null)
            {
                DestroyImmediate(mouseRotation);
                Debug.Log($"Removed PlayerMouseRotation from {gameObject.name}");
            }
            else
            {
                Debug.Log($"No PlayerMouseRotation found on {gameObject.name}");
            }
        }

        /// <summary>
        /// Show current setup status
        /// </summary>
        [ContextMenu("Show Setup Status")]
        public void ShowSetupStatus()
        {
            Debug.Log($"Setup Status for {gameObject.name}:");
            
            // Check PlayerMouseRotation
            PlayerMouseRotation mouseRotation = GetComponent<PlayerMouseRotation>();
            if (mouseRotation != null)
            {
                Debug.Log($"  ✅ PlayerMouseRotation: Enabled");
                Debug.Log($"  - Rotate Entire Character: {mouseRotation.rotateEntireCharacter}");
                Debug.Log($"  - Rotation Speed: {mouseRotation.rotationSpeed}");
                Debug.Log($"  - Rotation Offset: {mouseRotation.rotationOffset}");
                Debug.Log($"  - Use Flipping: {mouseRotation.useFlipping}");
            }
            else
            {
                Debug.Log($"  ❌ PlayerMouseRotation: Not found");
            }
            
            // Check conflicting components
            CharacterOrientation2D orientation2D = GetComponent<CharacterOrientation2D>();
            if (orientation2D != null)
            {
                Debug.Log($"  ⚠️ CharacterOrientation2D: {orientation2D.FacingMode} (may conflict)");
            }
            
            CharacterOrientation3D orientation3D = GetComponent<CharacterOrientation3D>();
            if (orientation3D != null)
            {
                Debug.Log($"  ⚠️ CharacterOrientation3D: {orientation3D.RotationMode} (may conflict)");
            }
            
            // Check camera
            Camera camera = Camera.main;
            if (camera != null)
            {
                Debug.Log($"  ✅ Main Camera: Found ({camera.name})");
            }
            else
            {
                Debug.Log($"  ❌ Main Camera: Not found (required for mouse rotation)");
            }
        }
    }
}
