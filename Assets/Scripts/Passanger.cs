using System.Collections;
using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using DG.Tweening;

public class Passanger : MonoBehaviour,IColorable
{
    public ColorsEnum color;
    public Tile currentTile;

    public Renderer renderer;

    private Animator animator;

    public SplinePositioner splinePositioner;
    public float distance;

    public Sequence sequence;

    private bool isRunning;

    public void Awake()
    {
        animator = GetComponent<Animator>();

        sequence = DOTween.Sequence();
    }
    private void OnEnable()
    {
     //   splinePositioner.enabled = true;
    }

    private void OnDisable()
    {
        PlayIdleAnim();
        sequence.Kill();
        DOTween.Kill(this.transform);
        sequence.Kill();
       // DOTween.KillAll();
        splinePositioner.enabled = true;

    }

    public void SetColor(ColorsEnum clr)
    {
        color = clr;

        renderer.material.color = ColorManager.instance.GetMaterialFromColor(color).color;
    }
    public void PlayIdleAnim()
    {
        animator.Play("Idle");
    }
    public void PlayRunAnim()
    {
        isRunning = true;
        animator.Play("Run");
    }
    public void SetDistance(float f)
    {
        distance = f;
        splinePositioner.SetDistance(f);
    }
    public void SubstractDistance(float f,float time)
    {
        float y = distance - f;

        sequence.Append(DOTween.To(() => distance, x => distance = x, y, time/* - .001f*/).SetEase(Ease.Linear)
            .OnStart(() =>
            {

                PlayRunAnim();
            })
            .OnUpdate(() =>
            {
                splinePositioner.SetDistance(distance);
            })
        .OnComplete(() =>
        {
            isRunning = false;

            Run.After(.05f, () =>
            {
                if (!isRunning && splinePositioner != null && splinePositioner.enabled == true)
                    PlayIdleAnim();
            });

        }));

       
      //  distance -= f;
       
    }

}
