using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AutoSetAnchorPosForIphonex : MonoBehaviour
{
    public Canvas mCanvas;
    // Use this for initialization
    void Awake()
    {
        if (Screen.width == 1125 && Screen.height == 2436)// if is iphoneX vertical
        {
            CanvasScaler cs = mCanvas.GetComponent<CanvasScaler>();
            float referenceHeight = cs.referenceResolution.y;
            float iphonexSafeAreaHeightOffset = (132f / 2436f) * referenceHeight;
            RectTransform rtf = GetComponent<RectTransform>();
			rtf.offsetMin = new Vector2(0, iphonexSafeAreaHeightOffset);
			rtf.offsetMax = new Vector2(0,-iphonexSafeAreaHeightOffset);
		}
        else if (Screen.height == 1125 && Screen.width == 2436)// if is iphoneX Horizontal
        {
            CanvasScaler cs = mCanvas.GetComponent<CanvasScaler>();
            float referenceHeight = cs.referenceResolution.y;
            float iphonexSafeAreaHeightOffset = (63f / 1125f) * referenceHeight;
            float iphonexSafeAreaWidthOffset = (referenceHeight/1125f)*132f;
            RectTransform rtf = GetComponent<RectTransform>();
            rtf.offsetMin = new Vector2(iphonexSafeAreaWidthOffset, iphonexSafeAreaHeightOffset);
            rtf.offsetMax = new Vector2(-iphonexSafeAreaWidthOffset, 0);
        }
    }
}
