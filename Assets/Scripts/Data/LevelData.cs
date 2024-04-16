using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[Serializable]
public class ItemSpecs
{
    public Vector2 position;
    public ColorsEnum colors;
    public BusDirection direction;
    public BusType busType;
    public int numberOfPassanger;
    public bool isStaticBus;

    public ItemSpecs(Vector2 v,int passanger,BusDirection direction, BusType busType, ColorsEnum colors, bool isStaticBus)
    {
        position = v;
        numberOfPassanger = passanger;
        this.direction = direction;
        this.busType = busType;
        this.colors = colors;
        this.isStaticBus = isStaticBus;
    }
}
[Serializable]
public class ObstacleSpecs
{
    public Vector2 position;
    public bool isGroundObstacle;

    public ObstacleSpecs(Vector2 position, bool isGroundObstacle)
    {
        this.position = position;
        this.isGroundObstacle = isGroundObstacle;
    }
}

[Serializable]
public class PassangerClass
{
    public ColorsEnum color;
    public int count;

    public PassangerClass(ColorsEnum color, int number)
    {
        this.color = color;
        this.count = number;
    }
}

[Serializable]
public class PassangerCheckClass
{
    public ColorsEnum color;
    public int currentNumber;
    public int targetNumber;

    public PassangerCheckClass(ColorsEnum color, int currentNumber, int targetNumber)
    {
        this.color = color;
        this.currentNumber = currentNumber;
        this.targetNumber = targetNumber;
    }
}


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelData", order = 0)]
public class LevelData : ScriptableObject
{
    public int graphWidth;
    public int graphHeight;

    public List<ItemSpecs> Items = new List<ItemSpecs>();
    public List<ObstacleSpecs> Obstacles = new List<ObstacleSpecs>();
 //   public List<ColorsEnum> passangers = new List<ColorsEnum>();

    public List<PassangerClass> passangers = new List<PassangerClass>();
    public List<PassangerCheckClass> passangerChecks = new List<PassangerCheckClass>();


    public Vector3 cameraPos;
    public Vector3 cameraAngle;
    public float cameraSize;



    private void OnValidate()
    {
     /*   if (passangers.Count <= 0)
        {*/
            passangerChecks.Clear();
//            passangers2.Clear();

            foreach (var item in Items)
            {
                AddItemToPassangerCheckList(item);
            }

            foreach (var item in passangers)
            {
                AddCurrentPassangerCheckList(item);
            }
      //  }


    }
    private void AddItemToPassangerCheckList(ItemSpecs item)
    {
        foreach (var child in passangerChecks)
        {
            if (child.color==item.colors)
            {
                if (item.busType == BusType.Short)
                    child.targetNumber += (6 - item.numberOfPassanger);
                else
                    child.targetNumber += (12 - item.numberOfPassanger);

                return;
            }
        }
        int i = 0;

        if (item.busType == BusType.Short)
            i += (6 - item.numberOfPassanger);
        else
            i += (12 - item.numberOfPassanger);

        passangerChecks.Add(new PassangerCheckClass(item.colors,0,i));
    }
    private void AddCurrentPassangerCheckList(PassangerClass item)
    {
        foreach (var child in passangerChecks)
        {
            if (child.color == item.color)
            {
                child.currentNumber += item.count;
                return;
            }
        }
      /*  int i = 0;

        if (item.busType == BusType.Short)
            i += (6 - item.numberOfPassanger);
        else
            i += (12 - item.numberOfPassanger);

        passangerChecks.Add(new PassangerCheckClass(item.colors, 0, i));*/
    }


}