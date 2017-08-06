using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Characters;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
	[SerializeField] private Monster _monster;
	[SerializeField] private float _spawnTime;

	private List<Monster> _activeMonsters = new List<Monster>();
	private List<Monster> _diedMonsters = new List<Monster>();

	private float _nextSpawnTime;
	private float _nextCleanTime;
	private bool _allowSpawn;

	private const float CLEAN_TIME = 2f;
	private const int MAX_MONSTER_COUNT = 10;
	private static int _monstersCount = 0;

	private void Awake()
	{
		_monstersCount = 0;
	}

	private void Update()
	{
		if (Time.timeSinceLevelLoad > _nextCleanTime)
		{
			Clean();
		}

		if (Time.timeSinceLevelLoad > _nextSpawnTime)
		{
			TrySpawn();
		}
	}

	private void TrySpawn()
	{
		_nextSpawnTime = Time.timeSinceLevelLoad + _spawnTime;

		if (_monstersCount >= MAX_MONSTER_COUNT)
		{
			return;
		}
		_monstersCount++;

		Monster newMonster;
		if (_diedMonsters.Count == 0)
		{
			newMonster = Instantiate(_monster);
		}
		else
		{
			newMonster = _diedMonsters[0];
			_diedMonsters.RemoveAt(0);
		}

		newMonster.transform.position = transform.position;
		newMonster.Reset();
		newMonster.gameObject.SetActive(true);
		_activeMonsters.Add(newMonster);
	}

	private void Clean()
	{
		_nextCleanTime = Time.timeSinceLevelLoad + CLEAN_TIME;
		for (int i = 0; i < _activeMonsters.Count; i++)
		{
			var activeMonster = _activeMonsters[i];
			if (activeMonster.IsDied())
			{
				activeMonster.gameObject.SetActive(false);
				_activeMonsters.Remove(activeMonster);
				_diedMonsters.Add(activeMonster);
				_monstersCount--;
				i--;
			}
		}
	}
}
