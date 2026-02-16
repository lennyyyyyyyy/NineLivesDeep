using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class ActionTile : Tile {
    public enum ActionCode {
        EXITTOSHOP,
        EXITTOMINEFIELD,
        EXITTOTRIAL,
        GIVETRIALREWARD,
        WINGAME
    }

    [System.NonSerialized]
    public ActionCode actionCode;
    [System.NonSerialized]
	public int amount = 1;
	protected Action action;
	protected Material m;
	public void Init(ActionCode actionCode) {
        this.actionCode = actionCode;
		if (actionCode == ActionCode.EXITTOSHOP) {
			m = UIManager.s.tileExitMat;
			action = () => {
                Floor.s.ExitFloor("shop", Floor.s.floor);
            };
		} else if (actionCode == ActionCode.EXITTOMINEFIELD) {
			m = UIManager.s.tileExitMat;
			action = () => {
                Floor.s.ExitFloor("minefield", Floor.s.floor+1);
            };
		} else if (actionCode == ActionCode.EXITTOTRIAL) {
			m = UIManager.s.tileTrialMat;
            action = () => {
                Floor.s.ExitFloor("trial", Floor.s.floor);
            };
		} else if (actionCode == ActionCode.GIVETRIALREWARD) {
			m = UIManager.s.tileTrialMat;
			action = () => {
				Tile t = Floor.s.ReplaceTile(PrefabManager.s.tilePrefab, Floor.s.width-1, Floor.s.height-2).GetComponent<Tile>();
				t.PositionUnbuilt();
				t.Build(2f);
				if (Player.s.floorDeathCount > 0) {
					Floor.s.PlacePickupSprite(
                        new FlagPool() {
                            chooseUnseen = new List<Type>() { typeof(Consumable) }
                        },
                        PickupSprite.SpawnType.TRIAL, 0, new Vector2Int(Floor.s.width-1, Floor.s.height-2)
                    );
				} else {
					Floor.s.PlacePickupSprite(
                        new FlagPool() {
                            chooseUnseen = new List<Type>() { typeof(Flag) }
                        },
                        PickupSprite.SpawnType.TRIAL, 0, new Vector2Int(Floor.s.width-1, Floor.s.height-2)
                    );
				}
			};
		} else if (actionCode == ActionCode.WINGAME) {
            m = UIManager.s.tileExitMat;
            action = () => {
                EventManager.s.OnGameWin?.Invoke();
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
			action?.Invoke();
			amount--;
		}
	}
	protected override void Start() {
		GetComponent<SpriteRenderer>().material = m;
		base.Start();
	}
    protected override void OnPlayerArriveAtCoord(int x, int y) {
        base.OnPlayerArriveAtCoord(x, y);
        if (coord.x == x && coord.y == y) {
            PerformAction();
        }
    }
}
