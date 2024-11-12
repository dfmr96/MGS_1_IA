using System.Collections.Generic;
using UnityEngine;

namespace Enemy.EnemyStates
{
    public class EnemyChaseState : StatePathfinding<StateEnum>
    {
        public EnemyChaseState(Transform entity, IMove move, EnemyView anim, float distanceToPoint = 0.2f) : 
            base(entity, move, anim.GetComponent<Animator>(), distanceToPoint)
        {
            AlertManager.Instance.OnLastPlayerPositionChanged += OnPlayerPositionChanged;
        }

        public override void Enter()
        {
            base.Enter();
            SetPathAStarPlus(AlertManager.Instance.PlayerLastPosition);
        }

        private void OnPlayerPositionChanged()
        {
            SetPathAStarPlus(AlertManager.Instance.PlayerLastPosition);
        }
    }
}