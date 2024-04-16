using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using System.Collections;
using Lofelt.NiceVibrations;

public abstract class Bus : MonoBehaviour,IColorable
{
    public ColorsEnum color;
    public BusDirection direction;

    public MeshRenderer busBody,busDoorR,busDoorL;

    public BusController busController;

    public int numberOfPassanger;

    public List<Renderer> fakePassangerRenderers = new List<Renderer>();

    private Vector3 bodyStartPos;

    private float busBodyAnimSize = .05f;

    public bool isStaticBus;
    public bool canMove;
    private bool isDoorOpen;
    public bool isSelected;

    private float rightDoorOriginalPos, leftDoorOriginalPos;

    private Passanger currentWaitingPassanger;

    [SerializeField]private Transform doorPos;


    public void SetItem(int posX, int posY, ColorsEnum color, int numberOfPassanger,BusDirection direction, bool isStaticBus)
    {
        rightDoorOriginalPos = busDoorR.transform.localPosition.z;
        leftDoorOriginalPos = busDoorL.transform.localPosition.z;

        this.direction = direction;
        this.numberOfPassanger = numberOfPassanger;
        this.isStaticBus = isStaticBus;

        canMove = true;

        busController.SetCurrentNode(GridManager.gridGraph.Grid[posX, posY]);
        SetDirection(direction);
        SetDisableAllPassangers();
        SetColor(color);
        SetNumberOfPassanger(numberOfPassanger);

        Run.After(Random.Range(0f, 2f), () =>
        {
            if (Application.isPlaying)
            {
                BusIdleAnim();
            }
           
        });
    }
    public void OnDisable()
    {
        StopAllCoroutines();

        DOTween.Kill(transform);
        DOTween.Kill(busDoorR.transform);
        DOTween.Kill(busDoorL.transform);

        busDoorR.transform.localPosition = new Vector3(busDoorR.transform.localPosition.x, busDoorR.transform.localPosition.y, rightDoorOriginalPos);
        busDoorL.transform.localPosition = new Vector3(busDoorL.transform.localPosition.x, busDoorL.transform.localPosition.y, leftDoorOriginalPos);


    }
    public virtual void SetDirection(BusDirection direction)
    {
        this.direction = direction;

        if (direction == BusDirection.Vertical)
            transform.localEulerAngles = new Vector3(0,180,0);
        else
            transform.localEulerAngles = new Vector3(0, 90, 0);

    }
    public virtual void SetColor(ColorsEnum color)
    {
        this.color = color;
    }
    public void SetNumberOfPassanger(int numberOfPassanger)
    {
        for (int i = 0; i < numberOfPassanger; i++)
        {
            if (!fakePassangerRenderers[i].transform.parent.gameObject.activeSelf)
            {
                fakePassangerRenderers[i].transform.parent.gameObject.SetActive(true);
                SetFakePassangerAnim(fakePassangerRenderers[i].transform.parent.gameObject);
            }
           
        }
    }
    public void ShowNextPassanger()
    {
        for (int i = 0; i < numberOfPassanger; i++)
        {
            if (!fakePassangerRenderers[i].transform.parent.gameObject.activeSelf)
            {
                fakePassangerRenderers[i].transform.parent.gameObject.SetActive(true);
                fakePassangerRenderers[i].transform.parent.GetChild(2).GetComponent<ParticleSystem>().Play();
                SetFakePassangerAnim(fakePassangerRenderers[i].transform.parent.gameObject);
                BusPassangerAnim();
                break;
            }

        }
    }
    private void SetDisableAllPassangers()
    {
        foreach (var item in fakePassangerRenderers)
        {
            item.transform.parent.gameObject.SetActive(false);
        }
    }
    public void IncreaseNumberOfPassanger()
    {
        numberOfPassanger += 1;
    }
    public void DisableCollider()
    {
        GetComponent<Collider>().enabled = false;
    }
    public Node GetCurrentNode()
    {
        return busController.currentNode;
    }
    private void BusIdleAnim()
    {
        DOTween.Kill(busBody.transform);

        SetBusBodyPosition();

        busBody.transform.DOLocalMoveY(busBody.transform.localPosition.y + busBodyAnimSize, .2f)
            .SetEase(Ease.Linear)
            .SetLoops(-1,LoopType.Yoyo);
    }
    private void BusPassangerAnim()
    {
        DOTween.Kill(busBody.transform);

        SetBusBodyPosition();

        busBody.transform.DOLocalMoveY(busBody.transform.localPosition.y + busBodyAnimSize*5f, .1f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                BusIdleAnim();
            });
    }
    private void SetBusBodyPosition()
    {
        if (bodyStartPos == Vector3.zero)
            bodyStartPos = busBody.transform.localPosition;
        else
            busBody.transform.localPosition = bodyStartPos;
    }
    private void SetFakePassangerAnim(GameObject fakePassanger)
    {
        Vector3 startPos = fakePassanger.transform.localPosition;

        fakePassanger.transform.localPosition += Vector3.up * 1f;
        fakePassanger.transform.DOLocalMoveY(startPos.y, .2f).SetEase(Ease.Linear);
    }
    public void PassangerComing(Passanger passanger)
    {
        canMove = false;
        currentWaitingPassanger = passanger;
    }
    public void PassangerArrived(Passanger passanger)
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);

        if (currentWaitingPassanger==passanger)
        {
            canMove = true;
            BusDoorClose();
        }
            
    }
    public IEnumerator BusMoveCoroutine(List<Node> path)
    {
        if (!canMove)
            yield break;

        canMove = false;
        GetCurrentNode().tileType = TileType.Empty;


        for (int k = 0; k < path.Count; k++)
        {
            transform.DOMove(path[k].worldPosition, .1f).SetEase(Ease.Linear);
            transform.DOLookAt(path[k].worldPosition, 0);

            yield return new WaitForSeconds(.1f);
        }

        transform.DOMove(GridManager.GetNode(GridManager.gridWidth - 1, 0).worldPosition + Vector3.right * 25f, .2f).SetEase(Ease.Linear);
        transform.DOLookAt(GridManager.GetNode(GridManager.gridWidth - 1, 0).worldPosition + Vector3.right * 5f, 0);
    }
    public void BusDoorOpen()
    {
        if (isDoorOpen)
            return;

        isDoorOpen = true;

        DOTween.Kill(busDoorR.transform);
        DOTween.Kill(busDoorL.transform);

        busDoorR.transform.localPosition = new Vector3(busDoorR.transform.localPosition.x, busDoorR.transform.localPosition.y,rightDoorOriginalPos);
        busDoorL.transform.localPosition = new Vector3(busDoorL.transform.localPosition.x, busDoorL.transform.localPosition.y, leftDoorOriginalPos);


        busDoorR.transform.DOLocalMoveZ(busDoorR.transform.localPosition.z + .5f, .2f).SetEase(Ease.Linear);
        busDoorL.transform.DOLocalMoveZ(busDoorL.transform.localPosition.z - .5f, .2f).SetEase(Ease.Linear);
    }
    public void BusDoorClose()
    {
        if (!isDoorOpen)
            return;

        isDoorOpen = false;

        DOTween.Kill(busDoorR.transform);
        DOTween.Kill(busDoorL.transform);

        busDoorR.transform.localPosition = new Vector3(busDoorR.transform.localPosition.x, busDoorR.transform.localPosition.y, rightDoorOriginalPos+.5f);
        busDoorL.transform.localPosition = new Vector3(busDoorL.transform.localPosition.x, busDoorL.transform.localPosition.y, leftDoorOriginalPos-.5f);


        busDoorR.transform.DOLocalMoveZ(rightDoorOriginalPos, .2f).SetEase(Ease.Linear);
        busDoorL.transform.DOLocalMoveZ(leftDoorOriginalPos, .2f).SetEase(Ease.Linear);
    }
    public Vector3 GetDoorPosition()
    {
        return doorPos.position;
    }



    public abstract bool IsItemFull();
    public abstract void SetObjectTileAfterDropped(List<Tile> collisionTileList);
    public abstract void SetObjectPosition(Node node);

#if UNITY_EDITOR
    public void OnValidate()
    {
        if (!Application.isPlaying && !EditorUtility.IsPersistent(this))
        {
            SetColor(color);
            SetDisableAllPassangers();
            SetNumberOfPassanger(numberOfPassanger);
            SetDirection(direction);
        }
    }
#endif
}
