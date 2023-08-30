public interface IReadOnlyActorStatus
{
    public float Food { get; }
    public float Water { get; }
    public float HP { get; }
    public float LifeSpan { get; }
    public float BreedingRate { get; }
    public string StateName { get; }
}
