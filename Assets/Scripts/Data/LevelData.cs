using System.Collections.Generic;

/// <summary>
/// JSON model class that holds level data.
/// This class ONLY holds data, contains no logic.
/// </summary>
[System.Serializable]
public class LevelData
{
    /// <summary>Level number (1, 2, 3...)</summary>
    public int level_number;
    
    /// <summary>Grid width (number of cells)</summary>
    public int grid_width;
    
    /// <summary>Grid height (number of cells)</summary>
    public int grid_height;
    
    /// <summary>Total move count for the player</summary>
    public int move_count;
    
    /// <summary>
    /// Grid content array.
    /// Starts from bottom-left, goes row by row upwards.
    /// Values: "r" (red), "g" (green), "b" (blue), "y" (yellow), 
    /// "bo" (box), "s" (stone), "v" (vase), "rand" (random color),
    /// "vro" (vertical rocket), "hro" (horizontal rocket)
    /// </summary>
    public List<string> grid;
}
