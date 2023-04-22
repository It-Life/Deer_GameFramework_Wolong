// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-28 13-57-10  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-28 13-57-10  
//版 本 : 0.1 
// ===============================================
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityGameFramework.Runtime;

public enum BaseCamera:int
{
    MainCamera,
    ArCamera,
    BattleCamera
}

[DisallowMultipleComponent]
[AddComponentMenu("Deer/Camera")]
public partial class CameraComponent : GameFrameworkComponent
{
    /// <summary>
    /// 主相机
    /// </summary>
    private Camera m_MainCamera;
    private UniversalAdditionalCameraData m_MainCameraUniData;
    public Camera MainCamera 
    {
        get => m_MainCamera;
        set => m_MainCamera = value;
    }

    private Camera m_CurUseCamera;
    public Camera CurUseCamera => m_CurUseCamera;

    /// <summary>
    /// 相机灵敏度
    /// </summary>
    private float CameraSensitivity = 0.1f;

    protected override void Awake()
    {
        base.Awake();

        m_MainCamera = transform.Find("MainCamera").GetComponent<Camera>();
        m_MainCameraUniData = m_MainCamera.GetComponent<UniversalAdditionalCameraData>();
        m_CurUseCamera = m_MainCamera;
        OnFreeLookAwark();
        
        OnUICameraAwark();
        CinemachineCore.GetInputAxis = GetAxisCustom;
    }

    public float GetAxisCustom(string axisName)
    {
        if (axisName == "Mouse X")
        {
            if (Input.GetMouseButton(0))
            {
                return Input.GetAxis("Mouse X");
            }
            else
            {
                return 0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (Input.GetMouseButton(0))
            {
                return Input.GetAxis("Mouse Y");
            }
            else
            {
                return 0;
            }
        }
        return 0;
    }


/*    #region 小地图相机管理
    /// <summary>
    /// 设置小地图跟随主角
    /// </summary>
    /// <param name="transform"></param>
    public void SetMiniMapFollowTarget(Transform transform)
    {
        m_MiniMapFollowTarget = transform;
        m_FollowMiniMapCamera.transform.position = transform.position + new Vector3(0, 10, 0);
        m_FollowMiniMapCamera.transform.LookAt(transform);
        m_OffsetPosition = m_FollowMiniMapCamera.transform.position - transform.position;
    }
    /// <summary>
    /// 小地图变焦放大
    /// </summary>
    public void MiniMapZoomIn()
    {
        m_FollowMiniMapCamera.fieldOfView += 40;
    }
    /// <summary>
    /// 小地图变焦缩小
    /// </summary>
    public void MiniMapZoomOut()
    {
        m_FollowMiniMapCamera.fieldOfView -= 40;
    }
    #endregion*/

}