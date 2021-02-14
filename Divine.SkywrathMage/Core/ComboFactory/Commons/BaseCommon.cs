using System;

using Divine.Core.ComboFactory.Combos;
using Divine.Core.ComboFactory.Helpers;
using Divine.SDK.Managers.Update;

namespace Divine.Core.ComboFactory.Commons
{
    public abstract class BaseCommon : IDisposable
    {
        public abstract BaseAbilities Abilities { get; }

        public abstract BaseDamageCalculation DamageCalculation { get; }

        public abstract BaseLinkenBreaker LinkenBreaker { get; }

        public abstract BaseKillSteal KillSteal { get; }

        public abstract BaseCombo Combo { get; }

        public virtual BaseRenderer Renderer { get; set; }

        private readonly AutoItemManager AutoItem;

        public BaseCommon()
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (Renderer == null)
                {
                    Renderer = new BaseRenderer(this);
                }
            });
        }

        public virtual void Dispose()
        {
            Renderer.Dispose();

            AutoItem.Dispose();

            Combo.Dispose();
            KillSteal.Dispose();

            DamageCalculation.Dispose();
            Abilities.Dispose();
        }
    }
}