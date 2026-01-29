using UnityEngine;

/// <summary>
/// Vase obstacle.
/// Rules:
/// - Takes maximum 1 damage from blast
/// - Takes 1 damage from rocket
/// - Total 2 health
/// - CAN fall
/// </summary>
public class Vase : Obstacle
{
    [Header("Vase Sprites")]
    [Tooltip("Intact vase sprite")]
    public Sprite intactSprite;
    [Tooltip("Cracked vase sprite")]
    public Sprite crackedSprite;
    
    protected override void Awake()
    {
        maxHealth = 2;
        base.Awake();
    }
    
    protected override void Start()
    {
        base.Start();
        UpdateVisual();
    }
    
    /// <summary>
    /// Vase CAN fall.
    /// </summary>
    public override bool CanFall()
    {
        return true;
    }
    
    /// <summary>
    /// Takes maximum 1 damage from blast.
    /// </summary>
    public override void OnBlastHit()
    {
        // Debug log removed

        TakeDamage(1); // Blast only deals 1 damage
    }
    
    /// <summary>
    /// Takes 1 damage from rocket.
    /// </summary>
    public override void OnRocketHit()
    {
        // Debug log removed

        TakeDamage(1);
    }
    
    /// <summary>
    /// Visual is updated when damaged.
    /// </summary>
    public override void TakeDamage(int amount = 1)
    {
        base.TakeDamage(amount);
        UpdateVisual();
    }
    
    /// <summary>
    /// Updates sprite based on health status.
    /// </summary>
    private void UpdateVisual()
    {
        Sprite targetSprite = currentHealth >= 2 ? intactSprite : crackedSprite;
        
        if (targetSprite == null) return;
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = targetSprite;
        }
        
        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.sprite = targetSprite;
        }
    }
}
