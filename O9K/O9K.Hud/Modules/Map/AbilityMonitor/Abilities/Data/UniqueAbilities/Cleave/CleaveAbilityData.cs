namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data.UniqueAbilities.Cleave
{
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using Core.Managers.Entity;

    using Divine;
    using Divine.SDK.Extensions;

    using Helpers.Notificator;

    internal class CleaveAbilityData : AbilityFullData
    {
        public override void AddDrawableAbility(
            List<IDrawableAbility> drawableAbilities,
            Particle particle,
            Team allyTeam,
            INotificator notificator)
        {
            var owner = EntityManager9.GetUnit(particle.Owner.Handle);

            if (owner?.IsVisible != false)
            {
                return;
            }

            var position = this.RawParticlePosition ? particle.Position : particle.GetControlPoint(this.ControlPoint);
            if (position.IsZero || position.Distance2D(owner.BaseUnit.Position) < 50)
            {
                return;
            }

            var ownerName = owner.Name;

            var drawableAbility = new DrawableAbility
            {
                AbilityTexture = this.AbilityId.ToString(),
                HeroTexture = ownerName,
                MinimapHeroTexture = ownerName,
                ShowUntil = GameManager.RawGameTime + this.TimeToShow,
                Position = position.SetZ(350)
            };

            owner.ChangeBasePosition(drawableAbility.Position);

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
}