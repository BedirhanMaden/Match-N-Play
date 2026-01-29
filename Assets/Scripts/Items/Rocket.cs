using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Rocket item. Explodes horizontally or vertically when tapped.
/// Rules:
/// - Created when 4+ cubes merge
/// - Explodes on tap
/// - If hits another rocket, that rocket also explodes
/// - If two rockets are adjacent, 3x3 combo
/// </summary>
public class Rocket : GridItem
{
    /// <summary>Rocket direction</summary>
    public enum RocketDirection
    {
        Horizontal,
        Vertical
    }
    
    [Header("Rocket Settings")]
    [Tooltip("Explosion direction of rocket")]
    public RocketDirection direction = RocketDirection.Horizontal;
    
    [Header("Sprites")]
    [Tooltip("Horizontal rocket sprite")]
    public Sprite horizontalSprite;
    [Tooltip("Vertical rocket sprite")]
    public Sprite verticalSprite;
    
    /// <summary>GridManager reference</summary>
    private GridManager gridManager;
    
    /// <summary>Has explosion already started</summary>
    private bool hasExploded = false;
    
    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        UpdateSprite();
    }
    
    /// <summary>
    /// Rocket can fall.
    /// </summary>
    public override bool CanFall()
    {
        return true;
    }
    
    /// <summary>
    /// Updates sprite based on direction.
    /// </summary>
    private void UpdateSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = direction == RocketDirection.Horizontal 
                ? horizontalSprite 
                : verticalSprite;
        }
        
        // For UI Image
        UnityEngine.UI.Image img = GetComponent<UnityEngine.UI.Image>();
        if (img != null)
        {
            img.sprite = direction == RocketDirection.Horizontal 
                ? horizontalSprite 
                : verticalSprite;
        }
    }
    
    /// <summary>
    /// Explosion starts when rocket is tapped.
    /// </summary>
    public override void OnTap()
    {
        base.OnTap();
        Explode(true); // Player tapped: Use move
    }
    
    /// <summary>
    /// When rocket is hit by another rocket.
    /// </summary>
    public override void OnRocketHit()
    {
        Explode(); // Chain reaction: Do NOT use move
    }
    
    /// <summary>
    /// Performs the explosion.
    /// </summary>
    /// <param name="fromTap">If true, consumes a move.</param>
    public void Explode(bool fromTap = false)
    {
        if (hasExploded) return;
        hasExploded = true;
        
        // Debug log removed

        
        // Spawn explosion particles
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.SpawnRocketParticles(transform.position);
            
            // Spawn flying heads animation
            bool isHorizontal = direction == RocketDirection.Horizontal;
            ParticleManager.Instance.SpawnRocketHeads(transform.position, isHorizontal);
        }
        
        if (gridManager == null || currentCell == null)
        {
            DestroyItem();
            return;
        }
        
        // First check for adjacent rockets (Combo)
        bool hasCombo = CheckForCombo();
        
        if (hasCombo)
        {
            // 3x3 combo explosion
            ExplodeCombo();
        }
        else
        {
            // Normal single direction explosion
            ExplodeSingleDirection();
        }
        
        // Use move (ONLY if player tapped manually)
        if (fromTap && LevelManager.Instance != null)
        {
            LevelManager.Instance.UseMove();
        }
        
        // Destroy self
        DestroyItem();
        
        // Update hints immediately (stale hints removal)
        gridManager.UpdateRocketHints();
        
        // Execute fall algorithm
        gridManager.Invoke(nameof(GridManager.ExecuteFall), 0.3f);
    }
    
    /// <summary>
    /// Checks if there's an adjacent rocket.
    /// </summary>
    private bool CheckForCombo()
    {
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };
        
        for (int i = 0; i < 4; i++)
        {
            int nx = currentCell.x + dx[i];
            int ny = currentCell.y + dy[i];
            
            Cell neighborCell = gridManager.GetCell(nx, ny);
            if (neighborCell != null && neighborCell.item is Rocket otherRocket && !otherRocket.hasExploded)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 3x3 combo explosion.
    /// </summary>
    private void ExplodeCombo()
    {
        // Debug log removed

        
        // Hit all items in 3x3 area
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                Cell cell = gridManager.GetCell(currentCell.x + dx, currentCell.y + dy);
                if (cell != null && cell.item != null && cell.item != this)
                {
                    if (cell.item is Rocket otherRocket)
                    {
                        otherRocket.Explode();
                    }
                    else
                    {
                        cell.item.OnRocketHit();
                    }
                }
            }
        }
        
        // Also explode horizontally and vertically
        ExplodeLine(true);  // Horizontal
        ExplodeLine(false); // Vertical
    }
    
    /// <summary>
    /// Single direction explosion.
    /// </summary>
    private void ExplodeSingleDirection()
    {
        ExplodeLine(direction == RocketDirection.Horizontal);
    }
    
    /// <summary>
    /// Explosion effect in specified direction.
    /// </summary>
    private void ExplodeLine(bool horizontal)
    {
        List<Cell> cells = gridManager.GetCellsInDirection(currentCell, horizontal);
        
        foreach (Cell cell in cells)
        {
            if (cell.item != null && cell.item != this)
            {
                if (cell.item is Rocket otherRocket && !otherRocket.hasExploded)
                {
                    otherRocket.Explode();
                }
                else
                {
                    cell.item.OnRocketHit();
                }
            }
        }
    }
}
