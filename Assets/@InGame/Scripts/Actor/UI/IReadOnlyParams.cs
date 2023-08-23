/// <summary>
/// Actorのステータスの各種パラメータを読み取るために使用する
/// パラメータを参照して云々の処理を行う際はこのInterfaceを使用すること
/// </summary>
public interface IReadOnlyParams : IReadOnlyEvaluate, IReadOnlyObjectInfo
{
    float Food { get; }
    float Water { get; }
    float HP { get; }
    float LifeSpan { get; }
    float BreedingRate { get; }
}

public interface IReadOnlyEvaluate
{
    string ActionName { get; }
}

public interface IReadOnlyObjectInfo
{
    string Name { get; }
}