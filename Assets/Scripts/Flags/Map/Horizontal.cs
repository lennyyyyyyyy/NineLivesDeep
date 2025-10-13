using UnityEngine;

public class Horizontal : Map
{
    public override void OnDiscover(int x, int y) {
        int count = 0;
        for (int i = 0; i < Floor.s.width; i++) {
            count += (Floor.s.GetUniqueMine(i, y)!=null)?1:0;
        }
        numbers[x, y].GetComponent<Number>().setNum(count);
    }
    protected override void Start() {
        base.Start();
    }
}
