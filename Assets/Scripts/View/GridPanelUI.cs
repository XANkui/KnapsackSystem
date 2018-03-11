using UnityEngine;
using System.Collections;

public class GridPanelUI : MonoBehaviour
{
    public Transform[] Grids;

    public Transform GetEmptyGrid()
    {
        for (int i = 0; i < Grids.Length; i++)
        {
            if (Grids[i].childCount == 0)
                return Grids[i];
        }
        return null;
    }
}

