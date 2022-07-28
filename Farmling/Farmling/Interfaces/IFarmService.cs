using Divine.Entity.Entities.Units;
using Farmling.Models;

namespace Farmling.Interfaces;

public interface IFarmService
{
    public Dictionary<uint, PredictionData> PredictionData { get; set; }
    float GetTimeToHitTarget(Unit target);
}
