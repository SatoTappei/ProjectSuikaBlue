/// <summary>
/// Actor�̃X�e�[�^�X�̊e��p�����[�^��ǂݎ�邽�߂Ɏg�p����
/// �p�����[�^���Q�Ƃ��ĉ]�X�̏������s���ۂ͂���Interface���g�p���邱��
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