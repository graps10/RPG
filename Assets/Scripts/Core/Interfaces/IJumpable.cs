using Enemies.Base;

namespace Core.Interfaces
{
    public interface IJumpable
    {
        EnemyState JumpState { get; }
        bool CanJump();
    }
}