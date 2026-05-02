using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Orbits/New Upgrade")]
public class Upgrade : ScriptableObject
{
    [Header("Info")]
    public string UpgradeName;
    [TextArea] public string UpgradeDescription;
    public Sprite UpgradeIcon;

    [Header("Stat Multipliers")]
    public float DamageMultiplier = 0f;
    public float SpeedMultiplier = 0f;
    public float SizeMultiplier = 0f;

    [Header("Orbit Buffs")]
    public int ExtraOrbiters = 0;
    public int ExtraOrbits = 0;

    [Header("Rules")]
    [Tooltip("Maximum amount of times this upgrade can show up")]
    public int MaxShowTimes = 5;
}
