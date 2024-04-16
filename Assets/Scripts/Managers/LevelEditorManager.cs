using Lean.Pool;
using UnityEngine;

public class LevelEditorManager : MonoBehaviour
{
    public LevelManager levelManager;
    private int width,height;

   public void CreateTileMap(int x, int y)
    {
        width = x;
        height = y;

        GridManager.CreateGrid(width, height);

        Tile tile = null;
        Vector3 tilePosition = Vector3.zero;


        for (int i = 0; i < GridManager.gridGraph.Width; i++)
        {
            for (int j = 0; j < GridManager.gridGraph.Height; j++)
            {
                tilePosition = new Vector3(i * levelManager.xOffset, 0, j * levelManager.yOffset);

                tile = Instantiate(levelManager.tileObject, tilePosition, Quaternion.identity).GetComponent<Tile>();
                tile.name = i + " " + j;
                tile.transform.SetParent(levelManager.levelParent);

                GridManager.gridGraph.Grid[i, j].tileObject = tile.gameObject;
                GridManager.gridGraph.Grid[i, j].worldPosition = tilePosition;

                tile.SetTileSpecs(i, j);

                if (i % 2 == 0)
                    tile.SetMaterial(j % 2);
                else
                {
                    if (j % 2 == 0)
                        tile.SetMaterial(1);
                    else
                        tile.SetMaterial(0);
                }
            }
        }
    }
    public void CreateShortBus(int x , int y, BusDirection direction=BusDirection.Vertical, ColorsEnum colors=ColorsEnum.Orange,int numberOfPassanger=6, bool isStaticBus=false)
    {
        int posX = x;
        int posY = y;

        Bus itemObject = Instantiate(levelManager.shortBus, new Vector3(posX * levelManager.xOffset, 0, posY * levelManager.yOffset)
            , Quaternion.identity).GetComponent<Bus>();

        itemObject.transform.SetParent(levelManager.levelParent);

        GridManager.gridGraph.Grid[posX, posY].tileType = TileType.Bus;
        GridManager.gridGraph.Grid[posX, posY].currentBus = itemObject;

        itemObject.SetItem(posX,posY,colors,numberOfPassanger,direction,isStaticBus);
    }
    public void CreateLongBus(int x, int y, BusDirection direction = BusDirection.Vertical, ColorsEnum colors = ColorsEnum.Orange, int numberOfPassanger = 12, bool isStaticBus=false)
    {
        var newDirection = direction;


        int firstposX = x;
        int firstposY = y;

        int secondPosY = 0;
        int secondPosX = 0;

        if (newDirection == BusDirection.Vertical && (y + 1 >= GridManager.gridHeight || GridManager.GetNode(x,y+1).currentBus))
            newDirection = BusDirection.Horizontal;
           

        if (newDirection == BusDirection.Vertical)
        {
            secondPosX = x;
            secondPosY = y + 1;

        }
        else
        {
            secondPosX = x+1;
            secondPosY = y;
        }


        
        

        float posX = (firstposX * levelManager.xOffset + secondPosX * levelManager.xOffset) / 2f;
        float posY = (firstposY * levelManager.yOffset + secondPosY * levelManager.yOffset) / 2f;

        Bus itemObject = Instantiate(levelManager.longBus, new Vector3(posX, 0, posY)
            , Quaternion.identity).GetComponent<Bus>();

        GridManager.gridGraph.Grid[firstposX, firstposY].tileType = TileType.Bus;
        GridManager.gridGraph.Grid[secondPosX, secondPosY].tileType = TileType.Bus;
        GridManager.gridGraph.Grid[firstposX, firstposY].currentBus = itemObject;
        GridManager.gridGraph.Grid[secondPosX, secondPosY].currentBus = itemObject;

        itemObject.transform.SetParent(levelManager.levelParent);

        itemObject.SetItem(firstposX, firstposY, colors, numberOfPassanger, newDirection, isStaticBus);
    }
    public void CreateObstacle(int x,int y)
    {
        int posX = x;
        int posY = y;

        var obstacle = Instantiate(levelManager.obstacleObject, new Vector3(posX * levelManager.xOffset, 0, posY * levelManager.yOffset)
             , Quaternion.identity).GetComponent<Obstacle>();

        obstacle.transform.eulerAngles = new Vector3(-90, 0, 0);
        obstacle.SetObstacle(posX, posY);
        obstacle.transform.SetParent(levelManager.levelParent);


        GridManager.gridGraph.Grid[posX, posY].tileType = TileType.Obstacle;
    }
    public void CreateGroundObstacle(int x, int y)
    {
        int posX = x;
        int posY = y;

        var obstacle = Instantiate(levelManager.groundObstacleObject, new Vector3(posX * levelManager.xOffset, 0, posY * levelManager.yOffset)
             , Quaternion.identity).GetComponent<Obstacle>();

        obstacle.SetObstacle(posX, posY);
        obstacle.transform.SetParent(levelManager.levelParent);


        GridManager.gridGraph.Grid[posX, posY].tileType = TileType.Obstacle;
    }
    public void ClearAllItem()
    {
        foreach (Transform item in levelManager.levelParent)
        {
            Destroy(item.gameObject);
        }
    }
    public void LoadData(LevelData levelData)
    {
        CreateTileMap(levelData.graphWidth,levelData.graphHeight);

        foreach (var item in levelData.Obstacles)
        {
            if (!item.isGroundObstacle)
                CreateObstacle((int)item.position.x, (int)item.position.y);
            else
                CreateGroundObstacle((int)item.position.x, (int)item.position.y);
        }

        foreach (var item in levelData.Items)
        {
            if (item.busType==BusType.Short)
            {
                CreateShortBus((int)item.position.x, (int)item.position.y,item.direction,item.colors,item.numberOfPassanger,item.isStaticBus);
            }
            else
            {
                CreateLongBus((int)item.position.x, (int)item.position.y,item.direction,item.colors,item.numberOfPassanger,item.isStaticBus);;
            }
           
        }

        CreateBusStation();

        Camera.main.transform.position=levelData.cameraPos;
        Camera.main.transform.localEulerAngles= levelData.cameraAngle ;
        Camera.main.orthographicSize= levelData.cameraSize;
    }
    private void CreateBusStation()
    {
        float posX = (GridManager.GetNode(0, 0).worldPosition.x + GridManager.GetNode(GridManager.gridWidth - 1, 0).worldPosition.x) / 2f;
        float posZ = GridManager.GetNode(0, GridManager.gridHeight - 1).worldPosition.z + 8.5f;

        var station = LeanPool.Spawn(levelManager.busStation, new Vector3(posX, 0, posZ), Quaternion.identity);

        station.transform.SetParent(levelManager.levelParent);
    }
}
