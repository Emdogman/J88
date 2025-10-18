using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// UI component that displays the beer meter and updates based on BeerManager
    /// </summary>
    [AddComponentMenu("TopDown Engine/UI/Beer Meter UI")]
    public class BeerMeterUI : MonoBehaviour
    {
        [Header("UI References")]
        
        /// <summary>
        /// The background image of the beer meter
        /// </summary>
        [Tooltip("The background image of the beer meter")]
        public Image BackgroundImage;

        /// <summary>
        /// The fill image that shows the current beer level
        /// </summary>
        [Tooltip("The fill image that shows the current beer level")]
        public Image FillImage;

        /// <summary>
        /// The first zone divider line (at 33%)
        /// </summary>
        [Tooltip("The first zone divider line (at 33%)")]
        public Image ZoneDivider1;

        /// <summary>
        /// The second zone divider line (at 66%)
        /// </summary>
        [Tooltip("The second zone divider line (at 66%)")]
        public Image ZoneDivider2;

        /// <summary>
        /// Optional text component to display beer percentage
        /// </summary>
        [Tooltip("Optional text component to display beer percentage")]
        public Text BeerPercentageText;

        [Header("Visual Settings")]
        
        /// <summary>
        /// Color for zone 1 (0-33%) - High Drift
        /// </summary>
        [Tooltip("Color for zone 1 (0-33%) - High Drift")]
        public Color Zone1Color = new Color(1f, 0.2f, 0.2f, 1f); // Red

        /// <summary>
        /// Color for zone 2 (34-66%) - Drift
        /// </summary>
        [Tooltip("Color for zone 2 (34-66%) - Drift")]
        public Color Zone2Color = new Color(1f, 0.8f, 0.2f, 1f); // Orange

        /// <summary>
        /// Color for zone 3 (67-100%) - Normal
        /// </summary>
        [Tooltip("Color for zone 3 (67-100%) - Normal")]
        public Color Zone3Color = new Color(0.2f, 1f, 0.2f, 1f); // Green

        /// <summary>
        /// Whether to show debug information
        /// </summary>
        [Tooltip("Whether to show debug information")]
        public bool ShowDebugInfo = false;
        
        /// <summary>
        /// If true, auto-setup will override editor positioning. If false, respects editor positioning.
        /// </summary>
        [Tooltip("If true, auto-setup will override editor positioning. If false, respects editor positioning.")]
        public bool OverrideEditorPositioning = false;

        /// <summary>
        /// Whether to use periodic refresh as a fallback if events don't work
        /// </summary>
        [Tooltip("Whether to use periodic refresh as a fallback if events don't work")]
        public bool UsePeriodicRefresh = true;

        /// <summary>
        /// How often to refresh the beer meter (in seconds)
        /// </summary>
        [Tooltip("How often to refresh the beer meter (in seconds)")]
        public float RefreshInterval = 0.1f;

        protected float _lastRefreshTime;

        protected virtual void Start()
        {
            AutoSetupUI();
            EnsureFillImageSetup();
            InitializeUI();
            SubscribeToEvents();
            
            // Force initial update from BeerManager if available
            if (BeerManager.HasInstance)
            {
                float currentBeer = BeerManager.Instance.CurrentBeer;
                UpdateBeerMeterFill(currentBeer);
                UpdateBeerMeterColor(currentBeer);
                UpdateBeerPercentageText(currentBeer);
            }
        }

        protected virtual void Update()
        {
            // Periodic refresh as fallback if events don't work
            if (UsePeriodicRefresh && BeerManager.HasInstance)
            {
                if (Time.time - _lastRefreshTime >= RefreshInterval)
                {
                    float currentBeer = BeerManager.Instance.CurrentBeer;
                    UpdateBeerMeterFill(currentBeer);
                    UpdateBeerMeterColor(currentBeer);
                    UpdateBeerPercentageText(currentBeer);
                    _lastRefreshTime = Time.time;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        /// <summary>
        /// Initializes the UI components
        /// </summary>
        protected virtual void InitializeUI()
        {
            // Get initial beer level from BeerManager if available
            float initialBeerLevel = 50f; // Default fallback
            if (BeerManager.HasInstance)
            {
                initialBeerLevel = BeerManager.Instance.CurrentBeer;
            }

            // Set initial beer level
            if (FillImage != null)
            {
                float normalizedLevel = initialBeerLevel / 100f;
                FillImage.fillAmount = normalizedLevel;
            }

            // Position zone dividers
            PositionZoneDividers();

            // Set initial color based on current beer level
            UpdateBeerMeterColor(initialBeerLevel);
            UpdateBeerPercentageText(initialBeerLevel);
        }

        /// <summary>
        /// Subscribes to BeerManager events
        /// </summary>
        protected virtual void SubscribeToEvents()
        {
            if (BeerManager.HasInstance)
            {
                BeerManager.OnBeerLevelChanged += OnBeerLevelChanged;
                BeerManager.OnBeerZoneChanged += OnBeerZoneChanged;
                
                if (ShowDebugInfo)
                {
                    Debug.Log("BeerMeterUI subscribed to BeerManager events");
                }
            }
            else
            {
                if (ShowDebugInfo)
                {
                    Debug.LogWarning("BeerManager not found! BeerMeterUI will not receive updates.");
                }
            }
        }

        /// <summary>
        /// Unsubscribes from BeerManager events
        /// </summary>
        protected virtual void UnsubscribeFromEvents()
        {
            if (BeerManager.HasInstance)
            {
                BeerManager.OnBeerLevelChanged -= OnBeerLevelChanged;
                BeerManager.OnBeerZoneChanged -= OnBeerZoneChanged;
            }
        }

        /// <summary>
        /// Handles beer level changes
        /// </summary>
        /// <param name="beerLevel">The new beer level</param>
        protected virtual void OnBeerLevelChanged(float beerLevel)
        {
            if (ShowDebugInfo)
            {
                Debug.Log($"BeerMeterUI: OnBeerLevelChanged called with {beerLevel:F1}%");
            }
            
            UpdateBeerMeterFill(beerLevel);
            UpdateBeerMeterColor(beerLevel);
            UpdateBeerPercentageText(beerLevel);
        }

        /// <summary>
        /// Handles beer zone changes
        /// </summary>
        /// <param name="zoneChangeEvent">The zone change event</param>
        protected virtual void OnBeerZoneChanged(BeerZoneChangeEvent zoneChangeEvent)
        {
            UpdateBeerMeterColor(zoneChangeEvent.BeerLevel);
            
            if (ShowDebugInfo)
            {
                Debug.Log($"Beer zone changed to {zoneChangeEvent.NewZone}, beer level: {zoneChangeEvent.BeerLevel:F1}");
            }
        }

        /// <summary>
        /// Updates the beer meter fill amount
        /// </summary>
        /// <param name="beerLevel">The current beer level (0-100)</param>
        protected virtual void UpdateBeerMeterFill(float beerLevel)
        {
            if (FillImage != null)
            {
                // Ensure proper fill settings
                FillImage.type = Image.Type.Filled;
                FillImage.fillMethod = Image.FillMethod.Horizontal;
                FillImage.fillOrigin = 0; // Left to right
                
                float normalizedLevel = Mathf.Clamp01(beerLevel / 100f);
                FillImage.fillAmount = normalizedLevel;
                
                if (ShowDebugInfo)
                {
                    Debug.Log($"Beer meter fill updated: {beerLevel:F1}% -> {normalizedLevel:F2} fill amount");
                }
            }
            else
            {
                Debug.LogWarning("BeerMeterUI: FillImage is null! Cannot update beer meter fill.");
            }
        }

        /// <summary>
        /// Updates the beer meter color based on the current zone
        /// </summary>
        /// <param name="beerLevel">The current beer level</param>
        protected virtual void UpdateBeerMeterColor(float beerLevel)
        {
            if (FillImage == null)
            {
                return;
            }

            Color targetColor;
            
            if (beerLevel >= 67f)
            {
                targetColor = Zone3Color; // Zone 3: Normal (Green)
            }
            else if (beerLevel >= 34f)
            {
                targetColor = Zone2Color; // Zone 2: Drift (Orange)
            }
            else
            {
                targetColor = Zone1Color; // Zone 1: High Drift (Red)
            }

            FillImage.color = targetColor;
        }

        /// <summary>
        /// Updates the beer percentage text if available
        /// </summary>
        /// <param name="beerLevel">The current beer level</param>
        protected virtual void UpdateBeerPercentageText(float beerLevel)
        {
            if (BeerPercentageText != null)
            {
                BeerPercentageText.text = $"Beer: {beerLevel:F0}%";
            }
        }

        /// <summary>
        /// Positions the zone divider lines at 33% and 66%
        /// </summary>
        protected virtual void PositionZoneDividers()
        {
            if (ZoneDivider1 != null)
            {
                RectTransform divider1Rect = ZoneDivider1.GetComponent<RectTransform>();
                if (divider1Rect != null)
                {
                    // Position at 33% of the meter width
                    divider1Rect.anchorMin = new Vector2(0.33f, 0f);
                    divider1Rect.anchorMax = new Vector2(0.33f, 1f);
                    divider1Rect.offsetMin = Vector2.zero;
                    divider1Rect.offsetMax = Vector2.zero;
                }
            }

            if (ZoneDivider2 != null)
            {
                RectTransform divider2Rect = ZoneDivider2.GetComponent<RectTransform>();
                if (divider2Rect != null)
                {
                    // Position at 66% of the meter width
                    divider2Rect.anchorMin = new Vector2(0.66f, 0f);
                    divider2Rect.anchorMax = new Vector2(0.66f, 1f);
                    divider2Rect.offsetMin = Vector2.zero;
                    divider2Rect.offsetMax = Vector2.zero;
                }
            }
        }

        /// <summary>
        /// Ensures FillImage is properly configured
        /// </summary>
        protected virtual void EnsureFillImageSetup()
        {
            if (FillImage != null)
            {
                // Force proper Image settings for fill
                FillImage.type = Image.Type.Filled;
                FillImage.fillMethod = Image.FillMethod.Horizontal;
                FillImage.fillOrigin = 0; // Left to right
                
                // Create a simple white texture if no sprite
                if (FillImage.sprite == null)
                {
                    Texture2D whiteTexture = new Texture2D(1, 1);
                    whiteTexture.SetPixel(0, 0, Color.white);
                    whiteTexture.Apply();
                    FillImage.sprite = Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                    
                    if (ShowDebugInfo)
                    {
                        Debug.Log("BeerMeterUI: Created default white sprite for FillImage");
                    }
                }
            }
        }

        /// <summary>
        /// Automatically sets up the UI components if they're not assigned
        /// </summary>
        protected virtual void AutoSetupUI()
        {
            // Auto-find background image if not assigned
            if (BackgroundImage == null)
            {
                BackgroundImage = GetComponent<Image>();
                if (BackgroundImage == null)
                {
                    // Create a background image if none exists
                    GameObject backgroundGO = new GameObject("BeerMeterBackground");
                    backgroundGO.transform.SetParent(transform);
                    backgroundGO.transform.localPosition = Vector3.zero;
                    backgroundGO.transform.localScale = Vector3.one;
                    
                    RectTransform bgRect = backgroundGO.AddComponent<RectTransform>();
                    bgRect.anchorMin = Vector2.zero;
                    bgRect.anchorMax = Vector2.one;
                    bgRect.offsetMin = Vector2.zero;
                    bgRect.offsetMax = Vector2.zero;
                    
                    BackgroundImage = backgroundGO.AddComponent<Image>();
                    BackgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
                }
            }

            // Auto-find or create fill image
            if (FillImage == null)
            {
                Transform fillTransform = transform.Find("BeerMeterFill");
                if (fillTransform != null)
                {
                    FillImage = fillTransform.GetComponent<Image>();
                }
                else
                {
                    // Create fill image
                    GameObject fillGO = new GameObject("BeerMeterFill");
                    fillGO.transform.SetParent(transform);
                    fillGO.transform.localPosition = Vector3.zero;
                    fillGO.transform.localScale = Vector3.one;
                    
                    RectTransform fillRect = fillGO.AddComponent<RectTransform>();
                    fillRect.anchorMin = Vector2.zero;
                    fillRect.anchorMax = Vector2.one;
                    fillRect.offsetMin = Vector2.zero;
                    fillRect.offsetMax = Vector2.zero;
                    
                    FillImage = fillGO.AddComponent<Image>();
                    
                    // Force proper Image settings for fill
                    FillImage.type = Image.Type.Filled;
                    FillImage.fillMethod = Image.FillMethod.Horizontal;
                    FillImage.fillOrigin = 0; // Left to right
                    
                    // Create a simple white texture for the fill (required for Image component)
                    Texture2D whiteTexture = new Texture2D(1, 1);
                    whiteTexture.SetPixel(0, 0, Color.white);
                    whiteTexture.Apply();
                    FillImage.sprite = Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                    
                    // Set initial color
                    FillImage.color = Zone2Color; // Start with zone 2 color
                    
                    // Get initial beer level from BeerManager if available
                    float initialBeerLevel = 50f; // Default fallback
                    if (BeerManager.HasInstance)
                    {
                        initialBeerLevel = BeerManager.Instance.CurrentBeer;
                    }
                    FillImage.fillAmount = initialBeerLevel / 100f;
                }
            }

            // Auto-find or create zone dividers
            if (ZoneDivider1 == null)
            {
                Transform divider1Transform = transform.Find("ZoneDivider1");
                if (divider1Transform != null)
                {
                    ZoneDivider1 = divider1Transform.GetComponent<Image>();
                }
                else
                {
                    ZoneDivider1 = CreateZoneDivider("ZoneDivider1", 0.33f);
                }
            }

            if (ZoneDivider2 == null)
            {
                Transform divider2Transform = transform.Find("ZoneDivider2");
                if (divider2Transform != null)
                {
                    ZoneDivider2 = divider2Transform.GetComponent<Image>();
                }
                else
                {
                    ZoneDivider2 = CreateZoneDivider("ZoneDivider2", 0.66f);
                }
            }

            // Auto-find or create percentage text
            if (BeerPercentageText == null)
            {
                Transform textTransform = transform.Find("BeerPercentageText");
                if (textTransform != null)
                {
                    BeerPercentageText = textTransform.GetComponent<Text>();
                }
                else
                {
                    // Create percentage text
                    GameObject textGO = new GameObject("BeerPercentageText");
                    textGO.transform.SetParent(transform);
                    textGO.transform.localPosition = Vector3.zero;
                    textGO.transform.localScale = Vector3.one;
                    
                    RectTransform textRect = textGO.AddComponent<RectTransform>();
                    textRect.anchorMin = new Vector2(0.5f, 0.5f);
                    textRect.anchorMax = new Vector2(0.5f, 0.5f);
                    textRect.sizeDelta = new Vector2(200, 30);
                    textRect.anchoredPosition = new Vector2(0, 40);
                    
                    BeerPercentageText = textGO.AddComponent<Text>();
                    BeerPercentageText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    BeerPercentageText.fontSize = 16;
                    BeerPercentageText.color = Color.white;
                    BeerPercentageText.alignment = TextAnchor.MiddleCenter;
                    BeerPercentageText.text = "Beer: 50%";
                }
            }

            // Ensure we have a Canvas component
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                // Create a canvas if we don't have one
                GameObject canvasGO = new GameObject("BeerMeterCanvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
                
                // Move this object to be a child of the canvas
                transform.SetParent(canvasGO.transform);
            }

            // Set up the main rect transform
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                rectTransform = gameObject.AddComponent<RectTransform>();
            }
            
            // Only set up default positioning if OverrideEditorPositioning is true OR if the UI hasn't been configured
            // Check if the rect transform has default values (indicating it hasn't been positioned)
            bool hasCustomPositioning = rectTransform.anchorMin != Vector2.zero || 
                                      rectTransform.anchorMax != Vector2.one || 
                                      rectTransform.offsetMin != Vector2.zero || 
                                      rectTransform.offsetMax != Vector2.zero ||
                                      rectTransform.localScale != Vector3.one;
            
            if (OverrideEditorPositioning || !hasCustomPositioning)
            {
                // Set up default positioning only if not configured in editor
                // Width: 20% to 80% of screen (60% total width)
                // Height: 90% to 95% of screen (5% total height)
                rectTransform.anchorMin = new Vector2(0.2f, 0.9f);
                rectTransform.anchorMax = new Vector2(0.8f, 0.95f);
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.localScale = Vector3.one;
            }

            if (ShowDebugInfo)
            {
                Debug.Log("BeerMeterUI auto-setup completed!");
            }
        }

        /// <summary>
        /// Creates a zone divider at the specified position
        /// </summary>
        /// <param name="name">Name of the divider</param>
        /// <param name="position">Position as a fraction (0-1)</param>
        /// <returns>The created divider image</returns>
        protected virtual Image CreateZoneDivider(string name, float position)
        {
            GameObject dividerGO = new GameObject(name);
            dividerGO.transform.SetParent(transform);
            dividerGO.transform.localPosition = Vector3.zero;
            dividerGO.transform.localScale = Vector3.one;
            
            RectTransform dividerRect = dividerGO.AddComponent<RectTransform>();
            dividerRect.anchorMin = new Vector2(position, 0f);
            dividerRect.anchorMax = new Vector2(position, 1f);
            dividerRect.offsetMin = Vector2.zero;
            dividerRect.offsetMax = Vector2.zero;
            dividerRect.sizeDelta = new Vector2(2, 0); // 2 pixel wide line
            
            Image dividerImage = dividerGO.AddComponent<Image>();
            dividerImage.color = new Color(1f, 1f, 1f, 0.5f); // Semi-transparent white
            
            return dividerImage;
        }

        /// <summary>
        /// Manually updates the beer meter (for testing or direct control)
        /// </summary>
        /// <param name="beerLevel">The beer level to display</param>
        public virtual void SetBeerLevel(float beerLevel)
        {
            beerLevel = Mathf.Clamp(beerLevel, 0f, 100f);
            UpdateBeerMeterFill(beerLevel);
            UpdateBeerMeterColor(beerLevel);
            UpdateBeerPercentageText(beerLevel);
        }

        /// <summary>
        /// Context menu method to force auto-setup
        /// </summary>
        [ContextMenu("Force Auto Setup")]
        protected virtual void ForceAutoSetup()
        {
            AutoSetupUI();
            Debug.Log("BeerMeterUI auto-setup forced!");
        }

        /// <summary>
        /// Context menu method to test the beer meter with different levels
        /// </summary>
        [ContextMenu("Test Beer Level 25%")]
        protected virtual void TestBeerLevel25()
        {
            SetBeerLevel(25f);
        }

        /// <summary>
        /// Context menu method to test the beer meter with different levels
        /// </summary>
        [ContextMenu("Test Beer Level 50%")]
        protected virtual void TestBeerLevel50()
        {
            SetBeerLevel(50f);
        }

        /// <summary>
        /// Context menu method to test the beer meter with different levels
        /// </summary>
        [ContextMenu("Test Beer Level 75%")]
        protected virtual void TestBeerLevel75()
        {
            SetBeerLevel(75f);
        }

        /// <summary>
        /// Context menu method to refresh the beer meter from BeerManager
        /// </summary>
        [ContextMenu("Refresh from BeerManager")]
        public virtual void RefreshFromBeerManager()
        {
            if (BeerManager.HasInstance)
            {
                float currentBeer = BeerManager.Instance.CurrentBeer;
                UpdateBeerMeterFill(currentBeer);
                UpdateBeerMeterColor(currentBeer);
                UpdateBeerPercentageText(currentBeer);
                Debug.Log($"Beer meter refreshed from BeerManager: {currentBeer:F1}%");
            }
            else
            {
                Debug.LogWarning("BeerManager not found! Cannot refresh beer meter.");
            }
        }

        /// <summary>
        /// Context menu method to test fill amount directly
        /// </summary>
        [ContextMenu("Test Fill 25%")]
        public virtual void TestFill25()
        {
            UpdateBeerMeterFill(25f);
            Debug.Log("BeerMeterUI: Testing 25% fill");
        }

        /// <summary>
        /// Context menu method to test fill amount directly
        /// </summary>
        [ContextMenu("Test Fill 50%")]
        public virtual void TestFill50()
        {
            UpdateBeerMeterFill(50f);
            Debug.Log("BeerMeterUI: Testing 50% fill");
        }

        /// <summary>
        /// Context menu method to test fill amount directly
        /// </summary>
        [ContextMenu("Test Fill 75%")]
        public virtual void TestFill75()
        {
            UpdateBeerMeterFill(75f);
            Debug.Log("BeerMeterUI: Testing 75% fill");
        }

        /// <summary>
        /// Context menu method to debug FillImage setup
        /// </summary>
        [ContextMenu("Debug FillImage Setup")]
        public virtual void DebugFillImageSetup()
        {
            if (FillImage == null)
            {
                Debug.LogError("BeerMeterUI: FillImage is NULL!");
                return;
            }

            Debug.Log($"BeerMeterUI: FillImage found - Type: {FillImage.type}, FillMethod: {FillImage.fillMethod}, FillAmount: {FillImage.fillAmount}");
            Debug.Log($"BeerMeterUI: FillImage GameObject: {FillImage.gameObject.name}, Active: {FillImage.gameObject.activeInHierarchy}");
            Debug.Log($"BeerMeterUI: FillImage Component Enabled: {FillImage.enabled}");
            Debug.Log($"BeerMeterUI: FillImage Sprite: {(FillImage.sprite != null ? FillImage.sprite.name : "NULL")}");
            Debug.Log($"BeerMeterUI: FillImage Color: {FillImage.color}");
        }

        /// <summary>
        /// Context menu method to fix FillImage setup
        /// </summary>
        [ContextMenu("Fix FillImage Setup")]
        public virtual void FixFillImageSetup()
        {
            if (FillImage == null)
            {
                Debug.LogError("BeerMeterUI: FillImage is NULL! Cannot fix.");
                return;
            }

            // Force proper Image settings
            FillImage.type = Image.Type.Filled;
            FillImage.fillMethod = Image.FillMethod.Horizontal;
            FillImage.fillOrigin = 0; // Left to right
            
            // Create a simple white texture if no sprite
            if (FillImage.sprite == null)
            {
                Texture2D whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();
                FillImage.sprite = Sprite.Create(whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                Debug.Log("BeerMeterUI: Created default white sprite for FillImage");
            }
            
            // Set a visible color
            FillImage.color = new Color(1f, 0.5f, 0f, 1f); // Orange color
            
            // Test with 50% fill
            FillImage.fillAmount = 0.5f;
            
            Debug.Log("BeerMeterUI: FillImage setup fixed - should now show 50% orange fill");
        }
    }
}
