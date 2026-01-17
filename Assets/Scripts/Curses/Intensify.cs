using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Intensify : Curse {
    [System.NonSerialized]
	public Curse intensifiedCurse;

	public override void Modify(ref Modifiers modifiers) {
	}
    public void SetIntensifiedCurse(Curse curse) {
		if (intensifiedCurse != null) {
			intensifiedCurse.intensifiedBy.Remove(this);
		}
        intensifiedCurse = curse;
        intensifiedCurse.intensifiedBy.Add(this);
        tooltipData.flavor =  "Currently intensifying " + intensifiedCurse.tooltipData.name;
        Init(tooltipData: tooltipData);	
		Player.s.RecalculateModifiers();
    }
	private void SetRandomIntensifiedCurse() {
		List<GameObject> options = new List<GameObject>(PlayerUIItemModule.s.curses);
		options = options.Where(curse => curse.GetComponent<Intensify>() == null).ToList();
        if (options.Count > 0) {
            SetIntensifiedCurse(options[Random.Range(0, options.Count)].GetComponent<Curse>());
        }
	}
	private void OnEnable() {
		EventManager.s.OnFloorChangeBeforeNewLayout += SetRandomIntensifiedCurse;
	}
	private void OnDisable() {
		EventManager.s.OnFloorChangeBeforeNewLayout -= SetRandomIntensifiedCurse;
	}
}

