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

		private AnimationSupport _animator;
		private MonsterState _state;
		private Rigidbody _rg;

		private float _lastChangedStateTime = 0;
		private Tank _player;
		private Transform _transform;
		private HealthSupport _healthSupport;

		// сколько раз проиграть анимацию покоя после спавна моба
		private const int PLAY_IDLE_COUNT = 3;
		private const float ROTATE_SPEED = 2f;
		private const int MOVE_SPEED = 1;
		private const int DISTANCE_ATTACK_SQR = 6;
		private const float DAMAGE = 10;

		// допустимо 0..1 - (0 - нанесение дамага происходит в начале анимации атаки, 1 - в конце, 0.5 - в середине)
		private const float DAMAGED_TIME_MOMENT = 0.5f;

		private bool _attackCommited;

		private void Start()
		{
			_rg = GetComponent<Rigidbody>();
			_animator = GetComponent<AnimationSupport>();
			_transform = GetComponent<Transform>();
			_player = FindObjectOfType<Tank>();

			_healthSupport = GetComponent<HealthSupport>();
			_healthSupport.DamagedEvent += OnDamagedHandler;
			_healthSupport.DieEvent += OnDieHandler;

			Reset();
		}

		private void OnDamagedHandler()
		{
			ChangeState(MonsterState.Damaged);
		}

		private void OnDieHandler()
		{
			ChangeState(MonsterState.Die);
		}

		private void Reset()
		{
			SetDefaultState();
			_healthSupport.Reset();
		}

		private void SetDefaultState()
		{
			ChangeState(MonsterState.Idle);
		}

		private void ChangeState(MonsterState state)
		{
			_lastChangedStateTime = Time.timeSinceLevelLoad;
			_state = state;
			_animator.PlayAnimFromState(state);
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
			_rg.MoveRotation( Quaternion.Slerp(_transform.rotation, targetRotation, ROTATE_SPEED*Time.deltaTime));
		}

		private void MoveToPlayer()
		{
			Vector3 newPosition = _rg.position + transform.TransformDirection(0, 0, MOVE_SPEED*Time.deltaTime);
			_rg.MovePosition(newPosition);
		}

		private void IdleStateUpdate()
		{
			RotateToPlayer();
			TryChangeStateFromTimer(_animator.GetAnimDuration(MonsterState.Idle) * PLAY_IDLE_COUNT, MonsterState.Move);
		}

		private void MoveStateUpdate()
		{
			if (Vector3.SqrMagnitude(_transform.position - _player.transform.position) < DISTANCE_ATTACK_SQR)
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
				_player.TakeDamage(DAMAGE);
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
