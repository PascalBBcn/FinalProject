using UnityEngine;

public class LaserWeapon : MonoBehaviour, WeaponInterface
{
    public LaserWeaponData weaponData;
    public Transform firePoint;

    private LineRenderer lineRenderer;
    private bool isFiring;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = weaponData.laserWidth;
        lineRenderer.endWidth = weaponData.laserWidth;
        lineRenderer.startColor = weaponData.laserColour;
        lineRenderer.endColor = weaponData.laserColour;
        lineRenderer.enabled = false;
    }

    void Update()
    {

        if (isFiring) Shoot();
    }

    public void StartShooting()
    {
        isFiring = true;
        lineRenderer.enabled = true;
    }
    public void StopShooting()
    {
        isFiring = false;
        lineRenderer.enabled = false;
    }
    private void Shoot()
    {
        Vector3 startPos = firePoint.position;
        Vector3 endPos;

        RaycastHit2D hit = Physics2D.Raycast(startPos, firePoint.up, weaponData.laserDistance);
        if (hit.collider != null)
        {
            endPos = hit.point;
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Destroy(hit.collider.gameObject);
            }
        }    
        else endPos = startPos + firePoint.up * weaponData.laserDistance;

        startPos.z = 0f;
        endPos.z = 0f;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
}
