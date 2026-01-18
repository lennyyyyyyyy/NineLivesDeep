using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Crank : Entity {
    [System.NonSerialized]
	public bool direction; // false is ccw, true is cw
	private bool isRotating = false;

	protected override void BeforeInit() {
        base.BeforeInit();
	 	direction = Random.value < 0.5f;
	}
    public virtual void Init(bool? direction = null) {
        this.direction = direction ?? this.direction; 
    }
	public void ToggleDirection() {
		direction = !direction;
	}
	public override bool IsInteractable() {
		return base.IsInteractable() && !isRotating;
	} 
	public override void Interact() {
		isRotating = true;
		HelperManager.s.DelayAction(() => { isRotating = false; }, 0.5f);
		List<Vector2Int> endCoords = new List<Vector2Int>(),
						 ringCoords = new List<Vector2Int>() {
							 									GetCoord() + new Vector2Int(0, 1),
																GetCoord() + new Vector2Int(1, 1),
  															    GetCoord() + new Vector2Int(1, 0),
															    GetCoord() + new Vector2Int(1, -1),
															    GetCoord() + new Vector2Int(0, -1),
															    GetCoord() + new Vector2Int(-1, -1),
															    GetCoord() + new Vector2Int(-1, 0),
															    GetCoord() + new Vector2Int(-1, 1)
															 };
		if (direction) {
			for (int i = 0; i < ringCoords.Count; i++) {
				endCoords.Add(ringCoords[(i + 1) % ringCoords.Count]);
			}
		} else {
			for (int i = 0; i < ringCoords.Count; i++) {
				endCoords.Add(ringCoords[(i - 7 + ringCoords.Count) % ringCoords.Count]);
			}
		}  
		Floor.s.MoveTiles(ringCoords, endCoords);
	}
}
