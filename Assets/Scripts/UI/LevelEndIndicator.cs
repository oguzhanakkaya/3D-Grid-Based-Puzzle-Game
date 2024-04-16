using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelEndIndicator : MonoBehaviour
{
    public LevelCompletedUI levelCompletedUI;

    private void Awake()
    {
        transform.parent.DOLocalRotate(new Vector3(0, 0, 89), 1.25f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var multp = other.GetComponent<LevelCompletedMultiplier>();

        if (multp)
        {
            multp.transform.GetChild(0).transform.localScale = Vector3.one*1.25f;
            levelCompletedUI.IndicatorChanged(multp.multiplier);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        var multp = other.GetComponent<LevelCompletedMultiplier>();

        if (multp)
        {
            multp.transform.GetChild(0).transform.localScale = Vector3.one ; 
        }
    }
}
