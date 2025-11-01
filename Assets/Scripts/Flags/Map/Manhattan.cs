using UnityEngine;

class Manhattan : Map {
    public override void OnDiscover(int x, int y) {
        int min = -1; 
		foreach (GameObject t in Floor.s.tiles.Values) {
			if (t.GetComponent<Tile>().GetUniqueMine() != null) {
				if (min == -1) {
					min = Mathf.Abs(t.GetComponent<Tile>().coord.x-x) + Mathf.Abs(t.GetComponent<Tile>().coord.y-y);
				} else {
					min = Mathf.Min(min, Mathf.Abs(t.GetComponent<Tile>().coord.x-x) + Mathf.Abs(t.GetComponent<Tile>().coord.y-y));
				}
			}
        }
		SetNumber(x, y, min);
    }
    protected override void Start() {
        base.Start();
    }
}
