using UnityEngine;

public interface IDamageable
{
    /// <param name="attacker">�U����:�_���[�W��^������</param>
    void Damage(GameObject attacker, int value = 1);
}
