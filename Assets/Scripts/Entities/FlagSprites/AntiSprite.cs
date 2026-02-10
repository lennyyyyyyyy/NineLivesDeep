using UnityEngine;

public class AntiSprite : FlagSprite {
    protected override void OnDrop(int x, int y) {
        base.OnDrop(x, y);
        if (!Floor.s.TileExistsAt(x, y)) return;
        Tile tile = Floor.s.GetTile(x, y).GetComponent<Tile>();
        for (int i=0; i<tile.entities.Count; i++) {
            FlagSprite flagSprite = tile.entities[i].GetComponent<FlagSprite>();
            if (flagSprite != null) {
                Flag newFlag = Instantiate(PrefabManager.s.flagPrefab).AddComponent(flagSprite.correspondingUIType) as Flag;
                newFlag.Init(initialCount: 1);
                flagSprite.Remove();
            }
        }
    }
    public override bool CoordAllowed(int x, int y) {
        return Floor.s.TileExistsAt(x, y);
    }
}
