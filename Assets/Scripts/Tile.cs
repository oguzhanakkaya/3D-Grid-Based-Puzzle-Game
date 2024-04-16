using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x, y;
    public Material mat1, mat2;

    public void SetTileSpecs(int a,int b)
    {
        x = a;
        y = b;
    }
    public Node GetNode()
    {
        return GridManager.GetNode(x,y);
    }

    public void SetMaterial(int i)
    {
        if (i == 0)
            transform.GetChild(0).GetComponent<Renderer>().material = mat1;
        else
            transform.GetChild(0).GetComponent<Renderer>().material = mat2;

    }
}
