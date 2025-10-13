using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class ActionTile : Tile
{
	public const int EXITTOSHOP = 0, EXITTOMINEFIELD = 1, EXITTOTRIAL = 2, GIVETRIALREWARD = 3;
	protected Action action;
	protected Material m;
	protected int amount = 1;
	private Action ExitAction(string newFloorType, int newFloor) {
		return () => { 
			Player.s.Remove();
			MainCamera.s.locked = true;
			MainCamera.s.ExitMotion();
			GameManager.s.DelayAction(() => {Floor.s.IntroAndCreateFloor(newFloorType, newFloor);}, 0.5f);
		};
	}	
	public void Init(int actionCode) {
		if (actionCode == EXITTOSHOP) {
			m = UIManager.s.tileExitMat;
			action = ExitAction("shop", Floor.s.floor);
		} else if (actionCode == EXITTOMINEFIELD) {
			m = UIManager.s.tileExitMat;
			action = ExitAction("minefield", Floor.s.floor+1);
		} else if (actionCode == EXITTOTRIAL) {
			m = UIManager.s.tileTrialMat;
			action = ExitAction("trial", Floor.s.floor);
		} else if (actionCode == GIVETRIALREWARD) {
			m = UIManager.s.tileTrialMat;
			action = () => {
				Tile t = Floor.s.PlaceTile(GameManager.s.tile_p, Floor.s.width-1, Floor.s.height-2).GetComponent<Tile>();
				t.PositionUnbuilt();
				t.Build(2f);
				if (Floor.s.floorDeathCount > 0) {
					Floor.s.PlacePickupSprite(Player.s.consumableFlagsUnseen, PickupSprite.TRIAL, 0, Floor.s.width-1, Floor.s.height-2);
				} else {
					Floor.s.PlacePickupSprite(Player.s.flagsUnseen, PickupSprite.TRIAL, 0, Floor.s.width-1, Floor.s.height-2);
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
}
