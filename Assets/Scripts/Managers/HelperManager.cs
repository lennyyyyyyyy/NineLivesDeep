using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;

/*
 * The HelperManager contains various helper functions. 
 */
public class HelperManager : MonoBehaviour {
    public static HelperManager s;

    private void Awake() {
        s = this;
    }

    // Loads a resource from Assets/Resources/path, but is safer if a resource isn't found.
	public T LoadResourceSafe<T>(string path) where T : UnityEngine.Object {
		T resource = Resources.Load<T>(path);
        if (resource != null) {
            Debug.LogError("Couldn't load resource at path: " + path);
        }
		if (typeof(T) == typeof(Texture2D)) {
			return resource ?? Resources.Load<T>("Textures/nulltex");
		}
		return resource;
	}
    public void SetGameLayer(GameObject g, int layer) {
        g.layer = layer;
        SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
        if (sr != null) {
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            sr.GetPropertyBlock(mpb);
            mpb.SetFloat(ShaderManager.s.LayerID, layer);
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
	public void SetupUIEventTriggers(GameObject g, EventTriggerType[] eventIDs, Action<PointerEventData>[] actions) { 
		EventTrigger trigger;
		EventTrigger.Entry entry;
		trigger = g.GetComponent<EventTrigger>() == null ? g.AddComponent<EventTrigger>() : g.GetComponent<EventTrigger>();
		for (int i = 0; i < eventIDs.Length; i++) {
			entry = new EventTrigger.Entry{eventID = eventIDs[i]};
			int iCopy = i; //capture variable for closure
			entry.callback.AddListener((data) => { actions[iCopy]((PointerEventData)data); });
			trigger.triggers.Add(entry);
		}
	}
    public void InstantiateBubble(Vector3 pos, string t, Color c, float time = 1f, float scale = 1f) {
        GameObject b = Instantiate(PrefabManager.s.bubblePrefab, pos, Quaternion.identity, UIManager.s.bubbleGroup.transform);
        b.GetComponent<Bubble>().Init(c, t, time, scale);
    }
    public void InstantiateBubble(GameObject g, string t, Color c, float radius = 0.5f, float time = 1f, float scale = 1f) {
        InstantiateBubble(g.transform.position + new Vector3(UnityEngine.Random.Range(-radius, radius), UnityEngine.Random.Range(-radius, radius), 0), t, c, time);
    }
    public Vector2 WorldSizeFromRT(RectTransform rt) {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return new Vector2(corners[2].x - corners[0].x, corners[1].y - corners[0].y);
    }
    public void FloatingHover(Transform t, float hoveredScale, float offset, Vector3 defaultRotation, float stretch=0.1f, float angle=10f, float period=0.65f, float power=0.65f) {
        Vector3 newScale = Vector3.one;

        newScale[0] = Mathf.Lerp(t.localScale[0], hoveredScale * Mathf.Pow(1+stretch, Mathf.Sin((period*Time.time)*(2*Mathf.PI))), 1 - Mathf.Pow(1 - power, Time.deltaTime / .15f));
        newScale[1] = Mathf.Lerp(t.localScale[1], Mathf.Pow(hoveredScale, 2) / newScale[0], 1 - Mathf.Pow(1 - power, Time.deltaTime / .15f));

        t.localScale = newScale;
        t.localEulerAngles = new Vector3(t.localEulerAngles.x, 
                                                t.localEulerAngles.y, 
                                                Mathf.LerpAngle(t.localEulerAngles.z, defaultRotation.z + angle * Mathf.Sin((period*Time.time + offset)*(2*Mathf.PI)), 1 - Mathf.Pow(1 - power, Time.deltaTime / .15f)));
    }
    public void UpdateAlpha(SpriteRenderer sr, float a) {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
    }
    public void UpdateColor(SpriteRenderer sr, Color c) {
        sr.color = c;
    }
    public void UpdateAnchoredPosition(RectTransform rt, Vector3 v) {
        rt.anchoredPosition = new Vector2(v.x, v.y);
    }
    public void UpdateSizeDelta(RectTransform rt, Vector3 v) {
        rt.sizeDelta = new Vector2(v.x, v.y);
    }
}
