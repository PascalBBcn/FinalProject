using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform player;

    void Start()
    {
        LocatePlayer();
    }
    void LateUpdate()
    {
        if (player == null)
        {
            LocatePlayer();
            return;
        }
        Vector3 position = player.position;
        position.z = transform.position.z;
        transform.position = position;
    }
    void LocatePlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;
    }
}
