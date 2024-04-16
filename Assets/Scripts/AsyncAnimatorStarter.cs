using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsyncAnimatorStarter : MonoBehaviour
{
    public void OnEnable()
    {
        var animator = GetComponent<Animator>();
        if (animator!=null)
        {
            animator.enabled = false;
            Run.After(Random.Range(0f, 2f), () =>
            {
                animator.enabled = true;
            });
        }
    }
}
