using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    // prefabs
    public GameObject tile_p,
		  		      tile_exit_p,
					  tile_puddle_p,
					  tile_mossy_p,
					  tile_background_p,
					  moss_p,
					  grass1_p,
					  grass2_p,
					  mine_p,
					  mineSprite_p,
					  number_p,
					  print_p,
					  blood_p,
					  heart_p,
					  flag_p,
					  curse_p,
					  flagSprite_p,
					  paw_p,
					  tooltip_p,
					  bubble_p,
					  psychicEye_p,
					  playerBit_p,
					  crank_p,
                      pillar_p,
                      vase_p,
                      tunnel_p;

    public GameObject underDarkenTarget;
    public static GameManager s;
    [System.NonSerialized]
    public int START = 0, GAME = 1;
    [System.NonSerialized]
    public int gamestate = 0;
    public float deathReviveDuration = 3;
    private Matrix4x4 disturbancePositions1 = Matrix4x4.zero, disturbancePositions2 = Matrix4x4.zero;
    private Vector4[] disturbancePositions = new Vector4[8];
    private Vector4 disturbanceTimes1 = -1 * Vector4.one, disturbanceTimes2 = -1 * Vector4.one;
    private float[] disturbanceTimes = new float[8];
    public readonly int LayerID = Shader.PropertyToID("_Layer"),
                         UnderDarkenID = Shader.PropertyToID("_UnderDarken"),
                         DPos1ID = Shader.PropertyToID("_DisturbancePositions1"),
                         DPos2ID = Shader.PropertyToID("_DisturbancePositions2"),
                         DTimes1ID = Shader.PropertyToID("_DisturbanceTimes1"),
                         DTimes2ID = Shader.PropertyToID("_DisturbanceTimes2");
    public static Action OnSTARTToGAME;
    public Scene scene;
    public PhysicsScene2D physicsScene;
    [System.NonSerialized]
	public HashSet<Collider2D> collidersUnderMouse = new HashSet<Collider2D>(),
		   					   collidersUnderMouseLastFrame = new HashSet<Collider2D>();
    void Awake() {
        LeanTween.init(20000);
        s = this;
        for (int i=0; i<4; i++) {
            disturbancePositions1.SetRow(i, 10000 * Vector4.one);
            disturbancePositions2.SetRow(i, 10000 * Vector4.one);
        }
    }
    void Start() {      
        Shader.SetGlobalFloat(LayerID, 0);
        Shader.SetGlobalFloat(UnderDarkenID, 1);
        scene = SceneManager.GetActiveScene();
        physicsScene = PhysicsSceneExtensions2D.GetPhysicsScene2D(scene);
    }
    private void Update() {
		collidersUnderMouseLastFrame = new HashSet<Collider2D>(collidersUnderMouse);
        collidersUnderMouse = new HashSet<Collider2D>(Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition)));

		// custom mouse events
        foreach (Collider2D c in collidersUnderMouse) {
			if (!collidersUnderMouseLastFrame.Contains(c)) {
                c.SendMessage("OnMouseEnterCustom", SendMessageOptions.DontRequireReceiver);
            }
			if (Input.GetMouseButtonDown(0)) {
				c.SendMessage("OnMouseDownCustom", SendMessageOptions.DontRequireReceiver);
			}
        }
		foreach (Collider2D c in collidersUnderMouseLastFrame) {
			if (c != null && !collidersUnderMouse.Contains(c)) {
                c.SendMessage("OnMouseExitCustom", SendMessageOptions.DontRequireReceiver);
            }
        }

    }
    public void STARTToGAME() {
        OnSTARTToGAME?.Invoke();
    }
    public void disturbShaders(float x, float y) {
        if (Time.time > Mathf.Max(disturbanceTimes[7] + 1, disturbanceTimes[0] + 0.125f)) {
            for (int i=7; i>0; i--) {
                disturbancePositions[i] = disturbancePositions[i-1];
                disturbanceTimes[i] = disturbanceTimes[i-1];
            }
            disturbancePositions[0] = new Vector4(x, y, 0, 0);
            disturbanceTimes[0] = Time.time;
            for (int i=0; i<4; i++) {
                disturbancePositions1.SetRow(i, disturbancePositions[i]);
                disturbanceTimes1[i] = disturbanceTimes[i];
                disturbancePositions2.SetRow(i, disturbancePositions[i+4]);
                disturbanceTimes2[i] = disturbanceTimes[i+4];
            }
            Shader.SetGlobalMatrix(DPos1ID, disturbancePositions1);
            Shader.SetGlobalVector(DTimes1ID, disturbanceTimes1);
            Shader.SetGlobalMatrix(DPos2ID, disturbancePositions2);
            Shader.SetGlobalVector(DTimes2ID, disturbanceTimes2);
        }
    }
    public void SetGameLayer(GameObject g, int layer) {
        g.layer = layer;
        SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
        if (sr != null) {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            sr.GetPropertyBlock(mpb);
            mpb.SetFloat(LayerID, layer);
            sr.SetPropertyBlock(mpb);
        }
    }
    public void SetGameLayerRecursive(GameObject g, int layer) {
        PerformActionRecursive(g, (GameObject obj) => { SetGameLayer(obj, layer); });
    }
    public void SetSortingLayerRecursive(GameObject g, string layerName) {
        PerformActionRecursive(g, (GameObject obj) => { 
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null) {
                sr.sortingLayerName = layerName;
            }
        });
    }
    public void PerformActionRecursive(GameObject g, in Action<GameObject> action) {
        action(g);
        foreach (Transform child in g.transform) {
            PerformActionRecursive(child.gameObject, action);
        }
    }
    public Component CopyComponent(Component original, GameObject destination) {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields(); 
        foreach (System.Reflection.FieldInfo field in fields) {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }
    public void DelayAction(Action action, float delay) {
        StartCoroutine(DelayActionCoroutine(action, delay));
    }
    private IEnumerator DelayActionCoroutine(Action action, float delay) {
        yield return new WaitForSeconds(delay);
        action.Invoke();
    }
    public void DelayActionFrames(Action action, int frames) {
        StartCoroutine(DelayActionFramesCoroutine(action, frames));
    }
    private IEnumerator DelayActionFramesCoroutine(Action action, int frames) {
        for (int i=0; i<frames; i++) {
            yield return null;
        }
        action.Invoke();
    }   
    public void Shuffle<T>(ref List<T> ts) {
		int count = ts.Count;
		int last = count - 1;
		for (int i = 0; i < last; i++) {
			int r = UnityEngine.Random.Range(i, count);
            (ts[r], ts[i]) = (ts[i], ts[r]);
        }
    }
    public void Shuffle<T>(ref T[] ts) {
        int count = ts.Length;
        int last = count - 1;
        for (int i = 0; i < last; i++) {
            int r = UnityEngine.Random.Range(i, count);
            (ts[r], ts[i]) = (ts[i], ts[r]);
        }
    }
}
