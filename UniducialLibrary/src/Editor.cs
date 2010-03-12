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
using System.Text;
using UnityEngine;
using UnityEditor;

namespace UniducialLibrary
{
    public class Editor
    {
        private const string FICUCIAL_COMPONENT_NAME = "FiducialController";

        private static Editor instance;

        public static Editor Instance
        {
            get
            {
                if (instance == null)
                {
                    new Editor();
                }

                return instance;
            }
        }

        public Editor()
        {
            if (instance != null)
            {
                Debug.LogError("Trying to create two instances of singleton.");
                return;
            }
            instance = this;
        }

        //attaches a fiducial input component to all selected Game Objects
        public void createFiducialComponent()
        {
            GameObject[] gameObjects = Selection.gameObjects;

            foreach (GameObject gameObject in Selection.gameObjects)
            {
                //make sure a GameObject can only be controlled by one marker
                if (gameObject.GetComponent(FICUCIAL_COMPONENT_NAME) != null)
                {
                    Debug.LogWarning("Game Object " + gameObject.name + " already has a fiducial controller attached");
                }
                else
                {
                    gameObject.AddComponent(FICUCIAL_COMPONENT_NAME);
                }
            }
        }

        //creates a new GameObject with a Fiducial Input component, a 
        public void createFiducialObject()
        {
            GameObject fiducialObject = new GameObject("FiducialObject");
            fiducialObject.AddComponent(typeof(Transform));
            fiducialObject.AddComponent(FICUCIAL_COMPONENT_NAME);
        }
    }
}
