using UnityEngine;

public class VerticalObject : MonoBehaviour
{
    public GameObject marker;
    public SpriteRenderer sr;
    protected virtual void Start() {
		if (sr == null) { sr = GetComponent<SpriteRenderer>(); }
		if (marker == null) { marker = gameObject; }
    }
    protected virtual void Update()
    {
        sr.sortingOrder = (int) (marker.transform.position.y * -100);
    }
}
