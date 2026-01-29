using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Main manager of the grid system.
/// Responsibilities: Grid creation, Cell array, Fall algorithm.
/// Move count and Win/Lose control are NOT this class's responsibility.
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Parent transform where grid items will be spawned")]
    public RectTransform gridRoot;
    
    [Header("Prefabs")]
    public GameObject[] cubePrefabs;      // Red, Green, Blue, Yellow
    public GameObject rocketPrefab;
    public GameObject boxPrefab;
    public GameObject stonePrefab;
    public GameObject vasePrefab;
    
    [Header("Grid Settings")]
    [Tooltip("Pixel size of one cell")]
    public float cellSize = 100f;
    
    [Tooltip("Vertical offset from center (Positive = Up, Negative = Down)")]
    public float gridOffsetY = 0f;

    [Tooltip("Optional background image (includes frame?) to scale with grid")]
    public RectTransform gridBackground;

    [Tooltip("Extra padding around the grid for the background/frame")]
    public float backgroundPadding = 20f;
    
    /// <summary>Grid cells (2D array)</summary>
    private Cell[,] cells;
    
    /// <summary>Grid width</summary>
    private int gridWidth;
    
    /// <summary>Grid height</summary>
    private int gridHeight;
    
    /// <summary>All obstacles in scene</summary>
    private List<Obstacle> obstacles = new List<Obstacle>();
    
    /// <summary>Is input locked</summary>
    private bool inputLocked = false;
    
    /// <summary>Has game ended (Win or Lose)</summary>
    private bool gameEnded = false;

    public static GridManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Subscribe to Win/Lose events to stop game
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelWin += OnGameEnded;
            LevelManager.Instance.OnLevelLose += OnGameEnded;
        }
        
        // Get level data from LevelManager and create grid
        if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevelData != null)
        {
            InitializeGrid(LevelManager.Instance.CurrentLevelData);
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelWin -= OnGameEnded;
            LevelManager.Instance.OnLevelLose -= OnGameEnded;
        }
    }
    
    /// <summary>
    /// Called when game ends (Win or Lose).
    /// </summary>
    private void OnGameEnded()
    {
        gameEnded = true;
        inputLocked = true;
        // Debug log removed

    }
    
    /// <summary>
    /// Creates grid based on level data.
    /// </summary>
    public void InitializeGrid(LevelData levelData)
    {
        gridWidth = levelData.grid_width;
        gridHeight = levelData.grid_height;
        
        // Set grid root size and center it
        if (gridRoot != null)
        {
            gridRoot.sizeDelta = new Vector2(gridWidth * cellSize, gridHeight * cellSize);
            
            // Center the grid: set pivot to bottom-left and offset position
            gridRoot.pivot = new Vector2(0, 0);
            gridRoot.anchorMin = new Vector2(0.5f, 0.5f);
            gridRoot.anchorMax = new Vector2(0.5f, 0.5f);
            gridRoot.anchoredPosition = new Vector2(
                -(gridWidth * cellSize) / 2f,
                -(gridHeight * cellSize) / 2f + gridOffsetY
            );
        }
        
        // Resize Background if exists
        if (gridBackground != null)
        {
            float width = gridWidth * cellSize + backgroundPadding * 2;
            float height = gridHeight * cellSize + backgroundPadding * 2;
            gridBackground.sizeDelta = new Vector2(width, height);
            
            // Ensure it's behind the grid
            gridBackground.transform.SetAsFirstSibling();
        }
        
        // Create cell array
        cells = new Cell[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                cells[x, y] = new Cell(x, y);
            }
        }
        
        // Spawn items from JSON grid data
        SpawnItemsFromData(levelData.grid);
        
        // Initial hint check
        UpdateRocketHints();
        
        // Debug log removed

    }
    
    /// <summary>
    /// Scans the grid for 4+ matches and updates hint icons.
    /// Can be called externally (e.g. from Rocket).
    /// </summary>
    public void UpdateRocketHints()
    {
        // 1. First Pass: Reset ALL hints to false
        // This ensures no stale hints persist if logic skips them
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (cells[x, y] != null && cells[x, y].item is Cube c)
                {
                    c.ShowRocketHint(false);
                }
            }
        }
        
        // 2. Second Pass: Find clusters and enable hints where needed
        HashSet<Cube> visited = new HashSet<Cube>();
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (cells[x, y] == null || cells[x, y].IsEmpty()) continue;
                
                if (cells[x, y].item is Cube cube && !visited.Contains(cube))
                {
                    // Find cluster
                    List<Cube> cluster = FindMatchingCubes(cube);
                    bool showHint = cluster.Count >= 4;
                    
                    // Add to visited and update IF match found
                    foreach (Cube c in cluster)
                    {
                        visited.Add(c);
                        if (showHint)
                        {
                            c.ShowRocketHint(true);
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Spawns items from JSON grid array.
    /// Grid array starts from bottom-left.
    /// </summary>
    private void SpawnItemsFromData(List<string> gridData)
    {
        for (int i = 0; i < gridData.Count; i++)
        {
            // Convert index to coordinates (from bottom-left)
            int x = i % gridWidth;
            int y = i / gridWidth;
            
            if (x >= gridWidth || y >= gridHeight) continue;
            
            SpawnItem(gridData[i], x, y);
        }
    }
    
    /// <summary>
    /// Spawns item at specified cell.
    /// </summary>
    private void SpawnItem(string itemCode, int x, int y, float startYOffset = 0f)
    {
        GameObject prefab = GetPrefabByCode(itemCode);
        if (prefab == null) return;
        
        // Calculate target positions
        Vector3 targetPosition = new Vector3(
            x * cellSize + cellSize / 2f,
            y * cellSize + cellSize / 2f,
            0
        );
        
        // Determine start position
        Vector3 startPosition = targetPosition;
        if (startYOffset > 0)
        {
            startPosition += Vector3.up * startYOffset;
        }
        
        // Create item
        GameObject itemObj = Instantiate(prefab, gridRoot);
        itemObj.transform.localPosition = startPosition;
        
        // Get GridItem component and assign to cell
        GridItem gridItem = itemObj.GetComponent<GridItem>();
        if (gridItem != null)
        {
            Cell targetCell = cells[x, y];
            targetCell.SetItem(gridItem); // Set item to cell immediately logic-wise
            
            // If spawning with offset, trigger fall animation
            if (startYOffset > 0)
            {
                gridItem.MoveTo(targetCell, true);
            }
            else
            {
                // Ensure initial position is set correctly if no animation
                gridItem.MoveTo(targetCell, false);
            }
            
            // If obstacle, add to list
            
            // If obstacle, add to list
            if (gridItem is Obstacle obstacle)
            {
                obstacles.Add(obstacle);
            }
            
            // Set rocket direction if spawned from level data
            if (gridItem is Rocket rocket)
            {
                string lowerCode = itemCode.ToLower();
                if (lowerCode == "vro")
                {
                    rocket.direction = Rocket.RocketDirection.Vertical;
                }
                else if (lowerCode == "hro")
                {
                    rocket.direction = Rocket.RocketDirection.Horizontal;
                }
            }
        }
    }
    
    /// <summary>
    /// Returns prefab by item code.
    /// </summary>
    private GameObject GetPrefabByCode(string code)
    {
        switch (code.ToLower())
        {
            case "r": return cubePrefabs.Length > 0 ? cubePrefabs[0] : null; // Red
            case "g": return cubePrefabs.Length > 1 ? cubePrefabs[1] : null; // Green
            case "b": return cubePrefabs.Length > 2 ? cubePrefabs[2] : null; // Blue
            case "y": return cubePrefabs.Length > 3 ? cubePrefabs[3] : null; // Yellow
            case "bo": return boxPrefab;
            case "s": return stonePrefab;
            case "v": return vasePrefab;
            case "rand": return GetRandomCubePrefab();
            case "vro": return rocketPrefab; // Vertical Rocket
            case "hro": return rocketPrefab; // Horizontal Rocket
            default: return null;
        }
    }
    
    /// <summary>
    /// Returns random cube prefab.
    /// </summary>
    private GameObject GetRandomCubePrefab()
    {
        if (cubePrefabs == null || cubePrefabs.Length == 0) return null;
        return cubePrefabs[Random.Range(0, cubePrefabs.Length)];
    }
    
    /// <summary>
    /// Called when cube is tapped. Starts blast process.
    /// Rules: At least 2 adjacent same-color cubes required for blast.
    /// 4+ cubes creates a Rocket.
    /// </summary>
    public void OnCubeTapped(Cube tappedCube)
    {
        // Debug log removed

        
        // Block input if game has ended
        if (gameEnded)
        {
            // Debug log removed

            return;
        }
        
        if (inputLocked)
        {
            // Debug log removed

            return;
        }

        // Check moves
        if (LevelManager.Instance != null && LevelManager.Instance.RemainingMoves <= 0)
        {
            // Debug log removed

            return;
        }
        
        if (tappedCube == null || tappedCube.currentCell == null)
        {
            Debug.LogError("[GridManager.OnCubeTapped] TappedCube or its cell is NULL!");
            return;
        }
        
        // Find adjacent cubes (BFS)
        List<Cube> matchingCubes = FindMatchingCubes(tappedCube);
        // Debug log removed

        
        // At least 2 cubes required (including itself)
        if (matchingCubes.Count < 2)
        {
            // Debug log removed

            return;
        }
        
        inputLocked = true;
        // Debug log removed

        
        // Blast process - create rocket if 4+ cubes
        bool createRocket = matchingCubes.Count >= 4;
        // Debug log removed

        
        BlastCubes(matchingCubes, tappedCube, createRocket);
        
        // Update hints immediately after blast (before fall)
        UpdateRocketHints();
        
        // Use move
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.UseMove();
            // Debug log removed

        }
        
        // Fall algorithm
        Invoke(nameof(ExecuteFall), 0.2f);
    }
    
    /// <summary>
    /// Finds neighboring cubes of same color (BFS).
    /// </summary>
    private List<Cube> FindMatchingCubes(Cube startCube)
    {
        List<Cube> result = new List<Cube>();
        if (startCube == null || startCube.currentCell == null) return result;
        
        Queue<Cube> queue = new Queue<Cube>();
        HashSet<Cube> visited = new HashSet<Cube>();
        
        queue.Enqueue(startCube);
        visited.Add(startCube);
        
        while (queue.Count > 0)
        {
            Cube current = queue.Dequeue();
            result.Add(current);
            
            // Check 4 directions (NO diagonal)
            int[] dx = { 0, 0, 1, -1 };
            int[] dy = { 1, -1, 0, 0 };
            
            for (int i = 0; i < 4; i++)
            {
                int nx = current.currentCell.x + dx[i];
                int ny = current.currentCell.y + dy[i];
                
                if (nx < 0 || nx >= gridWidth || ny < 0 || ny >= gridHeight)
                    continue;
                
                Cell neighborCell = cells[nx, ny];
                if (neighborCell.item is Cube neighborCube && 
                    !visited.Contains(neighborCube) &&
                    neighborCube.cubeColor == startCube.cubeColor)
                {
                    visited.Add(neighborCube);
                    queue.Enqueue(neighborCube);
                }
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Blasts cubes and creates rocket if needed.
    /// </summary>
    private void BlastCubes(List<Cube> cubes, Cube tappedCube, bool createRocket)
    {
        Cell tappedCell = tappedCube.currentCell;
        
        // Damage adjacent obstacles
        DamageAdjacentObstacles(cubes);
        
        // Destroy cubes
        foreach (Cube cube in cubes)
        {
            cube.DestroyItem();
        }
        
        // Create rocket
        if (createRocket && rocketPrefab != null && tappedCell != null)
        {
            Vector3 position = new Vector3(
                tappedCell.x * cellSize + cellSize / 2f,
                tappedCell.y * cellSize + cellSize / 2f,
                0
            );
            
            GameObject rocketObj = Instantiate(rocketPrefab, gridRoot);
            rocketObj.transform.localPosition = position;
            
            Rocket rocket = rocketObj.GetComponent<Rocket>();
            if (rocket != null)
            {
                tappedCell.SetItem(rocket);
                
                // Assign random direction
                rocket.direction = Random.value > 0.5f 
                    ? Rocket.RocketDirection.Horizontal 
                    : Rocket.RocketDirection.Vertical;
            }
        }
    }
    
    /// <summary>
    /// Damages obstacles adjacent to blasted cubes.
    /// </summary>
    private void DamageAdjacentObstacles(List<Cube> blastedCubes)
    {
        HashSet<Obstacle> damagedObstacles = new HashSet<Obstacle>();
        
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };
        
        foreach (Cube cube in blastedCubes)
        {
            if (cube.currentCell == null) continue;
            
            for (int i = 0; i < 4; i++)
            {
                int nx = cube.currentCell.x + dx[i];
                int ny = cube.currentCell.y + dy[i];
                
                if (nx < 0 || nx >= gridWidth || ny < 0 || ny >= gridHeight)
                    continue;
                
                Cell neighborCell = cells[nx, ny];
                if (neighborCell.item is Obstacle obstacle && !damagedObstacles.Contains(obstacle))
                {
                    obstacle.OnBlastHit();
                    damagedObstacles.Add(obstacle);
                }
            }
        }
    }
    
    /// <summary>
    /// Executes fall algorithm.
    /// No Physics used, scans from bottom to top.
    /// </summary>
    public void ExecuteFall()
    {
        bool itemsMoved;
        
        do
        {
            itemsMoved = false;
            
            // Check each column (left to right)
            for (int x = 0; x < gridWidth; x++)
            {
                // Scan from bottom to top
                for (int y = 0; y < gridHeight; y++)
                {
                    Cell currentCell = cells[x, y];
                    
                    // If empty cell, look for falling item above
                    if (currentCell.IsEmpty())
                    {
                        // Look for fallable item in cells above
                        for (int aboveY = y + 1; aboveY < gridHeight; aboveY++)
                        {
                            Cell aboveCell = cells[x, aboveY];
                            
                            if (!aboveCell.IsEmpty() && aboveCell.item.CanFall())
                            {
                                // Drop item down
                                GridItem item = aboveCell.item;
                                aboveCell.Clear();
                                currentCell.SetItem(item);
                                item.MoveTo(currentCell, true);
                                
                                itemsMoved = true;
                                break;
                            }
                            // If non-fallable item exists, don't look higher in this column
                            else if (!aboveCell.IsEmpty() && !aboveCell.item.CanFall())
                            {
                                break;
                            }
                        }
                    }
                }
            }
        } while (itemsMoved); // Repeat until all items have fallen
        
        // Spawn new cubes at top empty cells
        SpawnNewCubesAtTop();
        
        // Check Win/Lose
        CheckGameState();
        
        // Update rocket hints
        UpdateRocketHints();
        
        inputLocked = false;
    }
    
    /// <summary>
    /// Spawns new cubes at top empty cells.
    /// </summary>
    private void SpawnNewCubesAtTop()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            // Track the stacking height for new items in this column
            // We start spawning just above the board (GridHeight)
            // As we fill empty spots from bottom to top, we increment this stack
            int spawnStackHeight = 0;
            
            // Iterate from BOTTOM to TOP to handle stacking correctly
            for (int y = 0; y < gridHeight; y++)
            {
                Cell cell = cells[x, y];
                
                if (cell.IsEmpty())
                {
                    // Check if there's a non-fallable obstacle above
                    // (Note: Since we iterate bottom-up, this check is slightly different but logic holds)
                    bool blockedAbove = false;
                    for (int checkY = y + 1; checkY < gridHeight; checkY++)
                    {
                        if (!cells[x, checkY].IsEmpty() && !cells[x, checkY].item.CanFall())
                        {
                            blockedAbove = true;
                            break;
                        }
                    }
                    
                    if (!blockedAbove)
                    {
                        // Calculate start Y position (Grid units)
                        // Base is GridHeight (just above top row)
                        // Add spawnStackHeight to stack them on top of each other
                        float spawnYIndex = gridHeight + spawnStackHeight;
                        spawnStackHeight++;
                        
                        // Calculate offset in pixels
                        // Offset = (SpawnY - TargetY) * CellSize
                        float startYOffset = (spawnYIndex - y) * cellSize;

                        // Spawn random cube with animation
                        SpawnItem("rand", x, y, startYOffset);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Checks game state (Win/Lose).
    /// </summary>
    private void CheckGameState()
    {
        // Remove destroyed obstacles from list
        obstacles.RemoveAll(o => o == null);
        
        // Notify LevelManager
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.CheckWinCondition(obstacles.Count);
        }
    }
    
    /// <summary>
    /// Returns cells for rocket explosion.
    /// </summary>
    public List<Cell> GetCellsInDirection(Cell startCell, bool horizontal)
    {
        List<Cell> result = new List<Cell>();
        
        if (horizontal)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                result.Add(cells[x, startCell.y]);
            }
        }
        else
        {
            for (int y = 0; y < gridHeight; y++)
            {
                result.Add(cells[startCell.x, y]);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets cell at specified coordinates.
    /// </summary>
    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return null;
        return cells[x, y];
    }
    
    /// <summary>
    /// Removes an obstacle from the list.
    /// </summary>
    public void RemoveObstacle(Obstacle obstacle)
    {
        obstacles.Remove(obstacle);
    }

    /// <summary>
    /// Replaces a random normal cube with a rocket.
    /// Used for "Moves to Rockets" celebration.
    /// </summary>
    public void ReplaceRandomCubeWithRocket()
    {
        List<Cell> cubeCells = new List<Cell>();
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (cells[x, y].item is Cube)
                {
                    cubeCells.Add(cells[x, y]);
                }
            }
        }
        
        if (cubeCells.Count > 0)
        {
            Cell targetCell = cubeCells[Random.Range(0, cubeCells.Count)];
            GridItem oldItem = targetCell.item;
            
            // Effect?
            // ParticleManager.Instance.SpawnTransformParticles(targetCell.transform.position);
            
            // Destroy OLD item (silently, no blast particles)
            Destroy(oldItem.gameObject);
            
            // Spawn Rocket
            if (rocketPrefab != null)
            {
                GameObject rocketObj = Instantiate(rocketPrefab, gridRoot);
                rocketObj.transform.localPosition = new Vector3(
                    targetCell.x * cellSize + cellSize / 2f,
                    targetCell.y * cellSize + cellSize / 2f,
                    0
                );
                
                Rocket rocket = rocketObj.GetComponent<Rocket>();
                targetCell.SetItem(rocket);
                
                // Random Direction
                rocket.direction = Random.value > 0.5f 
                    ? Rocket.RocketDirection.Horizontal 
                    : Rocket.RocketDirection.Vertical;
            }
        }
    }

    /// <summary>
    /// Explodes all rockets (and other specials if added later).
    /// Used for "Grand Finale".
    /// </summary>
    public float ExplodeAllSpecialItems()
    {
        List<Rocket> rockets = new List<Rocket>();
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (cells[x, y].item is Rocket rocket)
                {
                    rockets.Add(rocket);
                }
            }
        }
        
        // Explode them
        foreach (Rocket r in rockets)
        {
            r.Explode();
        }
        
        return rockets.Count > 0 ? 1.0f : 0f; // Return estimated wait time
    }
}
