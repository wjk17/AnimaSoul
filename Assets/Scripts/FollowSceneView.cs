using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
    public class FollowSceneView : MonoBehaviour {
    public bool on ;
    public Transform cam;
    public CinemachineVirtualCamera vCam;
    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            Follow();
    }
    void Follow()
    {
        var sceneViews = UnityEditor.SceneView.sceneViews;
        if (!on || sceneViews.Count == 0) return;

        var sceneView = (UnityEditor.SceneView)sceneViews[0];
        var scnCam = sceneView.camera;
        if (cam != null)
        {
            cam.position = scnCam.transform.position;
            cam.rotation = scnCam.transform.rotation;
        }
        if (vCam !=null)
        {
            vCam.m_Lens.NearClipPlane = scnCam.nearClipPlane;
            vCam.m_Lens.FarClipPlane = scnCam.farClipPlane;
            vCam.m_Lens.FieldOfView = scnCam.fieldOfView;
            vCam.m_Lens.Orthographic = scnCam.orthographic;
            vCam.m_Lens.OrthographicSize = scnCam.orthographicSize;
            vCam.transform.position = scnCam.transform.position;
            vCam.transform.rotation = scnCam.transform.rotation;
        }
    }
}
#endif
