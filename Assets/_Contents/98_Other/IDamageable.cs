using UnityEngine;

public interface IDamageable
{
    /// <param name="attacker">�U����:�_���[�W��^������</param>
    void Damage(int value, GameObject attacker = null);
}
