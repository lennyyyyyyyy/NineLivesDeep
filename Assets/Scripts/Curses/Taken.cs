using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class Taken : Curse {
	public override void Modify(ref Modifiers modifiers) {
        int takenCurseCount;
		if (intensifiedBy.Count > 0) {
            takenCurseCount = 2;
		} else {
            takenCurseCount = 1;
		}
		List<GameObject> options = PlayerUIItemModule.s.flags.ToList();
        foreach (GameObject g in Player.s.modifiers.takenFlags) {
            options.Remove(g);
        }
        List<GameObject> takenFlags = new List<GameObject>();
		for (int i = 0; i < takenCurseCount && options.Count > 0; i++) {
			int index = Random.Range(0, options.Count);
			GameObject flag = options[index];
			options.RemoveAt(index);
			takenFlags.Add(flag);
            Player.s.modifiers.takenFlags.Add(flag);
		}
		// tooltip update
		tooltipData.flavor = "Currently taking " + string.Join(", ", takenFlags.Select(f => f.GetComponent<Flag>().tooltipData.name));
		Init(tooltipData: tooltipData);
	}
}

