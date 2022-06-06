// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-12-12 12-15-15  
//修改作者 : 杜鑫 
//修改时间 : 2021-12-12 12-15-15  
//版 本 : 0.1 
// ===============================================

using System;
using System.Collections.Generic;
using UGFExtensions.SpriteCollection;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UGUISpriteAnimation : MonoBehaviour
{
    private Image mImageSource;
    private int mCurFrame = 0;
    private float mDelta = 0;
    private bool mIsPlaying = false;
    private Action mPlayEndAciton;
    public float m_FPS = 5;
    public SpriteCollection m_SpriteFrames;
    [Tooltip("设置播放正反")]
    public bool m_Foward = true;
    [Tooltip("设置自动播放")]
    public bool m_AutoPlay = false;
    [Tooltip("设置循环播放")]
    public bool m_Loop = false;
    [Tooltip("设置成原始图片大小")]
    public bool m_IsNativeSize = true;

    public int FrameCount
    {
        get
        {
            return m_SpriteFrames.GetSprites().Count;
        }
    }

    public Sprite GetSpriteByIndex(int nIndex) 
    {
        var sprites = m_SpriteFrames.GetSprites();
        sprites.TryGetValue(nIndex.ToString(), out Sprite sprite);
        return sprite;
    }
    public bool IsPlaying()
    {
        return mIsPlaying;
    }

    void Awake()
    {
        mImageSource = GetComponent<Image>();
    }

    void Start()
    {
        if (m_AutoPlay)
        {
            Play();
        }
        else
        {
            mIsPlaying = false;
        }
    }

    private void SetSprite(int index)
    {
        mImageSource.sprite = GetSpriteByIndex(index);
        if (m_IsNativeSize)
        {
            mImageSource.SetNativeSize();
        }
    }

    public void Play(Action action = null)
    {
        mIsPlaying = true;
        m_Foward = true;
        mPlayEndAciton = action;
    }

    public void PlayReverse(Action action = null)
    {
        mIsPlaying = true;
        m_Foward = false;
        mPlayEndAciton = action;
    }

    void Update()
    {
        if (!mIsPlaying || 0 == FrameCount)
        {
            return;
        }

        mDelta += Time.deltaTime;
        if (mDelta > 1 / m_FPS)
        {
            mDelta = 0;
            if(m_Foward)
            {
                mCurFrame++;
            }
            else
            {
                mCurFrame--;
            }

            if (mCurFrame >= FrameCount)
            {
                if (m_Loop)
                {
                    mCurFrame = 0;
                }
                else
                {
                    mPlayEndAciton?.Invoke();
                    mIsPlaying = false;
                    return;
                }
            }
            else if (mCurFrame<0)
            {
                if (m_Loop)
                {
                    mCurFrame = FrameCount-1;
                }
                else
                {
                    mIsPlaying = false;
                    return;
                }          
            }

            SetSprite(mCurFrame);
        }
    }

    public void Pause()
    {
        mIsPlaying = false;
    }

    public void Resume()
    {
        if (!mIsPlaying)
        {
            mIsPlaying = true;
        }
    }

    public void Stop()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        mIsPlaying = false;
    }

    public void Rewind()
    {
        mCurFrame = 0;
        SetSprite(mCurFrame);
        Play();
    }
}