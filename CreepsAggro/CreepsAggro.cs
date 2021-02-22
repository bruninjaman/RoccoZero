namespace CreepsAggro
{
    using System.Linq;
    using System.Threading.Tasks;

    using Divine;
    using Divine.Menu.EventArgs;
    using Divine.Menu.Items;
    using Divine.SDK.Extensions;
    using Divine.SDK.Managers.Update;

    //[ExportPlugin("Creeps Aggro", StartupMode.Auto, "IdcNoob")]
    internal class CreepsAggro : Bootstrapper
    {
        private Unit hero;

        private Team heroTeam;

        private Config config;

        private UpdateHandler updateHandler;

        protected override void OnActivate()
        {
            config = new Config();
            config.Aggro.ValueChanged += KeyPressed;
            config.UnAggro.ValueChanged += KeyPressed;

            hero = EntityManager.LocalHero;
            heroTeam = hero.Team;

            updateHandler = UpdateManager.Subscribe(300, false, OnUpdate);
        }

        protected override void OnDeactivate()
        {
            config.Aggro.ValueChanged -= KeyPressed;
            config.UnAggro.ValueChanged -= KeyPressed;
            UpdateManager.Unsubscribe(OnUpdate);
            config.Dispose();
        }

        private void Attack(Unit unit)
        {
            if (unit == null)
            {
                return;
            }

            hero.Attack(unit);

            hero.Stop();
        }

        private void KeyPressed(MenuHoldKey sender, HoldKeyEventArgs e)
        {
            updateHandler.IsEnabled = e.Value;
        }

        private void OnUpdate()
        {
            if (config.Aggro)
            {
                var enemy = EntityManager.GetEntities<Hero>()
                    .Where(x => x.IsVisible && x.IsValid && x.IsAlive && !x.IsInvulnerable() && x.Team != heroTeam)
                    .OrderBy(x => hero.FindRotationAngle(x.Position))
                    .FirstOrDefault();

                Attack(enemy);
            }
            else if (config.UnAggro)
            {
                var ally = EntityManager.GetEntities<Creep>()
                    .Where(x => x.IsVisible && x.IsValid && x.IsAlive && x.IsSpawned && !x.IsInvulnerable() && x.Team == heroTeam)
                    .OrderBy(x => hero.FindRotationAngle(x.Position))
                    .FirstOrDefault();

                Attack(ally);
            }
        }
    }
}