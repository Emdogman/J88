using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Simple button manager for main menu functionality
    /// Handles play game and quit game button actions
    /// </summary>
    [AddComponentMenu("TopDown Engine/UI/Main Menu Button Manager")]
    public class MainMenuButtonManager : TopDownMonoBehaviour
    {
        [Header("Scene Settings")]
        /// <summary>
        /// The name of the scene to load when Play Game is pressed
        /// </summary>
        [Tooltip("The name of the scene to load when Play Game is pressed")]
        public string GameSceneName = "GameScene";
        
        [Header("Button References")]
        /// <summary>
        /// Reference to the Play Game button (optional - for direct assignment)
        /// </summary>
        [Tooltip("Reference to the Play Game button (optional - for direct assignment)")]
        public Button PlayGameButton;
        
        /// <summary>
        /// Reference to the Quit Game button (optional - for direct assignment)
        /// </summary>
        [Tooltip("Reference to the Quit Game button (optional - for direct assignment)")]
        public Button QuitGameButton;
        
        [Header("Debug")]
        /// <summary>
        /// Show debug information in the console
        /// </summary>
        [Tooltip("Show debug information in the console")]
        public bool ShowDebugInfo = false;
        
        /// <summary>
        /// Initialization
        /// </summary>
        protected virtual void Start()
        {
            // Auto-assign buttons if not manually assigned
            if (PlayGameButton == null)
            {
                PlayGameButton = GameObject.Find("PlayGameButton")?.GetComponent<Button>();
            }
            
            if (QuitGameButton == null)
            {
                QuitGameButton = GameObject.Find("QuitGameButton")?.GetComponent<Button>();
            }
            
            // Subscribe to button events if buttons are found
            if (PlayGameButton != null)
            {
                PlayGameButton.onClick.AddListener(PlayGame);
                if (ShowDebugInfo)
                {
                    Debug.Log("MainMenuButtonManager: Play Game button connected");
                }
            }
            else if (ShowDebugInfo)
            {
                Debug.LogWarning("MainMenuButtonManager: Play Game button not found! Make sure to assign it manually or name it 'PlayGameButton'");
            }
            
            if (QuitGameButton != null)
            {
                QuitGameButton.onClick.AddListener(QuitGame);
                if (ShowDebugInfo)
                {
                    Debug.Log("MainMenuButtonManager: Quit Game button connected");
                }
            }
            else if (ShowDebugInfo)
            {
                Debug.LogWarning("MainMenuButtonManager: Quit Game button not found! Make sure to assign it manually or name it 'QuitGameButton'");
            }
        }
        
        /// <summary>
        /// Called when Play Game button is pressed
        /// Loads the specified game scene
        /// </summary>
        public virtual void PlayGame()
        {
            if (ShowDebugInfo)
            {
                Debug.Log($"MainMenuButtonManager: Play Game pressed - Loading scene: {GameSceneName}");
            }
            
            // Use TopDownEngine's scene loading system
            if (!string.IsNullOrEmpty(GameSceneName))
            {
                MMSceneLoadingManager.LoadScene(GameSceneName);
            }
            else
            {
                Debug.LogError("MainMenuButtonManager: GameSceneName is not set! Please assign a scene name in the inspector.");
            }
        }
        
        /// <summary>
        /// Called when Quit Game button is pressed
        /// Quits the application
        /// </summary>
        public virtual void QuitGame()
        {
            if (ShowDebugInfo)
            {
                Debug.Log("MainMenuButtonManager: Quit Game pressed - Exiting application");
            }
            
            // Use TopDownEngine's quit functionality
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        
        /// <summary>
        /// Clean up button listeners when destroyed
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (PlayGameButton != null)
            {
                PlayGameButton.onClick.RemoveListener(PlayGame);
            }
            
            if (QuitGameButton != null)
            {
                QuitGameButton.onClick.RemoveListener(QuitGame);
            }
        }
        
        /// <summary>
        /// Test method for Play Game (can be called from inspector)
        /// </summary>
        [ContextMenu("Test Play Game")]
        public virtual void TestPlayGame()
        {
            PlayGame();
        }
        
        /// <summary>
        /// Test method for Quit Game (can be called from inspector)
        /// </summary>
        [ContextMenu("Test Quit Game")]
        public virtual void TestQuitGame()
        {
            QuitGame();
        }
    }
}
