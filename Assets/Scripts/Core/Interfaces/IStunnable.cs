using Enemies.Base;

namespace Core.Interfaces
{
    public interface IStunnable
    {
        EnemyState StunnedState { get; }
        bool CanBeStunned();
    }
}