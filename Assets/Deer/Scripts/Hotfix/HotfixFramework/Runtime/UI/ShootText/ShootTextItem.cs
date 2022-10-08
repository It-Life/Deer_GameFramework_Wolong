// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-09-23 16-18-58
//修改作者:AlanDu
//修改时间:2022-09-23 16-18-58
//版 本:0.1 
// ===============================================
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
public class ShootTextItem : MonoBehaviour
{
    public CanvasGroup canvasGroup = null;
    public RectTransform rectTransform = null;

    public List<RectTransform> childTransformGroup = new List<RectTransform>();

    public List<Vector2> sizeDeltaGroup = new List<Vector2>();

    #region From ShootTextInfo
    public string content;
    public TextAnimationType animationType;
    public TextMoveType moveType;
    public double delayMoveTime;
    public int size;
    public Transform cacheTranform;
    public double initializedVerticalPositionOffset;
    public double initializedHorizontalPositionOffset;
    #endregion

    public double fadeCurveTime;

    public double xMoveOffeset;
    public double yMoveOffeset;

    public bool isMove = false;
    public void SetInfo(ShootTextInfo shootTextInfo)//执行顺序优于Start
    {
        content = shootTextInfo.content;
        animationType = shootTextInfo.animationType;
        moveType = shootTextInfo.moveType;
        delayMoveTime = Time.time + shootTextInfo.delayMoveTime;
        cacheTranform = shootTextInfo.cacheTranform;
        initializedHorizontalPositionOffset = shootTextInfo.initializedHorizontalPositionOffset;
        initializedVerticalPositionOffset = shootTextInfo.initializedVerticalPositionOffset;
    }
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        childTransformGroup = transform.GetComponentsInChildren<RectTransform>().ToList();
        childTransformGroup.Remove(childTransformGroup[0]);

        for (int i = 0; i < childTransformGroup.Count; i++)
        {
            sizeDeltaGroup.Add(childTransformGroup[i].sizeDelta);
        }
        int state = animationType == TextAnimationType.Normal ? 1 : 2;
        if (animationType == TextAnimationType.Normal)
        {
            transform.DOScale(new Vector3(0.7f,0.7f,1f),0f).OnComplete(delegate
            {
                transform.DOScale(new Vector3(1,1,1),1f);
            });
        }
        else
        {
            transform.DOScale(new Vector3(1.3f,1.3f,1f),0.7f).OnComplete(delegate
            {
                transform.DOScale(new Vector3(1,1,1),1f);
            });
        }
    }

    public void ChangeScale(double scale)
    {
        for (int i = 0; i < childTransformGroup.Count; i++)
        {
            Vector2 sizeDelta = sizeDeltaGroup[i];
            sizeDelta.x = sizeDelta.x * (float)scale;
            sizeDelta.y = sizeDelta.y * (float)scale;
            childTransformGroup[i].sizeDelta = sizeDelta;
        }
    }

    public void ChangeAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
    }
}