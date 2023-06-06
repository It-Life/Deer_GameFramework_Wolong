using Cinemachine;
using UnityEngine;

public partial class CameraComponent
{
    private CinemachineVirtualCamera m_OrbitCamera;
    private void OnOrbitAwake() 
    {
        m_OrbitCamera = transform.Find("OrbitCameraVirtual").GetComponent<CinemachineVirtualCamera>();
    }

    public Transform GetTransOrbitCamera()
    {
        return m_OrbitCamera.transform;
    }
    
    private void OrbitIsShow(bool isShow)
    {
        m_OrbitCamera.gameObject.SetActive(isShow);
    }
}
