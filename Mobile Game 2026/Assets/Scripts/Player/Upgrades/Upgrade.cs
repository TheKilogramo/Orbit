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


    [Header("Evolution Settings")]
    public BuffTag[] Tags; // what this upgrade "counts as"
    public BuffTag[] RequiredTags; //what must already be owned (one of each)
    public int RequiredTagCount = 0; //minimum unique tag types owned
    [Space]
    public bool IsSpecial = false;
    public SpecialEffect EffectID = SpecialEffect.None;


    public enum BuffTag { ExtraOrbiter, ExtraOrbit, DamageUp, SpeedUp, SizeUp }
    public enum SpecialEffect
    {
        None,
        Saturn, SaturnAdvanced,
        Meteor, MeteorAdvanced,
        Electric, ElectricAdvanced,
        IceGiant, IceGiantAdvanced,
        SolarFlare, SolarFlareAdvanced,
        Comet, CometAdvanced,
        Gravity, GravityAdvanced,
        Lightsaber, LightsaberAdvanced,
        Smash, SmashAdvanced,
        FrozenLightning, FrozenLightningAdvanced
    }
}