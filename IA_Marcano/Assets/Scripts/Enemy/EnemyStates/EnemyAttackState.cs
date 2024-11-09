using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemyAttackState : State<StateEnum>
    {
        IAttack _attack;

        public EnemyAttackState(IAttack attack, IMove entityMove)
        {
            _attack = attack;
            //entityMove.Stop();
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
    }
}
