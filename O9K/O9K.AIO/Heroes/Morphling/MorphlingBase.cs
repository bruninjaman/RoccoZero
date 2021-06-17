namespace O9K.AIO.Heroes.Morphling
{
    using Base;

    using Core.Entities.Metadata;

    using Divine.Entity.Entities.Units.Heroes.Components;

    [HeroId(HeroId.npc_dota_hero_morphling)]
    internal class MorphlingBase : BaseHero
    {
        public MorphlingBase()
        {
        }

        public override void CreateUnitManager()
        {
            this.UnitManager = new MorphlingUnitManager(this);
        }
    }
}