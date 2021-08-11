namespace O9K.AIO.Heroes.ArcWarden.Draw
{
    using System;

    using Core.Entities.Abilities.Base;
    using Core.Logger;
    using Core.Managers.Renderer.Utils;

    using Divine.Numerics;
    using Divine.Renderer;

    internal class AbilityDrawer
    {
        private readonly bool displayLevel;

        private readonly int maxLevel;

        private bool abilityEnabled = true;

        public AbilityDrawer(Ability9 ability)
        {
            this.Ability = ability;
            this.maxLevel = ability.MaximumLevel;
            this.displayLevel = this.maxLevel > 1;
        }

        public Ability9 Ability { get; }

        public bool ShouldDisplay
        {
            get
            {
                if (!this.abilityEnabled)
                {
                    return false;
                }

                if (!this.Ability.IsValid)
                {
                    return false;
                }

                return this.Ability.IsAvailable /* && !this.Ability.IsHidden*/;
            }
        }

        public void ChangeEnabled(bool enabled)
        {
            this.abilityEnabled = enabled;
        }

        public void Draw(Rectangle9 position, float cooldownSize)
        {
            try
            {
                RendererManager.DrawImage(this.Ability.TextureName, position, ImageType.Ability);

                if (this.Ability.IsCasting || this.Ability.IsChanneling)
                {
                    RendererManager.DrawRectangle(position - 3f, Color.LightGreen, 3);
                }
                else
                {
                    RendererManager.DrawRectangle(position - 1, Color.Black);
                }

                if (this.displayLevel)
                {
                    uint level = this.Ability.Level;

                    if (level == 0)
                    {
                        RendererManager.DrawImage("o9k.ability_0lvl_bg", position);

                        return;
                    }

                    string levelText = level.ToString("N0");
                    var levelSize = RendererManager.MeasureText(levelText, position.Width * 0.45f);
                    var levelPosition = position.SinkToBottomLeft(levelSize.X, levelSize.Y * 0.8f);

                    RendererManager.DrawImage("o9k.ability_lvl_bg", levelPosition);
                    RendererManager.DrawText(levelText, levelPosition, Color.White, FontFlags.VerticalCenter, position.Width * 0.45f);
                }

                if (this.Ability.IsDisplayingCharges)
                {
                    string chargesText = this.Ability.Charges.ToString("N0");
                    var chargesPosition = position.SinkToBottomRight(position.Width * 0.5f, position.Height * 0.5f);
                    RendererManager.DrawImage("o9k.charge_bg", chargesPosition);
                    RendererManager.DrawImage("o9k.outline_green", chargesPosition * 1.07f);
                    RendererManager.DrawText(chargesText, chargesPosition, Color.White, FontFlags.Center, position.Width * 0.45f);
                }

                if (this.Ability.IsChanneling)
                {
                    return;
                }

                float cooldown = this.Ability.RemainingCooldown;

                if (cooldown > 0)
                {
                    RendererManager.DrawImage("o9k.ability_cd_bg", position);

                    RendererManager.DrawText(
                        Math.Ceiling(cooldown).ToString("N0"),
                        position,
                        Color.White,
                        FontFlags.Center | FontFlags.VerticalCenter,
                        cooldownSize);
                }
                else if (this.Ability.ManaCost > this.Ability.Owner.Mana)
                {
                    RendererManager.DrawImage("o9k.ability_mana_bg", position);

                    RendererManager.DrawText(
                        Math.Ceiling((this.Ability.ManaCost - this.Ability.Owner.Mana) / this.Ability.Owner.ManaRegeneration)
                            .ToString("N0"),
                        position,
                        Color.White,
                        FontFlags.Center | FontFlags.VerticalCenter,
                        cooldownSize);
                }
                else if (!this.Ability.IsUsable)
                {
                    RendererManager.DrawImage("o9k.ability_0lvl_bg", position);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void DrawMinimalistic(Rectangle9 position, float cooldownSize)
        {
            try
            {
                RendererManager.DrawImage("o9k.ability_minimal_bg", position);
                float levelHeight = position.Height * 0.2f;

                float cooldown = this.Ability.RemainingCooldown;

                if (cooldown > 0)
                {
                    position = position.MoveTopBorder(cooldownSize * 0.8f);
                    RendererManager.DrawImage("o9k.ability_minimal_cd_bg", position);

                    RendererManager.DrawText(
                        Math.Ceiling(cooldown).ToString("N0"),
                        position.MoveTopBorder(cooldownSize * 0.2f),
                        Color.White,
                        FontFlags.Center | FontFlags.VerticalCenter,
                        cooldownSize);
                }
                else if (this.Ability.ManaCost > this.Ability.Owner.Mana)
                {
                    position = position.MoveTopBorder(cooldownSize * 0.8f);
                    RendererManager.DrawImage("o9k.ability_minimal_mana_bg", position);

                    RendererManager.DrawText(
                        Math.Ceiling((this.Ability.ManaCost - this.Ability.Owner.Mana) / this.Ability.Owner.ManaRegeneration)
                            .ToString("N0"),
                        position.MoveTopBorder(cooldownSize * 0.2f),
                        Color.White,
                        FontFlags.Center | FontFlags.VerticalCenter,
                        cooldownSize);
                }

                RendererManager.DrawRectangle(position - 1, this.Ability.IsCasting ? Color.LightGreen : Color.Black);

                var rec = position - 5;
                float levelWidth = rec.Width / this.maxLevel;
                float space = levelWidth * 0.05f;
                float posY = rec.Bottom - levelHeight;
                float levelDrawWidth = levelWidth - space * 2;
                uint lvl = this.Ability.Level;

                for (int i = 0; i < lvl; i++)
                {
                    var lvlPos = new Rectangle9(rec.X + space, posY, levelDrawWidth, levelHeight);
                    RendererManager.DrawImage("o9k.ability_level_rec", lvlPos);
                    rec = rec.MoveLeftBorder(levelWidth);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}