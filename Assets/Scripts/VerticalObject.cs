using UnityEngine;

public class VerticalObject : MonoBehaviour
{
    public GameObject marker;
    [System.NonSerialized]
    public SpriteRenderer sr;
    protected virtual void Start() {
        sr = GetComponent<SpriteRenderer>();
    }
    protected virtual void Update()
    {
        sr.sortingOrder = (int) (marker.transform.position.y * -100);
    }
}
