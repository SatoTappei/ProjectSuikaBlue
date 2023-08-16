/// <summary>
/// ActorのパラメータをUI側が読み取るために使用する
/// </summary>
public interface IReadOnlyParams
{
    float Food { get; }
    float Water { get; }
    float HP { get; }
    float LifeSpan { get; }
    float BreedingRate { get; }
}
