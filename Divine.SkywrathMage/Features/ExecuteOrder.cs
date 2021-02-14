using Divine.Core.Helpers;
using Divine.SkywrathMage.Menus;



namespace Divine.SkywrathMage.Features
{
    internal sealed class ExecuteOrder
    {
        private readonly SmartConcussiveShotMenu SmartConcussiveShotMenu;

        private readonly Abilities Abilities;

        public ExecuteOrder(Common common)
        {
            SmartConcussiveShotMenu = ((MoreMenu)common.MenuConfig.MoreMenu).SmartConcussiveShotMenu;

            Abilities = (Abilities)common.Abilities;

            Player.OnExecuteOrder += OnExecuteOrder;
        }

        public void Dispose()
        {
            Player.OnExecuteOrder -= OnExecuteOrder;
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (args.OrderId == OrderId.Ability)
            {
                var concussiveShot = Abilities.ConcussiveShot;
                if (args.Ability == concussiveShot.Base)
                {
                    var targetHit = concussiveShot.TargetHit;
                    if (targetHit == null)
                    {
                        if (SmartConcussiveShotMenu.AntiFailItem)
                        {
                            args.Process = false;
                        }

                        return;
                    }

                    var castDelay = concussiveShot.GetCastDelay();
                    var hitTime = concussiveShot.GetHitTime(targetHit) - (castDelay + 150);
                    MultiSleeper<string>.DelaySleep($"IsHitTime_{targetHit.Name}_{concussiveShot.Name}", castDelay + 50, hitTime);
                }
            }
        }
    }
}
