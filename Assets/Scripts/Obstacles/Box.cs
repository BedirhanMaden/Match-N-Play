using UnityEngine;

/// <summary>
/// Box obstacle.
/// Rules:
/// - Takes damage from blast (1 damage)
/// - Takes damage from rocket (1 damage)
/// - 1 health
/// - CANNOT fall
/// </summary>
public class Box : Obstacle
{
    protected override void Awake()
    {
        maxHealth = 1;
        base.Awake();
    }
    
    /// <summary>
    /// Box cannot fall.
    /// </summary>
    public override bool CanFall()
    {
        return false;
    }
    
    /// <summary>
    /// Takes damage when hit by blast.
    /// </summary>
    public override void OnBlastHit()
    {
        // Debug log removed

        TakeDamage(1);
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
