namespace O9K.ItemManager.Modules.Snatcher.Controllables
{
    using Core.Entities.Units;

    using Divine.Entity.Entities.PhysicalItems;

    internal class ArcWardenClone : Controllable
    {
        public ArcWardenClone(Unit9 unit)
            : base(unit)
        {
        }

        public override bool CanPick(PhysicalItem physicalItem)
        {
            return false;
        }
    }
}