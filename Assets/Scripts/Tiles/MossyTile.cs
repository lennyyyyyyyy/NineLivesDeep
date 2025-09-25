using UnityEngine;

public class MossyTile : Tile
{
    public int mossRowCount, grass1RowCount, grass2RowCount;
    public float mossHeightInRows, grassHeight1, grassHeight2, lightXStart, lightXEnd, lightZRange;
    public GameObject dust;
    protected override void Start() {
        base.Start();
        InstantiateRows(GameManager.s.moss_p, mossRowCount, 0.9f * mossHeightInRows / mossRowCount);
        InstantiateRows(GameManager.s.grass1_p, grass1RowCount, grassHeight1);
        InstantiateRows(GameManager.s.grass2_p, grass2RowCount, grassHeight2);
        dust.transform.eulerAngles = new Vector3(Random.Range(lightXStart, lightXEnd), 0, Random.Range(-lightZRange, lightZRange));
        mineMult = 2;
     }

    private void InstantiateRows(GameObject prefab, int rows, float height) {
        for (int i=0; i<=rows; i++) {
            GameObject g = Instantiate(prefab, transform.position + Vector3.up * (Mathf.Lerp(-0.45f, 0.45f, (float) i / rows) + height), Quaternion.identity);
            g.transform.localScale = new Vector3(1.3f, height*2, 1);
            g.transform.parent = transform;
        }
    }
}
