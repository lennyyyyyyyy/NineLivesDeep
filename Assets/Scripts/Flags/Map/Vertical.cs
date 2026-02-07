using UnityEngine;

public class Vertical : Map
{
    public override void OnDiscover(int x, int y) {
        int count = 0;
		foreach (GameObject t in Floor.s.tiles.Values) {
			if (t.GetComponent<Tile>().coord.x == x) {
				count += (t.GetComponent<Tile>().uniqueMine != null) ? 1 : 0;
			}
        }
		SetNumber(x, y, count);
    }
}
