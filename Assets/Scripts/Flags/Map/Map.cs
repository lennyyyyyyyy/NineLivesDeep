using UnityEngine;
using UnityEngine.EventSystems;
public class Map : Flag
{
    [System.NonSerialized]
    public GameObject[,] numbers;
    [System.NonSerialized]
    public bool active = false;

    protected override void OnPointerEnter(PointerEventData data) {
		base.OnPointerEnter(data);
        if (active) {
            LeanTween.value(gameObject, (Vector3 v) => UIManager.s.updateSizeDelta(rt, v), rt.sizeDelta, new Vector2(110, 110), 0.1f).setEase(LeanTweenType.easeInOutCubic);
        } else {
            LeanTween.value(gameObject, (Vector3 v) => UIManager.s.updateSizeDelta(rt, v), rt.sizeDelta, new Vector2(100, 100), 0.1f).setEase(LeanTweenType.easeInOutCubic);
        }
    } 
    protected override void OnPointerExit(PointerEventData data) {
		base.OnPointerExit(data);
        if (active) {
            LeanTween.value(gameObject, (Vector3 v) => UIManager.s.updateSizeDelta(rt, v), rt.sizeDelta, new Vector2(100, 100), 0.1f).setEase(LeanTweenType.easeInOutCubic);
        } else {
            LeanTween.value(gameObject, (Vector3 v) => UIManager.s.updateSizeDelta(rt, v), rt.sizeDelta, new Vector2(80, 80), 0.1f).setEase(LeanTweenType.easeInOutCubic);
        }
    }
    protected virtual void Activate() {
        LeanTween.value(gameObject, (Vector3 v) => UIManager.s.updateSizeDelta(rt, v), rt.sizeDelta, new Vector2(100, 100), 0.1f).setEase(LeanTweenType.easeInOutCubic);
        active = true;
        foreach (GameObject number in numbers) {
			if (number != null) {
				number.GetComponent<Number>().enter();
			}
        }
    }
    protected virtual void Deactivate() {
        LeanTween.value(gameObject, (Vector3 v) => UIManager.s.updateSizeDelta(rt, v), rt.sizeDelta, new Vector2(80, 80), 0.1f).setEase(LeanTweenType.easeInOutCubic);
        active = false;
        foreach (GameObject number in numbers) {
			if (number != null) {
				number.GetComponent<Number>().exit();
			}
        }
    }
    protected virtual void OnPointerClick(PointerEventData data) {
        if (active) {
            Deactivate();
        } else {
            foreach (GameObject g in Player.s.flags) {
                Flag flag = g.GetComponent<Flag>();
                if (flag is Map && (flag as Map).active) {
                    (flag as Map).Deactivate();
                }
            }
            Activate();
        }
    }
    public virtual void OnDiscover(int x, int y) { }
	public virtual bool TrySetNumber(int x, int y, int num) {
		if (x >= 0 && x < numbers.GetLength(0) && y >= 0 && y < numbers.GetLength(1) && numbers[x, y] != null) {
			numbers[x, y].GetComponent<Number>().setNum(num);
			return true;
		} else {
			Debug.Log("Tried to set number at invalid coord " + x + ", " + y);
			return false;
		}
	}
    public virtual void reset() {
        if (numbers != null) {
            foreach (GameObject n in numbers) {
                Destroy(n);
            }
        }
        numbers = new GameObject[Floor.s.width, Floor.s.height];
        for (int i = 0; i < Floor.s.width; i++) {
            for (int j = 0; j < Floor.s.height; j++) {
				if (Floor.s.tiles[i, j] != null) {	
                	numbers[i, j] = Instantiate(GameManager.s.number_p, Floor.s.tiles[i, j].transform);
                	numbers[i, j].transform.localPosition = Vector3.zero;
                	numbers[i, j].transform.localScale = Vector3.one;
                	numbers[i, j].GetComponent<Number>().init();
				}
            }
        }
    }
    protected virtual void OnEnable() {
        Floor.onFloorChangeAfterEntities += reset;
    }
    protected virtual void OnDisable() {
        Floor.onFloorChangeAfterEntities -= reset;
    }
    protected override void Start() {
        base.Start();
        entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerClick};
        entry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }
    protected override bool IsUsable() {
        return base.IsUsable();
    }
}
