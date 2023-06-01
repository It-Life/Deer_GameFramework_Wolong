using UnityEngine;
using System;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using HotfixFramework.Runtime;
using TMPro;

//NOTE! You should hava a Camera with "MainCamera" tag and a canvas with a Screen Space - Overlay mode to script works properly;

public class HealthBar : MonoBehaviour {

	[ShowInInspector,ReadOnly]
	public float curHealth;
	[ReadOnly]	
	public RectTransform HealthbarPrefab;			//Our health bar prefab;
	public float yOffset = 50f;							//Horizontal position offset;
	public bool keepSize = true;	                //keep distance independed size;
	public float scale = 500;							//Scale of the healthbar;
	public Vector2 sizeOffsets = new Vector2(0, -100);						//Use this to overwright healthbar width and height values;
	public bool DrawOFFDistance;					//Disable health bar if it out of drawDistance;
	public float drawDistance = 10;
	public bool showHealthInfo = true;                     //Show the health info on top of the health bar or not;
	public enum HealthInfoAlignment {top, center, bottom};
	public HealthInfoAlignment healthInfoAlignment = HealthInfoAlignment.center;
	public float healthInfoSize = 10;
    public AlphaSettings alphaSettings;
	//白霆添加
	private Slider healthVolume;         //Health bar images, should be named as "Health" and "Background";
	//白霆注销
	//private Image healthVolume, backGround;			//Health bar images, should be named as "Health" and "Background";
	private TextMeshProUGUI healthInfo;						//Health info, a healthbar's child Text object(should be named as HealthInfo);
	private CanvasGroup canvasGroup;
	private Vector2 healthbarPosition, healthbarSize, healthInfoPosition;
	private Transform CachedTransform;
	private float defaultHealth, lastHealth, camDistance, dist, pos, rate;
	private Camera cam;
	private Transform healthbarRoot;
	private bool CanRemoveObject;
	[HideInInspector]public Canvas canvas;
	private int m_SerialId = 0;
	private RectTransform healthbarRootRect;
    private void OnEnable()
    {
		cam = GameEntry.Camera.MainCamera;
		CachedTransform = this.transform;
		healthbarRoot = GameEntry.UI.GetHealthbarRoot().transform;
		canvas = healthbarRoot.GetComponent<Canvas>();
		healthbarRootRect = healthbarRoot.GetComponent<RectTransform>();
	}
	public void OnInitialized(string strHealthPrefabPath,int defaultHealth, Vector2 healthSizeDelta,int curHealth)
    {
		InitAlphaSettingsInfo();
		this.defaultHealth = defaultHealth;
		this.curHealth = curHealth;
		lastHealth = defaultHealth;
		healthbarSize = healthSizeDelta;
		m_SerialId = GameEntry.AssetObject.LoadGameObject(strHealthPrefabPath, "HealthBar",delegate(bool result, object gameObject,int serialId) {
			if (result)
			{
				HealthbarPrefab = ((GameObject)gameObject).transform.GetComponent<RectTransform>();
				HealthbarPrefab.gameObject.SetActive(true);
				GameEntry.UI.GetHealthbarRoot().AddHealthBar(HealthbarPrefab);
				HealthbarPrefab.position = new Vector2(-1000,-1000);
				HealthbarPrefab.rotation = Quaternion.identity;
				HealthbarPrefab.name = $"HealthBar - {serialId}";
				HealthbarPrefab.SetParent(healthbarRoot, false);
				canvasGroup = HealthbarPrefab.GetComponent<CanvasGroup>();

				//白霆添加
				healthVolume = HealthbarPrefab.GetComponent<Slider>();
				healthInfo = HealthbarPrefab.transform.Find("HealthInfo").GetComponent<TextMeshProUGUI>();
				healthVolume.value = 1;
				//白霆注销
				//healthVolume = HealthbarPrefab.transform.Find("Health").GetComponent<Image>();
				//backGround = HealthbarPrefab.transform.Find("Background").GetComponent<Image>();
				//healthInfo = HealthbarPrefab.transform.Find("HealthInfo").GetComponent<Text>();
				//healthVolume.fillAmount = curHealth / defaultHealth;
				//healthInfo.resizeTextForBestFit = true;
				healthInfo.rectTransform.anchoredPosition = Vector2.zero;
				healthInfoPosition = healthInfo.rectTransform.anchoredPosition;
/*				healthInfo.minHeight = 1;
				healthInfo.resizeTextMaxSize = 500;*/

				canvasGroup.alpha = alphaSettings.fullAplpha;
				canvasGroup.interactable = false;
				canvasGroup.blocksRaycasts = false;
				CanRemoveObject = false;
				IsToZero = false;
			}
			else 
			{
				Logger.Error("加载血条预制体失败");	
			}
		});
	}

	private void InitAlphaSettingsInfo() 
	{
		if (alphaSettings == null)
		{
			alphaSettings = new AlphaSettings();
		}
		alphaSettings.defaultAlpha = 0f;
		alphaSettings.defaultFadeSpeed = 0.1f;
		alphaSettings.fullAplpha = 0f;
		alphaSettings.fullFadeSpeed = 0.1f;
		alphaSettings.nullAlpha = 0f;
		alphaSettings.nullFadeSpeed = 0.1f;
        if (alphaSettings.onHit==null)
        {
			alphaSettings.onHit = new OnHit();
		}
		alphaSettings.onHit.fadeSpeed = 0.1F;
		alphaSettings.onHit.onHitAlpha = 1.0F;
		alphaSettings.onHit.duration = 1.0F;
	}

	public void SetDefaultAlpha(float defaultAlpha, float defaultFadeSpeed)
	{
        if (alphaSettings == null)
        {
			return;
        }
		alphaSettings.defaultAlpha = defaultAlpha;
		alphaSettings.defaultFadeSpeed = defaultFadeSpeed;
	}
	public void SetFullAplpha(float fullAplpha,float fullFadeSpeed)
	{
		if (alphaSettings == null)
		{
			return;
		}
		alphaSettings.fullAplpha = fullAplpha;
		alphaSettings.fullFadeSpeed = fullFadeSpeed;
	}
	public void SetNullAlpha(float nullAlpha, float nullFadeSpeed)
	{
		if (alphaSettings == null)
		{
			return;
		}
		alphaSettings.nullAlpha = nullAlpha;
		alphaSettings.nullFadeSpeed = nullFadeSpeed;
	}
	public void SetAlphaHit(float fadeSpeed, float onHitAlpha,float duration)
	{
		if (alphaSettings == null)
		{
			return;
		}
		if (alphaSettings.onHit == null)
		{
			return;
		}
		alphaSettings.onHit.fadeSpeed = fadeSpeed;
		alphaSettings.onHit.onHitAlpha = onHitAlpha;
		alphaSettings.onHit.duration = duration;
	}
	public void HideHealthbar() 
	{
		CanRemoveObject = true;
	}
	bool isCanvasFadeMoveFinish;

	//世界坐标转成UI中父节点的坐标, 并设置子节点的位置
	public void World2UI(Vector3 wpos, RectTransform uiParent, RectTransform uiTarget)
	{ 
		Vector3 spos = GameEntry.Camera.CurUseCamera.WorldToScreenPoint(wpos);
		Vector2 retPos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(uiParent, new Vector2(spos.x, spos.y), GameEntry.Camera.UICamera, out retPos);
		uiTarget.anchoredPosition = retPos;
		uiTarget.SetLocalPositionZ(0);
	}
	// Update is called once per frame
	void FixedUpdate () {
		if(!HealthbarPrefab)
			return;
		//HealthbarPrefab.transform.position = GameEntry.Camera.UICamera.WorldToScreenPoint(new Vector3(CachedTransform.position.x, CachedTransform.position.y + yOffset, 0));
		Vector3 worldPos = new Vector3(CachedTransform.position.x, CachedTransform.position.y + yOffset, CachedTransform.position.z);
		World2UI(worldPos, healthbarRootRect, HealthbarPrefab);
		float targetValue = curHealth / defaultHealth;
		//白霆添加
		healthVolume.value = Mathf.MoveTowards(healthVolume.value, targetValue, Time.deltaTime);

		/* 白霆注销
		healthVolume.fillAmount = Mathf.MoveTowards(healthVolume.fillAmount, curHealth / defaultHealth, Time.deltaTime);
		
		float maxDifference = 0.1F;

		if(backGround.fillAmount > healthVolume.fillAmount + maxDifference)
			backGround.fillAmount = healthVolume.fillAmount + maxDifference;
        if (backGround.fillAmount > healthVolume.fillAmount)
            backGround.fillAmount -= (1 / (defaultHealth / 100)) * Time.deltaTime;
        else
            backGround.fillAmount = healthVolume.fillAmount;
		*/
	}
	bool IsToZero;

	void Update()
	{
		if(!HealthbarPrefab)
			return;
		camDistance = Vector3.Dot(CachedTransform.position - cam.transform.position, cam.transform.forward);
		
		if(showHealthInfo)
			healthInfo.text = curHealth + " / "+defaultHealth;
		else
			healthInfo.text = "";

        if(lastHealth != curHealth)
        {
            rate = Time.time + alphaSettings.onHit.duration;
            lastHealth = curHealth;
        }

        if (curHealth > 0)
        {
			isCanvasFadeMoveFinish = false;
		}

		if (!OutDistance() && IsVisible())
        {
            if (curHealth <= 0)
            {
                if (alphaSettings.nullFadeSpeed > 0)
                {
					//白霆注销
					//if (backGround.fillAmount <= 0)
					//白霆添加
					if (healthVolume.value <= 0)
					{
						canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alphaSettings.nullAlpha, alphaSettings.nullFadeSpeed);
						if (canvasGroup.alpha == alphaSettings.nullAlpha)
						{
							if (!isCanvasFadeMoveFinish)
							{
								isCanvasFadeMoveFinish = true;
							}
						}
						else
						{
							isCanvasFadeMoveFinish = false;
						}
					}
					else 
					{
                        if (!IsToZero)
                        {
							canvasGroup.alpha = alphaSettings.onHit.onHitAlpha;
							IsToZero = true;
						}
					}
				}
                else
                    canvasGroup.alpha = alphaSettings.nullAlpha;
            }
            else if (curHealth == defaultHealth)
                canvasGroup.alpha = alphaSettings.fullFadeSpeed > 0 ? Mathf.MoveTowards(canvasGroup.alpha, alphaSettings.fullAplpha, alphaSettings.fullFadeSpeed) : alphaSettings.fullAplpha;
            else
            {
                if (rate > Time.time)
                    canvasGroup.alpha = alphaSettings.onHit.onHitAlpha;
                else
                    canvasGroup.alpha = alphaSettings.onHit.fadeSpeed > 0 ? Mathf.MoveTowards(canvasGroup.alpha, alphaSettings.defaultAlpha, alphaSettings.onHit.fadeSpeed) : alphaSettings.defaultAlpha;
            }
        }
        else
            canvasGroup.alpha = alphaSettings.defaultFadeSpeed > 0 ? Mathf.MoveTowards(canvasGroup.alpha, 0, alphaSettings.defaultFadeSpeed) : 1;

		if(curHealth <= 0)
			curHealth = 0;

		dist = keepSize ? camDistance / scale : scale;

		HealthbarPrefab.sizeDelta = new Vector2 (healthbarSize.x/*/(dist-sizeOffsets.x/100)*/, healthbarSize.y/*/(dist-sizeOffsets.y/100)*/);

		healthInfo.rectTransform.sizeDelta = new Vector2 (HealthbarPrefab.sizeDelta.x * healthInfoSize/10, 
		                                                  HealthbarPrefab.sizeDelta.y * healthInfoSize/10);
		
		healthInfoPosition.y = HealthbarPrefab.sizeDelta.y + (healthInfo.rectTransform.sizeDelta.y - HealthbarPrefab.sizeDelta.y) / 2;
		
		if(healthInfoAlignment == HealthInfoAlignment.top)
			healthInfo.rectTransform.anchoredPosition = healthInfoPosition;
		else if (healthInfoAlignment == HealthInfoAlignment.center)
			healthInfo.rectTransform.anchoredPosition = Vector2.zero;
		else
			healthInfo.rectTransform.anchoredPosition = -healthInfoPosition;

        if(curHealth > defaultHealth)
            defaultHealth = curHealth;

		if (CanRemoveObject && isCanvasFadeMoveFinish)
		{
			ClearHealthBat();
		}
	}
	public void ClearHealthBat()
    {
	    GameEntry.UI.GetHealthbarRoot().RemoveHealthBar(HealthbarPrefab);
	    GameEntry.AssetObject.HideObject(m_SerialId);
	    HealthbarPrefab = null;
	}
	bool IsVisible()
	{
		return canvas.pixelRect.Contains (HealthbarPrefab.position);
	}

    bool OutDistance()
    {
        return DrawOFFDistance == true && camDistance > drawDistance;
    }

    public float GetDefaultHealth()
    {
        return defaultHealth;
    }

    public float GetCurrentHealth()
    {
        return curHealth;
    }
	public void SetCurrentHealth(float curHealth)
	{
		this.curHealth = curHealth;
	}
}

[System.Serializable]
public class AlphaSettings
{
    
    public float defaultAlpha = 0.7F;           //Default healthbar alpha (health is bigger then zero and not full);
    public float defaultFadeSpeed = 0.1F;
    public float fullAplpha = 0.0F;             //Healthbar alpha when health is full;
    public float fullFadeSpeed = 0.1F;
    public float nullAlpha = 0.0F;              //Healthbar alpha when health is zero or less;
    public float nullFadeSpeed = 0.1F;
    public OnHit onHit;                         //On hit settings
}

[System.Serializable]
public class OnHit
{
    public float fadeSpeed = 0.1F;              //Alpha state fade speed;
    public float onHitAlpha = 1.0F;             //On hit alpha;
    public float duration = 1.0F;               //Duration of alpha state;
}
