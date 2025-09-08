using UnityEngine;

public class MaskFollowPlayer : MonoBehaviour
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
        transform.position = player.position;
    }
    void LocatePlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) player = playerObject.transform;
    }
}
