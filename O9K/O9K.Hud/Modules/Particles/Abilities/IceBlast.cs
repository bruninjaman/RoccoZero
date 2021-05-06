namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;

    using Core.Data;
    using Core.Entities.Metadata;
    using Core.Extensions;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Context;
    using Core.Managers.Particle;

    using Divine;
    using Divine.SDK.Extensions;

    using Helpers.Notificator;

    using MainMenu;

    using SharpDX;

    [AbilityId(AbilityId.ancient_apparition_ice_blast)]
    internal class IceBlast : AbilityModule
    {
        private readonly float growRadius;

        private readonly float maxRadius;

        private readonly float minRadius;

        private readonly float speed;

        private Particle effect;

        private float unitAddTime;

        public IceBlast(INotificator notificator, IHudMenu hudMenu)
            : base(notificator, hudMenu)
        {
            this.minRadius = new SpecialData(AbilityId.ancient_apparition_ice_blast, "radius_min").GetValue(1);
            this.maxRadius = new SpecialData(AbilityId.ancient_apparition_ice_blast, "radius_max").GetValue(1);
            this.growRadius = new SpecialData(AbilityId.ancient_apparition_ice_blast, "radius_grow").GetValue(1);
            this.speed = new SpecialData(AbilityId.ancient_apparition_ice_blast, "speed").GetValue(1);

            //todo add notification
            //this.notificationsEnabled = hudMenu.NotificationsMenu.GetOrAdd(new Menu("Abilities"))
            //    .GetOrAdd(new Menu("Used"))
            //    .GetOrAdd(new MenuSwitcher("Enabled"));
        }

        protected override void Disable()
        {
            EntityManager.EntityAdded -= this.OnAddEntity;
            Context9.ParticleManger.ParticleAdded -= this.OnParticleAdded;
        }

        protected override void Enable()
        {
            EntityManager.EntityAdded += this.OnAddEntity;
        }

        private void OnAddEntity(EntityAddedEventArgs e)
        {
            try
            {
                var unit = e.Entity as Unit;
                if (unit == null || unit.Team == this.OwnerTeam || unit.UnitType != 0 || unit.NetworkName != "CDOTA_BaseNPC")
                {
                    return;
                }

                if (unit.DayVision != GameData.AbilityVision[AbilityId.ancient_apparition_ice_blast])
                {
                    return;
                }

                this.unitAddTime = GameManager.RawGameTime;
                Context9.ParticleManger.ParticleAdded += this.OnParticleAdded;

                //if (!this.notificationsEnabled)
                //{
                //    return;
                //}

                //this.notificator.PushNotification(
                //    new AbilityNotification(
                //        nameof(HeroId.npc_dota_hero_ancient_apparition),
                //        nameof(AbilityId.ancient_apparition_ice_blast)));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnParticleAdded(Particle9 particle)
        {
            try
            {
                //if (!particle.Released)
                //{
                //    return;
                //}

                switch (particle.Name)
                {
                    case "particles/units/heroes/hero_ancient_apparition/ancient_apparition_ice_blast_final.vpcf":
                    case "particles/econ/items/ancient_apparition/aa_blast_ti_5/ancient_apparition_ice_blast_final_ti5.vpcf":
                    {
                        var startPosition = particle.GetControlPoint(0);
                        var endPosition = particle.GetControlPoint(1);
                        var time = GameManager.RawGameTime - (GameManager.Ping / 1000);
                        var flyingTime = time - this.unitAddTime;
                        var direction = startPosition + endPosition;
                        var position = startPosition.Extend2D(direction, flyingTime * this.speed);
                        var radius = Math.Min(this.maxRadius, Math.Max((flyingTime * this.growRadius) + this.minRadius, this.minRadius));

                        this.effect = ParticleManager.CreateParticle(
                            "particles/units/heroes/hero_ancient_apparition/ancient_apparition_ice_blast_marker.vpcf",
                            position.SetZ(384)); // todo correct z coord
                        this.effect.SetControlPoint(1, new Vector3(radius, 1, 1));
                        break;
                    }
                    case "particles/econ/items/ancient_apparition/aa_blast_ti_5/ancient_apparition_ice_blast_explode_ti5.vpcf":
                    case "particles/units/heroes/hero_ancient_apparition/ancient_apparition_ice_blast_explode.vpcf":
                        Context9.ParticleManger.ParticleAdded -= this.OnParticleAdded;
                        this.effect?.Dispose();
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}