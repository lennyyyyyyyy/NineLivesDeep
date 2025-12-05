using UnityEngine;
using System.Collections.Generic;

public class Intensify : Curse {
	public static Dictionary<GameObject, GameObject> intensifiedCurses = new Dictionary<GameObject, GameObject>(); 
	private GameObject intensifiedCurse;
	private void SwitchIntensifiedCurse() {
		List<GameObject> options = new List<GameObject>(Player.s.curses);
		options.Remove(gameObject);
		intensifiedCurse = options[Random.Range(0, options.Count)];

		// update the static list
		intensifiedCurses.Remove(gameObject);
		intensifiedCurses.Add(gameObject, intensifiedCurse);

		Player.s.RecalculateModifiers();
	}
	private void OnEnable() {
		Floor.onFloorChangeBeforeEntities += SwitchIntensifiedCurse;
	}
	private void OnDisable() {
		Floor.onFloorChangeBeforeEntities -= SwitchIntensifiedCurse;
	}
}

