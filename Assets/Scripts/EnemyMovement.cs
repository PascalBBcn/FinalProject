using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMovement : MonoBehaviour
{
    private EnemyStats stats;
    private float moveSpeed;
    public float pathUpdateInterval = 0.5f;
    public float range = 10f;
    private bool isChasing = false;

    private List<Vector2Int> currentPath = new List<Vector2Int>();
    private int currentPathIndex = 0;
    private Vector2Int lastPlayerPos;
    private Vector2Int enemyPos;
    private Rigidbody2D rb;

    public RectInt roomBounds; // To store which room each enemy is in

    private Transform playerTransform;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<EnemyStats>();
        moveSpeed = stats.MoveSpeed;
    }

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdatePath());
    }

    // Does not recalculate the path every frame but rather based on interval
    IEnumerator UpdatePath()
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
                    if (isChasing && playerPos != lastPlayerPos)
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
    }

    void MoveAlongPath()
    {
        if (currentPath == null || currentPathIndex >= currentPath.Count) 
            return;
        
        Vector3 targetPos = new Vector3(
            currentPath[currentPathIndex].x, 
            currentPath[currentPathIndex].y, 
            transform.position.z);
        
        Vector3 newPosition = Vector3.MoveTowards(
            transform.position, 
            targetPos, 
            moveSpeed * Time.deltaTime);
        
        rb.MovePosition(newPosition);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            currentPathIndex++;
        }
    }
}
