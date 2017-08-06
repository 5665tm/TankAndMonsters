using System;
using UnityEngine;

namespace Characters
{
	public class HealthComponent : MonoBehaviour
	{
		[SerializeField]
		private float _startedHealth;

		private float _currentHealth;

		public float Health
		{
			get
			{
				return IsDied() ? 0 : _currentHealth;
			}
		}

		public event Action DieEvent;
		public event Action DamagedEvent;
		public event Action<float> ChangedHpEvent;

		public void Awake()
		{
			Reset();
		}

		public void TakeDamage(float damage)
		{
			if (IsDied())
			{
				return;
			}

			_currentHealth -= damage;
			ChangedHpEvent.SafeCall(_currentHealth);

			if (IsDied())
			{
				DieEvent.SafeCall();
			}
			else
			{
				DamagedEvent.SafeCall();
			}
		}

		public bool IsDied()
		{
			return _currentHealth <= 0;
		}

		public void Reset()
		{
			_currentHealth = _startedHealth;
			ChangedHpEvent.SafeCall(_currentHealth);
		}
	}
}
