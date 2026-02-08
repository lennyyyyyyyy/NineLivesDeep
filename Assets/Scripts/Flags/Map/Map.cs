using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class Map : Flag {
    [System.NonSerialized]
    public Dictionary<Vector2Int, GameObject> numbers = new Dictionary<Vector2Int, GameObject>();
    [System.NonSerialized]
    public bool active = false;

    protected override void BeforeInit() {
        base.BeforeInit();
		HelperManager.s.SetupUIEventTriggers(gameObject,
                                             new EventTriggerType[] {EventTriggerType.PointerClick},
                                             new Action<PointerEventData>[] {OnPointerClick});
    }
    protected override void OnPointerEnter(PointerEventData data) {
		base.OnPointerEnter(data);
        LeanTween.value(gameObject, (Vector3 v) => HelperManager.s.UpdateSizeDelta(rt, v), rt.sizeDelta, new Vector2(100, 100), 0.1f).setEase(LeanTweenType.easeInOutCubic);
    } 
    protected override void OnPointerExit(PointerEventData data) {
		base.OnPointerExit(data);
        LeanTween.value(gameObject, (Vector3 v) => HelperManager.s.UpdateSizeDelta(rt, v), rt.sizeDelta, new Vector2(80, 80), 0.1f).setEase(LeanTweenType.easeInOutCubic);
    }
    protected virtual void Activate() {
        active = true;
        foreach (GameObject number in numbers.Values) {
			number.GetComponent<Number>().Enter();
        }
        addTooltip.ForceHoverEffectOn();
    }
    protected virtual void Deactivate() {
        active = false;
        foreach (GameObject number in numbers.Values) {
			number.GetComponent<Number>().Exit();
        }
        addTooltip.ForceHoverEffectOff();
    }
    protected virtual void OnPointerClick(PointerEventData data) {
        if (active) {
            Deactivate();
        } else {
            foreach (GameObject g in PlayerUIItemModule.s.flags) {
                Flag flag = g.GetComponent<Flag>();
                if (flag is Map && (flag as Map).active) {
                    (flag as Map).Deactivate();
                }
            }
            Activate();
        }
    }
    public virtual void OnDiscover(int x, int y) { }
	public virtual bool NumberExistsAt(int x, int y) {
		return numbers.ContainsKey(new Vector2Int(x, y));
	}
	public virtual GameObject GetNumber(int x, int y) {
		return NumberExistsAt(x, y) ? numbers[new Vector2Int(x, y)] : null;
	}
	public virtual void SetNumber(int x, int y, int num) {
		if (NumberExistsAt(x, y)) {
			numbers[new Vector2Int(x, y)].GetComponent<Number>().SetNum(num);
		} else {
			GameObject n = numbers[new Vector2Int(x, y)] = Instantiate(PrefabManager.s.numberPrefab, Floor.s.transform);
			n.transform.position = Floor.s.CoordToIdealPos(x, y); 
			n.transform.localScale = Vector3.one;
			n.GetComponent<Number>().Init(this, new Vector2Int(x, y));
			n.GetComponent<Number>().SetNum(num);
			if (active) {
				n.GetComponent<Number>().Enter();
			}
		}
	}
	public virtual void RemoveNumber(int x, int y) {
		if (NumberExistsAt(x, y)) {
			Destroy(numbers[new Vector2Int(x, y)]);
			numbers.Remove(new Vector2Int(x, y));
		}
	}
    public virtual void Reset() {
		foreach (GameObject n in numbers.Values) {
			Destroy(n);
		}
    }
	private void OnForceMapNumberActive(bool active) {
		foreach (GameObject n in numbers.Values) {
			n.SetActive(active);
		}
	}
    protected virtual void OnEnable() {
        EventManager.s.OnFloorChangeBeforeEntities += Reset;
        EventManager.s.OnForceMapNumberActive += OnForceMapNumberActive;
    }
    protected virtual void OnDisable() {
        EventManager.s.OnFloorChangeBeforeEntities -= Reset;
        EventManager.s.OnForceMapNumberActive -= OnForceMapNumberActive;
    }
    protected override bool IsUsable() {
        return base.IsUsable();
    }
}
