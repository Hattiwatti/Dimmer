using Dimmer.Settings;
using System;
using UnityEngine;
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
            BSLayerMask.PlayersPlace |
            BSLayerMask.DontShowInExternalMRCamera);

        private readonly DimmerConfig _config;

        private Material _overlayMat;

        private GameObject _cameraObject;
        private Camera _dimmerCamera;

        private int _originalCullingMask;

        private DimmerOverlay(DimmerConfig config)
        {
            _config = config;
        }

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

            _cameraObject = new GameObject("DimmerCamera");
            _cameraObject.hideFlags = HideFlags.HideAndDontSave;
            _dimmerCamera = _cameraObject.AddComponent<Camera>();

            _dimmerCamera.CopyFrom(Camera.main);
            _dimmerCamera.name = "DimmerCamera";
            _dimmerCamera.enabled = false;
            _dimmerCamera.tag = "Untagged";
            _dimmerCamera.cullingMask = DimmerCullingMask;
            _dimmerCamera.targetTexture = null;
            _dimmerCamera.clearFlags = CameraClearFlags.Nothing;

            _dimmerCamera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _dimmerCamera.transform.SetParent(Camera.main.transform, false);

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

            GameObject.Destroy(_cameraObject);
            GameObject.Destroy(_dimmerCamera);
            GameObject.Destroy(_overlayMat);
        }

        public void OnCameraPreCull(Camera camera)
        {
            if (!Plugin.IsPlayingChart || camera != Camera.main)
                return;

            _originalCullingMask = camera.cullingMask;

            camera.cullingMask &= ~DimmerCullingMask;
        }

        public void OnCameraPostRender(Camera camera)
        {
            if (!Plugin.IsPlayingChart)
                return;

            if (camera != Camera.main && _dimmerCamera != null)
                return;

            camera.cullingMask = _originalCullingMask;

            // Render dimmer overlay
            Graphics.Blit(camera.activeTexture, camera.activeTexture, _overlayMat);

            _dimmerCamera.targetTexture = camera.activeTexture;

            if (camera.stereoActiveEye != Camera.MonoOrStereoscopicEye.Mono)
            {
                Camera.StereoscopicEye activeEye = camera.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left ? Camera.StereoscopicEye.Left : Camera.StereoscopicEye.Right;

                Vector3 eyePos = new Vector3(Camera.main.stereoSeparation / 2, 0, 0);
                eyePos.x *= (activeEye == Camera.StereoscopicEye.Left) ? -1 : 1;

                _dimmerCamera.transform.SetLocalPositionAndRotation(eyePos, Quaternion.identity);
                _dimmerCamera.projectionMatrix = camera.GetStereoProjectionMatrix(activeEye);
                _dimmerCamera.nonJitteredProjectionMatrix = camera.GetStereoNonJitteredProjectionMatrix(activeEye);
            }
            else
            {
                _dimmerCamera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _dimmerCamera.projectionMatrix = camera.projectionMatrix;
                _dimmerCamera.nonJitteredProjectionMatrix = camera.nonJitteredProjectionMatrix;
            }

            // Render rest of the layers on top
            _dimmerCamera.Render();
        }

    }
}
