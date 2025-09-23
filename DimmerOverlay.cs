using Dimmer.Settings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Dimmer
{
    enum BSLayers
    {
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        ThirdPerson = 3,
        Water = 4,
        UI = 5,
        FirstPerson = 6,
        Notes = 8,
        NoteDebris = 9,
        Avatar = 10,
        Obstacles = 11,
        Sabers = 12,
        NeonLight =13,
        Environment = 14,
        GrabPassTexture1 = 15,
        CutEffectParticles = 16,
        NonReflectedParticles = 19,
        EnvironmentPhysics = 20,
        Event = 22,
        FixMRAlpha = 25,
        DontShowInExternalMRCamera = 27,
        PlayersPlace = 28,
        Skybox = 29,
        MRForegroundClipPlane = 30,
    }

    [Flags]
    enum BSLayerMask : int
    {
        Default = 1 << 0,
        TransparentFX = 1 << 1,
        IgnoreRaycast = 1 << 2,
        ThirdPerson = 1 << 3,
        Water = 1 << 4,
        UI = 1 << 5,
        FirstPerson = 1 << 6,
        Notes = 1 << 8,
        NoteDebris = 1 << 9,
        Avatar = 1 << 10,
        Obstacles = 1 << 11,
        Sabers = 1 << 12,
        NeonLight = 1 << 13,
        Environment = 1 << 14,
        GrabPassTexture1 = 1 << 15,
        CutEffectParticles = 1 << 16,
        NonReflectedParticles = 1 << 19,
        EnvironmentPhysics = 1 << 20,
        Event = 1 << 22,
        FixMRAlpha = 1 << 25,
        DontShowInExternalMRCamera = 1 << 27,
        PlayersPlace = 1 << 28,
        Skybox = 1 << 29,
        MRForegroundClipPlane = 1 << 30,
    }
    internal class DimmerOverlay : IInitializable, IDisposable
    {
        private static int DimmerCullingMask = (int)(
            BSLayerMask.Notes |
            BSLayerMask.NoteDebris |
            BSLayerMask.Sabers |
            BSLayerMask.UI |
            BSLayerMask.CutEffectParticles |
            BSLayerMask.Obstacles |
            BSLayerMask.FixMRAlpha |
            BSLayerMask.PlayersPlace |
            BSLayerMask.DontShowInExternalMRCamera |
            BSLayerMask.Avatar |
            BSLayerMask.FirstPerson |
            BSLayerMask.ThirdPerson);

        private readonly DimmerConfig _config;

        private Material _overlayMat;

        private Camera _dimmerCamera;
        private Camera _dimmerStereoCamera;

        private Dictionary<Camera, int> _originalCullingMasks = new Dictionary<Camera, int>();

        private DimmerOverlay(DimmerConfig config)
        {
            _config = config;
        }

        // From Camera2, need to copy the whole main camera gameobject to get proper visuals.
        static readonly HashSet<string> AllowedCameraComponents = new HashSet<string>()
        {
            "Camera", "BloomPrePass", "MainEffectController"
        };

        public void Initialize()
        {
            if (!_config.DimmerEnabled)
                return;

            // Move the glow around the platform to PlayersPlace layer so it doesn't get dimmed
            GameObject platformGlow = GameObject.Find("PlayersPlace/RectangleFakeGlow");
            if (platformGlow != null)
            {
                platformGlow.layer = (int)BSLayers.PlayersPlace;
            }

            var cameraGameObject = GameObject.Instantiate(Camera.main.gameObject);
            cameraGameObject.name = "DimmerCamera";
            cameraGameObject.hideFlags = HideFlags.HideAndDontSave;

            _dimmerCamera = cameraGameObject.GetComponent<Camera>();
            _dimmerCamera.name = "DimmerCamera";
            _dimmerCamera.enabled = false;
            _dimmerCamera.tag = "Untagged";
            _dimmerCamera.clearFlags = CameraClearFlags.Nothing;
            _dimmerCamera.stereoTargetEye = StereoTargetEyeMask.None;

            // From Camera2. Destroy components that are not needed
            foreach (var component in cameraGameObject.GetComponents<Behaviour>())
            {
                if (!AllowedCameraComponents.Contains(component.GetType().Name))
                    GameObject.DestroyImmediate(component);
            }

            var stereoCameraGameObject = GameObject.Instantiate(cameraGameObject);
            stereoCameraGameObject.name = "DimmerStereoCamera";
            _dimmerStereoCamera = stereoCameraGameObject.GetComponent<Camera>();
            _dimmerStereoCamera.name = "DimmerStereoCamera";
            _dimmerStereoCamera.enabled = false;
            _dimmerStereoCamera.depth = 1;
            _dimmerStereoCamera.stereoTargetEye = StereoTargetEyeMask.None;
            _dimmerStereoCamera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _dimmerStereoCamera.transform.SetParent(Camera.main.transform, false);

            _overlayMat = new Material(Shader.Find("Hidden/Internal-Colored"));
            _overlayMat.hideFlags = HideFlags.HideAndDontSave;
            _overlayMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            _overlayMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _overlayMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _overlayMat.SetInt("_ZWrite", 0);
            _overlayMat.SetColor("_Color", new Color(0f, 0f, 0f, _config.DimmerOpacity));

            Camera.onPreCull += OnCameraPreCull;
            Camera.onPostRender += OnCameraPostRender;
        }

        public void Dispose()
        {
            if (!_config.DimmerEnabled)
                return;

            Camera.onPreCull -= OnCameraPreCull;
            Camera.onPostRender -= OnCameraPostRender;
        }

        public void OnCameraPreCull(Camera camera)
        {
            if (!Plugin.IsPlayingChart)
                return;

            if (camera == _dimmerCamera || camera == _dimmerStereoCamera)
                return;

            if (camera != Camera.main && !_config.DimmerCamera2)
                return;

            _originalCullingMasks[camera] = camera.cullingMask;
            camera.cullingMask &= ~DimmerCullingMask;
        }

        public void OnCameraPostRender(Camera camera)
        {
            if (!Plugin.IsPlayingChart)
                return;

            if (camera == _dimmerCamera || camera == _dimmerStereoCamera)
                return;

            if (camera != Camera.main && !_config.DimmerCamera2)
                return;

            // Render dimmer overlay
            Graphics.Blit(camera.activeTexture, camera.activeTexture, _overlayMat);

            // Revert the camera culling mask back to original value just in case
            camera.cullingMask = _originalCullingMasks[camera];

            bool isSinglePassStereo = camera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Mono && camera.activeTexture.volumeDepth == 2;
            
            if (camera == Camera.main)
            {
                if (isSinglePassStereo)
                {
                    _dimmerStereoCamera.CopyFrom(camera);
                    _dimmerStereoCamera.name = "DimmerCamera";
                    _dimmerStereoCamera.cullingMask &= DimmerCullingMask;
                    _dimmerStereoCamera.tag = "Untagged";
                    _dimmerStereoCamera.clearFlags = CameraClearFlags.Nothing;
                }
                _dimmerStereoCamera.enabled = isSinglePassStereo;
            }
            
            if (!isSinglePassStereo)
            {
                _dimmerCamera.CopyFrom(camera);
                _dimmerCamera.name = "DimmerCamera";
                _dimmerCamera.cullingMask &= DimmerCullingMask;
                _dimmerCamera.tag = "Untagged";
                _dimmerCamera.targetTexture = camera.activeTexture;
                _dimmerCamera.clearFlags = CameraClearFlags.Nothing;
                _dimmerCamera.stereoTargetEye = StereoTargetEyeMask.None;
                _dimmerCamera.enabled = false;
                _dimmerCamera.Render();
            }
        }

    }
}
