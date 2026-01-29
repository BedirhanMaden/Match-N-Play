using UnityEngine;

/// <summary>
/// Base class for all grid items (Cube, Rocket, Obstacle).
/// Defined as an abstract class.
/// </summary>
public abstract class GridItem : MonoBehaviour
{
    /// <summary>Cell this item is in</summary>
    public Cell currentCell;
    
    /// <summary>World position of item (GridRoot local space)</summary>
    protected Vector3 targetPosition;
    
    /// <summary>Fall animation speed</summary>
    protected float fallSpeed = 10f;
    
    /// <summary>Is animation in progress</summary>
    protected bool isAnimating = false;
    
    /// <summary>
    /// Called when item is tapped.
    /// Subclasses can override this method.
    /// </summary>
    public virtual void OnTap()
    {
        // Default: do nothing
        // Debug log removed

    }
    
    /// <summary>
    /// Can this item fall?
    /// Cube and Rocket can fall, Box and Stone cannot, Vase can fall.
    /// </summary>
    public virtual bool CanFall()
    {
        return false; // Default: cannot fall
    }
    
    /// <summary>
    /// Called when item takes damage (for Obstacles).
    /// </summary>
    /// <param name="damageAmount">Amount of damage taken</param>
    public virtual void TakeDamage(int damageAmount = 1)
    {
        // Default: do nothing
    }
    
    /// <summary>
    /// Called when item is affected by blast.
    /// </summary>
    public virtual void OnBlastHit()
    {
        // Default: do nothing
    }
    
    /// <summary>
    /// Called when item is hit by rocket.
    /// </summary>
    public virtual void OnRocketHit()
    {
        // Default: do nothing
    }
    
    /// <summary>
    /// Destroys the item.
    /// </summary>
    public virtual void DestroyItem()
    {
        if (currentCell != null)
        {
            currentCell.Clear();
        }
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Moves item to specified cell.
    /// </summary>
    public virtual void MoveTo(Cell newCell, bool animate = true)
    {
        // Clear old cell
        if (currentCell != null)
        {
            currentCell.item = null;
        }
        
        // Assign to new cell
        currentCell = newCell;
        if (newCell != null)
        {
            newCell.item = this;
        }
        
        // Update position
        if (animate)
        {
            targetPosition = CalculateWorldPosition(newCell);
            isAnimating = true;
        }
        else
        {
            transform.localPosition = CalculateWorldPosition(newCell);
        }
    }
    
    /// <summary>
    /// Calculates world position of cell.
    /// </summary>
    protected Vector3 CalculateWorldPosition(Cell cell)
    {
        if (cell == null) return Vector3.zero;
        
        float cellSize = 100f; // In pixels
        float xPos = cell.x * cellSize + cellSize / 2f;
        float yPos = cell.y * cellSize + cellSize / 2f;
        
        return new Vector3(xPos, yPos, 0);
    }
    
    protected virtual void Update()
    {
        // Fall animation
        if (isAnimating)
        {
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition, 
                targetPosition, 
                fallSpeed * Time.deltaTime * 100f
            );
            
            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.1f)
            {
                transform.localPosition = targetPosition;
                isAnimating = false;
            }
        }
    }
}
