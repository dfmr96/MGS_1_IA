using System;
using Enemy;
using UnityEngine;

[Serializable]
public class PlayerStateAiming<T> : State<T>
{
    private T _inputToMove;
    private T _inputToIdle;
    private PlayerView _playerView;
    private Collider target;
    
    private float _detectionRadius = 10f; 
    private LayerMask _enemyLayer; 
    private LayerMask _obstructionLayer;
    private Transform _playerTransform;
    [SerializeField] private Collider[] _enemiesInRange = new Collider[20]; 
    [SerializeField] private Transform _currentTarget;
    

    private float _damage;
    private readonly PlayerModel _playerModel;
    private readonly T _inputToDeath;


    public PlayerStateAiming(FSM<T> fsm, T inputToMove, T inputToIdle, PlayerView playerView, float detectionRadius, LayerMask enemyLayer, LayerMask obstructionLayer, float damage, T inputToDeath, PlayerModel playerModel)
    {
        _fsm = fsm;
        _inputToMove = inputToMove;
        _inputToIdle = inputToIdle;
        _playerView = playerView;
        _playerTransform = Constants.Player.transform;
        _detectionRadius = detectionRadius;
        _enemyLayer = enemyLayer;
        _obstructionLayer = obstructionLayer;
        _damage = damage;
        _inputToDeath = inputToDeath;
        _playerModel = playerModel;
        _playerModel.OnDead += ToDeath;
    }

    private void ToDeath()
    {
        _fsm.Transition(_inputToDeath);
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entro en Aiming");
        FindNearestVisibleEnemy();
        
    }

    public override void Execute()
    {
        base.Execute();
        var aim = Input.GetKey(KeyCode.X);
        var fire = Input.GetKeyDown(KeyCode.C);
        _playerView.OnAiming(aim);


        if (_currentTarget != null)
        {
            Vector3 direction = (_currentTarget.position - _playerTransform.position).normalized;
            _playerTransform.forward = direction;
        }


        if (fire)
        {
            AudioManager.Instance.PlayAudioOneShot(AudioManager.Instance.SocomSFX);
            Debug.Log("Disparó");
            if (_currentTarget == null) return;
            if (_currentTarget.TryGetComponent<EnemyController>(out EnemyController enemy))
            {
                enemy.TakeDamage(_damage);
            }
        }
        
        if (Input.GetKeyUp(KeyCode.X))
        {
            Debug.Log("A Idle");
            _fsm.Transition(_inputToIdle);
        }
    }

    void FindNearestVisibleEnemy()
    {
        int enemyCount = Physics.OverlapSphereNonAlloc(_playerTransform.position, _detectionRadius, _enemiesInRange, _enemyLayer);
        float minDistance = Mathf.Infinity;
        _currentTarget = null;

        for (int i = 0; i < enemyCount; i++)
        {
            Transform enemyTransform = _enemiesInRange[i].transform;
            float distanceToEnemy = Vector3.Distance(_playerTransform.position, enemyTransform.position);

            if (!Physics.Linecast(_playerTransform.position, enemyTransform.position, _obstructionLayer))
            {
                // Elegir el enemigo más cercano
                if (distanceToEnemy < minDistance)
                {
                    minDistance = distanceToEnemy;
                    _currentTarget = enemyTransform;
                }
            }
        }
    }
}