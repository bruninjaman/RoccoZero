using SharpDX;

namespace Divine.Core.Entities.Abilities
{
    public interface IActiveAbility
    {
        float CastPoint { get; }

        bool CanHit(Unit target);

        int GetCastDelay(Unit target);

        int GetCastDelay(Vector3 position);

        int GetCastDelay();

        int GetHitTime(Unit target);

        int GetHitTime(Vector3 position);
    }
}
