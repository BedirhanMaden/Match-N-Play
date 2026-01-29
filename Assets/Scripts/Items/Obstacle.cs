using UnityEngine;

/// <summary>
/// Base class for all obstacles.
/// Defined as an abstract class.
/// Each obstacle type inherits from this class.
/// </summary>
public abstract class Obstacle : GridItem
{
    [Header("Obstacle Settings")]
    [Tooltip("Starting health")]
    public int maxHealth = 1;
    
    [Tooltip("Current health")]
    protected int currentHealth;
    
    [Header("Damage Sprites")]
    [Tooltip("Damaged appearance sprite (optional)")]
    public Sprite damagedSprite;
    
    /// <summary>GridManager reference</summary>
    protected GridManager gridManager;
    
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }
    
    protected virtual void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
    }
    
    /// <summary>
    /// Deals damage to obstacle.
    /// </summary>
    /// <param name="amount">Damage amount</param>
    public override void TakeDamage(int amount = 1)
    {
        currentHealth -= amount;
        // Debug log removed

        
        // Switch to damaged sprite
        if (currentHealth > 0 && damagedSprite != null)
        {
            UpdateSpriteToDamaged();
        }
        
        // If health depleted, destroy
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Updates sprite to damaged version.
    /// </summary>
    protected virtual void UpdateSpriteToDamaged()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && damagedSprite != null)
        {
            sr.sprite = damagedSprite;
        }
        
        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
        if (img != null && damagedSprite != null)
        {
            img.sprite = damagedSprite;
        }
    }
    
    /// <summary>
    /// Called when obstacle dies.
    /// </summary>
    protected virtual void Die()
    {
        // Debug log removed

        
        // Spawn particles
        if (ParticleManager.Instance != null)
        {
            string typeCode = GetObstacleTypeCode();
            ParticleManager.Instance.SpawnObstacleParticles(transform.position, typeCode);
        }
        
        // Notify GoalUI about obstacle destruction
        NotifyGoalUI();
        
        // Remove from GridManager
        if (gridManager != null)
        {
            gridManager.RemoveObstacle(this);
        }
        
        DestroyItem();
    }
    
    /// <summary>
    /// Notifies GoalUI that this obstacle was destroyed.
    /// </summary>
    protected virtual void NotifyGoalUI()
    {
        GoalUI goalUI = FindAnyObjectByType<GoalUI>();
        if (goalUI != null)
        {
            // Determine obstacle type code
            string typeCode = GetObstacleTypeCode();
            goalUI.OnObstacleDestroyed(typeCode);
            // Debug log removed

        }
    }
    
    /// <summary>
    /// Returns the type code for this obstacle (bo, s, v).
    /// Override in subclasses.
    /// </summary>
    protected virtual string GetObstacleTypeCode()
    {
        // Default implementation - try to determine from class name
        string className = GetType().Name.ToLower();
        if (className.Contains("box")) return "bo";
        if (className.Contains("stone")) return "s";
        if (className.Contains("vase")) return "v";
        return "";
    }
    
    /// <summary>
    /// When affected by blast. Subclasses can override.
    /// </summary>
    public override void OnBlastHit()
    {
        // Default: do nothing
        // Box and Vase will override
    }
    
    /// <summary>
    /// When hit by rocket.
    /// </summary>
    public override void OnRocketHit()
    {
        TakeDamage(1);
    }
}
