using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
		}
		public void GameplayChange(GameplayState newState)
		{
			OnChangeGameplay(newState);
			m_currentEvent = () => {; };

			switch (newState)
			{
				case (GameplayState.BEFORE_START):
					OnPreStart();
					m_currentEvent = BeforePlayingUpdate;
					break;

				case (GameplayState.PAUSE):
					OnPause();
					m_currentEvent = PauseUpdate;
					break;

				case (GameplayState.PLAYING):
					OnPlaying();
					m_currentEvent = PlayingUpdate;
					break;

				case (GameplayState.AFTER_MATCH):
					OnEndGameplay();
					m_currentEvent = AfterMatchUpdate;
					break;
			}
		}

		protected IGameplay gameplay { get { return m_gameplay; } }

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
