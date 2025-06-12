using UnityEngine;
// Using interface as weapon derivates have different code
public interface WeaponInterface
{
    // Player presses shoot button
    void StartShooting();
    // Player releases shoot button
    void StopShooting();
}
