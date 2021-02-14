namespace Divine.Core.Entities.Abilities.Components
{
    public interface IHasDamageAmplifier
    {
        DamageType AmplifierType { get; }

        float DamageAmplification { get; }
    }
}