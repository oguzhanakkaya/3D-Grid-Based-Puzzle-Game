using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LongBus : Bus
{
    public MeshRenderer busBody2;
    public BusController busController;

    private MaterialPropertyBlock m_PropertyBlock;

    public override bool IsItemFull()
    {
        return numberOfPassanger >= 12;
    }
    public override void SetDirection(BusDirection direction)
    {
        base.SetDirection(direction);

        if (!Application.isPlaying)
            SetObjectPosition(GetCurrentNode());
    }
    public override void SetColor(ColorsEnum colorEnum)
    {
        base.SetColor(colorEnum);

        Color32 color = Color.white;

        if (Application.isPlaying)
        {
            Color32 passangerColor = ColorManager.instance.GetMaterialFromColor(this.color).color;

            if (!isStaticBus)
                color = ColorManager.instance.GetMaterialFromColor(this.color).color;
            else
                color = Color.gray;

            busBody.materials[2].color = color;
            busDoorR.materials[1].color = color;
            busDoorL.materials[1].color = color;
            busBody2.materials[0].color = color;

            foreach (var item in fakePassangerRenderers)
            {
                item.material.color = passangerColor;
            }
        }
        else
        {
#if UNITY_EDITOR
            if (EditorUtility.IsPersistent(this))
                return;

            color = GameObject.FindObjectOfType<ColorManager>().GetMaterialFromColor(this.color).color;


            m_PropertyBlock = new MaterialPropertyBlock();




            color = GameObject.FindObjectOfType<ColorManager>().GetMaterialFromColor(this.color).color;

            m_PropertyBlock.SetColor("_BaseColor", color);
            busBody.SetPropertyBlock(m_PropertyBlock, 2);
            busDoorR.SetPropertyBlock(m_PropertyBlock, 1);
            busDoorL.SetPropertyBlock(m_PropertyBlock, 1);
            busBody2.SetPropertyBlock(m_PropertyBlock, 0);


            if (isStaticBus)
                m_PropertyBlock.SetColor("_BaseColor", Color.gray);
            else
                m_PropertyBlock.SetColor("_BaseColor", new Color32(34, 34, 75, 255));

            busBody.SetPropertyBlock(m_PropertyBlock, 3);



            foreach (var item in fakePassangerRenderers)
            {
                item.SetPropertyBlock(m_PropertyBlock);
            }

            /*    busBody.sharedMaterials[2].color = mat.color;
                busDoorR.sharedMaterials[1].color = mat.color;
                busDoorL.sharedMaterials[1].color = mat.color;
                busBody2.sharedMaterials[0].color = mat.color;

                foreach (var item in fakePassangerRenderers)
                {
                    item.sharedMaterial.color = mat.color;
                }
            */
#endif
        }

    }
    public override void SetObjectPosition(Node node)
    {
        Node currentNode = GridManager.GetNode((int)node.Position.x, (int)node.Position.y);
        Node nextNode = GridManager.GetNextNodeForLongBus(currentNode,direction);

        var posX = (currentNode.worldPosition.x + nextNode.worldPosition.x) / 2f;
        var posZ = (currentNode.worldPosition.z + nextNode.worldPosition.z) / 2f;


        transform.position = new Vector3(posX,
                                         transform.position.y,
                                         posZ);
    }
    public override void SetObjectTileAfterDropped(List<Tile> collisionTileList)
    {
        if (collisionTileList.Count == 2)
        {

            Tile tile1 = collisionTileList[0];
            Tile tile2 = collisionTileList[1];

            Node node1 = GridManager.GetNode(tile1.x, tile1.y);
            Node node2 = GridManager.GetNode(tile2.x, tile2.y);

            bool node1Available = node1.tileType == TileType.Empty || (node1.tileType == TileType.Bus && node1.currentBus == this);
            bool node2Available = node2.tileType == TileType.Empty || (node2.tileType == TileType.Bus && node2.currentBus == this);


            if (node1Available && node2Available && tile1.y== tile2.y)
                if (tile1.x < tile2.x)
                    busController.SetCurrentNode(GridManager.GetNode(tile1.x, tile1.y));
                else
                    busController.SetCurrentNode(GridManager.GetNode(tile2.x, tile2.y));
            else if(node1Available && node2Available)
                if (tile1.y < tile2.y)
                    busController.SetCurrentNode(GridManager.GetNode(tile1.x, tile1.y));
                else
                    busController.SetCurrentNode(GridManager.GetNode(tile2.x, tile2.y));
        }
        else
        {
            busController.SetCurrentNode(GridManager.GetClosestNodeForLongBus(transform, direction,this));
        }





        /*   if (collisionTileList.Count>0)
           {
               Tile closestTile = collisionTileList[0];

               foreach (var tile in collisionTileList)
               {
                   Node node = GridManager.GetNode(tile.x, tile.y);

                   if ((Vector3.Distance(tile.transform.position, transform.position) <
                       Vector3.Distance(closestTile.transform.position, transform.position)) &&
                       node.tileType == TileType.Empty &&
                       GridManager.GetNextNodeForLongBus(node, direction)?.tileType == TileType.Empty)
                   {
                       closestTile = tile;
                   }
               }
               busController.SetCurrentNode(GridManager.GetClosestNodeForLongBus(transform, direction));

           }
           else
           {

               busController.SetCurrentNode(GridManager.GetClosestNodeForLongBus(transform, direction));

           }

           */
        //  GridManager.GetClosestNodeForLongBus(transform, direction);
        //  busController.SetCurrentNode(GridManager.GetClosestNodeForLongBus(transform,direction));
    }
}
