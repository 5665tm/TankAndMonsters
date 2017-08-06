using UnityEngine;

namespace Characters
{
	public class Tank : MonoBehaviour
	{
		private Rigidbody _rg;

		[SerializeField] private float _speedMove;
		[SerializeField] private float _speedRotate;

		void Awake()
		{
			_rg = GetComponent<Rigidbody>();
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
		}
	}
}
