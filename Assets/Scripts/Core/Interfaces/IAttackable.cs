using Enemies.Base;

namespace Core.Interfaces
{
    public interface IAttackable
    {
        EnemyState AttackState { get; }
        bool CanAttack();
    }
}