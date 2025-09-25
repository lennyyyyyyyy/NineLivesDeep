using UnityEngine;
using System.Collections.Generic;

public class Parallax : MonoBehaviour
{
    public Vector3 referencePos = Vector3.zero, referenceScale = Vector3.one;
    public float depth = 1;
    public List<SpriteRenderer> sortWithDepth = new List<SpriteRenderer>();

    public virtual void SetDepth(float newDepth) {
        depth = newDepth;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        sortWithDepth.Add(GetComponent<SpriteRenderer>());
    }
    // Update is called once per frame
    protected virtual void Update()
    {
        float cameraDistance = Camera.main.orthographicSize / 3.75f;
        Vector3 offset = referencePos - MainCamera.s.transform.position;

        transform.position = cameraDistance * offset / (depth + cameraDistance - 1) + MainCamera.s.transform.position;
        transform.localScale = cameraDistance * referenceScale  / (depth + cameraDistance - 1);

        foreach (SpriteRenderer s in sortWithDepth) {
            if (s != null) {
                s.sortingOrder = Mathf.RoundToInt(-100 * depth);
            }
        }
    }
}
