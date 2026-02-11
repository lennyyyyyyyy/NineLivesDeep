using UnityEngine;
using System.Collections.Generic;

public class Tile : Parallax {
    public float mineMult = 1;

    public Vector2Int coord;
	public List<GameObject> entities = new List<GameObject>();
    public int oneUpCount = 0;

    public float externalDepthImpulse = 0;
	public SpriteRenderer sr;
    public bool underTileActive = true;
    public GameObject uniqueObstacle;
    public GameObject uniqueMine;
    protected GameObject underTile;
    protected float targetDepth = 1f;
    protected float period;
    protected BoxCollider2D collider;
	
	public void AddEntity(GameObject g) { // use Entity.Move() instead
        Entity e = g.GetComponent<Entity>();
        if (e == null) return;
        if (e.obstacle && uniqueObstacle != null) {
            Debug.Log("Tried to add second obstacle to tile at " + coord.ToString());
            return;
        } 
        if (e is MineSprite && uniqueMine != null) {
            Debug.Log("Tried to add second mine to tile at " + coord.ToString());
            return;
        }
        entities.Add(g);
        if (e.obstacle) {
            uniqueObstacle = g;
        }
        if (e is MineSprite) {
            uniqueMine = g;
        }
	}
    public void RemoveEntity(GameObject g) { // use Entity.Remove() instead
        entities.Remove(g);
        Entity e = g.GetComponent<Entity>();
        if (e == null) return;
        if (e.obstacle) {
            uniqueObstacle = null;
        } 
        if (e is MineSprite) {
            uniqueMine = null;
        }
    }
    public bool HasEntityOfType<T>() {
        foreach (GameObject g in entities) {
            if (g.GetComponent<T>() != null) {
                return true;
            }
        }
        return false;
    }
    protected override void Start() {
        base.Start();
        underTile = new GameObject("UnderTile");
        underTile.transform.parent = transform;
        underTile.AddComponent<Parallax>();
        underTile.AddComponent<SpriteRenderer>().color = new Color(.177f, .0973f, .0737f, 1f);
        underTile.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        underTile.SetActive(underTileActive);
        collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        period = Random.Range(4f, 6f);
		//put the correct theme for the floor
		MaterialPropertyBlock mpb = new MaterialPropertyBlock();
		sr.GetPropertyBlock(mpb);
		mpb.SetFloat(ShaderManager.s.ThemeID, Floor.s.floor/3 + 1);
		sr.SetPropertyBlock(mpb);
    }
    protected override void Update() {
        base.Update();
        underTile.GetComponent<Parallax>().referencePos = referencePos;
        float prop = 1 - Mathf.Pow(1 - ConstantsManager.s.tileAdjacentDragSpeed, Time.deltaTime / .15f);
        float adjacentDrag = 0;
        int adjacentTiles = 0;
        foreach ((int dx, int dy) in new (int, int)[] { (1,0), (-1,0), (0,1), (0,-1) }) {
            if (Floor.s.TileExistsAt(coord.x + dx, coord.y + dy)) {
                adjacentDrag += Floor.s.GetTile(coord.x + dx, coord.y + dy).GetComponent<Tile>().depth - depth;
                adjacentTiles++;
            }
        }
        externalDepthImpulse += ConstantsManager.s.tileAdjacentDragPower * Time.deltaTime * adjacentDrag / Mathf.Max(1, adjacentTiles);
        depth += externalDepthImpulse * prop;
        externalDepthImpulse *= 1 - prop;
        depth = Mathf.Lerp(depth, targetDepth + 0.012f * Mathf.Sin(Time.time * Mathf.PI * 2 / period), 1 - Mathf.Pow(1 - ConstantsManager.s.tileDampingPower, Time.deltaTime / .15f));
        underTile.GetComponent<Parallax>().depth = depth + 0.02f;
    }
    public virtual void PositionUnbuilt() {
		LeanTween.cancel(gameObject);
        targetDepth = 1.5f;
        referencePos = Floor.s.CoordToIdealPos(coord.x, coord.y).normalized + Quaternion.Euler(0, 0, 90 * Random.Range(0, 4)) * new Vector3(0, 40, 0);
    }
    // Destroy tile while animated - API
	public virtual void Unbuild(float duration) {
		LeanTween.cancel(gameObject);
		LeanTween.value(gameObject, (float f) => {targetDepth = f;}, targetDepth, 1.5f, duration).setEase(LeanTweenType.easeInOutQuint);
		LeanTween.value(gameObject, (Vector3 v) => {referencePos = v;}, referencePos, referencePos + Quaternion.Euler(0, 0, 90 * Random.Range(0, 4)) * new Vector3(0, 40, 0), duration).setEase(LeanTweenType.easeInOutQuint).setOnComplete(() => {
			Destroy(gameObject);
		});
	}
    // Animate to the proper position, if invalid then animate destruction - API
    public virtual void Build(float duration) {
		if (coord == Floor.INVALID_COORD) {
			Unbuild(duration);
		} else {
			LeanTween.cancel(gameObject);
			LeanTween.value(gameObject, (float f) => {targetDepth = f;}, targetDepth, 1f, duration).setEase(LeanTweenType.easeInOutQuint);
			LeanTween.value(gameObject, (Vector3 v) => {referencePos = v;}, referencePos, Floor.s.CoordToIdealPos(coord.x, coord.y), duration).setEase(LeanTweenType.easeInOutQuint);
		}
    }
    public virtual void PutUnder() {
        HelperManager.s.SetGameLayerRecursive(gameObject, LayerMask.NameToLayer("Under"));
        HelperManager.s.PerformActionRecursive(gameObject, (GameObject g) => {
            SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
            if (sr != null) {
                if (sr.sortingLayerName == "Player") {
                    sr.sortingLayerName = "UnderPlayer";
                } else if (sr.sortingLayerName == "Floor") {
                    sr.sortingLayerName = "UnderFloor";
                }
            }
        });
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, (float f) => {targetDepth = f;}, targetDepth, 1.3f, FlagSprite.overToUnderDuration);
    }
    public virtual void PutOver() {
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, (float f) => {targetDepth = f;}, targetDepth, 1f, FlagSprite.overToUnderDuration).setOnComplete(() => {
            PutOverEnd();
        });
    }
    public virtual void PutOverEnd() {
        HelperManager.s.SetGameLayerRecursive(gameObject, LayerMask.NameToLayer("Default"));
        HelperManager.s.PerformActionRecursive(gameObject, (GameObject g) => {
            SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
            if (sr != null) {
                if (sr.sortingLayerName == "UnderPlayer") {
                    sr.sortingLayerName = "Player";
                } else if (sr.sortingLayerName == "UnderFloor") {
                    sr.sortingLayerName = "Floor";
                }
            }
        });
    }
    protected virtual void OnPlayerArriveAtCoord(int x, int y) {
        if (coord.x == x && coord.y == y && oneUpCount > 0) {
            Instantiate(PrefabManager.s.flagPrefab).AddComponent<You>().Init(initialCount: oneUpCount);
            oneUpCount = 0;
        }
    }
    protected virtual void OnEnable() {
        EventManager.s.OnPlayerArriveAtCoord += OnPlayerArriveAtCoord;
    }
    protected virtual void OnDisable() {
        EventManager.s.OnPlayerArriveAtCoord -= OnPlayerArriveAtCoord;
    }
}
