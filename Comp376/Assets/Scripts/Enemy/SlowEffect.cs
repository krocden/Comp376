using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowEffect : MonoBehaviour
{
    //private Animator mAnimator;
    private SpriteRenderer mSpriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        //mAnimator = GetComponent<Animator>();
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableSlow()
    {
        //gameObject.SetActive(true);
        //mAnimator.enabled = true;
        mSpriteRenderer.enabled = true;
    }

    public void DisableSlow()
    {
        //gameObject.SetActive(false);
        //mAnimator.enabled = false;
        mSpriteRenderer.enabled = false;
    }
}
