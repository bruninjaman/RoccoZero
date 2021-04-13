namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data.UniqueAbilities.Poof
{
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using Core.Managers.Entity;

    using Divine;
    using Divine.SDK.Extensions;

    using Helpers.Notificator;

    using SharpDX;

    internal class PoofAbilityData : AbilityFullData
    {
        public override void AddDrawableAbility(
            List<IDrawableAbility> drawableAbilities,
            Particle particle,
            Team allyTeam,
            INotificator notificator)
        {
            if (particle.GetControlPoint(1) == new Vector3(-1))
            {
                return;
            }

            var owner = this.SearchOwner || !(particle.Owner is Unit)
                            ? EntityManager9.Abilities.FirstOrDefault(x => x.Id == this.AbilityId && x.Owner.Team != allyTeam)?.Owner
                            : EntityManager9.GetUnit(particle.Owner.Handle);

            if (owner?.IsVisible != false)
            {
                return;
            }

            string ownerName;

            if (owner.IsHero)
            {
                ownerName = owner.Name;
            }
            else
            {
                ownerName = owner.Owner?.Name;

                if (ownerName == null)
                {
                    return;
                }
            }

            var drawableAbility = new DrawableAbility
            {
                AbilityTexture = this.AbilityId.ToString(),
                HeroTexture = ownerName,
                MinimapHeroTexture = ownerName,
                ShowUntil = GameManager.RawGameTime + this.TimeToShow,
                Position = (this.RawParticlePosition ? particle.Position : particle.GetControlPoint(this.ControlPoint)).SetZ(350)
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