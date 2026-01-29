using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages particle effects for cube blasts, obstacle destruction, and rocket explosions.
/// Singleton pattern - place on a GameObject in the scene.
/// </summary>
public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }
    
    [Header("Particle Settings")]
    [Tooltip("Particle prefab to instantiate")]
    public GameObject particlePrefab;
    
    [Tooltip("Number of particles per effect")]
    public int particlesPerEffect = 8;
    
    [Tooltip("Particle size in pixels")]
    public float particleSize = 40f;
    
    [Tooltip("Particle spread radius")]
    public float spreadRadius = 50f;
    
    [Tooltip("Particle lifetime")]
    public float particleLifetime = 0.5f;
    
    [Tooltip("Particle move speed")]
    public float particleSpeed = 200f;
    
    [Header("Color Particles")]
    public Sprite redParticle;
    public Sprite greenParticle;
    public Sprite blueParticle;
    public Sprite yellowParticle;
    
    [Header("Obstacle Particles (Multiple variants)")]
    [Tooltip("Box particles - all 3 variants")]
    public Sprite[] boxParticles;
    
    [Tooltip("Stone particles - all 3 variants")]
    public Sprite[] stoneParticles;
    
    [Tooltip("Vase particles - all 3 variants")]
    public Sprite[] vaseParticles;
    

    
    [Header("Celebration Settings")]
    [Tooltip("Particle sprite for celebration")]
    public Sprite celebrationParticle;

    [Tooltip("Size of celebration particles")]
    public float celebrationParticleSize = 60f;
    
    [Tooltip("Speed of celebration particles")]
    public float celebrationParticleSpeed = 300f;
    
    [Tooltip("Number of particles per celebration burst")]
    public int celebrationParticlesPerBurst = 20;
    
    [Header("Rocket Settings")]
    [Tooltip("Explosion Particle")]
    public Sprite rocketParticle;
    [Tooltip("Smoke Particle")]
    public Sprite smokeParticle;

    [Space(10)]
    [Tooltip("Rocket Head Horizontal Left")]
    public Sprite horizontalRocketLeft;
    [Tooltip("Rocket Head Horizontal Right")]
    public Sprite horizontalRocketRight;
    [Tooltip("Rocket Head Vertical Top")]
    public Sprite verticalRocketTop;
    [Tooltip("Rocket Head Vertical Bottom")]
    public Sprite verticalRocketBottom;
    
    [Tooltip("Rocket Head Speed")]
    public float rocketHeadSpeed = 500f;
    [Tooltip("Rocket Head Size")]
    public float rocketHeadSize = 80f;

    [Tooltip("Prefab for Rocket Trail (ParticleSystem)")]
    public GameObject rocketTrailPrefab; // NEW: Replaces manual spawning if assigned

    [Header("Rendering Settings")]
    [Tooltip("Target Canvas for UI particles. Assign manually to ensure correct sorting.")]
    public Canvas targetCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
            // Auto-find canvas if not assigned, to act as fallback
            if (targetCanvas == null)
            {
                targetCanvas = FindAnyObjectByType<Canvas>();
                if (targetCanvas == null)
                {
                    Debug.LogWarning("[ParticleManager] No Canvas found in scene! Particles may not be visible.");
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Spawns particles at position with specified sprite.
    /// </summary>
    public void SpawnParticles(Vector3 worldPosition, Sprite particleSprite, int count = -1, float sizeOverride = -1, float speedOverride = -1)
    {
        if (particleSprite == null) return;
        
        int particleCount = count > 0 ? count : particlesPerEffect;
        
        for (int i = 0; i < particleCount; i++)
        {
            SpawnSingleParticle(worldPosition, particleSprite, sizeOverride, speedOverride);
        }
    }
    
    /// <summary>
    /// Spawns particles using random sprites from an array.
    /// </summary>
    public void SpawnParticlesRandomized(Vector3 worldPosition, Sprite[] sprites, int count = -1)
    {
        if (sprites == null || sprites.Length == 0) return;
        
        int particleCount = count > 0 ? count : particlesPerEffect;
        
        for (int i = 0; i < particleCount; i++)
        {
            // Pick random sprite from array
            Sprite randomSprite = sprites[Random.Range(0, sprites.Length)];
            SpawnSingleParticle(worldPosition, randomSprite);
        }
    }
    
    /// <summary>
    /// Spawns cube blast particles.
    /// </summary>
    public void SpawnCubeParticles(Vector3 position, Cube.CubeColor color)
    {
        Sprite sprite = GetCubeParticleSprite(color);
        SpawnParticles(position, sprite);
    }
    
    /// <summary>
    /// Spawns obstacle destruction particles with randomized variants.
    /// </summary>
    public void SpawnObstacleParticles(Vector3 position, string obstacleType)
    {
        Sprite[] sprites = GetObstacleParticleSprites(obstacleType);
        SpawnParticlesRandomized(position, sprites);
    }
    
    /// <summary>
    /// Spawns rocket explosion particles.
    /// </summary>
    public void SpawnRocketParticles(Vector3 position)
    {
        SpawnParticles(position, rocketParticle, 12);
        SpawnParticles(position, smokeParticle, 6);
    }

    /// <summary>
    /// Spawns celebration explosion particles with custom settings.
    /// </summary>
    public void SpawnCelebrationParticles(Vector3 position)
    {
        SpawnParticles(position, celebrationParticle, celebrationParticlesPerBurst, celebrationParticleSize, celebrationParticleSpeed);
        // Optional: Add some smoke/sparkle mixing if defined
        if (smokeParticle != null)
        {
            SpawnParticles(position, smokeParticle, celebrationParticlesPerBurst / 2, celebrationParticleSize * 0.8f, celebrationParticleSpeed * 0.8f);
        }
    }

    /// <summary>
    /// Spawns rocket heads flying in opposite directions.
    /// </summary>
    public void SpawnRocketHeads(Vector3 position, bool isHorizontal)
    {
        if (isHorizontal)
        {
            SpawnSingleRocketHead(position, horizontalRocketLeft, Vector3.left);
            SpawnSingleRocketHead(position, horizontalRocketRight, Vector3.right);
        }
        else
        {
            SpawnSingleRocketHead(position, verticalRocketTop, Vector3.up);
            SpawnSingleRocketHead(position, verticalRocketBottom, Vector3.down);
        }
    }
    
    private void SpawnSingleRocketHead(Vector3 position, Sprite sprite, Vector3 direction)
    {
        if (sprite == null) return;
        
        GameObject head = new GameObject("RocketHead");
        
        // Use cached canvas if available
        Canvas canvas = targetCanvas;
        // Fallback search if somehow null and not cached yet (should be covered by Awake)
        if (canvas == null) canvas = FindAnyObjectByType<Canvas>();

        if (canvas != null)
        {
            head.transform.SetParent(canvas.transform, false);
            
            RectTransform rt = head.AddComponent<RectTransform>();
            rt.position = position;
            rt.sizeDelta = new Vector2(rocketHeadSize, rocketHeadSize);
            rt.localScale = Vector3.one;
            
            // Force sorting on top of everything (Trail is 100)
            Canvas headCanvas = head.AddComponent<Canvas>();
            headCanvas.overrideSorting = true;
            headCanvas.sortingOrder = 110;
            
            UnityEngine.UI.Image img = head.AddComponent<UnityEngine.UI.Image>();
            img.sprite = sprite;
            img.raycastTarget = false;
            img.preserveAspect = true;
            
            RocketHeadBehavior behavior = head.AddComponent<RocketHeadBehavior>();
            behavior.Initialize(direction, rocketHeadSpeed);
        }
        else
        {
            head.transform.SetParent(transform);
            head.transform.position = position;
            head.transform.localScale = Vector3.one * (rocketHeadSize / 100f);
            
            SpriteRenderer sr = head.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 110; // Updated to match UI sorting
            
            RocketHeadBehavior behavior = head.AddComponent<RocketHeadBehavior>();
            behavior.Initialize(direction, rocketHeadSpeed / 100f);
        }
    }


    
    /// <summary>
    /// Spawns a single particle.
    /// </summary>
    private void SpawnSingleParticle(Vector3 position, Sprite sprite, float sizeOverride = -1, float speedOverride = -1)
    {
        // Determine size and speed
        float size = sizeOverride > 0 ? sizeOverride : particleSize;
        float speed = speedOverride > 0 ? speedOverride : particleSpeed;

        // Create particle GameObject
        GameObject particle = new GameObject("Particle");
        
        // Use cached canvas if available
        Canvas canvas = targetCanvas;
        // Fallback search if somehow null
        if (canvas == null) canvas = FindAnyObjectByType<Canvas>();
        
        if (canvas != null)
        {
            // UI-based particle - parent to canvas first
            particle.transform.SetParent(canvas.transform, false);
            
            RectTransform rt = particle.AddComponent<RectTransform>();
            rt.position = position;
            rt.sizeDelta = new Vector2(size, size); // Adjustable
            rt.localScale = Vector3.one; // Ensure scale is 1
            
            UnityEngine.UI.Image img = particle.AddComponent<UnityEngine.UI.Image>();
            img.sprite = sprite;
            img.raycastTarget = false;
            img.preserveAspect = true;
            
            // Add particle behavior
            ParticleBehavior behavior = particle.AddComponent<ParticleBehavior>();
            behavior.Initialize(GetRandomDirection(), speed, particleLifetime);
        }
        else
        {
            // World-space particle
            particle.transform.SetParent(transform);
            particle.transform.position = position;
            particle.transform.localScale = Vector3.one * (size / 100f); // Adjustable
            
            SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 100;
            
            ParticleBehavior behavior = particle.AddComponent<ParticleBehavior>();
            behavior.Initialize(GetRandomDirection(), speed / 100f, particleLifetime);
        }
    }
    
    /// <summary>
    /// Gets random outward direction.
    /// </summary>
    private Vector2 GetRandomDirection()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    
    /// <summary>
    /// Gets particle sprite for cube color.
    /// </summary>
    private Sprite GetCubeParticleSprite(Cube.CubeColor color)
    {
        switch (color)
        {
            case Cube.CubeColor.Red: return redParticle;
            case Cube.CubeColor.Green: return greenParticle;
            case Cube.CubeColor.Blue: return blueParticle;
            case Cube.CubeColor.Yellow: return yellowParticle;
            default: return redParticle;
        }
    }
    
    /// <summary>
    /// Gets particle sprites array for obstacle type.
    /// </summary>
    private Sprite[] GetObstacleParticleSprites(string type)
    {
        switch (type.ToLower())
        {
            case "bo":
            case "box": return boxParticles;
            case "s":
            case "stone": return stoneParticles;
            case "v":
            case "vase": return vaseParticles;
            default: return boxParticles;
        }
    }
}

/// <summary>
/// Simple particle behavior - moves in direction and fades out.
/// </summary>
public class ParticleBehavior : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float lifetime;
    private float elapsed;
    private float startScale;
    
    private UnityEngine.UI.Image uiImage;
    private SpriteRenderer spriteRenderer;
    
    public void Initialize(Vector2 dir, float spd, float life)
    {
        direction = dir;
        speed = spd;
        lifetime = life;
        elapsed = 0f;
        startScale = transform.localScale.x;
        
        uiImage = GetComponent<UnityEngine.UI.Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Add slight random rotation
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
    }
    
    private void Update()
    {
        elapsed += Time.deltaTime;
        
        // Move outward
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        
        // Slow down over time
        speed *= 0.95f;
        
        // Scale down and fade
        float t = elapsed / lifetime;
        float scale = Mathf.Lerp(startScale, 0f, t);
        transform.localScale = Vector3.one * scale;
        
        // Fade alpha
        float alpha = Mathf.Lerp(1f, 0f, t);
        if (uiImage != null)
        {
            Color c = uiImage.color;
            c.a = alpha;
            uiImage.color = c;
        }
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }
        
        // Destroy when lifetime ends
        if (elapsed >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
