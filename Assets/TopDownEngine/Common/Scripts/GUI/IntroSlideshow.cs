using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Displays a series of UI images as a slideshow on scene start
    /// Pauses the game until all slides are viewed
    /// Press Space to advance to the next slide
    /// 
    /// Default setup: 2 slides that display in sequence
    /// </summary>
    public class IntroSlideshow : MonoBehaviour
    {
        [Header("Slides")]
        [Tooltip("Array of UI Images to display in order (make sure they are children of this Canvas)")]
        public Image[] Slides;

        [Header("Settings")]
        [Tooltip("Key to press to advance to the next slide")]
        public KeyCode AdvanceKey = KeyCode.Space;

        [Tooltip("If true, the game time will be paused during the slideshow")]
        public bool PauseGameDuringSlides = true;

        [Tooltip("Fade duration between slides (0 for instant switch)")]
        public float FadeDuration = 0.3f;

        [Header("Optional Text Prompt")]
        [Tooltip("Optional text to show 'Press Space to Continue' (leave empty to disable)")]
        public Text PromptText;

        [Tooltip("The text to display in the prompt")]
        public string PromptMessage = "Press Space to Continue";

        [Header("UI Elements to Hide")]
        [Tooltip("Optional: UI elements to hide during the slideshow (like tutorial overlays)")]
        public GameObject[] UIElementsToHide;

        // Private variables
        private int _currentSlideIndex = 0;
        private bool _slideshowActive = false;
        private bool _isTransitioning = false;
        private CanvasGroup[] _slideCanvasGroups;
        private bool[] _originalUIElementStates;

        /// <summary>
        /// Initialize the slideshow on start
        /// </summary>
        protected virtual void Start()
        {
            // Validate slides
            if (Slides == null || Slides.Length == 0)
            {
                Debug.LogWarning("IntroSlideshow: No slides assigned! Slideshow will not run.");
                enabled = false;
                return;
            }

            // Setup canvas groups for fading
            SetupCanvasGroups();

            // Start the slideshow
            StartSlideshow();
        }

        /// <summary>
        /// Setup canvas groups for each slide to enable fading
        /// </summary>
        protected virtual void SetupCanvasGroups()
        {
            _slideCanvasGroups = new CanvasGroup[Slides.Length];

            for (int i = 0; i < Slides.Length; i++)
            {
                if (Slides[i] != null)
                {
                    // Get or add CanvasGroup component
                    _slideCanvasGroups[i] = Slides[i].GetComponent<CanvasGroup>();
                    if (_slideCanvasGroups[i] == null)
                    {
                        _slideCanvasGroups[i] = Slides[i].gameObject.AddComponent<CanvasGroup>();
                    }

                    // Hide all slides initially
                    Slides[i].gameObject.SetActive(false);
                    _slideCanvasGroups[i].alpha = 0f;
                }
            }
        }

        /// <summary>
        /// Start the slideshow
        /// </summary>
        protected virtual void StartSlideshow()
        {
            _slideshowActive = true;
            _currentSlideIndex = 0;

            // Hide specified UI elements
            HideUIElements();

            // Pause the game using TopDown Engine's pause system
            if (PauseGameDuringSlides)
            {
                if (GameManager.HasInstance)
                {
                    GameManager.Instance.Pause(PauseMethods.NoPauseMenu, false);
                    Debug.Log($"IntroSlideshow: Game PAUSED using GameManager - Paused = {GameManager.Instance.Paused}");
                }
                else
                {
                    Debug.LogWarning("IntroSlideshow: GameManager not found! Using Time.timeScale fallback.");
                    Time.timeScale = 0f;
                }
            }
            else
            {
                Debug.LogWarning("IntroSlideshow: PauseGameDuringSlides is FALSE - game will NOT pause!");
            }

            // Show the first slide
            ShowSlide(_currentSlideIndex);

            // Update prompt text
            UpdatePromptText();
        }

        /// <summary>
        /// Show a specific slide
        /// </summary>
        /// <param name="index">The slide index to show</param>
        protected virtual void ShowSlide(int index)
        {
            if (index < 0 || index >= Slides.Length)
            {
                return;
            }

            // Activate the slide
            Slides[index].gameObject.SetActive(true);

            // Fade in
            if (FadeDuration > 0f)
            {
                StartCoroutine(FadeInSlide(index));
            }
            else
            {
                _slideCanvasGroups[index].alpha = 1f;
            }

            Debug.Log($"IntroSlideshow: Showing slide {index + 1}/{Slides.Length}");
        }

        /// <summary>
        /// Hide a specific slide
        /// </summary>
        /// <param name="index">The slide index to hide</param>
        protected virtual void HideSlide(int index)
        {
            if (index < 0 || index >= Slides.Length)
            {
                return;
            }

            // Fade out (but disable immediately so it's not interactive)
            if (FadeDuration > 0f)
            {
                // Start fade but disable GameObject immediately
                StartCoroutine(FadeOutAndDisableSlide(index));
            }
            else
            {
                _slideCanvasGroups[index].alpha = 0f;
                Slides[index].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Fade in a slide
        /// </summary>
        protected virtual IEnumerator FadeInSlide(int index)
        {
            float elapsed = 0f;
            CanvasGroup canvasGroup = _slideCanvasGroups[index];

            while (elapsed < FadeDuration)
            {
                elapsed += Time.unscaledDeltaTime; // Use unscaled time for paused game
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / FadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// Fade out a slide and disable it immediately
        /// </summary>
        protected virtual IEnumerator FadeOutAndDisableSlide(int index)
        {
            CanvasGroup canvasGroup = _slideCanvasGroups[index];
            
            // Immediately disable the GameObject (it becomes invisible/non-interactive)
            Slides[index].gameObject.SetActive(false);
            canvasGroup.alpha = 0f;
            
            Debug.Log($"IntroSlideshow: Slide {index + 1} disabled immediately");
            
            yield return null;
        }
        
        /// <summary>
        /// Fade out a slide (kept for compatibility)
        /// </summary>
        protected virtual IEnumerator FadeOutSlide(int index)
        {
            float elapsed = 0f;
            CanvasGroup canvasGroup = _slideCanvasGroups[index];

            while (elapsed < FadeDuration)
            {
                elapsed += Time.unscaledDeltaTime; // Use unscaled time for paused game
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / FadeDuration);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            Slides[index].gameObject.SetActive(false);
        }

        /// <summary>
        /// Update prompt text
        /// </summary>
        protected virtual void UpdatePromptText()
        {
            if (PromptText != null)
            {
                PromptText.text = PromptMessage;
                PromptText.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Handle input to advance slides
        /// </summary>
        protected virtual void Update()
        {
            if (!_slideshowActive || _isTransitioning)
            {
                return;
            }

            // Check for advance key press
            if (Input.GetKeyDown(AdvanceKey))
            {
                AdvanceSlide();
            }
        }

        /// <summary>
        /// Advance to the next slide or end the slideshow
        /// </summary>
        protected virtual void AdvanceSlide()
        {
            _isTransitioning = true;

            // Hide current slide
            HideSlide(_currentSlideIndex);

            // Move to next slide
            _currentSlideIndex++;

            // Check if we've reached the end
            if (_currentSlideIndex >= Slides.Length)
            {
                // End slideshow
                StartCoroutine(EndSlideshow());
            }
            else
            {
                // Show next slide
                StartCoroutine(TransitionToNextSlide());
            }
        }

        /// <summary>
        /// Transition to the next slide
        /// </summary>
        protected virtual IEnumerator TransitionToNextSlide()
        {
            // Wait for fade out to complete
            if (FadeDuration > 0f)
            {
                yield return new WaitForSecondsRealtime(FadeDuration);
            }

            // Show next slide
            ShowSlide(_currentSlideIndex);

            _isTransitioning = false;
        }

        /// <summary>
        /// End the slideshow and unpause the game
        /// </summary>
        protected virtual IEnumerator EndSlideshow()
        {
            // Wait for fade out to complete
            if (FadeDuration > 0f)
            {
                yield return new WaitForSecondsRealtime(FadeDuration);
            }

            _slideshowActive = false;

            // Hide ALL slides (including background)
            HideAllSlides();

            // Hide prompt text
            if (PromptText != null)
            {
                PromptText.gameObject.SetActive(false);
            }

            // Restore hidden UI elements
            RestoreUIElements();

            // Unpause the game using TopDown Engine's pause system
            if (PauseGameDuringSlides)
            {
                if (GameManager.HasInstance)
                {
                    GameManager.Instance.UnPause(PauseMethods.NoPauseMenu);
                    Debug.Log($"IntroSlideshow: Game UNPAUSED using GameManager - Paused = {GameManager.Instance.Paused}");
                }
                else
                {
                    Debug.LogWarning("IntroSlideshow: GameManager not found! Using Time.timeScale fallback.");
                    Time.timeScale = 1f;
                }
            }

            // Disable this component
            enabled = false;
        }

        /// <summary>
        /// Public method to skip the entire slideshow
        /// </summary>
        public virtual void SkipSlideshow()
        {
            if (!_slideshowActive)
            {
                return;
            }

            // Hide ALL slides (including background)
            HideAllSlides();

            // End immediately
            _slideshowActive = false;
            _isTransitioning = false;

            // Hide prompt
            if (PromptText != null)
            {
                PromptText.gameObject.SetActive(false);
            }

            // Restore hidden UI elements
            RestoreUIElements();

            // Unpause game using TopDown Engine's pause system
            if (PauseGameDuringSlides)
            {
                if (GameManager.HasInstance)
                {
                    GameManager.Instance.UnPause(PauseMethods.NoPauseMenu);
                    Debug.Log($"IntroSlideshow: Game UNPAUSED (skipped) using GameManager - Paused = {GameManager.Instance.Paused}");
                }
                else
                {
                    Debug.LogWarning("IntroSlideshow: GameManager not found! Using Time.timeScale fallback.");
                    Time.timeScale = 1f;
                }
            }

            enabled = false;
        }

        /// <summary>
        /// Hide specified UI elements at slideshow start
        /// </summary>
        protected virtual void HideUIElements()
        {
            if (UIElementsToHide == null || UIElementsToHide.Length == 0)
            {
                return;
            }

            _originalUIElementStates = new bool[UIElementsToHide.Length];

            for (int i = 0; i < UIElementsToHide.Length; i++)
            {
                if (UIElementsToHide[i] != null)
                {
                    // Store original state
                    _originalUIElementStates[i] = UIElementsToHide[i].activeSelf;
                    
                    // Hide the element
                    UIElementsToHide[i].SetActive(false);
                    Debug.Log($"IntroSlideshow: Hidden UI element '{UIElementsToHide[i].name}'");
                }
            }
        }

        /// <summary>
        /// Restore UI elements to their original state
        /// </summary>
        protected virtual void RestoreUIElements()
        {
            if (UIElementsToHide == null || UIElementsToHide.Length == 0 || _originalUIElementStates == null)
            {
                return;
            }

            for (int i = 0; i < UIElementsToHide.Length; i++)
            {
                if (UIElementsToHide[i] != null && i < _originalUIElementStates.Length)
                {
                    // Restore original state
                    UIElementsToHide[i].SetActive(_originalUIElementStates[i]);
                    Debug.Log($"IntroSlideshow: Restored UI element '{UIElementsToHide[i].name}' to {_originalUIElementStates[i]}");
                }
            }
        }

        /// <summary>
        /// Hide all slides including background
        /// </summary>
        protected virtual void HideAllSlides()
        {
            for (int i = 0; i < Slides.Length; i++)
            {
                if (Slides[i] != null)
                {
                    Slides[i].gameObject.SetActive(false);
                    _slideCanvasGroups[i].alpha = 0f;
                }
            }
            Debug.Log("IntroSlideshow: All slides hidden");
        }
    }
}



