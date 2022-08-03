namespace Farmling.Models;

public record PredictionData
{
    public PredictionData(float predictedHealth, float timeBeforeHit, bool willTargetDie, List<Hit> allHits, List<Hit> comingHits, float myDamage)
    {
        PredictedHealth = predictedHealth;
        TimeBeforeHit = timeBeforeHit;
        WillTargetDie = willTargetDie;
        AllHits = allHits;
        ComingHits = comingHits;
        MyDamage = myDamage;
    }

    public PredictionData(float predictedHealth, float timeBeforeHit, bool willTargetDie)
    {
        PredictedHealth = predictedHealth;
        TimeBeforeHit = timeBeforeHit;
        WillTargetDie = willTargetDie;
        AllHits = new List<Hit>();
        ComingHits = new List<Hit>();
    }

    public float PredictedHealth { get; }
    public float TimeBeforeHit { get; }
    public bool WillTargetDie { get; }
    public List<Hit> AllHits { get; }
    public List<Hit> ComingHits { get; }
    public float MyDamage { get; }
}
