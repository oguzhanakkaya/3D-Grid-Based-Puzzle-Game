using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Dreamteck.Splines;
using Lofelt.NiceVibrations;
using Lean.Pool;

public class QueueManager : MonoBehaviour
{
    public static QueueManager instance;

    private EventBus _eventBus;

    public Queue<Passanger> passangerQueue = new Queue<Passanger>();

    public Passanger passangerObject;

    private Vector3 passangersPivotPos;

    public float passangerOffset;

    public float moveSpeed;
    public float queueMoveSpeed;


    private float splinePos = 1.75f;

    public Transform spline;

    private bool isTimeOver;
    private bool isPassangerOperationContinue;
    private bool isQueueNotAvailable;
    private Passanger currentOperationPassanger;


    public void Initialize()
    {
        instance = this;

        _eventBus = ServiceLocator.Instance.Resolve<EventBus>();
        _eventBus.Subscribe<GameEvents.OnItemUnclicked>(OnItemUnclicked);
        _eventBus.Subscribe<GameEvents.OnTimeOver>(OnTimeOver);
        _eventBus.Subscribe<GameEvents.OnLevelLoaded>(OnLevelLoaded);

    }
    private void OnTimeOver()
    {
        isTimeOver = true;
        LevelFailed();
    }
    private void OnLevelLoaded()
    {
        isQueueNotAvailable = false;
        isPassangerOperationContinue = false;
        isTimeOver = false;
        TryGetPassangerFromQueue();
    }
    private void SetSpline()
    {
        if (spline==null)
            spline = LevelManager.Instance.transform.GetChild(0);


        spline.transform.position = GridManager.GetNode(GridManager.gridWidth-1, GridManager.gridHeight-1)
                                                        .worldPosition + Vector3.forward * 6.5f;
    }

    public void SpawnPassangers(LevelData levelData)
    {
        passangerQueue.Clear();
        SetSpline();

        var lastGridObjectPos = GridManager.GetNode(levelData.graphWidth - 1, levelData.graphHeight - 1).worldPosition;
        passangersPivotPos = lastGridObjectPos+Vector3.forward*5f;

        float x = 0;
        foreach (var passanger in levelData.passangers)
        {
            for (int i = 0; i < passanger.count; i++)
            {
                var pass = LeanPool.Spawn(passangerObject, passangersPivotPos, Quaternion.identity).GetComponent<Passanger>();
                passangersPivotPos += Vector3.forward * passangerOffset;

                pass.transform.SetParent(LevelManager.Instance.levelParent);

                pass.transform.DOLookAt(lastGridObjectPos, 0);
                pass.SetColor(passanger.color);

                pass.GetComponent<SplinePositioner>().spline = spline.GetChild(0).GetComponent<SplineComputer>();
                pass.SetDistance(x);

                x += splinePos;

                passangerQueue.Enqueue(pass);
            }
        }
    }

    private void OnItemUnclicked()
    {
        if (isQueueNotAvailable)
            return;

        TryGetPassangerFromQueue();
    }
    private void TryGetPassangerFromQueue()
    {
        if (passangerQueue.Count <= 0)
            return;


        var passanger = passangerQueue.Peek();
        var path = GridManager.MakePath(passanger);
        var canTargetLastTile = GridManager.CanHaveTargetLastTile(passanger);

        if (canTargetLastTile!=null)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);

            PassangerOperationStarted(passanger);

            passangerQueue.Dequeue();
            passanger.splinePositioner.enabled = false;
            StartCoroutine(MoveFromPath(canTargetLastTile, passanger,passangerQueue.Count <= 0));
            AdjustQueue();
        }
        else if (path!=null && path.Count>0)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);

            PassangerOperationStarted(passanger);

            passangerQueue.Dequeue();
            passanger.splinePositioner.enabled = false;

            StartCoroutine(MoveFromPath(path,passanger, passangerQueue.Count <= 0));
            AdjustQueue();
        }
        else
        {
            isQueueNotAvailable = false;
        }
    }
    private void AdjustQueue()
    {
        foreach (var item in passangerQueue)
            item.SubstractDistance(splinePos, queueMoveSpeed);


        StartCoroutine(StartGetPassangerCoroutine());
      /*  Run.After(queueMoveSpeed, () =>
        {
            TryGetPassangerFromQueue();
        });

        */
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
           // TryGetPassangerFromQueue();
        }
    }
    private IEnumerator StartGetPassangerCoroutine()
    {
        yield return new WaitForSeconds(queueMoveSpeed);
        TryGetPassangerFromQueue();
    }
    private IEnumerator MoveFromPath(List<Node> path,Passanger passanger,bool isLastPassanger)
    {
        passanger.PlayRunAnim();
        path[path.Count - 1].currentBus.IncreaseNumberOfPassanger();
        path[path.Count - 1].currentBus.PassangerComing(passanger);

        float offset = 0;
        float moveSpeedOffset = 0;

        for (int i = 0; i < path.Count; i++)
        {
            if (i == 0) offset = .2f;
            else offset = 0f;


            if (i == path.Count-1) moveSpeedOffset = -.1f;
            else moveSpeedOffset = 0f;

            if (i==path.Count-1)
            {
                path[i].currentBus.BusDoorOpen();
                passanger.transform.DOMove(path[i].currentBus.GetDoorPosition(), moveSpeed + offset).SetEase(Ease.Linear);
            }
            else
                passanger.transform.DOMove(path[i].worldPosition, moveSpeed + offset).SetEase(Ease.Linear);

            passanger.transform.DOLookAt(path[i].worldPosition, 0);

            yield return new WaitForSeconds(moveSpeed+ moveSpeedOffset + offset);
        }

        Destroy(passanger.gameObject);

        path[path.Count - 1].currentBus.ShowNextPassanger();
        path[path.Count - 1].currentBus.PassangerArrived(passanger);
        PassangerArrived(passanger);


        // yield return new WaitForSeconds(.5f);

        if (isLastPassanger)
            _eventBus.Fire(new GameEvents.OnLevelCompleted());

    }
    private void LevelFailed()
    {
        GameManager.Instance.SetInputManager(false);

        if (!isPassangerOperationContinue)
            _eventBus.Fire(new GameEvents.OnLevelFailed());
        else
        {
            StartCoroutine(LevelFailedCoroutine());
        }
    }
    private IEnumerator LevelFailedCoroutine()
    {
        yield return new WaitUntil(()=>!isPassangerOperationContinue);
        _eventBus.Fire(new GameEvents.OnLevelFailed());

    }
    public void PassangerBoosterClicked()
    {
        var passanger = passangerQueue.Peek();
        passangerQueue.Dequeue();

        bool isLastPassanger = passangerQueue.Count <= 0;


        passanger.splinePositioner.enabled = false;

        var target = GridManager.GetTarget(passanger);
        var targetPos = target[0].worldPosition;

        passanger.transform.DOJump(targetPos, 22.5f, 1, .75f).SetEase(Ease.Linear).OnStart(() =>
        {
            target[0].currentBus.IncreaseNumberOfPassanger();
        })
            .OnComplete(() =>
        {
            Destroy(passanger.gameObject);
            target[0].currentBus.ShowNextPassanger();

            if (isLastPassanger)
                _eventBus.Fire(new GameEvents.OnLevelCompleted());
        });

        
        AdjustQueue();

    }
    private void PassangerOperationStarted(Passanger passanger)
    {
        isQueueNotAvailable = true;
        isPassangerOperationContinue = true;
        currentOperationPassanger = passanger;
    }
    private void PassangerArrived(Passanger passanger)
    {
        if (passanger==currentOperationPassanger)
            isPassangerOperationContinue = false;
    }
}
