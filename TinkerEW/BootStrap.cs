using Divine.Entity;
using Divine.Service;

namespace TinkerEW;

public class BootStrap : Bootstrapper
{
    private Context? Context;

    protected override void OnActivate()
    {
        if (EntityManager.LocalHero.Name == "npc_dota_hero_tinker")
        {
            Context = new Context();
        }
    }

    protected override void OnDeactivate()
    {
        Context?.Dispose();
    }
}