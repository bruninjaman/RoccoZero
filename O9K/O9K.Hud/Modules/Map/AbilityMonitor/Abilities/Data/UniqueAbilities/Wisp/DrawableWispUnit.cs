namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data.UniqueAbilities.Wisp;

using Base;

using Core.Entities.Units;
using Core.Helpers;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Particle.Particles;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

internal class DrawableWispUnit : IDrawableAbility
{
    private readonly string minimapHeroTexture;

    private readonly Particle particle;

    private readonly Unit9 unit;

    public DrawableWispUnit(Unit9 unit, Particle particle)
    {
        this.unit = unit;
        this.particle = particle;
        this.HeroTexture = unit.Name;
        this.minimapHeroTexture = unit.Name;
    }

    public AbilityId AbilityId { get; set; }

    public string AbilityTexture { get; } = string.Empty;

    public bool Draw
    {
        get
        {
            return !this.unit.IsVisible && this.unit.IsAlive && this.particle?.IsValid == true;
        }
    }

    public string HeroTexture { get; }

    public bool IsShowingRange { get; } = false;

    public bool IsValid
    {
        get
        {
            return this.unit.IsValid && this.particle.IsValid;
        }
    }

    public Vector3 Position { get; set; }

    public void DrawOnMap(IMinimap minimap)
    {
        var particlePosition = this.particle.Position;
        if (particlePosition.IsZero)
        {
            return;
        }

        var position = minimap.WorldToScreen(particlePosition, 45 * Hud.Info.ScreenRatio);
        if (position.IsZero)
        {
            return;
        }

        RendererManager.DrawImage("o9k.outline_red", position * 1.12f);
        RendererManager.DrawImage(this.HeroTexture, position, ImageType.RoundUnit);
    }

    public void DrawOnMinimap(IMinimap minimap)
    {
        var particlePosition = this.particle.Position;
        if (particlePosition.IsZero)
        {
            return;
        }

        var position = minimap.WorldToMinimap(particlePosition, 25 * Hud.Info.ScreenRatio);
        if (position.IsZero)
        {
            return;
        }

        RendererManager.DrawImage("o9k.outline_red", position * 1.08f);
        RendererManager.DrawImage(this.minimapHeroTexture, position, ImageType.MiniUnit);
    }

    public void RemoveRange()
    {
    }
}