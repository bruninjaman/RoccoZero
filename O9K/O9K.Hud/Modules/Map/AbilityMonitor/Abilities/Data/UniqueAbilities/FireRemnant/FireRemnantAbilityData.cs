namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data.UniqueAbilities.FireRemnant
{
    using System.Collections.Generic;
    using System.Linq;

    using Base;

    using Core.Managers.Entity;

    using Divine;
    using Divine.SDK.Extensions;

    using Helpers.Notificator;

    internal class FireRemnantAbilityData : AbilityFullData
    {
        public int StartControlPoint { get; set; } = 0;

        public override void AddDrawableAbility(
            List<IDrawableAbility> drawableAbilities,
            Particle particle,
            Team allyTeam,
            INotificator notificator)
        {
            if (particle.Name.Contains("dash"))
            {
                drawableAbilities.RemoveAll(x => x is DrawableFireRemnantAbility);
            }
            else
            {
                var owner = EntityManager9.GetUnit(particle.Owner.Owner.Handle);
                if (owner == null)
                {
                    return;
                }

                var startPosition = particle.GetControlPoint(this.StartControlPoint);

                if (!owner.IsVisible)
                {
                    var drawableAbilityStart = new DrawableAbility
                    {
                        AbilityTexture = this.AbilityId.ToString(),
                        HeroTexture = owner.Name,
                        MinimapHeroTexture = owner.Name,
                        ShowUntil = GameManager.RawGameTime + this.TimeToShow,
                        Position = startPosition.SetZ(350)
                    };

                    owner.ChangeBasePosition(drawableAbilityStart.Position);
                    drawableAbilities.Add(drawableAbilityStart);
                }

                var remnants = drawableAbilities.OfType<DrawableFireRemnantAbility>().ToArray();
                var unit = EntityManager.GetEntities<Unit>()
                    .FirstOrDefault(
                        x => x.IsAlive && x.Name == "npc_dota_ember_spirit_remnant" && x.Distance2D(startPosition) < 1500
                             && remnants.All(z => z.Unit != x));

                if (unit == null)
                {
                    return;
                }

                var drawableAbility = new DrawableFireRemnantAbility
                {
                    AbilityTexture = this.AbilityId.ToString(),
                    HeroTexture = owner.Name,
                    MinimapHeroTexture = owner.Name,
                    Position = particle.GetControlPoint(this.ControlPoint).SetZ(350),
                    Duration = this.Duration,
                    ShowUntil = GameManager.RawGameTime + this.Duration,
                    ShowHeroUntil = GameManager.RawGameTime + this.TimeToShow,
                    Owner = owner.BaseEntity,
                    Unit = unit
                };

                drawableAbilities.Add(drawableAbility);
            }
        }
    }
}