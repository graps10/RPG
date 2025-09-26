using Enemies.Base;
using UnityEngine;

namespace Enemies.DeathBringer
{
    public class DeathBringerSpellCastState : EnemyState<EnemyDeathBringer>
    {
        protected const float ENTER_SPELL_TIMER = 0.5f;
        
        private int _amountOfSpells;
        private float _spellTimer;


        public DeathBringerSpellCastState(EnemyDeathBringer enemy, EnemyStateMachine stateMachine, int animBoolName) : 
            base(enemy, stateMachine, animBoolName) { }

        public override void Enter()
        {
            base.Enter();

            _amountOfSpells = enemy.GetAmountOfSpells();
            _spellTimer = ENTER_SPELL_TIMER;
        }

        public override void Update()
        {
            base.Update();

            _spellTimer -= Time.deltaTime;

            if (CanCast())
                enemy.CastSpell();

            if (_amountOfSpells <= 0)
                stateMachine.ChangeState(enemy.TeleportState);
        }

        public override void Exit()
        {
            base.Exit();

            enemy.lastTimeCast = Time.time;
        }

        private bool CanCast()
        {
            if (_amountOfSpells > 0 && _spellTimer < 0)
            {
                _amountOfSpells--;
                _spellTimer = enemy.GetSpellCooldown();
                return true;
            }

            return false;
        }
    }
}