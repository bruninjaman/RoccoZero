namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data.UniqueAbilities.Maelstorm;

using System.Collections.Generic;
using System.Linq;

using Base;

using Core.Managers.Entity;
using Divine.Extensions;
using Divine.Game;
using Divine.Particle.Particles;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Abilities.Components;

using Helpers.Notificator;

internal class MaelstormAbilityData : AbilityFullData
{
    public override void AddDrawableAbility(
        List<IDrawableAbility> drawableAbilities,
        Particle particle,
        Team allyTeam,
        INotificator notificator)
    {
        var position = (this.RawParticlePosition ? particle.Position : particle.GetControlPoint(this.ControlPoint)).SetZ(350);
        var owners = EntityManager9.Abilities.Where(x => x.Id == this.AbilityId || x.Id == AbilityId.item_mjollnir || x.Id == AbilityId.item_gungir)
            .Select(x => x.Owner)
            .Where(x => x.CanUseAbilities && x.IsAlive)
            .ToList();

        var ownerName = owners.Count == 1 ? owners.FirstOrDefault().Name : null;

        var drawableAbility = new DrawableAbility
        {
            AbilityTexture = this.AbilityId.ToString(),
            HeroTexture = ownerName,
            MinimapHeroTexture = ownerName,
            ShowUntil = GameManager.RawGameTime + this.TimeToShow,
            Position = position
        };

/*        owner.ChangeBasePosition(drawableAbility.Position);
*/
        if (this.Replace)
        {
            var exist = drawableAbilities.LastOrDefault(
                x => x.AbilityTexture == drawableAbility.AbilityTexture && x.HeroTexture == drawableAbility.HeroTexture);

            if (exist != null)
            {
                drawableAbilities.Remove(exist);
            }
        }

        drawableAbilities.Add(drawableAbility);
    }
}