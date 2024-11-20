using DefaultNamespace.Randoms;

namespace Enemy
{
    public class EnemyDeadState : State<StateEnum>
    {
        private EnemyView _enemyView;
        private EnemyAudio _enemyAudio;

        public EnemyDeadState(EnemyView enemyView, EnemyAudio enemyAudio)
        {
            _enemyView = enemyView;
            _enemyAudio = enemyAudio;
        }

        public override void Enter()
        {
            base.Enter();
            _enemyView.OnDead(true);
            _enemyAudio.PlayRandomDeathAudio();
            ItemSpawner.Instance.SpawnRandomItem(_enemyView.transform.position);
        }
    }
}