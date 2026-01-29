using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles tap/click input on grid items.
/// Attach this to GridManager or create as separate InputManager.
/// Uses raycasting to detect which item was clicked.
/// </summary>
public class GridInputHandler : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Canvas containing the grid")]
    public Canvas canvas;
    
    [Tooltip("GridRoot RectTransform")]
    public RectTransform gridRoot;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;
    
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = Camera.main;
        
        if (canvas == null)
        {
            canvas = FindAnyObjectByType<Canvas>();
        }
        
        if (enableDebugLogs)
        {
            // Debug log removed

        }
    }
    
    private void Update()
    {
        // Check for mouse click or touch
        if (Input.GetMouseButtonDown(0))
        {
            // Debug log removed

            HandleInput(Input.mousePosition);
        }
        
        // Touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Debug log removed

            HandleInput(Input.GetTouch(0).position);
        }
    }
    
    /// <summary>
    /// Handles input at screen position.
    /// </summary>
    /// <param name="screenPosition">Screen position of input</param>
    private void HandleInput(Vector2 screenPosition)
    {
        // Raycast to find grid item
        GridItem clickedItem = RaycastForGridItem(screenPosition);
        
        if (clickedItem != null)
        {
            // Debug log removed

            clickedItem.OnTap();
        }
        else
        {
            // Debug log removed

        }
    }
    
    /// <summary>
    /// Raycasts to find GridItem at screen position.
    /// </summary>
    private GridItem RaycastForGridItem(Vector2 screenPosition)
    {
        // Method 1: Physics2D Raycast (for 2D colliders)
        if (mainCamera != null)
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            
            if (hit.collider != null)
            {
                // Debug log removed

                GridItem item = hit.collider.GetComponent<GridItem>();
                if (item != null) return item;
            }
        }
        
        // Method 2: UI Raycast (for UI-based items with Graphic Raycaster)
        if (EventSystem.current != null)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };
            
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            
            // Debug log removed

            
            foreach (var result in results)
            {
                // Debug log removed

                
                GridItem item = result.gameObject.GetComponent<GridItem>();
                if (item != null) return item;
                
                // Also check parent
                item = result.gameObject.GetComponentInParent<GridItem>();
                if (item != null) return item;
            }
        }
        else
        {
            if (enableDebugLogs) Debug.LogWarning("EventSystem.current is null! Make sure you have an EventSystem in the scene.");
        }
        
        return null;
    }
}
