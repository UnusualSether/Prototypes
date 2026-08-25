public class Cell_
{
    public int x, y, z;
    public int g;
    public int h;
    public int f;

    public bool walkable;
    public Cell_ PreveousCell;

    public Cell_(int PosX, int PosY, int PosZ, int gCost, int hCost, Cell_ PreveousCell)
    {
        x = PosX; y = PosY; z = PosZ; g = gCost; h = hCost; f = gCost + hCost; walkable = true;
    }
    public void CalcFCost() {f = g+h;}
    public override string ToString()
    {
        return x + "," + y + "," + z;
    }
}
