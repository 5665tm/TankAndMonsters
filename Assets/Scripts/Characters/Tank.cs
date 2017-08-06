using System;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
	public class Tank : MonoBehaviour
	{
		private Rigidbody _rg;
		private HealthComponent _healthComponent;

		[SerializeField] private float _speedMove;
		[SerializeField] private float _speedRotate;
		[SerializeField] private List<WeaponComponent> _weapons;

		private int _activeWeaponIndex = 0;

		public event Action<string> WeaponChangedEvent;

		void Start()
		{
			_rg = GetComponent<Rigidbody>();
			_healthComponent = GetComponent<HealthComponent>();
			ChangeWeapon();
		}

		void FixedUpdate()
		{
			if (Input.GetKey("w"))
			{
				Vector3 newPosition = _rg.position + transform.TransformDirection(0, 0, _speedMove*Time.deltaTime);
				_rg.MovePosition(newPosition);
			}
			else if (Input.GetKey("s"))
			{
				Vector3 newPosition = _rg.position + transform.TransformDirection(0, 0, -_speedMove*Time.deltaTime);
				_rg.MovePosition(newPosition);
			}

			if (Input.GetKey("a"))
			{
				Vector3 eulerAngleVelocity = new Vector3(0, -_speedRotate, 0);
				Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
				_rg.MoveRotation(_rg.rotation * deltaRotation);
			}
			else if (Input.GetKey("d"))
			{
				Vector3 eulerAngleVelocity = new Vector3(0, _speedRotate, 0);
				Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
				_rg.MoveRotation(_rg.rotation * deltaRotation);
			}

			if (Input.GetMouseButton(0))
			{
				Attack();
			}

			if (Input.GetKeyDown("q"))
			{
				ChangeWeapon();
			}
		}

		public void TakeDamage(float damage)
		{
			_healthComponent.TakeDamage(damage);
		}

		private void Attack()
		{
			WeaponComponent weapon = _weapons[_activeWeaponIndex];
			weapon.Attack();
		}

		private void ChangeWeapon()
		{
			_activeWeaponIndex = _activeWeaponIndex == 0 ? 1 : 0;
			WeaponChangedEvent.SafeCall(_weapons[_activeWeaponIndex].GetIdentifier());
		}
	}
}