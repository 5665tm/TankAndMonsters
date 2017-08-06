using System;
using UnityEngine;

namespace Characters
{
	public class Monster : MonoBehaviour
	{
		public enum MonsterState
		{
			Idle,
			Move,
			Attack,
			Damaged,
			Die
		}

		private MonsterAnimationComponent _animator;
		private MonsterState _state;
		private Rigidbody _rg;

		private float _lastChangedStateTime = 0;
		private Tank _player;
		private Transform _transform;
		private HealthComponent _healthComponent;

		// сколько раз проиграть анимацию покоя после спавна моба
		[SerializeField] private int _playIdleCount = 3;
		[SerializeField] private float _rotateSpeed = 2f;
		[SerializeField] private float _moveSpeed = 1;
		[SerializeField] private float distanceAttackSqr = 6;
		[SerializeField] private float _damage = 10;

		// допустимо 0..1 - (0 - нанесение дамага происходит в начале анимации атаки, 1 - в конце, 0.5 - в середине)
		private const float DAMAGED_TIME_MOMENT = 0.5f;

		private bool _attackCommited;

		private void Start()
		{
			_rg = GetComponent<Rigidbody>();
			_animator = GetComponent<MonsterAnimationComponent>();
			_transform = GetComponent<Transform>();
			_player = FindObjectOfType<Tank>();

			_healthComponent = GetComponent<HealthComponent>();
			_healthComponent.DamagedEvent += OnDamagedHandler;
			_healthComponent.DieEvent += OnDieHandler;

			Reset();
		}

		public bool IsDied()
		{
			return _healthComponent.IsDied();
		}

		private void OnDamagedHandler()
		{
			ChangeState(MonsterState.Damaged);
		}

		private void OnDieHandler()
		{
			ChangeState(MonsterState.Die);
		}

		public void Reset()
		{
			SetDefaultState();
			if (_healthComponent != null)
			{
				_healthComponent.Reset();
			}
		}

		private void SetDefaultState()
		{
			ChangeState(MonsterState.Idle);
		}

		private void ChangeState(MonsterState state)
		{
			_lastChangedStateTime = Time.timeSinceLevelLoad;
			_state = state;
			if (_animator != null)
			{
				_animator.PlayAnimFromState(state);
			}
		}

		private void TryChangeStateFromTimer(float time, MonsterState newState)
		{
			if (_lastChangedStateTime + time < Time.timeSinceLevelLoad)
			{
				ChangeState(newState);
			}
		}

		private void RotateToPlayer()
		{
			Quaternion targetRotation = Quaternion.LookRotation(_player.transform.position - _transform.position);
			_rg.MoveRotation( Quaternion.Slerp(_transform.rotation, targetRotation, _rotateSpeed*Time.deltaTime));
		}

		private void MoveToPlayer()
		{
			Vector3 newPosition = _rg.position + transform.TransformDirection(0, 0, _moveSpeed*Time.deltaTime);
			_rg.MovePosition(newPosition);
		}

		private void IdleStateUpdate()
		{
			RotateToPlayer();
			TryChangeStateFromTimer(_animator.GetAnimDuration(MonsterState.Idle) * _playIdleCount, MonsterState.Move);
		}

		private void MoveStateUpdate()
		{
			if (Vector3.SqrMagnitude(_transform.position - _player.transform.position) < distanceAttackSqr)
			{
				_attackCommited = false;
				ChangeState(MonsterState.Attack);
			}

			RotateToPlayer();
			MoveToPlayer();
		}

		private void AttackStateUpdate()
		{
			var attackDuration = _animator.GetAnimDuration(MonsterState.Attack);
			if (!_attackCommited && _lastChangedStateTime + attackDuration*DAMAGED_TIME_MOMENT < Time.timeSinceLevelLoad)
			{
				_player.TakeDamage(_damage);
				_attackCommited = true;
			}
			RotateToPlayer();
			TryChangeStateFromTimer(_animator.GetAnimDuration(MonsterState.Attack), MonsterState.Move);
		}

		private void DamagedStateUpdate()
		{
			TryChangeStateFromTimer(_animator.GetAnimDuration(MonsterState.Damaged), MonsterState.Move);
		}

		protected virtual void Update()
		{
			switch (_state)
			{
				case MonsterState.Idle:
					IdleStateUpdate();
					break;
				case MonsterState.Move:
					MoveStateUpdate();
					break;
				case MonsterState.Attack:
					AttackStateUpdate();
					break;
				case MonsterState.Damaged:
					DamagedStateUpdate();
					break;
			}
		}
	}
}