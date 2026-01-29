using UnityEngine;

/// <summary>
/// Class representing a grid cell.
/// Each cell holds grid coordinates and item reference.
/// This class is NOT a GameObject, only holds data.
/// </summary>
[System.Serializable]
public class Cell
{
    /// <summary>Grid X coordinate (starts from 0, left to right)</summary>
    public int x;
    
    /// <summary>Grid Y coordinate (starts from 0, bottom to top)</summary>
    public int y;
    
    /// <summary>Item in this cell (Cube, Rocket, Obstacle or null)</summary>
    public GridItem item;
    
    /// <summary>
    /// Creates a new Cell.
    /// </summary>
    /// <param name="x">Grid X coordinate</param>
    /// <param name="y">Grid Y coordinate</param>
    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.item = null;
    }
    
    /// <summary>
    /// Checks if the cell is empty.
    /// </summary>
    public bool IsEmpty()
    {
        return item == null;
    }
    
    /// <summary>
    /// Clears the cell (sets item to null).
    /// </summary>
    public void Clear()
    {
        item = null;
    }
    
    /// <summary>
    /// Places a new item in the cell.
    /// </summary>
    public void SetItem(GridItem newItem)
    {
        item = newItem;
        if (newItem != null)
        {
            newItem.currentCell = this;
        }
    }
}
