using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
	public class ParticlesAutoDestroy : MonoBehaviour
	{
		private ParticleSystem[] m_particles;

		private void Awake()
		{
			m_particles = GetComponents<ParticleSystem>();
		}
		private void FixedUpdate()
		{
			foreach (ParticleSystem particle in m_particles)
			{
				if (particle.IsAlive())
				{
					return;
				}
			}

			Destroy(gameObject);
		}
	}
}
