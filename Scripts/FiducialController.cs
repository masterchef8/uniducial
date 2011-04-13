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

using System;
using System.Collections.Generic;
using UnityEngine;

public class FiducialController : MonoBehaviour
{
    public int markerID = 0;

    public enum RotationAxis { Forward, Back, Up, Down, Left, Right };

    //translation
    public bool isPositionMapped = false;
    public bool invertX = false;
    public bool invertY = false;

    //rotation
    public bool isRotationMapped = false;
    public bool autoHideGO = false;
    private bool controlsGUIElement = false;

    
    public float cameraOffset = 10;
    public RotationAxis rotationAxis = RotationAxis.Back;
    private UniducialLibrary.TuioManager tuioManager;
    private Camera mainCamera;

    //members
    private Vector2 screenPosition;
    private Vector3 worldPosition;
    private Vector2 direction;
    private float angle;
    private float angleDegrees;
    private float speed;
    private float acceleration;
    private float rotationSpeed;
    private float rotationAcceleration;
    private bool isVisible;

    public float rotationMultiplier = 1;

    void Start()
    {
        tuioManager = UniducialLibrary.TuioManager.Instance;
        //set port explicitly (default is 3333)
        tuioManager.TuioPort = 3334;
        tuioManager.connect();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").camera;

        //check if the game object needs to be transformed in normalized 2d space
        if (isAttachedToGUIComponent())
        {
            Debug.LogWarning("Rotation of GUIText or GUITexture is not supported. Use a plane with a texture instead.");
            controlsGUIElement = true;
        }

        screenPosition = Vector2.zero;
        worldPosition = Vector3.zero;
        direction = Vector2.zero;
        angle = 0f;
        angleDegrees = 0;
        speed = 0f;
        acceleration = 0f;
        rotationSpeed = 0f;
        rotationAcceleration = 0f;
        isVisible = true;
    }

    void Update()
    {
        if (tuioManager.isMarkerAlive(markerID))
        {
            TUIO.TuioObject marker = tuioManager.getMarker(markerID);

            //update parameters
            screenPosition.x = marker.getX();
            screenPosition.y = marker.getY();
            angle = marker.getAngle() * rotationMultiplier;
            angleDegrees = marker.getAngleDegrees() * rotationMultiplier;
            speed = marker.getMotionSpeed();
            acceleration = marker.getMotionAccel();
            rotationSpeed = marker.getRotationSpeed() * rotationMultiplier;
            rotationAcceleration = marker.getRotationAccel();
            direction.x = marker.getXSpeed();
            direction.y = marker.getYSpeed();
            isVisible = true;

            //set game object to visible, if it was hidden before
            showGameObject();

            //update transform component
            updateTransform();
        }
        else
        {
            //automatically hide game object when marker is not visible
            if (autoHideGO)
            {
                hideGameObject();
            }

            isVisible = false;
        }
    }


    void OnApplicationQuit()
    {
        if (tuioManager.IsConnected)
        {
            tuioManager.disconnect();
        }
    }

    private void updateTransform()
    {
        //position mapping
        if (isPositionMapped)
        {
            //calculate world position with respect to camera view direction
            float xPos = screenPosition.x;
            float yPos = screenPosition.y;
            if (invertX) xPos = 1 - xPos;
            if (invertY) yPos = 1 - yPos;

            if (controlsGUIElement)
            {
                transform.position = new Vector3(xPos, 1 - yPos, 0);
            }
            else
            {
                Vector3 position = new Vector3(xPos * mainCamera.GetScreenWidth(), (1 - yPos) * mainCamera.GetScreenHeight(), cameraOffset);
                worldPosition = mainCamera.ScreenToWorldPoint(position);
                //worldPosition += cameraOffset * mainCamera.transform.forward;
                transform.position = worldPosition;
            }
        }

        //rotation mapping
        if (isRotationMapped)
        {
            Quaternion rotation = Quaternion.identity;

            switch (rotationAxis)
            {
                case RotationAxis.Forward:
                    rotation = Quaternion.AngleAxis(angleDegrees, Vector3.forward);
                    break;
                case RotationAxis.Back:
                    rotation = Quaternion.AngleAxis(angleDegrees, Vector3.back);
                    break;
               case RotationAxis.Up:
                    rotation = Quaternion.AngleAxis(angleDegrees, Vector3.up);
                    break;
               case RotationAxis.Down:
                    rotation = Quaternion.AngleAxis(angleDegrees, Vector3.down);
                    break;
               case RotationAxis.Left:
                    rotation = Quaternion.AngleAxis(angleDegrees, Vector3.left);
                    break;
               case RotationAxis.Right:
                    rotation = Quaternion.AngleAxis(angleDegrees, Vector3.right);
                    break;
            }
            transform.localRotation = rotation;
        }
    }

    private void showGameObject()
    {
        if (controlsGUIElement)
        {
            //show GUI components
            if (gameObject.guiText != null && !gameObject.guiText.enabled)
            {
                gameObject.guiText.enabled = true;
            }
            if (gameObject.guiTexture != null && !gameObject.guiTexture.enabled)
            {
                gameObject.guiTexture.enabled = true;
            }
        }
        else
        {
            if (gameObject.renderer != null && !gameObject.renderer.enabled)
            {
                gameObject.renderer.enabled = true;
            }
        }
    }

    private void hideGameObject()
    {
        if (controlsGUIElement)
        {
            //hide GUI components
            if (gameObject.guiText != null && gameObject.guiText.enabled)
            {
                gameObject.guiText.enabled = false;
            }
            if (gameObject.guiTexture != null && gameObject.guiTexture.enabled)
            {
                gameObject.guiTexture.enabled = false;
            }
        }
        else
        {
            //set 3d game object to visible, if it was hidden before
            if (gameObject.renderer != null && gameObject.renderer.enabled)
            {
                gameObject.renderer.enabled = false;
            }
        }
    }

    #region Getter

    public bool isAttachedToGUIComponent()
    {
        return (gameObject.guiText != null || gameObject.guiTexture != null);
    }
    public Vector2 ScreenPosition
    {
        get { return screenPosition; }
    }
    public Vector3 WorldPosition
    {
        get { return worldPosition; }
    }
    public Vector2 MovementDirection
    {
        get { return direction; }
    }
    public float Angle
    {
        get { return angle; }
    }
    public float AngleDegrees
    {
        get { return angleDegrees; }
    }
    public float Speed
    {
        get { return speed; }
    }
    public float Acceleration
    {
        get { return acceleration; }
    }
    public float RotationSpeed
    {
        get { return rotationSpeed; }
    }
    public float RotationAcceleration
    {
        get { return rotationAcceleration; }
    }
    public bool IsVisible
    {
        get { return isVisible; }
    }
    #endregion

}

