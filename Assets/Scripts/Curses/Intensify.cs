using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Intensify : Curse {
	private Curse intensifiedCurse;
	private void SwitchIntensifiedCurse() {
		if (intensifiedCurse != null) {
			intensifiedCurse.intensified = false;
		}
		
		List<GameObject> options = new List<GameObject>(Player.s.curses);
		options = options.Where(curse => curse.GetComponent<Intensify>() == null).ToList();
		intensifiedCurse = options[Random.Range(0, options.Count)].GetComponent<Curse>();
		intensifiedCurse.intensified = true;

		tooltipData.flavor =  "Currently intensifying " + intensifiedCurse.tooltipData.name;
		SetData(tooltipData: tooltipData);	

		Player.s.RecalculateModifiers();
	}
	private void OnEnable() {
		Floor.onFloorChangeBeforeNewLayout += SwitchIntensifiedCurse;
	}
	private void OnDisable() {
		Floor.onFloorChangeBeforeNewLayout -= SwitchIntensifiedCurse;
	}
}

