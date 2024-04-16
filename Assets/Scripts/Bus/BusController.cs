using System.Collections.Generic;
using UnityEngine;

public class BusController : MonoBehaviour
{
    public Rigidbody rigidbody;
    public List<Tile> collisionTileList = new List<Tile>();

    public Node currentNode;

    public Bus bus;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        currentNode = null;
    }

    public void ItemSelected()
    {
        bus.isSelected = true;

        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX |
                                RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        rigidbody.transform.position = new Vector3(rigidbody.transform.position.x,
                                                 .25f,
                                                  rigidbody.transform.position.z);

        collisionTileList.Clear();
    }
    public void ItemDropped()
    {
        bus.isSelected = false;

        rigidbody.isKinematic = true;
        rigidbody.constraints = RigidbodyConstraints.None;
        rigidbody.transform.position = new Vector3(rigidbody.transform.position.x,
                                                 0,
                                                 rigidbody.transform.position.z);


        bus.SetObjectTileAfterDropped(collisionTileList);
        bus.SetObjectPosition(currentNode);
    }

    public void OnTriggerEnter(Collider collision)
    {
        var tile = collision.gameObject.GetComponent<Tile>();

        if (tile)
            collisionTileList.Add(tile);
    }
    public void OnTriggerExit(Collider collision)
    {
        var tile = collision.gameObject.GetComponent<Tile>();

        if (tile && collisionTileList.Contains(tile))
            collisionTileList.Remove(tile);
    }
    public void SetCurrentNode(Node node)
    {
        SetCurrentNodeToNull();
        currentNode = node;

        SetNode(node);

        if (bus is LongBus)
            SetNode(GridManager.GetNextNodeForLongBus(currentNode, bus.direction));
        
    }
    private void SetNode(Node node)
    {
        node.SetCurrentBus(bus);
    }
    public void SetCurrentNodeToNull()
    {

        if (currentNode != null)
            currentNode.SetCurrentBus(null);

        if (bus is LongBus)
            if (currentNode != null)
                GridManager.GetNextNodeForLongBus(currentNode,bus.direction).SetCurrentBus(null);

    }

}
