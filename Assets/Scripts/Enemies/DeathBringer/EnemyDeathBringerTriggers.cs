using Enemies.Base;

namespace Enemies.DeathBringer
{
    public class EnemyDeathBringerTriggers : EnemyAnimationTriggers
    {
        private EnemyDeathBringer enemyDeathBringer => GetComponentInParent<EnemyDeathBringer>();

        private void Relocate() => enemyDeathBringer.FindAvailableTeleportPosition();
        private void MakeInvisible() => enemyDeathBringer.Fx.MakeTransparent(true);
        private void MakeVisible() => enemyDeathBringer.Fx.MakeTransparent(false);
    }
}
