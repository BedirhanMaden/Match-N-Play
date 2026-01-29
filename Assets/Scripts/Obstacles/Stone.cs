using UnityEngine;

/// <summary>
/// Stone obstacle.
/// Rules:
/// - Does NOT take damage from blast
/// - ONLY takes damage from rocket (1 damage)
/// - 1 health
/// - CANNOT fall
/// </summary>
public class Stone : Obstacle
{
    protected override void Awake()
    {
        maxHealth = 1;
        base.Awake();
    }
    
    /// <summary>
    /// Stone cannot fall.
    /// </summary>
    public override bool CanFall()
    {
        return false;
    }
    
    /// <summary>
    /// Does NOT take damage from blast.
    /// </summary>
    public override void OnBlastHit()
    {
        // Stone is not affected by blast
        // Debug log removed

    }
    
    /// <summary>
    /// Takes damage when hit by rocket.
    /// </summary>
    public override void OnRocketHit()
    {
        // Debug log removed

        TakeDamage(1);
    }
}
