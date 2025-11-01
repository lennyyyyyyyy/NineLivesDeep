using UnityEngine;

public class Entity : VerticalObject {
	public bool obstacle = true;
    protected bool hovered = false;
	protected override void Start() {
		base.Start();
	}
	public virtual GameObject GetTile() {
		if (transform.parent != null && transform.parent.GetComponent<Tile>() != null) {
			return transform.parent.gameObject;
		} else {
			return null;
		}
	}
	public virtual Vector2Int GetCoord() {
		if (GetTile() != null) {
			return GetTile().GetComponent<Tile>().coord;
		} else {
			return Floor.INVALID_COORD;
		}
	}
	public virtual void Move(GameObject tile, bool reposition = true) {
		if (transform.parent != null && transform.parent.GetComponent<Tile>() != null) {
			transform.parent.GetComponent<Tile>().entities.Remove(gameObject);
		}
		tile.GetComponent<Tile>().AddEntity(gameObject);
		Vector3 oldScale = transform.localScale;
		transform.parent = tile.transform;
		transform.localScale = oldScale;	
		if (reposition) {
			transform.localPosition = Vector3.zero;
		}
        Player.s.destroyPrints();
        Player.s.updatePrints();
	}
	public virtual bool Move(int x, int y, bool reposition = true) {
		if (CoordAllowed(x, y)) {
			Move(Floor.s.GetTile(x, y), reposition);
			return true;
		} else {
			Debug.Log("Tried to move entity to invalid coord " + x + ", " + y);
			return false;
		}
	}
	public virtual void Remove() {
		if (GetTile() != null) {
			GetTile().GetComponent<Tile>().entities.Remove(gameObject);
		}
		Destroy(gameObject);
	}
	public virtual bool CoordAllowed(int x, int y) {
		return Floor.s.TileExistsAt(x, y) && Floor.s.GetTile(x, y).GetComponent<ActionTile>() == null; 
	}
    protected virtual void OnMouseEnter() {
		hovered = true;
	}
	protected virtual void OnMouseExit() {
		hovered = false;
	}
	public virtual bool IsInteractable() {
		return Mathf.Abs(GetCoord().x - Player.s.GetCoord().x) <= Player.s.modifiers.interactRange && Mathf.Abs(GetCoord().y - Player.s.GetCoord().y) <= Player.s.modifiers.interactRange;
	}
	public virtual void Interact() {
	}
	protected virtual void OnMouseDown() {
		if (IsInteractable()) {
			Interact();
		}
	}
}
