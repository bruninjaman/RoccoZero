namespace O9K.Evader.Evader.EvadeModes
{
    using System.Collections.Generic;
    using System.Linq;

    using Core.Managers.Assembly;

    using Metadata;

    using Modes;

    using O9K.Core.Managers.Context;

    using Pathfinder.Obstacles;

    internal enum EvadeMode
    {
        Dodge,

        Counter,

        Blink,

        Disable,

        GoldSpend
    }

    internal class EvadeModeManager : IEvaderService, IEvadeModeManager
    {
        private readonly IAbilityManager abilityManager;

        private readonly IActionManager actionManager;

        private readonly IAssemblyEventManager9 eventManager;

        private readonly IMainMenu menu;

        private readonly Dictionary<EvadeMode, EvadeBaseMode> modes = new Dictionary<EvadeMode, EvadeBaseMode>(4);

        private readonly IPathfinder pathfinder;

        public EvadeModeManager(
            IAbilityManager abilityManager,
            IPathfinder pathfinder,
            IActionManager actionManager,
            IMainMenu menu)
        {
            this.eventManager = Context9.AssemblyEventManager;
            this.abilityManager = abilityManager;
            this.pathfinder = pathfinder;
            this.actionManager = actionManager;
            this.menu = menu;
        }

        public static bool MoveCamera { get; private set; }

        public LoadOrder LoadOrder { get; } = LoadOrder.EvadeModeManager;

        public void Activate()
        {
            this.modes.Add(
                EvadeMode.Dodge,
                new DodgeMode(this.actionManager, this.abilityManager.UsableDodgeAbilities, this.pathfinder, this.menu));
            this.modes.Add(EvadeMode.Counter, new CounterMode(this.actionManager, this.abilityManager.UsableCounterAbilities));
            this.modes.Add(EvadeMode.Blink, new BlinkMode(this.actionManager, this.abilityManager.UsableBlinkAbilities));
            this.modes.Add(EvadeMode.Disable, new DisableMode(this.actionManager, this.abilityManager.UsableDisableAbilities));

            this.eventManager.OrderBlockerMoveCamera += this.EventManager_OnOrderBlockerMoveCamera;
        }

        public void Dispose()
        {
            this.eventManager.OrderBlockerMoveCamera -= this.EventManager_OnOrderBlockerMoveCamera;

            this.modes.Clear();
        }

        public IEnumerable<EvadeBaseMode> GetEvadeModes(IObstacle obstacle)
        {
            var order = obstacle.EvadableAbility.UseCustomPriority ? obstacle.EvadableAbility.Priority : this.menu.Settings.DefaultPriority;

            return order.Select(x => this.modes[x]);
        }

        private void EventManager_OnOrderBlockerMoveCamera(object sender, bool e)
        {
            MoveCamera = e;
        }
    }
}