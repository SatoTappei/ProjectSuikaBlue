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
    }
}