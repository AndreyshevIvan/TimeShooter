using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyGame;

namespace GameUtils
{
	public struct Utils
	{
		public static void SetWidth(RectTransform rect, float width)
		{
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}
		public static void SetHeight(RectTransform rect, float height)
		{
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}
		public static void SetSize(RectTransform rect, float size)
		{
			SetWidth(rect, size);
			SetHeight(rect, size);
		}
		public static string ToMoney(uint value)
		{
			if (value < 1000)
			{
				return value.ToString();
			}

			uint kCount = value / 1000;
			uint mod = value % 1000;
			return kCount.ToString() + '.' + mod.ToString()[0] + " k";
		}
		public static bool UpdateTimer(ref float timer, float coldown, bool isGTime = false)
		{
			bool isReady = timer >= coldown;
			if (isReady)
			{
				return true;
			}

			timer += (isGTime) ? GTime.timeStep : Time.fixedDeltaTime;
			return false;
		}
		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (value.CompareTo(min) < 0)
			{
				return min;
			}
			else if (value.CompareTo(max) > 0)
			{
				return max;
			}

			return min;
		}
		public static bool IsContain<T>(T value, T min, T max) where T : IComparable<T>
		{
			return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
		}
		public static T GetOther<T>(Collider other) where T : Component
		{
			T otherBody = other.GetComponent<T>();
			return otherBody;
		}
		public static List<T> ToList<T>(T[] arr)
		{
			return new List<T>(arr);
		}
		public static Vector3 RandomVect(float min, float max)
		{
			float x = UnityEngine.Random.Range(min, max);
			float y = UnityEngine.Random.Range(min, max);
			float z = UnityEngine.Random.Range(min, max);

			return new Vector3(x, y, z);
		}
		public static List<T> GetChilds<T>(Component parent) where T : Component
		{
			List<T> list = GetAllComponents<T>(parent);
			list.Remove(parent.GetComponent<T>());
			return list;
		}
		public static List<T> GetAllComponents<T>(Component parent) where T : Component
		{
			List<T> list = ToList(parent.GetComponentsInChildren<T>());
			return list;
		}
		public static int FromSreen(float factor)
		{
			return (int)(Screen.width * factor);
		}
		public static void DoAnyTimes(int iterCount, Action action)
		{
			if (iterCount <= 0)
			{
				return;
			}

			for (int i = 0; i < iterCount; i++)
			{
				action();
			}
		}
		public static void DestroyAll<T>(List<T> list) where T : Component
		{
			list.ForEach(element => { if (element) Component.Destroy(element); });
		}
		public static void DoAfterTime(MonoBehaviour instance, float time, EventDelegate afterTimeEvent)
		{
			time = (time < 0) ? 0 : time;
			instance.StartCoroutine(AfterTimeEvent(time, afterTimeEvent));
		}
		public static void FadeElement(Transform obj, float alpha, float duration)
		{
			List<Graphic> graphic = Utils.GetAllComponents<Graphic>(obj);
			graphic.ForEach(element => element.CrossFadeAlpha(alpha, duration, true));
		}
		public static bool IsHappen(float probability)
		{
			float random = UnityEngine.Random.Range(0.0f, 1.0f);
			return random <= probability;
		}
		public static void FadeList(List<Graphic> list, float fade, float duration = 0)
		{
			if (list == null)
			{
				return;
			}

			list.ForEach(x =>
			{
				if (x) x.CrossFadeAlpha(fade, duration, true);
			});
		}
		public static void Swap(ref float first, ref float second)
		{
			float tmp = first;
			first = second;
			second = tmp;
		}
		public static Vector3 RndDirBetween(float minAngle, float maxAngle)
		{
			minAngle = Mathf.Clamp(minAngle, 0, 360);
			maxAngle = Mathf.Clamp(maxAngle, 0, 360);

			if (minAngle > maxAngle) Swap(ref minAngle, ref maxAngle);
			float delta = maxAngle - minAngle;
			float rads = (minAngle + UnityEngine.Random.Range(0, delta)) * Mathf.PI / 180;

			return new Vector3(Mathf.Cos(rads), 0, Mathf.Sin(rads));
		}

		private static IEnumerator AfterTimeEvent(float time, EventDelegate afterTimeEvent)
		{
			yield return new WaitForSeconds(time);
			if (afterTimeEvent != null) afterTimeEvent();
		}
	}
}
