using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class Taken : Curse {
	public static HashSet<GameObject> takenFlags = new HashSet<GameObject>();
	public override void Modify(ref Modifiers modifiers) {
		if (intensifiedBy.Count > 0) {
			modifiers.takenCurseCount += 2;
		} else {
			modifiers.takenCurseCount += 1;
		}
		takenFlags.Clear();
		List<GameObject> options = PlayerUIItemModule.s.flags.ToList();
		for (int i = 0; i < modifiers.takenCurseCount && options.Count > 0; i++) {
			int index = Random.Range(0, options.Count);
			GameObject flag = options[index];
			options.RemoveAt(index);
			takenFlags.Add(flag);
		}
		// tooltip update
		tooltipData.flavor = "Currently taking " + string.Join(", ", takenFlags.Select(f => f.GetComponent<Flag>().tooltipData.name));
		Init(tooltipData: tooltipData);
	}
}

