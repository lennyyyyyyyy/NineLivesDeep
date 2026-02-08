using UnityEngine;
using TMPro;

public class Number : MonoBehaviour {
	protected Map map;
    [System.NonSerialized]
	public Vector2Int coord;
    [System.NonSerialized]
    public int num;
	protected TMP_Text text;
	private void Update() {
		if (Random.value < 1 - Mathf.Pow(1 - Player.s.modifiers.mapNumberDisappearChancePerSecond, Time.deltaTime)) {
			Destroy(gameObject);
		}
	}
    public void Init(Map map, Vector2Int coord) {
		this.map = map;
		this.coord = coord;
		text = GetComponent<TMP_Text>();
        text.enabled = false;
        gameObject.SetActive(Player.s.mapNumberActive);
    }
    public void SetNum(int num) {
        this.num = num;
        text.text = num.ToString();
        LeanTween.scale(gameObject, Vector3.one, Random.Range(0.5f, 1.5f)).setRepeat(-1).setLoopPingPong().setEase(LeanTweenType.easeInOutCubic);
        transform.eulerAngles = Vector3.forward * Random.Range(-30f, -10f);
        LeanTween.rotate(gameObject, Vector3.forward * Random.Range(10f, 30f), Random.Range(0.5f, 1.5f)).setRepeat(-1).setLoopPingPong().setEase(LeanTweenType.easeInOutCubic);
    }
    public void Enter() {
        text.enabled = true;
    }
    public void Exit() {
        text.enabled = false;
    }
	private void OnDestroy() {
		map.RemoveNumber(coord.x, coord.y);
	}
}
