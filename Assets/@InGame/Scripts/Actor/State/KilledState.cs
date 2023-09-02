namespace PSB.InGame
{
    public class KilledState : DeathState
    {
        public KilledState(DataContext context) :
            base(context, StateType.Killed, ParticleType.Killed, "Ž€–S‚µ‚½") { }
    }
}
