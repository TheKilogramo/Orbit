using System.Collections.Generic;
using UnityEngine;
using static Upgrade;

public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ApplyEffect(SpecialEffect effect)
    {
        switch (effect)
        {
            case SpecialEffect.Saturn: ApplySaturn(); break;
            case SpecialEffect.SaturnAdvanced: ApplySaturnAdvanced(); break;

            case SpecialEffect.Meteor: ApplyMeteor(); break;
            case SpecialEffect.MeteorAdvanced: ApplyMeteorAdvanced(); break;

            case SpecialEffect.Electric: ApplyElectric(); break;
            case SpecialEffect.ElectricAdvanced: ApplyElectricAdvanced(); break;

            case SpecialEffect.IceGiant: ApplyIceGiant(); break;
            case SpecialEffect.IceGiantAdvanced: ApplyIceGiantAdvanced(); break;

            case SpecialEffect.SolarFlare: ApplySolarFlare(); break;
            case SpecialEffect.SolarFlareAdvanced: ApplySolarFlareAdvanced(); break;

            case SpecialEffect.Comet: ApplyComet(); break;
            case SpecialEffect.CometAdvanced: ApplyCometAdvanced(); break;

            case SpecialEffect.Gravity: ApplyGravity(); break;
            case SpecialEffect.GravityAdvanced: ApplyGravityAdvanced(); break;

            case SpecialEffect.Lightsaber: ApplyLightsaber(); break;
            case SpecialEffect.LightsaberAdvanced: ApplyLightsaberAdvanced(); break;

            case SpecialEffect.Smash: ApplySmash(); break;
            case SpecialEffect.SmashAdvanced: ApplySmashAdvanced(); break;

            case SpecialEffect.FrozenLightning: ApplyFrozenLightning(); break;
            case SpecialEffect.FrozenLightningAdvanced: ApplyFrozenLightningAdvanced(); break;
        }
    }

    // ─── Saturn ───────────────────────────────────────────
    // Medium: debris around orbit that deals small damage
    private void ApplySaturn() { }
    // Advanced: 
    private void ApplySaturnAdvanced() { }

    // ─── Meteor ───────────────────────────────────────────
    // Medium: enemies set on fire on hit
    private void ApplyMeteor() { }
    // Advanced: 
    private void ApplyMeteorAdvanced() { }

    // ─── Electric ─────────────────────────────────────────
    // Medium: enemies chain lightning when hit
    private void ApplyElectric() { }
    // Advanced: 
    private void ApplyElectricAdvanced() { }

    // ─── Ice Giant ────────────────────────────────────────
    // Medium: enemies get slowed on hit
    private void ApplyIceGiant() { }
    // Advanced: 
    private void ApplyIceGiantAdvanced() { }

    // ─── Solar Flare ──────────────────────────────────────
    // Medium: orbiters occasionally emit bursts of damage
    private void ApplySolarFlare() { }
    // Advanced: 
    private void ApplySolarFlareAdvanced() { }

    // ─── Comet ────────────────────────────────────────────
    // Medium: orbits shoot out comets every once in a while
    private void ApplyComet() { }
    // Advanced: 
    private void ApplyCometAdvanced() { }

    // ─── Gravity ──────────────────────────────────────────
    // Medium: orbiters pull enemies towards them
    private void ApplyGravity() { }
    // Advanced: 
    private void ApplyGravityAdvanced() { }

    // ─── Lightsaber ───────────────────────────────────────
    // Medium: orbit turns solid and deals damage
    private void ApplyLightsaber() { }
    // Advanced: 
    private void ApplyLightsaberAdvanced() { }

    // ─── Smash ────────────────────────────────────────────
    // Medium: destroying enemies smashes them into debris that deals damage
    private void ApplySmash() { }
    // Advanced: 
    private void ApplySmashAdvanced() { }

    // ─── Frozen Lightning ─────────────────────────────────
    // Medium: chain slow (slows less than ice giant)
    private void ApplyFrozenLightning() { }
    // Advanced: 
    private void ApplyFrozenLightningAdvanced() { }
}