using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Physics2DxSystem.Editor
{
    [InitializeOnLoad]
    public class Camera2DxEditorUpdater
    {
        private static HashSet<Camera2Dx> updatingCamera2Dxes = new HashSet<Camera2Dx>();

        static Camera2DxEditorUpdater()
        {
            if(!EditorApplication.isPlaying)
            {
                EditorApplication.update += UpdateCamera2Dxes;
            }
            EditorApplication.playModeStateChanged += ChangeCamera2DxUpdateStatus;
        }

        private static void ChangeCamera2DxUpdateStatus(PlayModeStateChange playModeStateChange)
        {
            EditorApplication.update -= UpdateCamera2Dxes;

            if(playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.update += UpdateCamera2Dxes;
            }
        }

        static void UpdateCamera2Dxes()
        {
            var settings = Resources.Load<Settings>(nameof(Settings));
            if(!settings.camera2DxLiveUpdate)
            {
                return;
            }

            foreach(var camera2Dx in Object.FindObjectsOfType<Camera2Dx>())
            {
                if(updatingCamera2Dxes.Contains(camera2Dx))
                {
                    if(!camera2Dx.isActiveAndEnabled)
                    {
                        updatingCamera2Dxes.Remove(camera2Dx);
                    }
                    else
                    {
                        camera2Dx.SetView(settings.is2DNot3D);
                    }
                }
                else if(camera2Dx.isActiveAndEnabled)
                {
                    var camera = camera2Dx.camera;

                    camera2Dx.fieldOfView = camera.fieldOfView;
                    camera2Dx.usePhysicalProperties = camera.usePhysicalProperties;
                    camera2Dx.focalLength = camera.focalLength;
                    camera2Dx.sensorSize = camera.sensorSize;
                    camera2Dx.lensShift = camera.lensShift;
                    camera2Dx.gateFit = camera.gateFit;
                    camera2Dx.orthographicSize = camera.orthographicSize;

                    updatingCamera2Dxes.Add(camera2Dx);
                }
            }
        }
    }
}

