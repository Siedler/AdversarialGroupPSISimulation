using System;
using UnityEngine;

namespace Scrips.EventManager {
	public class TimeEventManager : MonoBehaviour {

		public static TimeEventManager current;
		
		public void Awake() {
			current = this;
		}
		
		public event Action<int> OnTick;

		public void Tick(int timeStep) {
			OnTick?.Invoke(timeStep);
		}
	}
}