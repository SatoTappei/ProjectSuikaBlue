using UnityEngine;

public interface IDamageable
{
    /// <param name="attacker">UŒ‚‘¤:ƒ_ƒ[ƒW‚ğ—^‚¦‚½‘¤</param>
    void Damage(int value, GameObject attacker = null);
}
