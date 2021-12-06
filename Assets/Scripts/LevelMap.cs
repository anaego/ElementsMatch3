using System;

[Serializable]
public class LevelMap
{
    public int Columns = 5;
    public int Rows = 6;
    public ElementType[,] TypeMap;
}
