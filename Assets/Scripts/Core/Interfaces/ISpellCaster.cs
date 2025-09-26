using Enemies.Base;

namespace Core.Interfaces
{
    public interface ISpellCaster
    {
        EnemyState SpellCastState { get; }
        bool CanCastSpell();
        void CastSpell();
    }
}