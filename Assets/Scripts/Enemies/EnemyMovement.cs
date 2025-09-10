using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMovement : MonoBehaviour
{
    // REFERENCES/COMPONENTS
    private EnemyStats stats;
    protected Rigidbody2D rb;
    protected Transform playerTransform;

    // PATHFINDING/MOVEMENT
    private float moveSpeed;
    public float range = 10f;
    private bool isChasing = false;
    private Vector2Int enemyPos;
    private Vector2Int lastPlayerPos;
    public float pathUpdateInterval = 0.1f;

    // Protected instead of private, so can be used by derived classes (Boss)
    protected List<Vector2Int> currentPath = new List<Vector2Int>();
    protected int currentPathIndex = 0; 
    
    public RectInt roomBounds; // To store which room each enemy is in

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<EnemyStats>();
        moveSpeed = stats.MoveSpeed;
    }

    public void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdatePath());
        if (stats.IsBoss) StartCoroutine(CheckPlayerProximity());
    }

    // Does not recalculate the path every frame but rather based on interval
    private IEnumerator UpdatePath()
    {
        while (true)
        {
            if (playerTransform == null)
            {
                yield return new WaitForSeconds(pathUpdateInterval);
                continue;
            }
            // Convert positions to grid coordinates
            enemyPos = new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.y));

            Vector2Int playerPos = new Vector2Int(
                Mathf.RoundToInt(playerTransform.position.x),
                Mathf.RoundToInt(playerTransform.position.y));

            if (roomBounds.Contains(playerPos))
            {
                RectInt smallerRoomBounds = new RectInt(
                    roomBounds.x + 1,
                    roomBounds.y + 1,
                    roomBounds.width - 2,
                    roomBounds.height - 2
                );
                if (smallerRoomBounds.Contains(playerPos))
                {
                    if (Vector2.Distance(enemyPos, playerPos) <= range && !isChasing)
                    {
                        isChasing = true;
                    }
                    if (isChasing)
                    {
                        currentPath = Pathfinding.AStar(enemyPos, playerPos);
                        if (currentPath != null && currentPath.Count > 0)
                        {
                            currentPathIndex = 0;
                            lastPlayerPos = playerPos;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    // rb physics movement has better performance in fixedUpdate
    void FixedUpdate()
    {
        MoveAlongPath();
        rb.velocity = Vector2.zero;
    }

    // This class is being overriden by derivative classes (Boss)
    protected virtual void MoveAlongPath()
    {
        if (currentPath == null || currentPathIndex >= currentPath.Count) return;
        

        Vector3 targetPos = new Vector3(
            currentPath[currentPathIndex].x,
            currentPath[currentPathIndex].y,
            transform.position.z);

        Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.fixedDeltaTime); // Better to use fixedDeltaTime since within FixedUpdate

        rb.MovePosition(newPosition);

        if (Vector2.Distance(newPosition, targetPos) < 0.9f)
        {
            currentPathIndex++;
        }
    }
    
    // If a boss, show health bar if player in room bounds
    private IEnumerator CheckPlayerProximity()
    {
        while (true)
        {
            if (playerTransform != null)
            {
                Vector2Int playerPos = new Vector2Int(
                Mathf.RoundToInt(playerTransform.position.x),
                Mathf.RoundToInt(playerTransform.position.y));

                if (roomBounds.Contains(playerPos)) GameSession.instance.bossHealthBarContainer.SetActive(true);

            }
            yield return new WaitForSeconds(1f);
        }
    }
}
