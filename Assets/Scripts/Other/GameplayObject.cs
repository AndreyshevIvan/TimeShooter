using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GameFactory;

namespace MyGame
{
	public class GameplayObject : MonoBehaviour
	{
		public void Init(IGameplay gameplay)
		{
			if (gameplay == null)
			{
				return;
			}

			m_gameplay = gameplay;
			OnInitGameplayComplete();
			GameplayChange(m_gameplay.state);
		}
		public void GameplayChange(GameplayState newState)
		{
			OnChangeGameplay(newState);
			m_currentEvent = () => { };

			switch (newState)
			{
				case (GameplayState.BEFORE_START):
					OnPreStart();
					m_currentEvent = BeforePlayingUpdate;
					m_currentEvent += beforePlayingUpd;
					break;

				case (GameplayState.PAUSE):
					OnPause();
					m_currentEvent = PauseUpdate;
					m_currentEvent += pauseUpd;
					break;

				case (GameplayState.PLAYING):
					OnPlaying();
					m_currentEvent = PlayingUpdate;
					m_currentEvent += playingUpd;
					break;

				case (GameplayState.AFTER_MATCH):
					OnEndGameplay();
					m_currentEvent = AfterMatchUpdate;
					m_currentEvent += afterPlayingUpd;
					break;
			}
		}

		protected IGameplay gameplay { get { return m_gameplay; } }
		protected EventDelegate beforePlayingUpd { get; set; }
		protected EventDelegate playingUpd { get; set; }
		protected EventDelegate pauseUpd { get; set; }
		protected EventDelegate afterPlayingUpd { get; set; }

		protected virtual void OnInitGameplayComplete() { }
		protected void FixedUpdate()
		{
			if (m_gameplay == null)
			{
				return;
			}

			if (m_currentEvent != null) m_currentEvent();
		}

		protected virtual void OnChangeGameplay(GameplayState newState) { }
		protected virtual void OnPreStart() { }
		protected virtual void OnPlaying() { }
		protected virtual void OnEndGameplay() { }
		protected virtual void OnPause() { }

		protected virtual void BeforePlayingUpdate() { }
		protected virtual void PlayingUpdate() { }
		protected virtual void PauseUpdate() { }
		protected virtual void AfterMatchUpdate() { }

		private IGameplay m_gameplay;
		private EventDelegate m_currentEvent;
	}
}
