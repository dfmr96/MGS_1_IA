using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemyAttackState : State<StateEnum>
    {
        IAttack _attack;
        private EnemyView _enemyView;

        public EnemyAttackState(IAttack attack, IMove entityMove, EnemyView enemyView)
        {
            _attack = attack;
            _enemyView = enemyView;
            //entityMove.Stop();
        }

        public override void Enter()
        {
            base.Enter();
            _enemyView.OnAttack(true);
        }

        public override void Execute()
        {
            if (_attack.AttackCooldown == null || !_attack.AttackCooldown.IsCooldown())
            {
                Debug.Log("Atacado");
                _attack.Attack();
            }
            base.Execute();
        }

        public override void Sleep()
        {
            base.Sleep();
            _enemyView.OnAttack(false);
        }
    }
}
