using Cinemachine;
using UnityEngine;

public partial class CameraComponent
{
    private CinemachineFreeLook m_FollowFreeCamera;

    private void OnFreeLookAwake() 
    {
        m_FollowFreeCamera = transform.Find("FollowFreeViewVirtual").GetComponent<CinemachineFreeLook>();
    }

    public void FollowAndFreeViewTarget(Transform followTrans, Transform lookAtTrans)
    {
        if (followTrans != null)
        {
            m_FollowFreeCamera.Follow = followTrans;
            m_FollowFreeCamera.LookAt = lookAtTrans;
            OpenVCamera(1);
        }
        else
        {
            OpenVCamera(0);
        }
    }

    private void FollowFreeIsShow(bool isShow)
    {
        m_FollowFreeCamera.gameObject.SetActive(isShow);
    }
}
