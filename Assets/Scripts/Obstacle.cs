using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static GameEvents;

public class Obstacle : MonoBehaviour
{
    private EventBus _eventBus;

    public int x, y;

    public bool groundObstacle;

    public List<GameObject> sidewalks = new List<GameObject>();

    private void OnDisable()
    {
        foreach (var item in sidewalks)
        {
            item.gameObject.SetActive(true);
        }
    }

    public void SetObstacle(int x,int y)
    {
        this.x = x;
        this.y = y;

        if (Application.isPlaying && groundObstacle)
        {
            _eventBus = ServiceLocator.Instance.Resolve<EventBus>();
            _eventBus.Subscribe<GameEvents.OnLevelLoaded>(OnLevelLoaded);
        }
      
    }
    private void OnLevelLoaded()
    {
        CheckNeighbours();
    }
    private void CheckNeighbours()
    {
        List<Node> nodes = new List<Node>
        {
            GridManager.GetNode(x, y + 1),
            GridManager.GetNode(x, y - 1),
            GridManager.GetNode(x + 1, y),
            GridManager.GetNode(x - 1, y)
        };

        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i]==null || (nodes[i] != null && nodes[i].tileType==TileType.GroundObstacle))
            {
                sidewalks[i].gameObject.SetActive(false);
            }
        }


       /* Node upNode = ;
        Node downNode = GridManager.GetNode(x, y -1);
        Node rightNode = GridManager.GetNode(x+1, y );
        Node leftNode = GridManager.GetNode(x-1, y );
       */
    }
    
}
