/*
Copyright (c) 2010 André Gröschel

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using UnityEngine;
using System.Collections;
using UnityEditor;
using UniducialLibrary;

[CustomEditor(typeof(FiducialController))]
public class UniducialInspector : UnityEditor.Editor
{
    private FiducialController controller;

    public UniducialInspector()
    {



    }

    private void onEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        controller = base.target as FiducialController;
        Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").camera;

        EditorGUILayout.BeginHorizontal();
        controller.markerID = EditorGUILayout.IntField("Marker ID", controller.markerID);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        controller.autoHideGO = EditorGUILayout.Toggle("Auto-hide GameObject", controller.autoHideGO);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();
        controller.isPositionMapped = EditorGUILayout.Toggle("Control Position", controller.isPositionMapped);
        EditorGUILayout.EndHorizontal();

        if (controller.isPositionMapped)
        {

            EditorGUILayout.BeginHorizontal();
            controller.invertX = EditorGUILayout.Toggle("Invert X-Axis", controller.invertX);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            controller.invertY = EditorGUILayout.Toggle("Invert Y-Axis", controller.invertY);
            EditorGUILayout.EndHorizontal();

            if (!mainCamera.isOrthoGraphic && !controller.isAttachedToGUIComponent())
            {
                EditorGUILayout.BeginHorizontal();
                controller.cameraOffset = EditorGUILayout.Slider("Camera offset", controller.cameraOffset, mainCamera.nearClipPlane, mainCamera.farClipPlane);
                EditorGUILayout.EndHorizontal();
            }
        }


        EditorGUILayout.Separator();

        if (!controller.isAttachedToGUIComponent())
        {
            EditorGUILayout.BeginHorizontal();
            controller.isRotationMapped = EditorGUILayout.Toggle("Control Rotation", controller.isRotationMapped);
            EditorGUILayout.EndHorizontal();

            if (controller.isRotationMapped)
            {
                EditorGUILayout.BeginHorizontal();
                controller.rotationAxis = (FiducialController.RotationAxis)EditorGUILayout.EnumPopup("Rotation Axis", controller.rotationAxis);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                controller.rotationMultiplier = EditorGUILayout.Slider("Rotation Factor", controller.rotationMultiplier, 0.01f, 5f);
                EditorGUILayout.EndHorizontal();
            }
        }


        if (GUI.changed)
            EditorUtility.SetDirty(controller);
    }
}
