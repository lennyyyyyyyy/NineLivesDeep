using UnityEngine;

public class Entity : VerticalObject {
    [System.NonSerialized]
    public Vector2Int coord = new Vector2Int(-1, -1);
	protected virtual void Start() {
		base.Start();
		marker = gameObject;
	}
	public virtual void Move(int x, int y, bool reposition = true) {
		if (coord.x != -1) {
			Floor.s.entities[coord.x, coord.y].Remove(gameObject);
		}
		Floor.s.entities[x, y].Add(gameObject);
		coord = new Vector2Int(x, y);
		transform.parent = Floor.s.tiles[x, y].transform;
		if (reposition) {
			transform.localPosition = Vector3.zero;
		}
	}
	public virtual void Remove() {
		if (coord.x != -1) {
			Floor.s.entities[coord.x, coord.y].Remove(gameObject);
		}
		Destroy(gameObject);
	}
	public virtual bool CoordAllowed(int x, int y) {
		return Floor.s.within(x, y) && Floor.s.tiles[x, y] != null && Floor.s.tiles[x, y].GetComponent<ActionTile>() == null; 
	}
}
