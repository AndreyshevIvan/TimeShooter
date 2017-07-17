using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
	public class FPSDisplay : MonoBehaviour
	{
		private void Awake()
		{
			m_field = GetComponent<Text>();
		}
		private void Update()
		{
			m_deltaTime += (Time.deltaTime - m_deltaTime) * 0.1f;

			float msec = m_deltaTime * 1000.0f;
			float fps = 1.0f / m_deltaTime;
			m_field.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		}

		private float m_deltaTime = 0.0f;
		private Text m_field;
	}
}
