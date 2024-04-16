using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class FakeBus : MonoBehaviour
{
    public List<Renderer> fakePassangerRenderers = new List<Renderer>();

    public List<MeshRenderer> meshes = new List<MeshRenderer>();

    public MeshRenderer busBody, busDoorR, busDoorL, busBody2;

    public void SetFakeBus(ColorsEnum colors,int numberOfPassanger)
    {
        SetDisableAllPassangers();
        SetNumberOfPassanger(numberOfPassanger);
        SetColor(colors);
    }
    private void SetColor(ColorsEnum color)
    {
        Color32 clr = ColorManager.instance.GetMaterialFromColor(color).color;
        clr.a = 64;

        busBody.materials[1].color = clr;
        busDoorR.materials[1].color = clr;
        busDoorL.materials[1].color = clr;

        if (busBody.materials.Length>5)
            busBody.materials[5].color = clr;

        if (busBody2)
            busBody2.materials[0].color = clr;

        foreach (var item in fakePassangerRenderers)
        {
            item.material.color = clr;
        }
    }
    private void SetNumberOfPassanger(int numberOfPassanger)
    {
        for (int i = 0; i < numberOfPassanger; i++)
        {
            fakePassangerRenderers[i].transform.parent.gameObject.SetActive(true);
        }
    }
    private void SetDisableAllPassangers()
    {
        foreach (var item in fakePassangerRenderers)
        {
            item.transform.parent.gameObject.SetActive(false);
        }
    }
}
