using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Characters;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameContext : MonoBehaviour
{
	[SerializeField] private GameObject _introGo;
	[SerializeField] private GameObject _inGameGo;
	[SerializeField] private GameObject _endedGo;
	[SerializeField] private Text _hpText;
	[SerializeField] private HealthSupport _playerHealthSupport;

	private MenuState _state;
	private enum MenuState
	{
		Intro,
		InGame,
		Ended
	}

	void Awake()
	{
		_playerHealthSupport.ChangedHpEvent += UpdateHpIndicator;
		_playerHealthSupport.DieEvent += OnPlayerDiedHandler;

		OnPlayerDiedHandler();

		ChangeState(MenuState.Intro);
	}

	private void UpdateHpIndicator(float hp)
	{
		_hpText.text = hp.ToString(CultureInfo.InvariantCulture);
	}

	private void ChangeState(MenuState newState)
	{
		_state = newState;
		_introGo.SetActive(_state == MenuState.Intro);
		_inGameGo.SetActive(_state == MenuState.InGame);
		_endedGo.SetActive(_state == MenuState.Ended);
	}

	public void Go()
	{
		Time.timeScale = 1;
		ChangeState(MenuState.InGame);
	}

	private void OnPlayerDiedHandler()
	{
		Time.timeScale = 0;
		ChangeState(MenuState.Ended);
	}

	public void Restart()
	{
		Time.timeScale = 0;
		SceneManager.LoadScene("Game");
	}
}
