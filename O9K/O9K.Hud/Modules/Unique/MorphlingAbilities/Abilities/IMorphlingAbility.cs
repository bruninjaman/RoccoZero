namespace O9K.Hud.Modules.Unique.MorphlingAbilities.Abilities
{
    using Core.Managers.Renderer.Utils;

    using Divine;

    internal interface IMorphlingAbility
    {
        AbilitySlot AbilitySlot { get; }

        uint Handle { get; }

        bool Display(bool isMorphed);

        void Draw(Rectangle9 position, float textSize);

        bool Update(bool isMorphed);
    }
}