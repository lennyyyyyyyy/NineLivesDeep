using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class ActionTile : Tile {
    public enum ActionCode {
        EXITTOSHOP,
        EXITTOMINEFIELD,
        EXITTOTRIAL,
        GIVETRIALREWARD
    }

    [System.NonSerialized]
    public ActionCode actionCode;
    [System.NonSerialized]
	public int amount = 1;
	protected Action action;
	protected Material m;
    // Creates the default sequence for exiting a floor into a new floor
	protected Action ExitAction(string newFloorType, int newFloor) {
		return () => { 
			Player.s.Remove(false);
			MainCamera.s.locked = true;
			MainCamera.s.ExitMotion();
			HelperManager.s.DelayAction(() => {Floor.s.IntroAndCreateFloor(newFloorType, newFloor);}, 0.5f);
		};
	}	
	public void Init(ActionCode actionCode) {
        this.actionCode = actionCode;
		if (actionCode == ActionCode.EXITTOSHOP) {
			m = UIManager.s.tileExitMat;
			action = ExitAction("shop", Floor.s.floor);
		} else if (actionCode == ActionCode.EXITTOMINEFIELD) {
			m = UIManager.s.tileExitMat;
			action = ExitAction("minefield", Floor.s.floor+1);
		} else if (actionCode == ActionCode.EXITTOTRIAL) {
			m = UIManager.s.tileTrialMat;
			action = ExitAction("trial", Floor.s.floor);
		} else if (actionCode == ActionCode.GIVETRIALREWARD) {
			m = UIManager.s.tileTrialMat;
			action = () => {
				Tile t = Floor.s.ReplaceTile(PrefabManager.s.tilePrefab, Floor.s.width-1, Floor.s.height-2).GetComponent<Tile>();
				t.PositionUnbuilt();
				t.Build(2f);
				if (Player.s.floorDeathCount > 0) {
					Floor.s.PlacePickupSprite(CatalogManager.s.allConsumableFlagTypes, PickupSprite.SpawnType.TRIAL, 0, new Vector2Int(Floor.s.width-1, Floor.s.height-2));
				} else {
					Floor.s.PlacePickupSprite(PlayerUIItemModule.s.flagsUnseen, PickupSprite.SpawnType.TRIAL, 0, new Vector2Int(Floor.s.width-1, Floor.s.height-2));
				}
			};
		}
	}
	public void Init(Material mat, Action a, int am) {
		m = mat;
		action = a;
		amount = am;
	}
	public void PerformAction() {
		if (amount > 0) {
			action.Invoke();
			amount--;
		}
	}
	protected override void Start() {
		GetComponent<SpriteRenderer>().material = m;
		base.Start();
	}
    protected override void OnPlayerMoveToCoord(int x, int y) {
        base.OnPlayerMoveToCoord(x, y);
        if (coord.x == x && coord.y == y) {
            PerformAction();
        }
    }
}
