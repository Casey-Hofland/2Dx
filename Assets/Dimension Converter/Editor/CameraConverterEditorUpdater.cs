using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DimensionConverter.Editor
{
    [InitializeOnLoad]
    public class CameraConverterEditorUpdater
    {
        private static HashSet<CameraConverter> updatingCameraConverters = new HashSet<CameraConverter>();

        static CameraConverterEditorUpdater()
        {
            if(!EditorApplication.isPlaying)
            {
                EditorApplication.update += UpdateCameraConverters;
            }
            EditorApplication.playModeStateChanged += ChangeCameraConverterUpdateStatus;
        }

        private static void ChangeCameraConverterUpdateStatus(PlayModeStateChange playModeStateChange)
        {
            EditorApplication.update -= UpdateCameraConverters;

            if(playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.update += UpdateCameraConverters;
            }
        }

        static void UpdateCameraConverters()
        {
            var settings = Resources.Load<Settings>(nameof(Settings));
            if(settings == null || !settings.cameraConverterLiveUpdate)
            {
                return;
            }

            foreach(var cameraConverter in Object.FindObjectsOfType<CameraConverter>())
            {
                if(updatingCameraConverters.Contains(cameraConverter))
                {
                    if(!cameraConverter.isActiveAndEnabled)
                    {
                        updatingCameraConverters.Remove(cameraConverter);
                    }
                    else
                    {
                        cameraConverter.SetView(settings.is2DNot3D);
                    }
                }
                else if(cameraConverter.isActiveAndEnabled)
                {
                    var camera = cameraConverter.camera;

                    cameraConverter.fieldOfView = camera.fieldOfView;
                    cameraConverter.usePhysicalProperties = camera.usePhysicalProperties;
                    cameraConverter.focalLength = camera.focalLength;
                    cameraConverter.sensorSize = camera.sensorSize;
                    cameraConverter.lensShift = camera.lensShift;
                    cameraConverter.gateFit = camera.gateFit;
                    cameraConverter.orthographicSize = camera.orthographicSize;

                    updatingCameraConverters.Add(cameraConverter);
                }
            }
        }
    }
}

