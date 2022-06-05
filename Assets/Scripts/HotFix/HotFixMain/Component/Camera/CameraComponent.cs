// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-28 13-57-10  
//修改作者 : 杜鑫 
//修改时间 : 2021-08-28 13-57-10  
//版 本 : 0.1 
// ===============================================
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

public enum CameraType 
{
    FollowCamera = 0,
    RotateCamera = 1,
}

public class CameraModel 
{
    public CinemachineVirtualCamera CinemachineVirtual;
    public CameraType CameraType = CameraType.FollowCamera;
}
[DisallowMultipleComponent]
[AddComponentMenu("Deer/Camera")]
public class CameraComponent : GameFrameworkComponent
{
    /// <summary>
    /// 主相机
    /// </summary>
    private Camera m_MainCamera;
    public Camera MainCamera 
    {
        get { return m_MainCamera; }
        set { m_MainCamera = value; }
    }

    /// <summary>
    /// 第三人称跟随相机
    /// </summary>
    [SerializeField]
    private CinemachineVirtualCamera m_FollowLockCamera;
    /// <summary>
    /// 第三人称自由跟随相机
    /// </summary>
    [SerializeField]
    private CinemachineFreeLook m_FollowFreeCamera;
    [SerializeField]
    private CinemachineStateDrivenCamera m_FollowStateDrivenCamera;
    public bool DisFreeLookCameraRotation;
    /// <summary>
    /// 相机灵敏度
    /// </summary>
    private float CameraSensitivity = 0.5f;

    #region 小地图
    /// <summary>
    /// 小地图跟随相机
    /// </summary>
    [SerializeField]
    private Camera m_FollowMiniMapCamera;
    private Transform m_MiniMapFollowTarget;
    private Vector3 m_OffsetPosition;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        m_MainCamera = transform.Find("MainCamera").GetComponent<Camera>();
        m_FollowLockCamera = transform.Find("FollowLockViewVirtual").GetComponent<CinemachineVirtualCamera>();
        m_FollowFreeCamera = transform.Find("FollowFreeViewVirtual").GetComponent<CinemachineFreeLook>();
        m_FollowStateDrivenCamera = transform.Find("CMStateDrivenCamera").GetComponent<CinemachineStateDrivenCamera>();
        m_FollowMiniMapCamera = transform.Find("MiniMapCamera").GetComponent<Camera>();

        CinemachineCore.GetInputAxis = GetAxisCustom;
    }

    void Update()
    {
        if (m_MiniMapFollowTarget!= null)
        {
            m_FollowMiniMapCamera.transform.position = m_OffsetPosition + m_MiniMapFollowTarget.transform.position;
        }
        if (m_FollowFreeCamera != null && m_FollowFreeCamera.Follow != null)
        {
            m_FollowFreeCamera.Follow.transform.eulerAngles = new Vector3(m_FollowFreeCamera.Follow.transform.eulerAngles.x, m_MainCamera.transform.rotation.eulerAngles.y, m_FollowFreeCamera.Follow.transform.eulerAngles.z);
            //m_MainCamera.transform.position = new Vector3(m_FollowFreeCamera.LookAt.transform.position.x, m_MainCamera.transform.position.y, m_FollowFreeCamera.LookAt.transform.position.z);
        }
    }
    public void OpenCameraType()
    {
        //m_FollowCamera.
    }
    #region FreeLookCamera
    public void SetFreeLookCameraRotateSpeed(float xSpeed,float ySpeed)
    {
        m_FollowFreeCamera.m_XAxis.m_MaxSpeed = xSpeed;
        m_FollowFreeCamera.m_YAxis.m_MaxSpeed = ySpeed;
    }
    public void SetFreeLookCameraXAxis(float speed, float accelTime, float decelTime)
    {
        m_FollowFreeCamera.m_XAxis.m_MaxSpeed = speed;
        m_FollowFreeCamera.m_XAxis.m_AccelTime = accelTime;
        m_FollowFreeCamera.m_XAxis.m_DecelTime = decelTime;
    }
    public void SetFreeLookCameraYAxis(float speed, float accelTime, float decelTime)
    {
        m_FollowFreeCamera.m_YAxis.m_MaxSpeed = speed;
        m_FollowFreeCamera.m_YAxis.m_AccelTime = accelTime;
        m_FollowFreeCamera.m_YAxis.m_DecelTime = decelTime;
    }
    public void FollowAndFreeViewTarget(Transform followTrans, Transform lookAtTrans)
    {
        if (followTrans != null)
        {
            m_FollowFreeCamera.Follow = followTrans;
            m_FollowFreeCamera.LookAt = lookAtTrans;
            m_FollowFreeCamera.gameObject.SetActive(true);
        }
        else
        {
            m_FollowFreeCamera.gameObject.SetActive(false);
        }
    }

    public void ChangeFreeViewCameraFov(float fov)
    {
        m_FollowFreeCamera.m_Lens.FieldOfView = fov;
    }
    public CinemachineFreeLook GetFollowFreeCamera()
    {
        return m_FollowFreeCamera;
    }
    #endregion

    public void LookAtTarget(Transform transform) 
    {
        m_FollowLockCamera.LookAt = transform;  
    }

    public void FollowTarget(Transform transform)
    {
        m_FollowLockCamera.Follow = transform;
    }

    public void FollowTarget(Transform transform, Vector3 position)
    {
        m_FollowStateDrivenCamera.Follow = transform;
        m_FollowLockCamera.transform.localPosition = position;
    }

    public void FollowTarget(Transform transform,Vector3 position, Quaternion quaternion)
    {
        m_FollowLockCamera.Follow = transform;
        m_FollowLockCamera.transform.localPosition = position;
        m_FollowLockCamera.transform.localRotation = quaternion;
    }
    
    public void FollowAndLockViewTarget(Transform followTrans,Transform lookAtTrans)
    {
        if (followTrans != null)
        {
            m_FollowLockCamera.Follow = followTrans;
            m_FollowLockCamera.LookAt = lookAtTrans;
            m_FollowLockCamera.gameObject.SetActive(true);
        }
        else 
        {
            m_FollowLockCamera.gameObject.SetActive(false);
        }
    }
    
    public void CameraActive(bool isActive)
    {
        m_MainCamera.gameObject.SetActive(isActive);   
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


    #region 小地图相机管理
    /// <summary>
    /// 设置小地图跟随主角
    /// </summary>
    /// <param name="transform"></param>
    public void SetMiniMapFollowTarget(Transform transform) 
    {
        m_MiniMapFollowTarget = transform;
        m_FollowMiniMapCamera.transform.position = transform.position + new Vector3(0,10,0);
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
    #endregion

}