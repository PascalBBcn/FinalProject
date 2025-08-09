using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transitions : MonoBehaviour
{
    public Animator fadeAnimator;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void StartFadeTransition()
    {
        fadeAnimator.ResetTrigger("Start");
        fadeAnimator.SetTrigger("Start");
    }
}
