using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class ActionTile : Tile
{
	public const int EXITTOSHOP = 0, EXITTOMINEFIELD = 1, EXITTOTRIAL = 2, GIVETRIALREWARD = 3;
	protected Action action;
	protected Sprite s;
	protected int amount = 1;
	private Action ExitAction(string newFloorType, int newFloor) {
		return () => { 
			Player.s.transform.parent = null;
			MainCamera.s.locked = true;
			MainCamera.s.ExitMotion();
			GameManager.s.DelayAction(() => {Floor.s.IntroAndCreateFloor(newFloorType, newFloor);}, 0.5f);
		};
	}	
	public void Init(int actionCode) {
		if (actionCode == EXITTOSHOP) {
			s = UIManager.s.tile_exit_s;
			action = ExitAction("shop", Floor.s.floor);
		} else if (actionCode == EXITTOMINEFIELD) {
			s = UIManager.s.tile_exit_s;
			action = ExitAction("minefield", Floor.s.floor+1);
		} else if (actionCode == EXITTOTRIAL) {
			s = UIManager.s.tile_trial_s;
			action = ExitAction("trial", Floor.s.floor);
		} else if (actionCode == GIVETRIALREWARD) {
			s = UIManager.s.tile_trial_s;
			action = () => {
				Tile t = Floor.s.PlaceTile(GameManager.s.tile_p, Floor.s.width-1, Floor.s.height-2).GetComponent<Tile>();
				t.PositionUnbuilt();
				t.Build(2f);
				if (Floor.s.floorDeathCount > 0) {
					Floor.s.PlacePickupSprite(Player.s.consumableFlagsUnseen, 0, Floor.s.width-1, Floor.s.height-2);
				} else {
					Floor.s.PlacePickupSprite(Player.s.flagsUnseen, 0, Floor.s.width-1, Floor.s.height-2);
				}
			};
		}
	}
	public void Init(Sprite sprite, Action a, int am) {
		s = sprite;
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
		base.Start();
		GetComponent<SpriteRenderer>().sprite = s;
	}
}
