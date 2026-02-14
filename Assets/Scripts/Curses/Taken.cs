using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Taken : Curse {
    [System.NonSerialized]
    public List<Flag> takenFlags = new List<Flag>();

    public void SetTakenFlags(List<Flag> flags) {
        if (!usable) return;
        foreach (Flag f in takenFlags) {
            f.takenBy.Remove(this);
        }
        takenFlags = flags;
        foreach (Flag f in takenFlags) {
            f.takenBy.Add(this);
        }
		tooltipData.flavor = "Currently taking " + string.Join(", ", takenFlags.Select(f => f.tooltipData.name));
        Init(tooltipData: tooltipData);	
		Player.s.RecalculateModifiers();
    }
	private void SetRandomTakenFlag() {
        int takenCurseCount;
		if (intensifiedBy.Count > 0) {
            takenCurseCount = 2;
		} else {
            takenCurseCount = 1;
		}
		List<GameObject> options = PlayerUIItemModule.s.flags.Where(flag => flag.GetComponent<You>() == null && flag.GetComponent<Exit>() == null).ToList();
        HelperManager.s.Shuffle(ref options, count: takenCurseCount);
        if (options.Count > takenCurseCount) {
            options.RemoveRange(takenCurseCount, options.Count - takenCurseCount);
        }
        SetTakenFlags(options.Select(g => g.GetComponent<Flag>()).ToList());
	}
	private void OnEnable() {
		EventManager.s.OnFloorChangeBeforeNewLayout += SetRandomTakenFlag;
	}
	private void OnDisable() {
		EventManager.s.OnFloorChangeBeforeNewLayout -= SetRandomTakenFlag;
	}
}

