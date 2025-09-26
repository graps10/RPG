using Enemies.Base;

namespace Core.Interfaces
{
    public interface ITeleportable
    {
        EnemyState TeleportState { get; }
        bool CanTeleport();
    }
}