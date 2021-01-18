namespace O9K.Hud.Modules.Unique.MorphlingAbilities.Abilities
{
    using System;

    using Core.Logger;
    using Core.Managers.Renderer.Utils;

    using Divine;

    using SharpDX;

    internal class ReplicateTimer : IMorphlingAbility
    {
        private readonly float cooldown;

        private readonly string texture;

        private readonly float updateTime;

        public ReplicateTimer(float cooldown)
        {
            this.AbilitySlot = (AbilitySlot)10; // force last
            this.Handle = uint.MaxValue;
            this.texture = nameof(AbilityId.morphling_morph_replicate);
            this.cooldown = cooldown;
            this.updateTime = GameManager.RawGameTime;
        }

        public AbilitySlot AbilitySlot { get; }

        public uint Handle { get; }

        private float RemainingCooldown
        {
            get
            {
                return (this.updateTime + this.cooldown) - GameManager.RawGameTime;
            }
        }

        public bool Display(bool isMorphed)
        {
            return true;
        }

        public void Draw(Rectangle9 position, float textSize)
        {
            try
            {
                RendererManager.DrawTexture(this.texture, position);
                RendererManager.DrawRectangle(position - 1, Color.Black);
                RendererManager.DrawTexture("o9k.ability_cd_bg", position);
                RendererManager.DrawText(
                    Math.Ceiling(this.RemainingCooldown).ToString("N0"),
                    position,
                    Color.White,
                    FontFlags.Center | FontFlags.VerticalCenter,
                    textSize);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public bool Update(bool isMorphed)
        {
            return this.RemainingCooldown > 0;
        }
    }
}