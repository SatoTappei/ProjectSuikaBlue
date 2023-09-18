namespace PSB.InGame
{
    public class RuleModule
    {
        DataContext _context;

        public RuleModule(DataContext context)
        {
            _context = context;
        }

        public bool IsDead() => _context.HP.Value <= 0;

        public bool IsHunger() => _context.Water.Value <= 0 && _context.Food.Value <= 0;

        // �ɐB��0�Ŕ��肷��ƁA�p�����[�^�̕ύX�����̌Ăяo������true�ɂȂ�Ȃ��Ȃ�
        public bool SkipMating() => _context.BreedingRate.Value <= 5; // TODO:�K���Ȓl
    }
}