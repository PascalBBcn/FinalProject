using UnityEngine;

public class FollowPlayerCamera : MonoBehaviour
{
    public Transform player;

    public Vector3 offset;

    [SerializeField] private float cameraSize = 10f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        if (cam != null)
        {
            cam.orthographicSize = cameraSize;
        }
    }

    void Update()
    {
        if(player != null) transform.position = player.position + offset;
    }
}
