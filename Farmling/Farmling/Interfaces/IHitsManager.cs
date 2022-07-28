using Divine.Entity.Entities;
using Divine.Entity.Entities.Units;
using Farmling.Models;

namespace Farmling.Interfaces;

public interface IHitsManager
{
    delegate void HitEvent(Hit projectile, bool added);

    public Dictionary<uint, List<Hit>> HitSources { get; set; }
    void TryToAddEntity(Entity entity);

    IReadOnlyList<Unit> GetUnitsInSystem();

    event HitEvent? Notify;
}
