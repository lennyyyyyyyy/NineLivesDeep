using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.Universal;
using System;

public class FlagSprite : CorrespondingSprite {
    public static float heldPower = .8f, heldOffset, heldPeriod, overToUnderDuration = 0.5f;

    [System.NonSerialized]
	public Placeable parent;
    [System.NonSerialized]
    public string state = "held";
    protected Light2D light;
	protected float placeImpulse = 0.2f;

    protected override void BeforeInit() {
        base.BeforeInit();
        light = GetComponentInChildren<Light2D>();
        marker = transform.Find("Marker").gameObject;
    }
    protected override void AfterInit() {
        base.AfterInit();
    }
    public virtual void Init(Placeable parent) {
        this.state = "held";
		this.parent = parent;
		Init(parent.GetType());
        //hide bad tiles
        foreach (GameObject tile in Floor.s.tiles.Values) {
			if (!CoordAllowed(tile.GetComponent<Tile>().coord.x, tile.GetComponent<Tile>().coord.y)) {
				tile.GetComponent<Tile>().PutUnder();
			}
        }
        //darken under
        ShaderManager.s.TweenUnderDarken(0.1f, overToUnderDuration);
		//amnesia curse only for placed flag sprites, not pickup sprites in the shop
		if (Player.s.modifiers.amnesiaUITypes.Contains(typeof(Flag))) {
			tooltipData.name = "???";
			tooltipData.flavor = "???";
			tooltipData.info = "???";
		}
		light.color = tooltipData.color;

        heldOffset = Random.Range(0f, 1f);
        heldPeriod = 0.65f + 0.15f * Random.Range(-1f, 1f);
        PlayerUIItemModule.s.OrganizeFlags();
        //hide map layers and prints
        EventManager.s.OnForceMapNumberActive?.Invoke(false);
        Player.s.UpdateActivePrints();
    }
    public virtual void Init(Vector2Int spawnCoord) {
        this.state = "dropped";
        transform.localScale = ConstantsManager.s.flagSpriteDroppedScale * Vector3.one; 
        sr.sortingLayerName = "Player";
        Init(CatalogManager.s.spriteTypeToUIType[GetType()]);
        Move(spawnCoord.x, spawnCoord.y);
		//amnesia curse only for placed flag sprites, not pickup sprites in the shop
		if (Player.s.modifiers.amnesiaUITypes.Contains(typeof(Flag))) {
			tooltipData.name = "???";
			tooltipData.flavor = "???";
			tooltipData.info = "???";
		}
		light.color = tooltipData.color;
    }
    protected override void Update() {
        base.Update();
        if (state == "held") {
            HelperManager.s.FloatingHover(transform, 0.8f, 0, transform.localEulerAngles, 0.05f, heldOffset, heldPeriod, heldPower);

            float idleAngle = 5f * Mathf.Sin((heldPeriod*Time.time + heldOffset)*(2*Mathf.PI));
            Vector3 swingDirection =   1f * new Vector3(-transform.up.x * transform.up.y, 1 - Mathf.Pow(transform.up.y, 2), 0) // gravity pulling center of mass
                                     + 3f * (1 - 1 / (0.07f * UIManager.s.mouseVelocity.magnitude + 1)) * UIManager.s.mouseVelocity.normalized // pulling force by mouse
                                     + 1 * transform.up; // inertia force
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 
                                                    transform.localEulerAngles.y, 
                                                    Mathf.LerpAngle(transform.localEulerAngles.z, Quaternion.LookRotation(Vector3.forward, swingDirection).eulerAngles.z + idleAngle, 1 - Mathf.Pow(1 - heldPower, Time.deltaTime / .15f)));
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //disturb shaders when moving and holding flag
            if (UIManager.s.mouseVelocity.magnitude > 0) {
                ShaderManager.s.DisturbShaders(transform.position.x, transform.position.y);
            }
        }
    }
    protected virtual void OnMouseUpCustom() {
        if (state == "held") {
            state = "falling";
			// if windy flag may land somewhere nearby
			Vector3 landingPos = transform.position + Floor.s.windDirection * Player.s.modifiers.windStrength * 1f;
            Vector2Int dropCoord = Floor.s.PosToCoord(landingPos);
            OnDrop(dropCoord.x, dropCoord.y);
            if (CoordAllowed(dropCoord.x, dropCoord.y)) {
				Move(dropCoord.x, dropCoord.y, reposition: false, rescale: false);
				if (Player.s.modifiers.windStrength != 0) {
					LeanTween.move(gameObject, landingPos, 0.15f);
				}
                LeanTween.scale(gameObject, ConstantsManager.s.flagSpriteDroppedScale * Vector3.one, 0.15f).setEase(LeanTweenType.easeInCubic);
                LeanTween.rotate(gameObject, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInCubic).setOnComplete(() => {
                    state = "dropped";
                    //disturb shaders when it hits the ground
                    ShaderManager.s.DisturbShaders(transform.position.x, transform.position.y);
                    //show map layers and prints
                    Player.s.UpdateActivePrints();
                    OnPlace();
                });
            } else {
				if (Player.s.modifiers.windStrength != 0) {
					LeanTween.move(gameObject, landingPos, 0.75f);
				}
                LeanTween.scale(gameObject, .2f*new Vector3[]{Vector3.left, Vector3.right, Vector3.up, Vector3.down}[Random.Range(0, 4)], .75f).setEase(LeanTweenType.easeInCubic);
                LeanTween.value(gameObject, (float f) => {light.intensity = f;}, light.intensity, 0f, .75f).setEase(LeanTweenType.easeInCubic);
                LeanTween.rotate(gameObject, Random.Range(0, 360f) * Vector3.forward, .75f).setEase(LeanTweenType.easeInCubic).setOnComplete(() => {
                    state = "dropped";
                   	Remove();
                    //show map layers and prints
                    Player.s.UpdateActivePrints();
                });
            }
            //reset tiles
			foreach (GameObject tile in Floor.s.tiles.Values) {
				tile.GetComponent<Tile>().PutOver();
            }
            //brighten under
            ShaderManager.s.TweenUnderDarken(1f, overToUnderDuration);
            //replace parent with cat paw animation
            PlayerUIItemModule.s.OrganizeFlags();
        }
    }
    protected virtual void OnDrop(int x, int y) {
        EventManager.s.OnForceMapNumberActive?.Invoke(true);
    }
    protected virtual void OnPlace() {
        parent.UpdateCount(parent.count - 1);
        Player.s.destroyPrints();
        Player.s.updatePrints();
        sr.sortingLayerName = "Player";
		FlagData parentFlagData = CatalogManager.s.typeToData[correspondingUIType] as FlagData;
        GameObject mine = Floor.s.GetUniqueMine(GetCoord().x, GetCoord().y);
        if (parentFlagData.placeableRemovesMines && mine != null) {
            Player.s.UpdateMoney(Player.s.money + Player.s.modifiers.mineDefuseMult);
            Type mineType = mine.GetComponent<MineSprite>().GetType();
            EventManager.s.OnMineDefused?.Invoke(PlayerUIItemModule.s.minesDefused.Contains(mineType));
            PlayerUIItemModule.s.NoticeMineDefused(mineType);
            Floor.s.GetUniqueMine(GetCoord().x, GetCoord().y).GetComponent<MineSprite>().Remove();
        }
        //reflection passive
        if (Player.s.modifiers.reflectionPassiveCount != 0 && GetType() != typeof(BaseSprite) && GetTile().GetComponent<Puddle>() != null) {
            Flag b = PlayerUIItemModule.s.typeToInstances[typeof(Base)][0].GetComponent<Flag>();
            b.UpdateCount(b.count + Player.s.modifiers.reflectionPassiveCount);
        }
        Tile tile = GetTile().GetComponent<Tile>();
		GetTile().GetComponent<Tile>().externalDepthImpulse += placeImpulse;	
        AudioManager.s.PlayCorrespondingTileEffect(tile);
    }
    public override bool CoordAllowed(int x, int y) {
		return Floor.s.TileExistsAt(x, y) &&
               Floor.s.GetTile(x, y).GetComponent<Tile>().uniqueObstacle == null;
    }
}
