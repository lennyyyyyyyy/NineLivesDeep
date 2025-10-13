using UnityEngine;

class Manhattan : Map {
    public override void OnDiscover(int x, int y) {
        int min = -1; 
        for (int i=0; i<Floor.s.width; i++) {
            for (int j=0; j<Floor.s.height; j++) {
                if (Floor.s.GetUniqueMine(i, j) != null) {
                    if (min == -1) {
                        min = Mathf.Abs(i-x) + Mathf.Abs(j-y);
                    } else {
                        min = Mathf.Min(min, Mathf.Abs(i-x) + Mathf.Abs(j-y));
                    }
                }
            }
        }
        numbers[x, y].GetComponent<Number>().setNum(min);
    }
    protected override void Start() {
        base.Start();
    }
}
