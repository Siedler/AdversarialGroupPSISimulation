using System;
using UnityEngine;

namespace Scrips.EventManager {
	public class TimeEventManager : MonoBehaviour {

		public static TimeEventManager current;
		
		public void Awake() {
			current = this;
		}
		
		public event Action OnTick;

		public void Tick() {
			OnTick?.Invoke();
		}
	}
}