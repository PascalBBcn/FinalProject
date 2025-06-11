using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier", menuName = "Pickup Stat Modifier")]
public class StatModifier : ScriptableObject
{
    public string modifierName;
    public Sprite modifierImage;

    public float damageMultiplier;
    public float fireRateMultiplier;
    public float bulletSpeedMultiplier;

    public float bulletSpreadModifier;
    public float bulletQuantityModifier;
}
