using UnityEngine;

public class Entity : VerticalObject {
	public bool setInitialData = false;
	public Sprite sprite;
	public TooltipData tooltipData;
	public bool obstacle = true;

	protected AddTooltipScene addTooltip;
	protected SpriteRenderer spriteRenderer;

	protected bool hovered = false;
	
	protected override void Start() {
		base.Start();

		addTooltip = (GetComponent<AddTooltipScene>() == null ? gameObject.AddComponent(typeof(AddTooltipScene)) as AddTooltipScene : GetComponent<AddTooltipScene>());
		spriteRenderer = GetComponent<SpriteRenderer>();

		if (setInitialData) {
			ApplyInitialData();
		} else {
			SetDefaultData();
		}
	}
	public virtual void SetInitialData(Sprite? sprite = null, TooltipData tooltipData = null, bool? obstacle = null) {
		setInitialData = true;
		this.sprite = sprite ?? this.sprite;
		this.tooltipData = tooltipData ?? this.tooltipData;
		this.obstacle = obstacle ?? this.obstacle;
	}
	protected virtual void ApplyInitialData() {
		sr.sprite = this.sprite;
		addTooltip.SetData(this.tooltipData, true);
	}
	public virtual void SetData(Sprite? sprite = null, TooltipData tooltipData = null, bool? obstacle = null) {
		SetInitialData(sprite, tooltipData, obstacle);
		ApplyInitialData();
	}
	protected virtual void SetDefaultData() {
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
	public virtual bool IsInteractable() {
		return Mathf.Abs(GetCoord().x - Player.s.GetCoord().x) <= Player.s.modifiers.interactRange && Mathf.Abs(GetCoord().y - Player.s.GetCoord().y) <= Player.s.modifiers.interactRange;
	}
	public virtual void Interact() {
	}
	protected virtual void OnMouseEnterCustom() {
		hovered = true;
	}
	protected virtual void OnMouseExitCustom() {
		hovered = false;
	}
	protected virtual void OnMouseDown() {
		if (IsInteractable()) {
			Interact();
		}
	}
}
