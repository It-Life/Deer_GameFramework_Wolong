using Cinemachine;
using UnityEngine;

public partial class CameraComponent
{
    private CinemachineFreeLook m_FollowFreeCamera;

    private void OnFreeLookAwark() 
    {
        m_FollowFreeCamera = transform.Find("FollowFreeViewVirtual").GetComponent<CinemachineFreeLook>();
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
}
