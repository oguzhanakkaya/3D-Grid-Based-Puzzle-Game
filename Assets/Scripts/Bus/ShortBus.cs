using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

public class ShortBus : Bus
{
    public BusController busController;

    private MaterialPropertyBlock m_PropertyBlock;


    public override bool IsItemFull()
    {
        return numberOfPassanger >= 6;
    }
    public override void SetObjectPosition(Node node)
    {
       
        transform.position = new Vector3(node.worldPosition.x,
                                         transform.position.y,
                                         node.worldPosition.z);
    }
    public override void SetColor(ColorsEnum colorEnum)
    {
        base.SetColor(colorEnum);

        Color32 colors = Color.white;

        if (Application.isPlaying)
        {
            Color32 passangerColor = ColorManager.instance.GetMaterialFromColor(this.color).color;


            if (!isStaticBus)
                colors = ColorManager.instance.GetMaterialFromColor(this.color).color;
            else
                colors = Color.gray;

            busBody.materials[1].color = colors;
            busDoorR.materials[1].color = colors;
            busDoorL.materials[1].color = colors;

            foreach (var item in fakePassangerRenderers)
            {
                item.material.color = passangerColor;
            }
        }
        else
        {
#if UNITY_EDITOR
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;

            m_PropertyBlock = new MaterialPropertyBlock();




            colors = GameObject.FindObjectOfType<ColorManager>().GetMaterialFromColor(this.color).color;

            m_PropertyBlock.SetColor("_BaseColor", colors);
            busBody.SetPropertyBlock(m_PropertyBlock, 1);
            busDoorR.SetPropertyBlock(m_PropertyBlock, 1);
            busDoorL.SetPropertyBlock(m_PropertyBlock, 1);


            if (isStaticBus)
                m_PropertyBlock.SetColor("_BaseColor", Color.gray);
            else
                m_PropertyBlock.SetColor("_BaseColor", new Color32(34,34,75,255));

            busBody.SetPropertyBlock(m_PropertyBlock, 2);


            foreach (var item in fakePassangerRenderers)
            {
                item.SetPropertyBlock(m_PropertyBlock);
            }

            /*  busBody.sharedMaterials[1].color = mat.color;
                  busDoorR.sharedMaterials[1].color = mat.color;
                  busDoorL.sharedMaterials[1].color = mat.color;

                  foreach (var item in fakePassangerRenderers)
                  {
                      item.sharedMaterial.color = mat.color;
                  }
          }*/
#endif
        }
    }

    public override void SetObjectTileAfterDropped(List<Tile> collisionTileList)
    {
          if (collisionTileList.Count == 0)
          {
              busController.SetCurrentNode(GridManager.GetClosestNodeForShortBus(transform));
          }
          else if (collisionTileList.Count == 1)
          {
              if (collisionTileList[0].GetNode().tileType == TileType.Empty ||
                (collisionTileList[0].GetNode().tileType == TileType.Bus && collisionTileList[0].GetNode().currentBus==this))
                busController.SetCurrentNode(collisionTileList[0].GetNode());
              else
                  busController.SetCurrentNode(GridManager.GetClosestNodeForShortBus(transform));

          }
          else if (collisionTileList.Count > 1)
          {
            /*  Tile closestTile = collisionTileList[0];

              foreach (var tile in collisionTileList)
              {
                  if (Vector3.Distance(tile.transform.position, transform.position) <
                      Vector3.Distance(closestTile.transform.position, transform.position))
                  {
                      closestTile = tile;
                  }
              }

              busController.SetCurrentNode(closestTile.GetNode());


            */
            Node node = GridManager.GetNodeFromTile(collisionTileList[0]);

            foreach (var item in collisionTileList)
            {
                Node currentNode = GridManager.GetNodeFromTile(item);

                bool currentNodeAvailable = currentNode.tileType == TileType.Empty ||
                        (currentNode.tileType == TileType.Bus && currentNode.currentBus == this);

                bool isNodeAvailable = node.tileType == TileType.Empty ||
                       (node.tileType == TileType.Bus && node.currentBus == this);

                if (!isNodeAvailable && currentNodeAvailable)
                {
                    node = currentNode;
                }

                if ((Vector3.Distance(currentNode.worldPosition, transform.position) <
                            Vector3.Distance(node.worldPosition, transform.position)) &&
                            currentNodeAvailable)
                {
                    node = currentNode;
                }
            }

            busController.SetCurrentNode(node);
        }
    }
}

