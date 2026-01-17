using UnityEngine;

class Wildcat : Passive {
    public override void Init() {
        FlagData flagData = CatalogManager.s.typeToData[GetType()] as FlagData;
        Init(tex2d: flagData.tex2d,
             tooltipData: flagData.tooltipData,
             placeableSpriteType: flagData.placeableSpriteType,
             placeableRemovesMines: flagData.placeableRemovesMines,
             consumableDefaultCount: flagData.consumableDefaultCount,
             showCount: flagData.showCount,
             initialCount: 5,
             allowedFloorTypes: flagData.allowedFloorTypes);
    }
    public override void UpdateCount(int newCount) {
        if (newCount == 0) {
            count = 5;
            Flag y = PlayerUIItemModule.s.typeToInstances[typeof(You)][0].GetComponent<Flag>();
            y.UpdateCount(y.count+1);
            
        } else {
            count = newCount;
            HelperManager.s.InstantiateBubble(gameObject, newCount.ToString() + " left...", Color.white);
        }
        tmpro.text = count.ToString();
    }
	protected virtual void OnPlayerMoveToCoord(int x, int y) {
		if (Floor.s.GetTile(x, y) && !Player.s.tilesVisited.Contains(Floor.s.GetTile(x, y)) && Floor.s.GetTile(x, y).GetComponent<MossyTile>() != null) {
			UpdateCount(count - 1);
		}
	}
	protected override void OnEnable() {
		base.OnEnable();
		EventManager.s.OnPlayerMoveToCoord += OnPlayerMoveToCoord;
	}
	protected override void OnDisable() {
		base.OnDisable();
		EventManager.s.OnPlayerMoveToCoord -= OnPlayerMoveToCoord;
	}
}
