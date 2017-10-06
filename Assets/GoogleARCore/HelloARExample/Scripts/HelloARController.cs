//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

namespace GoogleARCore.HelloAR
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;
    using GoogleARCore;

    /// <summary>
    /// Controlls the HelloAR example.
    /// </summary>
    public class HelloARController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera.
        /// </summary>
        public Camera m_firstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject m_trackedPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        //public GameObject m_andyAndroidPrefab;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject m_searchingForPlaneUI;

        private double[] values;
        private Vector3[,] tilesArray;

        private int elementNumber = 0;

        public GameObject cubePrefab;

        public GameObject tilePrefab;
        public List<GameObject> tiles = new List<GameObject>();

        public GameObject parentCubePrefab;

        public ParticleSystem embersEffect;

        public ParticleSystem fireTouch;

        public List<GameObject> cubes = new List<GameObject>();

        private List<TrackedPlane> m_newPlanes = new List<TrackedPlane>();

        private List<TrackedPlane> m_allPlanes = new List<TrackedPlane>();

        private Color[] m_planeColors = new Color[] {
            new Color(1.0f, 1.0f, 1.0f),
            new Color(0.956f, 0.262f, 0.211f),
            new Color(0.913f, 0.117f, 0.388f),
            new Color(0.611f, 0.152f, 0.654f),
            new Color(0.403f, 0.227f, 0.717f),
            new Color(0.247f, 0.317f, 0.709f),
            new Color(0.129f, 0.588f, 0.952f),
            new Color(0.011f, 0.662f, 0.956f),
            new Color(0f, 0.737f, 0.831f),
            new Color(0f, 0.588f, 0.533f),
            new Color(0.298f, 0.686f, 0.313f),
            new Color(0.545f, 0.764f, 0.290f),
            new Color(0.803f, 0.862f, 0.223f),
            new Color(1.0f, 0.921f, 0.231f),
            new Color(1.0f, 0.756f, 0.027f)
        };

        public void RandomizeValues()
        {
            System.Random r = new System.Random();

            values = new double[r.Next(2, 8)];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Math.Round(r.NextDouble() * 10, 2);
            }
        }

        

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update ()
        {
            _QuitOnConnectionErrors();

            // The tracking state must be FrameTrackingState.Tracking in order to access the Frame.
            if (Frame.TrackingState != FrameTrackingState.Tracking)
            {
                const int LOST_TRACKING_SLEEP_TIMEOUT = 15;
                Screen.sleepTimeout = LOST_TRACKING_SLEEP_TIMEOUT;
                return;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Frame.GetNewPlanes(ref m_newPlanes);

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            for (int i = 0; i < m_newPlanes.Count; i++)
            {
                // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                // coordinates.
                GameObject planeObject = Instantiate(m_trackedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                planeObject.GetComponent<TrackedPlaneVisualizer>().SetTrackedPlane(m_newPlanes[i]);

                // Apply a random color and grid rotation.
                planeObject.GetComponent<Renderer>().material.SetColor("_GridColor", m_planeColors[Random.Range(0,
                    m_planeColors.Length - 1)]);
                planeObject.GetComponent<Renderer>().material.SetFloat("_UvRotation", Random.Range(0.0f, 360.0f));
            }

            // Disable the snackbar UI when no planes are valid.
            bool showSearchingUI = true;
            Frame.GetAllPlanes(ref m_allPlanes);
            for (int i = 0; i < m_allPlanes.Count; i++)
            {
                if (m_allPlanes[i].IsValid)
                {
                    showSearchingUI = false;
                    break;
                }
            }

            m_searchingForPlaneUI.SetActive(showSearchingUI);

            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            // Add objects on touch, removes on second touch
            //if (cubes.Count == 0)
            //{
            //    RandomizeValues();
            //    PlaceFireTouch(touch);
            //    for (var i = 0; i < values.Length; i++)
            //    PlaceElement(touch, i);
            //}
            //else
            //{
            //    foreach (var cube in cubes)
            //    {
            //        Destroy(cube);
            //    }
            //    cubes = new List<GameObject>();
            //}

            makeTileArray();

            if (tiles.Count==0)
            {
                PlaceElement3(touch);
            } else
            {
                foreach (var tile in tiles)
                {
                    Destroy(tile);
                }
                tiles = new List<GameObject>();
            }
        }

        private void makeTileArray()
        {
            tilesArray = new Vector3[7,5];
            for (int i = 0; i < tilesArray.GetLength(0); i++)
            {
                for (int j = 0; j < tilesArray.GetLength(1); j++)
                {
                    tilesArray[i, j] = new Vector3(i,0,j);
                }
            }
        }

        private void PlaceFireTouch(Touch touch)
        {
            TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds | TrackableHitFlag.PlaneWithinPolygon;

            TrackableHit hit;
            if (Session.Raycast(m_firstPersonCamera.ScreenPointToRay(touch.position), raycastFilter, out hit))
            {
                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = Session.CreateAnchor(hit.Point, Quaternion.identity);

                // Intanstiate an Andy Android object as a child of the anchor; it's transform will now benefit
                // from the anchor's tracking.
                Instantiate(fireTouch, hit.Point + new Vector3(0, 0, 0), Quaternion.identity, anchor.transform);
            }
        }

        private void PlaceElement(Touch touch, int element)
        {
            TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds | TrackableHitFlag.PlaneWithinPolygon;
            

            TrackableHit hit;
            if (Session.Raycast(m_firstPersonCamera.ScreenPointToRay(touch.position), raycastFilter, out hit))
            {
                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = Session.CreateAnchor(hit.Point, Quaternion.identity);

                // Intanstiate an Andy Android object as a child of the anchor; it's transform will now benefit
                // from the anchor's tracking.
                var cube = Instantiate(cubePrefab, hit.Point + new Vector3(0 + element * 0.1f,(float) values[element]/40.0f,0), Quaternion.identity, anchor.transform);
                cube.GetComponentInChildren<Text>().text = values[element].ToString();

                var embers = Instantiate(embersEffect, hit.Point + new Vector3(0 + element * 0.1f, (float)values[element]/20, 0), Quaternion.identity, anchor.transform);
                embers.transform.Rotate(embers.transform.rotation.x - 90, embers.transform.rotation.y, embers.transform.rotation.z);

                // Scale the green cube
                foreach (Transform child in cube.transform)
                {
                    if (child.gameObject.tag == "GreenCube")
                    {
                        child.transform.localScale += new Vector3(0, (float)values[element] / 20 - child.transform.localScale.y, 0);
                        break;
                    }
                }
                cubes.Add(cube);
            }
        }

        private void PlaceElement2(Touch touch)
        {
            TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds | TrackableHitFlag.PlaneWithinPolygon;

            TrackableHit hit;
            if (Session.Raycast(m_firstPersonCamera.ScreenPointToRay(touch.position), raycastFilter, out hit))
            {
                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = Session.CreateAnchor(hit.Point, Quaternion.identity);

                var parentCube = Instantiate(parentCubePrefab,hit.Point, Quaternion.identity, anchor.transform);

                int element;
                for (var i = 0; i < values.Length; i++)
                {
                    element = i;
                    // Intanstiate an Andy Android object as a child of the anchor; it's transform will now benefit
                    // from the anchor's tracking.
                    var cube = Instantiate(cubePrefab, hit.Point + new Vector3(0 + element * 0.1f, (float)values[element] / 40.0f, 0), Quaternion.identity, parentCube.transform);
                    cube.GetComponentInChildren<Text>().text = values[element].ToString();

                    var embers = Instantiate(embersEffect, hit.Point + new Vector3(0 + element * 0.1f, (float)values[element] / 20, 0), Quaternion.identity, parentCube.transform);
                    embers.transform.Rotate(embers.transform.rotation.x - 90, embers.transform.rotation.y, embers.transform.rotation.z);

                    // Scale the green cube
                    foreach (Transform child in cube.transform)
                    {
                        if (child.gameObject.tag == "GreenCube")
                        {
                            child.transform.localScale += new Vector3(0, (float)values[element] / 20 - child.transform.localScale.y, 0);
                            break;
                        }
                    }
                    cubes.Add(cube);
                    cube.transform.parent = parentCube.transform;
                }
                //parentCube.transform.rotation = Quaternion.AngleAxis(180, new Vector3(0,1,0));
                parentCube.transform.LookAt(m_firstPersonCamera.transform);
                parentCube.transform.rotation = Quaternion.Euler(0.0f, parentCube.transform.rotation.eulerAngles.y+180, parentCube.transform.rotation.z);
            }
        }

        private void PlaceElement3(Touch touch)
        {
            TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds | TrackableHitFlag.PlaneWithinPolygon;
            
            TrackableHit hit;
            if (Session.Raycast(m_firstPersonCamera.ScreenPointToRay(touch.position), raycastFilter, out hit))
            {
                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = Session.CreateAnchor(hit.Point, Quaternion.identity);
                var parentTile = Instantiate(parentCubePrefab, hit.Point, Quaternion.identity, anchor.transform);
                //parentTile.transform.localScale = new Vector3(0.1f, 1f, 0.1f);
                //parentTile.transform.position = new Vector3(hit.Point.x, hit.Point.y, hit.Point.z);
                // Intanstiate an Andy Android object as a child of the anchor; it's transform will now benefit
                // from the anchor's tracking.
                Vector3 centerTransform = new Vector3();

                for (int i = 0; i < tilesArray.GetLength(0); i++)
                {
                    for (int j = 0; j < tilesArray.GetLength(1); j++)
                    {
                        var tile = Instantiate(tilePrefab, hit.Point + new Vector3(0 + tilesArray[i,j].x * 0.104f,0 , 0 + tilesArray[i, j].z * 0.104f), Quaternion.identity, parentTile.transform);
                        tile.transform.parent = parentTile.transform;
                        tiles.Add(tile);
                        if (i==3 && j == 2)
                        {
                            centerTransform = tile.transform.position;
                        }
                    }
                }

                //float newX = Math.Abs(hit.Point.x - centerTransform.x);
                //float newY = hit.Point.y - centerTransform.y;
                //float newZ = hit.Point.z - centerTransform.z;
                //float x2 = Math.Abs(hit.Point.x - newX);

                //parentTile.transform.position = new Vector3(hit.Point.x - newX, hit.Point.y, hit.Point.z);
                parentTile.transform.LookAt(m_firstPersonCamera.transform);
                parentTile.transform.rotation = Quaternion.Euler(0.0f, parentTile.transform.rotation.eulerAngles.y + 180, parentTile.transform.rotation.z);
                //parentTile.transform.position = new Vector3(parentTile.transform.position.x-centerTransform.x, parentTile.transform.position.y, parentTile.transform.position.z);
                //parentTile.transform.position = new Vector3(centerTransform.x, hit.Point.y, hit.Point.z);
                //parentTile.transform.position = m_firstPersonCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, m_firstPersonCamera.nearClipPlane));
            }
        }

        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            // Do not update if ARCore is not tracking.
            if (Session.ConnectionState == SessionConnectionState.DeviceNotSupported)
            {
                _ShowAndroidToastMessage("This device does not support ARCore.");
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                Application.Quit();
            }
            else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed)
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                Application.Quit();
            }
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        /// <param name="length">Toast message time length.</param>
        private static void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
