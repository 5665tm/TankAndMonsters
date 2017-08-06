using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using UnityEngine;

public class AnimationSupport : MonoBehaviour
{

	[SerializeField] private AnimationClip _idle;
	[SerializeField] private AnimationClip _move;
	[SerializeField] private AnimationClip _attack;
	[SerializeField] private AnimationClip _damaged;
	[SerializeField] private AnimationClip _die;

	private Animation _animation;

	private void Awake()
	{
		_animation = GetComponent<Animation>();
	}
	
	public void PlayAnimFromState(Monster.MonsterState newState)
	{
		_animation.Blend(GetAnimFromState(newState).name, 1, 0.2f);
	}

	public float GetAnimDuration(Monster.MonsterState state)
	{
		return GetAnimFromState(state).length;
	}

	private AnimationClip GetAnimFromState(Monster.MonsterState state)
	{
		switch (state)
		{
			case Monster.MonsterState.Idle:
				return _idle;
			case Monster.MonsterState.Move:
				return _move;
			case Monster.MonsterState.Attack:
				return _attack;
			case Monster.MonsterState.Damaged:
				return _damaged;
			case Monster.MonsterState.Die:
				return _die;
		}
		return _idle;
	}
}
