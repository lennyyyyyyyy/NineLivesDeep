using UnityEngine;
using System.Collections.Generic;

public class Tile : Parallax
{
    protected GameObject underTile;
    public bool underTileActive = true;
    public Vector2Int coord;
    [System.NonSerialized]
    public float mineMult = 1;
    protected float targetDepth = 1f;
    protected float period;
    [System.NonSerialized]
    public float externalDepthImpulse = 0;
    protected BoxCollider2D collider;
	public SpriteRenderer sr;
	public List<GameObject> entities = new List<GameObject>();
    
	protected static readonly int ThemeID = Shader.PropertyToID("_Theme");
	
	public GameObject GetUniqueFlag() {
		foreach (GameObject g in entities) {
			if (g.GetComponent<Flag>() != null) {
				return g;
			}
		}
		return null;
	}
	public GameObject GetUniqueMine() {
		foreach (GameObject g in entities) {
			if (g.GetComponent<MineSprite>() != null) {
				return g;
			}
		}
		return null;
	}
	public void AddEntity(GameObject g) {
		if (g.GetComponent<Flag>() != null && GetUniqueFlag() != null) {
			Debug.Log("Tried to add second flag to tile at " + coord.ToString());
		} else if (g.GetComponent<MineSprite>() != null && GetUniqueMine() != null) {
			Debug.Log("Tried to add second mine to tile at " + coord.ToString());
		} else {
			entities.Add(g);
		}
	}
    protected override void Start() {
        base.Start();
        underTile = new GameObject("UnderTile");
        underTile.transform.parent = transform;
        underTile.AddComponent<Parallax>();
        underTile.AddComponent<SpriteRenderer>().color = new Color(.177f, .0973f, .0737f, 1f);
        underTile.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        underTile.active = underTileActive;
        collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        period = Random.Range(4f, 6f);
		//put the correct theme for the floor
		MaterialPropertyBlock mpb = new MaterialPropertyBlock();
		sr.GetPropertyBlock(mpb);
		mpb.SetFloat(ThemeID, Floor.s.floor/3 + 1);
		sr.SetPropertyBlock(mpb);
    }
    protected override void Update()
    {
        base.Update();
        underTile.GetComponent<Parallax>().referencePos = referencePos;
        float prop = 1 - Mathf.Pow(1 - Floor.s.tileExternalPower, Time.deltaTime / .15f);
        float adjacentDrag = 0;
        int adjacentTiles = 0;
        foreach ((int dx, int dy) in new (int, int)[] { (1,0), (-1,0), (0,1), (0,-1) }) {
            if (Floor.s.within(coord.x + dx, coord.y + dy) && Floor.s.tiles[coord.x + dx, coord.y + dy] != null) {
                adjacentDrag += Floor.s.tiles[coord.x + dx, coord.y + dy].GetComponent<Tile>().depth - depth;
                adjacentTiles++;
            }
        }
        externalDepthImpulse += Floor.s.tileAdjacentDragPower * Time.deltaTime * adjacentDrag / Mathf.Max(1, adjacentTiles);
        depth += externalDepthImpulse * prop;
        externalDepthImpulse *= 1 - prop;
        depth = Mathf.Lerp(depth, targetDepth + 0.012f * Mathf.Sin(Time.time * Mathf.PI * 2 / period), 1 - Mathf.Pow(1 - Floor.s.tileDampingPower, Time.deltaTime / .15f));
        underTile.GetComponent<Parallax>().depth = depth + 0.02f;
    }
    public virtual void PositionUnbuilt() {
		LeanTween.cancel(gameObject);
        targetDepth = 1.5f;
        referencePos = new Vector3(-Floor.s.width/2f + coord.x + 0.5f, -Floor.s.height/2f + coord.y + 0.5f, 0).normalized + Quaternion.Euler(0, 0, 90 * Random.Range(0, 4)) * new Vector3(0, 40, 0);
    }
	public virtual void Unbuild(float duration) {
		LeanTween.cancel(gameObject);
		LeanTween.value(gameObject, (float f) => {targetDepth = f;}, targetDepth, 1.5f, duration).setEase(LeanTweenType.easeInOutQuint);
		LeanTween.value(gameObject, (Vector3 v) => {referencePos = v;}, referencePos, referencePos + Quaternion.Euler(0, 0, 90 * Random.Range(0, 4)) * new Vector3(0, 40, 0), duration).setEase(LeanTweenType.easeInOutQuint).setOnComplete(() => {
			Destroy(gameObject);
		});
	}
    public virtual void Build(float duration) {
		LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, (float f) => {targetDepth = f;}, targetDepth, 1f, duration).setEase(LeanTweenType.easeInOutQuint);
        LeanTween.value(gameObject, (Vector3 v) => {referencePos = v;}, referencePos, new Vector3(-Floor.s.width/2f + coord.x + 0.5f, -Floor.s.height/2f + coord.y + 0.5f, 0), duration).setEase(LeanTweenType.easeInOutQuint);
    }
    public virtual void PutUnder() {
        GameManager.s.SetGameLayerRecursive(gameObject, LayerMask.NameToLayer("Under"));
        GameManager.s.PerformActionRecursive(gameObject, (GameObject g) => {
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
        GameManager.s.SetGameLayerRecursive(gameObject, LayerMask.NameToLayer("Default"));
        GameManager.s.PerformActionRecursive(gameObject, (GameObject g) => {
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
}
