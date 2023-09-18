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

        // 繁殖率0で判定すると、パラメータの変更処理の呼び出し順でtrueにならなくなる
        public bool SkipMating() => _context.BreedingRate.Value <= 5; // TODO:適当な値
    }
}