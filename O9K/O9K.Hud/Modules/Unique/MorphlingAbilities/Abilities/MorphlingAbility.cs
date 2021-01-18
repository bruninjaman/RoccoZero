namespace O9K.Hud.Modules.Unique.MorphlingAbilities.Abilities
{
    using System;
    using System.Linq;

    using Core.Entities.Abilities.Base;
    using Core.Logger;
    using Core.Managers.Renderer.Utils;

    using Divine;

    using SharpDX;

    internal class MorphlingAbility : IMorphlingAbility
    {
        private readonly Ability9 ability;

        private readonly bool isAbilityReplicated;

        private readonly uint level;

        private readonly string texture;

        private float cooldown;

        private float updateTime;

        public MorphlingAbility(Ability9 ability)
        {
            this.ability = ability;
            this.texture = ability.Name;
            this.Handle = ability.Handle;
            this.AbilitySlot = this.ability.AbilitySlot;
            this.level = ability.Level;
            this.isAbilityReplicated = ability.BaseAbility.IsReplicated;
        }

        public AbilitySlot AbilitySlot { get; }

        public uint Handle { get; }

        private float RemainingCooldown
        {
            get
            {
                var cd = (this.updateTime + this.cooldown) - GameManager.RawGameTime;

                if (cd <= 0)
                {
                    this.cooldown = 0;
                }

                return cd;
            }
        }

        public bool Display(bool isMorphed)
        {
            return isMorphed != this.isAbilityReplicated;
        }

        public void Draw(Rectangle9 position, float textSize)
        {
            try
            {
                RendererManager.DrawTexture(this.texture, position);
                RendererManager.DrawRectangle(position - 1, Color.Black);

                if (this.level == 0)
                {
                    RendererManager.DrawTexture("o9k.ability_0lvl_bg", position);
                    return;
                }

                if (this.cooldown <= 0)
                {
                    return;
                }

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
            if (isMorphed != this.isAbilityReplicated)
            {
                return true;
            }

            if (!this.ability.IsValid)
            {
                return false;
            }

            if (this.ability.AbilitySlot < 0 && this.ability.Owner.BaseSpellbook.Spells.All(x => x.Handle != this.Handle))
            {
                return false;
            }

            if (this.level > 0)
            {
                this.updateTime = GameManager.RawGameTime;
                this.cooldown = this.ability.BaseAbility.Cooldown;
            }

            return true;
        }
    }
}