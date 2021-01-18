namespace O9K.Hud.Modules.Particles.Units
{
    using System;
    using System.Collections.Generic;

    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using Divine;

    using MainMenu;

    internal class TrueSight : IHudModule
    {
        private readonly Dictionary<uint, Particle> effects = new Dictionary<uint, Particle>();

        private readonly MenuSwitcher show;

        private Team ownerTeam;

        public TrueSight(IHudMenu hudMenu)
        {
            this.show = hudMenu.ParticlesMenu.Add(new MenuSwitcher("True sight", "trueSight"));
            this.show.AddTranslation(Lang.Cn, "真实视域");
        }

        public void Activate()
        {
            this.ownerTeam = EntityManager9.Owner.Team;
            this.show.ValueChange += this.ShowOnValueChange;
        }

        public void Dispose()
        {
            this.show.ValueChange -= this.ShowOnValueChange;
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;

            foreach (var effect in this.effects)
            {
                effect.Value.Dispose();
            }

            this.effects.Clear();
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            try
            {
                var modifier = e.Modifier;
                var sender = modifier.Owner;
                if (sender.Team != this.ownerTeam)
                {
                    return;
                }

                if (!(sender is Hero))
                {
                    return;
                }

                if (modifier.Name != "modifier_truesight" && modifier.Name != "modifier_item_dustofappearance")
                {
                    return;
                }

                if (this.effects.ContainsKey(sender.Handle))
                {
                    return;
                }

                var effect = ParticleManager.CreateParticle("particles/items2_fx/ward_true_sight.vpcf", ParticleAttachment.CenterFollow, sender);
                this.effects.Add(sender.Handle, effect);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnModifierRemoved(ModifierRemovedEventArgs e)
        {
            try
            {
                var modifier = e.Modifier;
                var sender = modifier.Owner;
                if (sender.Team != this.ownerTeam)
                {
                    return;
                }

                if (!(sender is Hero))
                {
                    return;
                }

                if (modifier.Name != "modifier_truesight" && modifier.Name != "modifier_item_dustofappearance")
                {
                    return;
                }

                if (!this.effects.TryGetValue(sender.Handle, out var effect))
                {
                    return;
                }

                effect.Dispose();
                this.effects.Remove(sender.Handle);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void ShowOnValueChange(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                ModifierManager.ModifierAdded += this.OnModifierAdded;
                ModifierManager.ModifierRemoved += this.OnModifierRemoved;
            }
            else
            {
                ModifierManager.ModifierAdded -= this.OnModifierAdded;
                ModifierManager.ModifierRemoved -= this.OnModifierRemoved;

                foreach (var effect in this.effects)
                {
                    effect.Value.Dispose();
                }

                this.effects.Clear();
            }
        }
    }
}