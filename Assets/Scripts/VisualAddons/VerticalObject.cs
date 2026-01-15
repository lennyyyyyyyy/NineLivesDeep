using UnityEngine;

public class VerticalObject : MonoBehaviour
{
    public GameObject marker;
    public SpriteRenderer sr;
    protected virtual void Awake() {
		if (sr == null) { sr = GetComponentInChildren<SpriteRenderer>(); }
		if (marker == null) { marker = gameObject; }
    }
    protected virtual void Update()
    {
        sr.sortingOrder = (int) (marker.transform.position.y * -100);
    }
}
