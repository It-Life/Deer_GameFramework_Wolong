using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public partial class CameraComponent
{
    private Camera m_UICamera;
    public Camera UICamera { get { return m_UICamera; } }

    private void OnUICameraAwark() 
    {
        GameObject goCamera = GameObject.FindGameObjectWithTag("UICamera");
        m_UICamera = goCamera.GetComponent<Camera>();
        m_UICamera.GetComponent<UniversalAdditionalCameraData>().renderType = CameraRenderType.Overlay;
        MainCamera.GetComponent<UniversalAdditionalCameraData>().cameraStack.Add(m_UICamera);
    }


}
