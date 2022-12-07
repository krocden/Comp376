//Referred to and adapted code from https://github.com/SpawnCampGames/YT/tree/main/DoomClone/e1m8

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingAngle : MonoBehaviour
{

    private Transform player;
    private SpriteRenderer mSpriteRenderer;
    private Animator mAnimator;
    private Monster mMonster;

    private float angle;

    private Vector3 playerPos;
    private Vector3 playerDir;

    private int lastIndex;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mAnimator = GetComponent<Animator>();
        mMonster = GetComponentInParent<Monster>();

        //mSpriteRenderer;

        SetTierColour(mMonster.GetTier());
    }

    // Update is called once per frame
    void Update()
    {
        mAnimator.SetFloat("spriteView", lastIndex);

        transform.LookAt(player, Vector3.up);

        playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        playerDir = playerPos - transform.position;

        angle = Vector3.SignedAngle(playerDir, transform.parent.forward, Vector3.up);
        //Debug.Log(angle);

        //if index is 1, enable flipx, if index is 2, disable flipx
        if(lastIndex == 1)
        {
            mSpriteRenderer.flipX = true;
        }
        else
        {
            mSpriteRenderer.flipX = false;
        }

        lastIndex = GetIndex(angle);
        //Debug.Log(lastIndex);
    }

    private int GetIndex(float angle)
    {
        //front
        if (angle > -45.0f && angle < 45.0f)
        {
            return 0;
        }
        //right
        if (angle >= 45.0f && angle < 135.0f)
        {
            return 1;
        }
        //left
        if (angle <= -45.0f && angle > -135.0f)
        {
            return 2;
        }
        //back
        if (angle >= 135.0f || angle <= -135.0f)
        {
            return 3;
        }

        return lastIndex;
    }

    private void SetTierColour(int a)
    {
        if(a == 1)
        {
            mSpriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);  //default/uncoloured
        }
        if(a == 2)
        {
            mSpriteRenderer.color = new Color(0.0f, 1.0f, 0.35f, 1.0f); //green
        }
        if(a == 3)
        {
            mSpriteRenderer.color = new Color(1.0f, 0.87f, 0.0f, 1.0f); //yellow
        }
        if(a == 4)
        {
            mSpriteRenderer.color = new Color(1.0f, 0.26f, 0.0f, 1.0f); //orange-red
        }
        if(a == 5)
        {
            mSpriteRenderer.color = new Color(1.0f, 0.0f, 0.73f, 1.0f); //purple
        }
    }
}
