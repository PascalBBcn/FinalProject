using UnityEngine;

public class LaserWeapon : MonoBehaviour, WeaponInterface
{
    public LaserWeaponData weaponData;
    public Transform firePoint;

    private LineRenderer lineRenderer;
    private bool isFiring;

    private float laserTimer = 0f;
    private bool laserActive = true;


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

    // Ensures the laser sound stops when picking up new weapon
    private void OnDisable()
    {
        if (isFiring)
        {
            AudioManager.Instance.StopLoopSFX();
            isFiring = false;
        }
    }
    private void OnDestroy()
    {
        if (isFiring)
        {
            AudioManager.Instance.StopLoopSFX();
            isFiring = false;
        }
    }

    void Update()
    {
        if (isFiring)
        {
            Shoot();

            if (weaponData.autoLaser)
            {
                laserTimer += Time.deltaTime;
                if (laserTimer >= weaponData.autoLaserRate)
                {
                    laserTimer = 0f;
                    laserActive = !laserActive;
                    lineRenderer.enabled = laserActive;

                    if (laserActive) AudioManager.Instance.PlaySFX("Laser");
                }
            }
            else lineRenderer.enabled = true;

        }

    }

    public void StartShooting()
    {
        isFiring = true;
        lineRenderer.enabled = true;
        if(!weaponData.autoLaser) AudioManager.Instance.PlayLoopSFX("Laser");
    }
    public void StopShooting()
    {
        isFiring = false;
        lineRenderer.enabled = false;
        AudioManager.Instance.StopLoopSFX();

    }
    private void Shoot()
    {
        Vector3 startPos = firePoint.position;
        Vector3 endPos;

        // Excludes the player mask
        int playerLayer = ~LayerMask.GetMask("Player");

        RaycastHit2D hit = Physics2D.Raycast(startPos, firePoint.up, weaponData.laserDistance, playerLayer);
        if (hit.collider != null)
        {
            endPos = hit.point;
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    float damagePerSecond = weaponData.laserDPS * Time.deltaTime;
                    enemyStats.TakeDamage(damagePerSecond);
                }
            }
        }
        else endPos = startPos + firePoint.up * weaponData.laserDistance;

        startPos.z = 0f;
        endPos.z = 0f;

        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }
}
