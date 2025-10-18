using UnityEngine;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Helper script to easily set up the Beer Meter Stun system
    /// </summary>
    public class BeerMeterStunSetupHelper : MonoBehaviour
    {
        [Header("Setup Options")]
        [Tooltip("Whether to show debug information during setup")]
        public bool ShowDebugInfo = true;

        [Tooltip("Whether to create BeerManager if it doesn't exist")]
        public bool CreateBeerManager = true;

        [Tooltip("Whether to create puke effect prefab if it doesn't exist")]
        public bool CreatePukeEffectPrefab = true;

        [Tooltip("Whether to destroy this helper after setup")]
        public bool DestroyAfterSetup = true;

        /// <summary>
        /// Sets up the complete beer meter stun system
        /// </summary>
        [ContextMenu("Setup Beer Meter Stun System")]
        public virtual void SetupBeerMeterStunSystem()
        {
            if (ShowDebugInfo)
            {
                Debug.Log("=== Setting up Beer Meter Stun System ===");
            }

            // Step 1: Ensure BeerManager exists
            SetupBeerManager();

            // Step 2: Add BeerMeterStunHandler to this character
            SetupBeerMeterStunHandler();

            // Step 3: Create puke effect prefab if needed
            if (CreatePukeEffectPrefab)
            {
                CreatePukeEffectPrefabIfNeeded();
            }

            if (ShowDebugInfo)
            {
                Debug.Log("=== Beer Meter Stun System Setup Complete! ===");
                Debug.Log("✅ BeerManager: " + (BeerManager.HasInstance ? "Found" : "Missing"));
                Debug.Log("✅ BeerMeterStunHandler: " + (GetComponent<BeerMeterStunHandler>() != null ? "Added" : "Missing"));
                Debug.Log("✅ Puke Effect: " + (CreatePukeEffectPrefab ? "Created" : "Manual setup required"));
            }

            if (DestroyAfterSetup)
            {
                Destroy(this);
            }
        }

        /// <summary>
        /// Sets up BeerManager if it doesn't exist
        /// </summary>
        protected virtual void SetupBeerManager()
        {
            if (!BeerManager.HasInstance)
            {
                if (CreateBeerManager)
                {
                    GameObject beerManagerGO = new GameObject("BeerManager");
                    BeerManager beerManager = beerManagerGO.AddComponent<BeerManager>();
                    beerManager.ShowDebugInfo = ShowDebugInfo;
                    beerManager.BeerSystemActive = true;
                    beerManager.CurrentBeer = 50f; // Start at 50%
                    beerManager.StartingBeer = 50f;
                    beerManager.DepletionRate = 1f; // 1% per second

                    if (ShowDebugInfo)
                    {
                        Debug.Log("✅ Created BeerManager");
                    }
                }
                else
                {
                    Debug.LogWarning("❌ BeerManager not found! Please create one manually or enable 'Create BeerManager'.");
                }
            }
            else
            {
                if (ShowDebugInfo)
                {
                    Debug.Log("✅ BeerManager already exists");
                }
            }
        }

        /// <summary>
        /// Adds BeerMeterStunHandler to this character
        /// </summary>
        protected virtual void SetupBeerMeterStunHandler()
        {
            BeerMeterStunHandler stunHandler = GetComponent<BeerMeterStunHandler>();
            if (stunHandler == null)
            {
                stunHandler = gameObject.AddComponent<BeerMeterStunHandler>();
                stunHandler.showDebugInfo = ShowDebugInfo;
                stunHandler.beerMeterThreshold = 100f;
                stunHandler.beerMeterResetValue = 15f;
                stunHandler.stunDuration = 1.5f;

                if (ShowDebugInfo)
                {
                    Debug.Log("✅ Added BeerMeterStunHandler to " + gameObject.name);
                }
            }
            else
            {
                if (ShowDebugInfo)
                {
                    Debug.Log("✅ BeerMeterStunHandler already exists on " + gameObject.name);
                }
            }
        }

        /// <summary>
        /// Creates a simple puke effect prefab if needed
        /// </summary>
        protected virtual void CreatePukeEffectPrefabIfNeeded()
        {
            // Create a simple green circle sprite for puke effect
            GameObject pukeEffect = new GameObject("PukeEffect");
            
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
            pukeScript.effectDuration = 3f;
            pukeScript.fadeInTime = 0.3f;
            pukeScript.fadeOutTime = 0.5f;
            pukeScript.finalScale = 1.5f;
            
            // Set up the prefab
            pukeEffect.transform.localScale = Vector3.zero; // Start small
            pukeEffect.SetActive(false); // Start inactive
            
            // Save as prefab (this would need to be done manually in the editor)
            if (ShowDebugInfo)
            {
                Debug.Log("✅ Created PukeEffect prefab - Save this as a prefab and assign it to BeerMeterStunHandler");
            }
        }

        /// <summary>
        /// Creates a simple circle texture
        /// </summary>
        protected virtual Texture2D CreateCircleTexture(int size, Color color)
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
        /// Quick test to set beer level to 100
        /// </summary>
        [ContextMenu("Test Beer Level 100")]
        public virtual void TestBeerLevel100()
        {
            if (BeerManager.HasInstance)
            {
                BeerManager.Instance.CurrentBeer = 100f;
                Debug.Log("✅ Set beer level to 100 - Stun should trigger!");
            }
            else
            {
                Debug.LogWarning("❌ BeerManager not found! Run setup first.");
            }
        }

        /// <summary>
        /// Quick test to reset beer level
        /// </summary>
        [ContextMenu("Reset Beer Level")]
        public virtual void ResetBeerLevel()
        {
            if (BeerManager.HasInstance)
            {
                BeerManager.Instance.CurrentBeer = 50f;
                Debug.Log("✅ Reset beer level to 50");
            }
            else
            {
                Debug.LogWarning("❌ BeerManager not found! Run setup first.");
            }
        }
    }
}