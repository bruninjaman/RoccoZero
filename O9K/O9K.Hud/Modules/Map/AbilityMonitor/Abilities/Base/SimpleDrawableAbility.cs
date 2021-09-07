namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Base;

using Core.Helpers;

using Divine.Renderer;

using Helpers;

internal class SimpleDrawableAbility : DrawableAbility
{
    public override void DrawOnMap(IMinimap minimap)
    {
        var position = minimap.WorldToScreen(this.Position, 35 * Hud.Info.ScreenRatio);
        if (position.IsZero)
        {
            return;
        }

        RendererManager.DrawImage("o9k.outline_red", position * 1.12f);
        RendererManager.DrawImage(this.AbilityTexture, position, ImageType.RoundAbility);
    }

    public override void DrawOnMinimap(IMinimap minimap)
    {
        var position = minimap.WorldToMinimap(this.Position, 25 * Hud.Info.ScreenRatio);
        if (position.IsZero)
        {
            return;
        }

        RendererManager.DrawImage("o9k.outline_red", position * 1.08f);
        RendererManager.DrawImage(this.AbilityTexture, position, ImageType.RoundAbility);
    }
}