using System.Globalization;
using Characters;
using Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Common
{
	public class GameContext : MonoBehaviour
	{
		[SerializeField] private GameObject _introGo;
		[SerializeField] private GameObject _inGameGo;
		[SerializeField] private GameObject _endedGo;
		[SerializeField] private Text _hpText;
		[SerializeField] private Text _gunText;

		private Tank _playerTank;
		private HealthComponent _playerHealthComponent;

		private MenuState _state;
		private enum MenuState
		{
			Intro,
			InGame,
			Ended
		}

		void Awake()
		{
			_playerTank = FindObjectOfType<Tank>();
			_playerHealthComponent = _playerTank.gameObject.GetComponent<HealthComponent>();
			_playerHealthComponent.ChangedHpEvent += UpdateHpIndicator;
			_playerHealthComponent.DieEvent += OnPlayerDiedHandler;
			_playerTank.WeaponChangedEvent += WeaponChangedHandler;

			OnPlayerDiedHandler();

			ChangeState(MenuState.Intro);
		}

		private void WeaponChangedHandler(string gunName)
		{
			_gunText.text = gunName;
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
}
