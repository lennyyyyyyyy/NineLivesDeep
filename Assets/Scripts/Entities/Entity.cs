using UnityEngine;

public class Entity : VerticalObject {
    [System.NonSerialized]
    public Vector2Int coord = new Vector2Int(-1, -1);
	public bool obstacle = true;
    protected bool hovered = false;
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
        Player.s.destroyPrints();
        Player.s.updatePrints();
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
