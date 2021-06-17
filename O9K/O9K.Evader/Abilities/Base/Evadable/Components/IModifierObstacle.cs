namespace O9K.Evader.Abilities.Base.Evadable.Components
{
    using Divine.Modifier.Modifiers;
    using Divine.Entity.Entities.Units;

    internal interface IModifierObstacle
    {
        bool AllyModifierObstacle { get; }

        void AddModifierObstacle(Modifier modifier, Unit sender);
    }
}