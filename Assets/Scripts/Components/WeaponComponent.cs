using System;
using System.Collections;
using Characters;
using UnityEngine;

namespace Components
{
	public class WeaponComponent : MonoBehaviour
	{
		[SerializeField] private string _identifier;
		[SerializeField] private GameObject _flareGo;
		[SerializeField] private Transform _fromPoint;
		[SerializeField] private float _reloadTime;
		[SerializeField] private float _damage;
		[SerializeField] private bool _penetrated;
		[SerializeField] private AudioClip _shootSound;
		[SerializeField] private float _distance;

		private float _lastShootTime;
		private AudioSource _audioSource;

		private void Start()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		public void Attack()
		{
			if (Time.timeSinceLevelLoad < _lastShootTime + _reloadTime)
			{
				return;
			}

			_audioSource.PlayOneShot(_shootSound);
			StartCoroutine(RunFlare());
			_lastShootTime = Time.timeSinceLevelLoad;
			FireProjectile();
		}

		private IEnumerator RunFlare()
		{
			const float FLARE_DURATION = 0.1f;
			_flareGo.SetActive(true);
			yield return new WaitForSeconds(FLARE_DURATION);
			_flareGo.SetActive(false);
		}

		public string GetIdentifier()
		{
			return _identifier;
		}


		private void FireProjectile()
		{
			_distance = 20;
			Ray ray = GetProjectileRay();

#if UNITY_EDITOR
			Debug.DrawRay(ray.origin, ray.direction * _distance, Color.red, 4, true);
#endif

			RaycastHit[] hits = Physics.RaycastAll(ray, _distance, -1, QueryTriggerInteraction.Ignore);
			if (hits.Length == 0)
			{
				return;
			}

			Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				if (hit.distance > _distance)
				{
					return;
				}

				Collider cld = hit.collider;
				if (cld.isTrigger || !cld.enabled)
					continue;
				GameObject hitGo = cld.gameObject;

				Monster monster = hitGo.GetComponent<Monster>();
				if (monster == null)
				{
					return;
				}

				var _healthComp = monster.GetComponent<HealthComponent>();
				if (!_healthComp.IsDied())
				{
					_healthComp.TakeDamage(_damage);
					if (!_penetrated)
					{
						break;
					}
				}
			}
		}

		private Ray GetProjectileRay()
		{
			var origin = _fromPoint.position;

			Ray ray = new Ray();
			ray.origin = origin;
			var dir = _fromPoint.forward;
			dir.Normalize();
			ray.direction = dir;

			return ray;
		}
	}
}
