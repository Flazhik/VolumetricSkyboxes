using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using VolumetricSkyboxes.Components;
using static VolumetricSkyboxes.Utils.RenderingUtils;
using static UnityEngine.Object;

namespace VolumetricSkyboxes.Patches
{
    [HarmonyPatch(typeof(CameraController))]
    public class CameraControllerPatch
    {
        private static Camera _backgroundCam;
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CameraController), "Start")]
        [HarmonyPriority(400)]
        public static void CameraController_Start_Postfix(CameraController __instance)
        {
            if (SceneHelper.CurrentScene != "Endless")
            {
                VolumetricSkyboxesPlugin.PortalCamera = Camera.main;
                return;
            }
            
            if (__instance.tag is not "MainCamera")
                return;

            SetupBackgroundCamera();
        }

        private static void SetupBackgroundCamera()
        {
            var cameras = Camera.allCameras;
            var mainCam = GetCamera("Main Camera");
            var virtualCam = GetCamera("Virtual Camera");

            if (_backgroundCam != null)
                return;
            
            _backgroundCam = Instantiate(virtualCam, mainCam.transform);
            _backgroundCam.name = "Background Camera";
            _backgroundCam.depth = mainCam.depth - 1;
            
            if (SceneHelper.CurrentScene != "Endless")
                return;
            
            _backgroundCam.gameObject.AddComponent<BackgroundCamera>();
            
            _backgroundCam.transform.position = mainCam.transform.position;
            _backgroundCam.transform.rotation = mainCam.transform.rotation;
            
            _backgroundCam.orthographic = false;
            
            mainCam.cullingMask &= ~BackgroundLayersMask;
            _backgroundCam.cullingMask = BackgroundLayersMask;
            
            mainCam.clearFlags = CameraClearFlags.Depth;
            _backgroundCam.clearFlags = CameraClearFlags.Skybox;
            VolumetricSkyboxesPlugin.PortalCamera = _backgroundCam;
            
            SceneManager.GetActiveScene().GetRootGameObjects()
                .Where(c => c.name == "Player").First();

            Camera GetCamera(string name) => cameras.First(cam => cam.name == name);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CameraController), "LateUpdate")]
        public static void CameraController_LateUpdate_Postfix(CameraController __instance)
        {
            if (_backgroundCam == null)
                return;

            _backgroundCam.fieldOfView = __instance.cam.fieldOfView;
        }
    }
}