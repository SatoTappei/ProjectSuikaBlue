/// <summary>
/// Actor�̃p�����[�^��UI�����ǂݎ�邽�߂Ɏg�p����
/// </summary>
public interface IReadOnlyParams
{
    float Food { get; }
    float Water { get; }
    float HP { get; }
    float LifeSpan { get; }
    float BreedingRate { get; }
}
