using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static GameEvents;
using Lean.Pool;

[Serializable]
public class LevelDataJSON
{
    public int id;
    public string level;
    public int time;
    public int difficulty;

    public LevelDataJSON(int id, string level, int time, int difficulty)
    {
        this.id = id;
        this.level = level;
        this.time = time;
        this.difficulty = difficulty;
    }
}
[Serializable]
public class LevelJSON
{
    public List<LevelDataJSON> levelDatas;
    public string LevelOrder;
    public string LoopLevelOrder;

    public LevelJSON(List<LevelDataJSON> levelDatas, string loopLevelData, string levelOrder)
    {
        this.levelDatas = levelDatas;
        this.LoopLevelOrder = loopLevelData;
        this.LevelOrder = loopLevelData;
    }
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public QueueManager queueManager;

    private EventBus _eventBus;

    public List<LevelData> levelsList = new List<LevelData>();
    public int level;
    private int time;
    private int currentLevelDifficulty;

    public float xOffset,yOffset;

    public bool isTryLevel;
    public LevelData testLevel;

    public List<LevelDataJSON> levelDataList;
    public string[] loopLevelOrder;
    public string[] levelOrder;


    public GameObject tileObject;
    public GameObject shortBus,longBus;
    public GameObject obstacleObject;
    public GameObject groundObstacleObject;
    public GameObject busStation;
    public GameObject fakeShortBus;
    public GameObject fakeLongBus;
    public GameObject tollGate;
    public Transform levelParent;
    public Transform spline;

    public Coroutine carCompletedCoroutine;

    public void Initialize()
    {

        _eventBus = ServiceLocator.Instance.Resolve<EventBus>();

        Instance = this;

        levelDataList = RemoteManager.instance.levelData.levelDatas;
        loopLevelOrder = RemoteManager.instance.levelData.LoopLevelOrder.Split(',');
        levelOrder = RemoteManager.instance.levelData.LevelOrder.Split(',');

        queueManager.Initialize();

        Run.After(2f, () =>
        {
            LoadNextLevel();
        });
        
    }
    public void LoadNextLevel()
    {
        queueManager.StopAllCoroutines();
        StopAllCoroutines();
        ClearLevel();

        if (isTryLevel)
        {
            SpawnLevelObjects(testLevel);
            _eventBus.Fire(new GameEvents.OnLevelLoaded(level,999999999,0));
            return;
        }


        level = PlayerPrefs.GetInt("Level", 0);
        LevelData currentLevelData = ParseLevelRemote(level);

        SpawnLevelObjects(currentLevelData);

        _eventBus.Fire(new GameEvents.OnLevelLoaded(level,time,currentLevelDifficulty));
    }
    private LevelData ParseLevelRemote(int levelNo)
    {
        currentLevelDifficulty = 0;
        time = 180;

        if (levelNo >= levelsList.Count)
            return levelsList[level%levelsList.Count];
        else
            return levelsList[level];
    }
    private void ClearLevel()
    {
        if (Application.isPlaying)
            LeanPool.DespawnAll();
        else
        {
            foreach (Transform item in levelParent)
            {
                Destroy(item.gameObject);
            }
        }
       
    }
    private void SpawnLevelObjects(LevelData currentLevelData)
    {
        GridManager.CreateGrid(currentLevelData.graphWidth, currentLevelData.graphHeight);
        CreateTileItems(currentLevelData);
        CreateBuses(currentLevelData);
        CreateObstacles(currentLevelData);
        CreateBusStation();
        SetCamera(currentLevelData);
        queueManager.SpawnPassangers(currentLevelData);
      
    }
    public void CreateTileItems(LevelData levelData)
    {
        int matCounter = 0;
        Tile tile = null;
        Vector3 tilePosition = Vector3.zero;

        for (int i = 0; i < GridManager.gridGraph.Width; i++)
        {
            for (int j = 0; j < GridManager.gridGraph.Height; j++)
            {
                tilePosition = new Vector3(i * xOffset, 0, j * yOffset);

                tile = LeanPool.Spawn(tileObject, tilePosition, Quaternion.identity).GetComponent<Tile>();
                tile.name = i + " " + j;
                tile.transform.SetParent(levelParent);

                GridManager.gridGraph.Grid[i, j].tileObject = tile.gameObject;
                GridManager.gridGraph.Grid[i, j].worldPosition = tilePosition;

                tile.SetTileSpecs(i, j);


                if (i%2==0)
                    tile.SetMaterial(j % 2);
                else
                {
                    if (j%2==0)
                        tile.SetMaterial(1);
                    else
                        tile.SetMaterial(0);
                }
            }
        }
    }
    private void CreateBuses(LevelData levelData)
    {
        foreach (var item in levelData.Items)
        {
            if (item.busType==BusType.Short)
                SpawnShortBus(item);
            else
            {
                SpawnLongBus(item);
            }
            
        }
    }
    private void SpawnShortBus(ItemSpecs item)
    {
        int posX = (int)item.position.x;
        int posY = (int)item.position.y;

        float zOffset = 0;

       /* if (item.direction==BusDirection.Horizontal)
        {
            zOffset = .55f;
        }*/

        Bus itemObject = LeanPool.Spawn(shortBus, new Vector3(posX * xOffset, 0, posY * yOffset-zOffset)
            , Quaternion.identity).GetComponent<Bus>();

        itemObject.transform.SetParent(levelParent);


        GridManager.gridGraph.Grid[posX, posY].tileType = TileType.Bus;
        GridManager.gridGraph.Grid[posX, posY].currentBus = itemObject;

        itemObject.SetItem(posX, posY, item.colors, item.numberOfPassanger, item.direction,item.isStaticBus);
    }
    private void SpawnLongBus(ItemSpecs item)
    {
        int firstposX = (int)item.position.x;
        int firstposY = (int)item.position.y;

        int secondPosY = 0;
        int secondPosX = 0;

        if (item.direction==BusDirection.Vertical)
        {
            secondPosX = (int)item.position.x ;
            secondPosY = (int)item.position.y +1;
        }
        else
        {
            secondPosX = (int)item.position.x+1;
            secondPosY = (int)item.position.y;
        }

        float posX = (firstposX * xOffset + secondPosX * xOffset) / 2f;
        float posY = (firstposY * yOffset + secondPosY * yOffset) / 2f;

        float zOffset = 0;

        if (item.direction == BusDirection.Horizontal)
        {
            zOffset = .55f;
        }

        Bus itemObject = LeanPool.Spawn(longBus, new Vector3(posX, 0, posY-zOffset)
            , Quaternion.identity).GetComponent<Bus>();

        itemObject.transform.SetParent(levelParent);

        GridManager.gridGraph.Grid[firstposX, firstposY].tileType = TileType.Bus;
        GridManager.gridGraph.Grid[secondPosX, secondPosY].tileType = TileType.Bus;
        GridManager.gridGraph.Grid[firstposX, firstposY].currentBus = itemObject;
        GridManager.gridGraph.Grid[secondPosX, secondPosY].currentBus = itemObject;

        itemObject.SetItem(firstposX, firstposY, item.colors, item.numberOfPassanger, item.direction,item.isStaticBus);
    }
    private void CreateObstacles(LevelData levelData)
    {
        GameObject obj;

        foreach (var item in levelData.Obstacles)
        {
            int posX = (int)item.position.x;
            int posY = (int)item.position.y;

            if (item.isGroundObstacle)
                obj = groundObstacleObject;
            else
                obj = obstacleObject;

           var obstacle= LeanPool.Spawn(obj, new Vector3(posX * xOffset, 0, posY * yOffset)
                , Quaternion.identity).GetComponent<Obstacle>();

            if (!item.isGroundObstacle)
                obstacle.transform.eulerAngles = new Vector3(-90,0,0);

            obstacle.SetObstacle(posX,posY);
            obstacle.transform.SetParent(levelParent);

            if (item.isGroundObstacle)
                GridManager.gridGraph.Grid[posX, posY].tileType = TileType.GroundObstacle;
            else
                GridManager.gridGraph.Grid[posX, posY].tileType = TileType.Obstacle;
        }
    }
    private void CreateBusStation()
    {
        float posX = (GridManager.GetNode(0, 0).worldPosition.x + GridManager.GetNode(GridManager.gridWidth - 1, 0).worldPosition.x) / 2f;
        float posZ = GridManager.GetNode(0, GridManager.gridHeight-1).worldPosition.z+8.5f ;

        var station= LeanPool.Spawn(busStation, new Vector3(posX,0,posZ),Quaternion.identity) ;

        station.transform.SetParent(levelParent);
    }

    public FakeBus SpawnFakeShortBus(Vector3 pos,Vector3 rot,int numberOfPassanger,ColorsEnum colors)
    {
       var fakeBus= LeanPool.Spawn(fakeShortBus).GetComponent<FakeBus>();

       fakeBus.transform.localEulerAngles = rot;
       fakeBus.transform.position = pos;

       fakeBus.SetFakeBus(colors,numberOfPassanger);

       return fakeBus;

    }
    public FakeBus SpawnFakeLongBus(Vector3 pos, Vector3 rot, int numberOfPassanger, ColorsEnum colors)
    {
        var fakeBus = LeanPool.Spawn(fakeLongBus).GetComponent<FakeBus>();

        fakeBus.transform.localEulerAngles = rot;
        fakeBus.transform.position = pos;

        fakeBus.SetFakeBus(colors, numberOfPassanger);

        return fakeBus;
    }
    public void SpawnToolgate(Vector3 pos)
    {
        var gate = LeanPool.Spawn(tollGate);
        gate.transform.localPosition = pos;
        gate.transform.SetParent(levelParent);
    }
    private void SetCamera(LevelData data)
    {
        Camera.main.transform.localPosition = data.cameraPos;
        Camera.main.transform.localEulerAngles = data.cameraAngle;
        Camera.main.orthographicSize = data.cameraSize;
    }
    public IEnumerator LevelCompletedCarEffect()
    {
        yield return new WaitForSeconds(.2f);

        for (int j = 0; j < GridManager.gridHeight; j++)
        {
            for (int i = GridManager.gridWidth - 1; i >= 0; i--)
            {
                if (GridManager.GetNode(i, j)!=null && (GridManager.GetNode(i, j).currentBus!=null))
                 {
                     Bus currentBus = GridManager.GetNode(i, j).currentBus;
                     var path= GridManager.MakePath(currentBus);

                     StartCoroutine(currentBus.BusMoveCoroutine(path));
                     yield return new WaitForSeconds(.25f);
                 }
                
            }
        }
    }

}
