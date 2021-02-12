using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.SkywrathMage.Menus;

using Ensage;
using Ensage.SDK.Extensions;
using Ensage.SDK.Menu.ValueBinding;

namespace Divine.SkywrathMage.Features
{
    internal sealed class EulControl : BaseTaskHandler
    {
        private readonly BladeMailMenu BladeMailMenu;

        private readonly Abilities Abilities;

        public EulControl(Common common)
        {
            BladeMailMenu = (BladeMailMenu)common.MenuConfig.BladeMailMenu;

            Abilities = (Abilities)common.Abilities;

            if (BladeMailMenu.EulControlItem)
            {
                Unit.OnModifierAdded += OnModifierAdded;
                Unit.OnModifierRemoved += OnModifierRemoved;
            }

            BladeMailMenu.EulControlItem.Changed += EulControlChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            BladeMailMenu.EulControlItem.Changed -= EulControlChanged;

            if (BladeMailMenu.EulControlItem)
            {
                Unit.OnModifierAdded -= OnModifierAdded;
                Unit.OnModifierRemoved -= OnModifierRemoved;
            }
        }

        private void EulControlChanged(object sender, ValueChangingEventArgs<bool> e)
        {
            if (e.Value)
            {
                Unit.OnModifierAdded += OnModifierAdded;
                Unit.OnModifierRemoved += OnModifierRemoved;
            }
            else
            {
                Cancel();

                Unit.OnModifierAdded -= OnModifierAdded;
                Unit.OnModifierRemoved -= OnModifierRemoved;
            }
        }

        private void OnModifierAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if (sender is Hero && sender.IsEnemy(Owner.Base) && Abilities.MysticFlare != null && args.Modifier.Name == Abilities.MysticFlare.TargetModifierName)
            {
                RunAsync();
            }
        }

        private void OnModifierRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            if (Abilities.MysticFlare != null && args.Modifier.Name == Abilities.MysticFlare.TargetModifierName)
            {
                Cancel();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            if (IsStopped)
            {
                return;
            }

            var eul = Abilities.Eul;
            if (eul != null && eul.CanBeCasted)
            {
                if (UnitManager<CHero, Enemy, NoIllusion>.Units.Any(x => x.IsVisible && x.IsAlive && x.HasModifier("modifier_item_blade_mail_reflect")))
                {
                    eul.UseAbility(Owner);
                    await Task.Delay(eul.GetCastDelay(), token);
                }
            }
        }
    }
}
