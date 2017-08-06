using UnityEngine;

namespace Characters
{
	public class Monster : MonoBehaviour
	{
		private Animator _animator;
		[SerializeField]
		private AnimationClip clip;
		[SerializeField] private string _animationMove;

		void Start()
		{
			_animator = GetComponent<Animator>();
			_animator.Play(_animationMove);
		}

		void Update()
		{
		}
	}
}
