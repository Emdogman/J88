using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MoreMountains.TopDownEngine;

namespace MoreMountains.TopDownEngine
{
    /// <summary>
    /// Manages the player's score and displays it on the UI
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [Header("Score Settings")]
        [Tooltip("Points awarded per enemy kill")]
        [SerializeField] private int pointsPerKill = 100;
        
        [Tooltip("Score text UI element (TextMeshPro or Legacy Text)")]
        [SerializeField] private TextMeshProUGUI scoreTextTMP;
        
        [Tooltip("Legacy Unity Text component (if not using TextMeshPro)")]
        [SerializeField] private Text scoreTextLegacy;
        
        [Tooltip("Prefix text before score number")]
        [SerializeField] private string scorePrefix = "Score: ";
        
        [Tooltip("Enemy tag to detect kills")]
        [SerializeField] private string enemyTag = "Enemy";
        
        [Tooltip("Enemy layer to detect kills")]
        [SerializeField] private LayerMask enemyLayer = -1;
        
        [Header("Debug")]
        [Tooltip("Enable debug logging")]
        [SerializeField] private bool debugMode = false;
        
        // Private fields
        private int _currentScore = 0;
        private bool _hasSubscribedToEnemies = false;
        
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ScoreManager Instance { get; private set; }
        
        /// <summary>
        /// Current score value
        /// </summary>
        public int CurrentScore => _currentScore;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        
        private void Start()
        {
            // Initialize score display
            UpdateScoreDisplay();
            
            // Subscribe to existing enemies
            SubscribeToAllEnemies();
            
            if (debugMode)
            {
                Debug.Log("ScoreManager: Initialized and ready");
            }
        }
        
        private void Update()
        {
            // Periodically check for new enemies that might have spawned
            if (Time.frameCount % 60 == 0) // Check every 60 frames (~1 second)
            {
                SubscribeToAllEnemies();
            }
        }
        
        /// <summary>
        /// Subscribes to all enemy Health components in the scene
        /// </summary>
        private void SubscribeToAllEnemies()
        {
            // Find all ChaserEnemy components
            ChaserEnemy[] enemies = FindObjectsOfType<ChaserEnemy>();
            
            foreach (var enemy in enemies)
            {
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    // Unsubscribe first to avoid duplicate subscriptions
                    enemyHealth.OnDeath -= OnEnemyKilled;
                    // Subscribe to death event
                    enemyHealth.OnDeath += OnEnemyKilled;
                }
            }
            
            if (debugMode && enemies.Length > 0)
            {
                Debug.Log($"ScoreManager: Subscribed to {enemies.Length} enemies");
            }
        }
        
        /// <summary>
        /// Called when an enemy is killed
        /// </summary>
        private void OnEnemyKilled()
        {
            AddScore(pointsPerKill);
        }
        
        /// <summary>
        /// Adds points to the score
        /// </summary>
        /// <param name="points">Points to add</param>
        public void AddScore(int points)
        {
            _currentScore += points;
            UpdateScoreDisplay();
            
            if (debugMode)
            {
                Debug.Log($"ScoreManager: Score increased by {points}. New score: {_currentScore}");
            }
        }
        
        /// <summary>
        /// Resets the score to zero
        /// </summary>
        public void ResetScore()
        {
            _currentScore = 0;
            UpdateScoreDisplay();
            
            if (debugMode)
            {
                Debug.Log("ScoreManager: Score reset to 0");
            }
        }
        
        /// <summary>
        /// Sets the score to a specific value
        /// </summary>
        /// <param name="score">New score value</param>
        public void SetScore(int score)
        {
            _currentScore = score;
            UpdateScoreDisplay();
            
            if (debugMode)
            {
                Debug.Log($"ScoreManager: Score set to {_currentScore}");
            }
        }
        
        /// <summary>
        /// Updates the score display text
        /// </summary>
        private void UpdateScoreDisplay()
        {
            string scoreText = scorePrefix + _currentScore.ToString();
            
            // Update TextMeshPro if available
            if (scoreTextTMP != null)
            {
                scoreTextTMP.text = scoreText;
            }
            
            // Update Legacy Text if available
            if (scoreTextLegacy != null)
            {
                scoreTextLegacy.text = scoreText;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from all enemies
            ChaserEnemy[] enemies = FindObjectsOfType<ChaserEnemy>();
            
            foreach (var enemy in enemies)
            {
                Health enemyHealth = enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.OnDeath -= OnEnemyKilled;
                }
            }
            
            // Clear singleton
            if (Instance == this)
            {
                Instance = null;
            }
        }
        
        [ContextMenu("Add 100 Points")]
        public void TestAddScore()
        {
            AddScore(100);
        }
        
        [ContextMenu("Reset Score")]
        public void TestResetScore()
        {
            ResetScore();
        }
        
        [ContextMenu("Show Debug Info")]
        public void ShowDebugInfo()
        {
            Debug.Log("=== Score Manager Debug Info ===");
            Debug.Log($"Current Score: {_currentScore}");
            Debug.Log($"Points Per Kill: {pointsPerKill}");
            Debug.Log($"TextMeshPro: {(scoreTextTMP != null ? "Found" : "Not Assigned")}");
            Debug.Log($"Legacy Text: {(scoreTextLegacy != null ? "Found" : "Not Assigned")}");
            
            ChaserEnemy[] enemies = FindObjectsOfType<ChaserEnemy>();
            Debug.Log($"Active Enemies: {enemies.Length}");
        }
    }
}

