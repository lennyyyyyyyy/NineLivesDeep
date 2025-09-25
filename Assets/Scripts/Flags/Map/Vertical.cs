using UnityEngine;

public class Vertical : Map
{
    public override void OnDiscover(int x, int y) {
        int count = 0;
        for (int j = 0; j < Floor.s.height; j++) {
            count += (Floor.s.mines[x, j]!=null)?1:0;
        }
        numbers[x, y].GetComponent<Number>().setNum(count);
    }
    protected override void Start() {
        base.Start();
    }
}
