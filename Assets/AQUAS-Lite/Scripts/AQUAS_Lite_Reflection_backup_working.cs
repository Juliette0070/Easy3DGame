// using UnityEngine;

// using System.Collections;



// namespace AQUAS_Lite

// {

//     [AddComponentMenu("AQUAS Lite/Reflection")]

//     [ExecuteInEditMode]

//     public class AQUAS_Lite_Reflection : MonoBehaviour

//     {

//         #region Variables

//         public bool m_DisablePixelLights = true;

//         public int m_TextureSize = 256;

//         public float m_ClipPlaneOffset = 0.07f;

//         public LayerMask m_ReflectLayers = -1;



//         private Hashtable m_ReflectionCameras = new Hashtable();

//         private RenderTexture m_ReflectionTexture = null;

//         private int m_OldReflectionTextureSize = 0;



//         private static bool s_InsideRendering = false;



//         public bool ignoreOcclusionCulling;



// #if UNITY_5_3 || UNITY_5_4 || UNITY_5_5

//         public bool disableInEditMode;

// #endif



//         // Unity 6 compatibility:

//         // OnWillRenderObject can be too late for issuing a nested Camera.Render().

//         // We prepare the reflection here and render it from Camera.onPreCull.

//         private Camera m_PendingCamera;

//         private Camera m_PendingReflectionCamera;

//         private bool m_ReflectionPending;



//         private void OnEnable()

//         {

//             Camera.onPreCull += OnCameraPreCull;

//         }



//         private void OnDisable()

//         {

//             Camera.onPreCull -= OnCameraPreCull;

//             m_PendingCamera = null;

//             m_PendingReflectionCamera = null;

//             m_ReflectionPending = false;



//             if (m_ReflectionTexture)

//             {

//                 DestroyImmediate(m_ReflectionTexture);

//                 m_ReflectionTexture = null;

//             }



//             foreach (DictionaryEntry kvp in m_ReflectionCameras)

//             {

//                 Camera reflectionCamera = kvp.Value as Camera;

//                 if (reflectionCamera)

//                     DestroyImmediate(reflectionCamera.gameObject);

//             }



//             m_ReflectionCameras.Clear();

//         }



//         public void OnWillRenderObject()

//         {

// #if UNITY_5_3 || UNITY_5_4 || UNITY_5_5

//             if (disableInEditMode && !Application.isPlaying)

//             {

//                 OnDisable();

//                 return;

//             }

// #endif



//             if (!enabled || !GetComponent<Renderer>() ||

//                 !GetComponent<Renderer>().sharedMaterial ||

//                 !GetComponent<Renderer>().enabled)

//                 return;



//             Camera cam = Camera.current;

//             if (!cam)

//                 return;



//             // Do not render a nested camera from OnWillRenderObject.

//             // Just remember which camera needs a reflection.

//             if (s_InsideRendering)

//                 return;



//             Camera reflectionCamera;

//             CreateMirrorObjects(cam, out reflectionCamera);



//             m_PendingCamera = cam;

//             m_PendingReflectionCamera = reflectionCamera;

//             m_ReflectionPending = true;

//         }



//         private void OnCameraPreCull(Camera cam)

//         {

//             if (!m_ReflectionPending ||

//                 m_PendingCamera != cam ||

//                 !m_PendingReflectionCamera)

//                 return;



//             if (s_InsideRendering)

//                 return;



//             RenderReflection(cam, m_PendingReflectionCamera);



//             m_ReflectionPending = false;

//             m_PendingCamera = null;

//             m_PendingReflectionCamera = null;

//         }



//         private void RenderReflection(Camera cam, Camera reflectionCamera)

//         {

//             s_InsideRendering = true;



//             int oldPixelLightCount = QualitySettings.pixelLightCount;

//             if (m_DisablePixelLights)

//                 QualitySettings.pixelLightCount = 0;



//             try

//             {

//                 UpdateCameraModes(cam, reflectionCamera);



//                 Vector3 pos = transform.position;

//                 Vector3 normal = transform.up;



//                 float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;

//                 Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);



//                 reflectionCamera.useOcclusionCulling = !ignoreOcclusionCulling;



//                 Matrix4x4 reflection = Matrix4x4.zero;

//                 CalculateReflectionMatrix(ref reflection, reflectionPlane);



//                 Vector3 oldpos = cam.transform.position;

//                 Vector3 newpos = reflection.MultiplyPoint(oldpos);



//                 reflectionCamera.worldToCameraMatrix =

//                     cam.worldToCameraMatrix * reflection;



//                 Vector4 clipPlane =

//                     CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);



//                 Matrix4x4 projection = cam.projectionMatrix;

//                 CalculateObliqueMatrix(ref projection, clipPlane);

//                 reflectionCamera.projectionMatrix = projection;



//                 reflectionCamera.cullingMask =

//                     ~(1 << 4) & m_ReflectLayers.value;



//                 reflectionCamera.targetTexture = m_ReflectionTexture;



//                 GL.invertCulling = true;



//                 reflectionCamera.transform.position = newpos;



//                 Vector3 euler = cam.transform.eulerAngles;

//                 reflectionCamera.transform.eulerAngles =

//                     new Vector3(0, euler.y, euler.z);



//                 reflectionCamera.Render();



//                 reflectionCamera.transform.position = oldpos;



//                 GL.invertCulling = false;



//                 Renderer renderer = GetComponent<Renderer>();

//                 Material[] materials = renderer.sharedMaterials;



//                 foreach (Material mat in materials)

//                 {

//                     if (!mat)

//                         continue;



//                     if (mat.HasProperty("_ReflectionTex"))

//                         mat.SetTexture("_ReflectionTex", m_ReflectionTexture);

//                 }



//                 Matrix4x4 scaleOffset = Matrix4x4.TRS(

//                     new Vector3(0.5f, 0.5f, 0.5f),

//                     Quaternion.identity,

//                     new Vector3(0.5f, 0.5f, 0.5f));



//                 Vector3 scale = transform.lossyScale;



//                 Matrix4x4 mtx =

//                     transform.localToWorldMatrix *

//                     Matrix4x4.Scale(new Vector3(

//                         1.0f / scale.x,

//                         1.0f / scale.y,

//                         1.0f / scale.z));



//                 mtx = scaleOffset *

//                       cam.projectionMatrix *

//                       cam.worldToCameraMatrix *

//                       mtx;



//                 foreach (Material mat in materials)

//                 {

//                     if (!mat)

//                         continue;



//                     if (mat.HasProperty("_ProjMatrix"))

//                         mat.SetMatrix("_ProjMatrix", mtx);

//                 }

//             }

//             finally

//             {

//                 GL.invertCulling = false;



//                 if (m_DisablePixelLights)

//                     QualitySettings.pixelLightCount = oldPixelLightCount;



//                 s_InsideRendering = false;

//             }

//         }



//         private void UpdateCameraModes(Camera src, Camera dest)

//         {

//             if (dest == null)

//                 return;



//             dest.clearFlags = src.clearFlags;

//             dest.backgroundColor = src.backgroundColor;



//             if (src.clearFlags == CameraClearFlags.Skybox)

//             {

//                 Skybox sky = src.GetComponent<Skybox>();

//                 Skybox mysky = dest.GetComponent<Skybox>();



//                 if (!sky || !sky.material)

//                 {

//                     mysky.enabled = false;

//                 }

//                 else

//                 {

//                     mysky.enabled = true;

//                     mysky.material = sky.material;

//                 }

//             }



//             dest.farClipPlane = src.farClipPlane;

//             dest.nearClipPlane = src.nearClipPlane;

//             dest.orthographic = src.orthographic;

//             dest.fieldOfView = src.fieldOfView;

//             dest.aspect = src.aspect;

//             dest.orthographicSize = src.orthographicSize;

//         }



//         private void CreateMirrorObjects(Camera currentCamera, out Camera reflectionCamera)

//         {

//             reflectionCamera = null;



//             if (!m_ReflectionTexture ||

//                 m_OldReflectionTextureSize != m_TextureSize)

//             {

//                 if (m_ReflectionTexture)

//                     DestroyImmediate(m_ReflectionTexture);



//                 m_ReflectionTexture =

//                     new RenderTexture(m_TextureSize, m_TextureSize, 16);



//                 m_ReflectionTexture.name =

//                     "__MirrorReflection" + GetInstanceID();



//                 m_ReflectionTexture.isPowerOfTwo = true;

//                 m_ReflectionTexture.hideFlags = HideFlags.DontSave;

//                 m_OldReflectionTextureSize = m_TextureSize;

//             }



//             reflectionCamera =

//                 m_ReflectionCameras[currentCamera] as Camera;



//             if (!reflectionCamera)

//             {

//                 GameObject go = new GameObject(

//                     "Mirror Refl Camera id" + GetInstanceID() +

//                     " for " + currentCamera.GetInstanceID(),

//                     typeof(Camera),

//                     typeof(Skybox));



//                 reflectionCamera = go.GetComponent<Camera>();

//                 reflectionCamera.enabled = false;

//                 reflectionCamera.transform.position = transform.position;

//                 reflectionCamera.transform.rotation = transform.rotation;



//                 if (!reflectionCamera.gameObject.GetComponent<FlareLayer>())

//                     reflectionCamera.gameObject.AddComponent<FlareLayer>();



//                 go.hideFlags = HideFlags.HideAndDontSave;

//                 m_ReflectionCameras[currentCamera] = reflectionCamera;

//             }

//         }



//         private static float sgn(float a)

//         {

//             if (a > 0.0f) return 1.0f;

//             if (a < 0.0f) return -1.0f;

//             return 0.0f;

//         }



//         private Vector4 CameraSpacePlane(

//             Camera cam,

//             Vector3 pos,

//             Vector3 normal,

//             float sideSign)

//         {

//             Vector3 offsetPos =

//                 pos + normal * m_ClipPlaneOffset;



//             Matrix4x4 m = cam.worldToCameraMatrix;

//             Vector3 cpos = m.MultiplyPoint(offsetPos);

//             Vector3 cnormal =

//                 m.MultiplyVector(normal).normalized * sideSign;



//             return new Vector4(

//                 cnormal.x,

//                 cnormal.y,

//                 cnormal.z,

//                 -Vector3.Dot(cpos, cnormal));

//         }



//         private static void CalculateObliqueMatrix(

//             ref Matrix4x4 projection,

//             Vector4 clipPlane)

//         {

//             Vector4 q = projection.inverse * new Vector4(

//                 sgn(clipPlane.x),

//                 sgn(clipPlane.y),

//                 1.0f,

//                 1.0f);



//             Vector4 c =

//                 clipPlane *

//                 (2.0F / Vector4.Dot(clipPlane, q));



//             projection[2] = c.x - projection[3];

//             projection[6] = c.y - projection[7];

//             projection[10] = c.z - projection[11];

//             projection[14] = c.w - projection[15];

//         }



//         private static void CalculateReflectionMatrix(

//             ref Matrix4x4 reflectionMat,

//             Vector4 plane)

//         {

//             reflectionMat.m00 =

//                 (1F - 2F * plane[0] * plane[0]);

//             reflectionMat.m01 =

//                 (-2F * plane[0] * plane[1]);

//             reflectionMat.m02 =

//                 (-2F * plane[0] * plane[2]);

//             reflectionMat.m03 =

//                 (-2F * plane[3] * plane[0]);



//             reflectionMat.m10 =

//                 (-2F * plane[1] * plane[0]);

//             reflectionMat.m11 =

//                 (1F - 2F * plane[1] * plane[1]);

//             reflectionMat.m12 =

//                 (-2F * plane[1] * plane[2]);

//             reflectionMat.m13 =

//                 (-2F * plane[3] * plane[1]);



//             reflectionMat.m20 =

//                 (-2F * plane[2] * plane[0]);

//             reflectionMat.m21 =

//                 (-2F * plane[2] * plane[1]);

//             reflectionMat.m22 =

//                 (1F - 2F * plane[2] * plane[2]);

//             reflectionMat.m23 =

//                 (-2F * plane[3] * plane[2]);



//             reflectionMat.m30 = 0F;

//             reflectionMat.m31 = 0F;

//             reflectionMat.m32 = 0F;

//             reflectionMat.m33 = 1F;

//         }

//     }

//     #endregion

// }