using UnityEngine;

/// <summary>
/// Controls the movement of a rocket part (head/tail) across the screen.
/// </summary>
public class RocketHeadBehavior : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private float lifetime = 3f; // Auto destroy to prevent leaks

    private float lastTrailTime;

    /// <summary>
    /// Initializes the rocket head movement.
    /// </summary>
    /// <summary>
    /// Initializes the rocket head movement.
    /// </summary>
    public void Initialize(Vector3 dir, float initialSpeed)
    {
        direction = dir;
        speed = initialSpeed;
        
        // Spawn Trail Prefab
        if (ParticleManager.Instance != null && ParticleManager.Instance.rocketTrailPrefab != null)
        {
            GameObject trail = Instantiate(ParticleManager.Instance.rocketTrailPrefab, transform);
            trail.transform.localPosition = Vector3.zero;
        }
        
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move in direction
        transform.position += direction * speed * Time.deltaTime;
        
        // Accelerate (Linear, time-based)
        // Add 50% speed per second
        speed += speed * 0.5f * Time.deltaTime;
    }
}
