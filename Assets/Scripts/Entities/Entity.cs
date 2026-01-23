using UnityEngine;

public class Entity : VerticalObject {
	public Sprite sprite;
	public TooltipData tooltipData;
	public bool obstacle = true;

	protected AddTooltipScene addTooltip;
	protected bool hovered = false;
	
    protected virtual void BeforeInit() {
        sprite = sr.sprite;
		addTooltip = (GetComponent<AddTooltipScene>() == null ? gameObject.AddComponent(typeof(AddTooltipScene)) as AddTooltipScene : GetComponent<AddTooltipScene>());
    }
    protected virtual void AfterInit() {
    }
	protected override void Awake() {
		base.Awake();
        BeforeInit();
        Init();
        AfterInit();
	}
    public virtual void Init(Sprite? sprite = null, TooltipData? tooltipData = null, bool? obstacle = null) {
		this.sprite = sprite ?? this.sprite;
		this.tooltipData = tooltipData ?? this.tooltipData;
		this.obstacle = obstacle ?? this.obstacle;
        sr.sprite = this.sprite;
        addTooltip.Init(this.tooltipData, true);
    }
    public virtual void Init() {
        Init(sprite: null, tooltipData: null, obstacle: null);
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
	public virtual bool Move(GameObject tile, bool reposition = true) {
        Tile tileComponent = tile.GetComponent<Tile>();
        if (tile == null || !CoordAllowed(tileComponent.coord.x, tileComponent.coord.y)) {
			Debug.Log("Tried to move entity to invalid coord " + tileComponent.coord.ToString());
			return false;
        }
        Remove(false);
		tileComponent.AddEntity(gameObject);
		Vector3 oldScale = transform.localScale;
		transform.parent = tile.transform;
		transform.localScale = oldScale;	
		if (reposition) {
			transform.localPosition = Vector3.zero;
		}
        Player.s.destroyPrints();
        Player.s.updatePrints();
        return true;
	}
	public virtual bool Move(int x, int y, bool reposition = true) {
        return Move(Floor.s.GetTile(x, y), reposition);
	}
	public virtual void Remove(bool destroy = true) {
		if (GetTile() != null) {
			GetTile().GetComponent<Tile>().RemoveEntity(gameObject);
            transform.parent = Floor.s.transform;
		}
        if (destroy) Destroy(gameObject);
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
