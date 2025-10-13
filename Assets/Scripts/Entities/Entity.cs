using UnityEngine;

public class Entity : VerticalObject {
    [System.NonSerialized]
    public Vector2Int coord = new Vector2Int(-1, -1);
	public bool obstacle = true;
    protected bool hovered = false;
	protected override void Start() {
		base.Start();
		marker = gameObject;
	}
	public virtual void Move(GameObject tile, bool reposition = true) {
		if (coord.x != -1) {
			tile.GetComponent<Tile>().entities.Remove(gameObject);
		}
		tile.GetComponent<Tile>().AddEntity(gameObject);
		coord = tile.GetComponent<Tile>().coord;
		Vector3 oldScale = transform.localScale;
		transform.parent = tile.transform;
		transform.localScale = oldScale;	
		if (reposition) {
			transform.localPosition = Vector3.zero;
		}
        Player.s.destroyPrints();
        Player.s.updatePrints();
	}
	public virtual void Move(int x, int y, bool reposition = true) {
		if (CoordAllowed(x, y)) {
			Move(Floor.s.tiles[x, y], reposition);
		} else {
			Debug.Log("Tried to move entity to invalid coord " + x + ", " + y);
		}
	}
	public virtual void Remove() {
		Floor.s.RemoveEntity(coord.x, coord.y, gameObject);
		Destroy(gameObject);
	}
	public virtual bool CoordAllowed(int x, int y) {
		return Floor.s.within(x, y) && Floor.s.tiles[x, y] != null && Floor.s.tiles[x, y].GetComponent<ActionTile>() == null; 
	}
    protected virtual void OnMouseEnter() {
		hovered = true;
	}
	protected virtual void OnMouseExit() {
		hovered = false;
	}
	public virtual bool IsInteractable() {
		return Mathf.Abs(coord.x - Player.s.coord.x) <= Player.s.modifiers.interactRange && Mathf.Abs(coord.y - Player.s.coord.y) <= Player.s.modifiers.interactRange;
	}
	public virtual void Interact() {
	}
	protected virtual void OnMouseDown() {
		if (IsInteractable()) {
			Interact();
		}
	}
}
