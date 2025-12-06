using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Rendering.Universal;
using System;

public class FlagSprite : CorrespondingSprite {
	public Placeable parent;

    public static float droppedScale = 0.6f, heldPower = .8f, heldOffset, heldPeriod, overToUnderDuration = 0.5f;
    [System.NonSerialized]
    public string state = "held";
    protected Light2D light;
	protected float placeImpulse = 0.2f;

    protected override void Start() {
        light = GetComponentInChildren<Light2D>();
        marker = transform.Find("Marker").gameObject;

        base.Start();

        // init random properties
        heldOffset = Random.Range(0f, 1f);
        heldPeriod = 0.65f + 0.15f * Random.Range(-1f, 1f);
        // update parent usable state
        UIManager.s.OrganizeFlags();
        //hide map layers and prints
        Player.s.UpdateSecondaryMapActive();
        Player.s.UpdateActivePrints();
        //hide bad tiles
        foreach (GameObject tile in Floor.s.tiles.Values) {
			if (!CoordAllowed(tile.GetComponent<Tile>().coord.x, tile.GetComponent<Tile>().coord.y)) {
				tile.GetComponent<Tile>().PutUnder();
			}
        }
        //darken under
        LeanTween.cancel(GameManager.s.underDarkenTarget);
        LeanTween.value(GameManager.s.underDarkenTarget, TweenUnderDarken, Shader.GetGlobalFloat(GameManager.s.UnderDarkenID), 0.1f, overToUnderDuration).setEase(LeanTweenType.easeInOutCubic);
    }
    protected override void Update() {
        base.Update();
        if (state == "held") {
            UIManager.s.floatingHover(transform, 0.8f, 0, transform.localEulerAngles, 0.05f, heldOffset, heldPeriod, heldPower);

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
                GameManager.s.disturbShaders(transform.position.x, transform.position.y);
            }
            //OnMouseUp bug
            if (!UnityEngine.InputSystem.Mouse.current.leftButton.isPressed) {
                OnMouseUp();
            }
        }
    }
	public virtual void SetInitialData(Placeable parent) {
		setInitialData = true;
		this.parent = parent;
		base.SetInitialData(parent.GetType());
	}
	protected override void ApplyInitialData() {
		base.ApplyInitialData();
		light.color = tooltipData.color;
	}
	public virtual void SetData(Placeable parent) {
		SetInitialData(parent);
		ApplyInitialData();
    }
    protected virtual void OnMouseUp() {
        if (state == "held") {
            state = "falling";
			// if windy flag may land somewhere nearby
			Vector3 landingPos = transform.position + Floor.s.windDirection * Player.s.modifiers.windStrength * 1f;
            Vector2Int dropCoord = Floor.s.PosToCoord(landingPos);
            if (CoordAllowed(dropCoord.x, dropCoord.y)) {
				Move(dropCoord.x, dropCoord.y, false);
				if (Player.s.modifiers.windStrength != 0) {
					LeanTween.move(gameObject, landingPos, 0.15f);
				}
                LeanTween.scale(gameObject, droppedScale*Vector3.one, 0.15f).setEase(LeanTweenType.easeInCubic);
                LeanTween.rotate(gameObject, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInCubic).setOnComplete(() => {
                    state = "dropped";
                    //disturb shaders when it hits the ground
                    GameManager.s.disturbShaders(transform.position.x, transform.position.y);
                    //show map layers and prints
                    Player.s.UpdateSecondaryMapActive();
                    Player.s.UpdateActivePrints();
                });
                OnPlace();
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
                    Player.s.UpdateSecondaryMapActive();
                    Player.s.UpdateActivePrints();
                });
            }
            //reset tiles
			foreach (GameObject tile in Floor.s.tiles.Values) {
				tile.GetComponent<Tile>().PutOver();
            }
            //brighten under
            LeanTween.cancel(GameManager.s.underDarkenTarget);
            LeanTween.value(GameManager.s.underDarkenTarget, TweenUnderDarken, Shader.GetGlobalFloat(GameManager.s.UnderDarkenID), 1f, overToUnderDuration).setEase(LeanTweenType.easeInOutCubic);
            //replace parent with cat paw animation
            UIManager.s.OrganizeFlags();
        }
    }
    protected virtual void OnPlace() {
        parent.UpdateCount(parent.count - 1);
        Player.s.destroyPrints();
        Player.s.updatePrints();
        sr.sortingLayerName = "Player";
		FlagData parentFlagData = UIManager.s.uiTypeToData[correspondingUIType] as FlagData;
        if (parentFlagData.placeableRemovesMines && Floor.s.GetUniqueMine(GetCoord().x, GetCoord().y) != null) {
            Player.s.UpdateMoney(Player.s.money + Player.s.modifiers.mineDefuseMult);
            Floor.s.GetUniqueMine(GetCoord().x, GetCoord().y).GetComponent<MineSprite>().Remove();
        }
        if (Player.s.hasFlag(typeof(Reflection)) && GetType() != typeof(BaseSprite) && GetTile().GetComponent<Puddle>() != null) {
            //reflection passive
            Flag b = UIManager.s.uiTypeToData[typeof(Base)].instances[0].GetComponent<Flag>();
            b.UpdateCount(b.count+1);
        }
		//give tile momentum downward
		GetTile().GetComponent<Tile>().externalDepthImpulse += placeImpulse;	
    }
	public override void Move(GameObject tile, bool reposition = true) {
		base.Move(tile, reposition);
	}
	public override void Remove() {
		base.Remove();
	}
    public override bool CoordAllowed(int x, int y) { 
        return base.CoordAllowed(x, y) && Floor.s.GetUniqueFlag(x, y) == null && !(x == Player.s.GetCoord().x && y == Player.s.GetCoord().y); 
    }
    protected static void TweenUnderDarken(float darken) {
        Shader.SetGlobalFloat(GameManager.s.UnderDarkenID, darken);
    }
}
