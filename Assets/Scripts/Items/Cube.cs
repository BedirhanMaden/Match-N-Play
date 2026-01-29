using UnityEngine;

/// <summary>
/// Cube item. Colored and blastable.
/// Rules:
/// - Blast with at least 2 neighboring cubes
/// - Creates Rocket if 4+ cubes merge
/// - Only horizontal and vertical neighbors count (NO diagonal)
/// </summary>
public class Cube : GridItem
{
    /// <summary>Cube colors</summary>
    public enum CubeColor
    {
        Red,
        Green,
        Blue,
        Yellow
    }
    
    [Header("Cube Settings")]
    [Tooltip("Color of this cube")]
    public CubeColor cubeColor = CubeColor.Red;
    
    [Header("Rocket Hint")]
    [Tooltip("Rocket icon shown when 4+ match exists")]
    public GameObject rocketHintIcon;
    
    /// <summary>GridManager reference</summary>
    private GridManager gridManager;
    
    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
    }
    
    /// <summary>
    /// Cube can fall.
    /// </summary>
    public override bool CanFall()
    {
        return true;
    }
    
    /// <summary>
    /// Blast process starts when cube is tapped.
    /// Called by Button onClick event.
    /// </summary>
    public override void OnTap()
    {
        // Debug log removed

        
        base.OnTap();
        
        if (gridManager != null)
        {
            // Debug log removed

            gridManager.OnCubeTapped(this);
        }
        else
        {
            Debug.LogError("[Cube.OnTap] GridManager is NULL! Cannot process tap.");
            // Try to find GridManager again
            gridManager = FindAnyObjectByType<GridManager>();
            if (gridManager != null)
            {
                // Debug log removed

                gridManager.OnCubeTapped(this);
            }
        }
    }
    
    /// <summary>
    /// Shows or hides rocket hint.
    /// </summary>
    public void ShowRocketHint(bool show)
    {
        if (rocketHintIcon != null)
        {
            rocketHintIcon.SetActive(show);
        }
    }
    
    /// <summary>
    /// When hit by rocket, cube is destroyed.
    /// </summary>
    public override void OnRocketHit()
    {
        // Debug log removed

        DestroyItem();
    }
    
    /// <summary>
    /// Destroys cube with particle effect.
    /// </summary>
    public override void DestroyItem()
    {
        // Spawn particles before destroying
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnCubeParticles(transform.position, cubeColor);
        }
        
        base.DestroyItem();
    }
}
