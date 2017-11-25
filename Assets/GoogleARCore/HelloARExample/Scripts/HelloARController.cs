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
using UnityEngine.EventSystems;
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

        public List<GameObject> planes = new List<GameObject>();

        private Spawner spawnerScript;

        public int[,] boardItemsArray;
        private int levelSizeX = 18;
        private int levelSizeZ = 14;

        private int emptyPlacement = 0;
        private int playerPlacement = -1;
        private int pickupPlacement = 1;
        private int wallPlacement = 2;
        private int goalPlacement = 3;
        private int obstaclePlacement = 4;

        private int elementNumber = 0;

        public GameObject cubePrefab;

        public GameObject tilePrefab;
        public List<GameObject> tiles = new List<GameObject>();
        public GameObject[,] tilesArray;

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

        private SessionComponent _arCoreDevice;
        public Renderer _rend;

        public void Start()
        {
            spawnerScript = gameObject.GetComponent<Spawner>();
            makePickupArray();
            _arCoreDevice = GameObject.FindGameObjectWithTag("ARCoreDevice").GetComponent<SessionComponent>();
            _rend = tilePrefab.GetComponent<Renderer>();
            _rend.material.shader = Shader.Find("Unlit/GrayscaleTexture");
            _rend.material.SetFloat("_ColorLevel", 0f);
        }       

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update ()
        {
            _QuitOnConnectionErrors();

            // Quit Unity if user presses the back-button on Android device
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

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
                //planeObject.GetComponent<Renderer>().material.SetColor("_GridColor", m_planeColors[Random.Range(0,
                //    m_planeColors.Length - 1)]);
                planeObject.GetComponent<Renderer>().material.SetColor("_GridColor", Color.green);
                planeObject.GetComponent<Renderer>().material.SetFloat("_UvRotation", Random.Range(0.0f, 360.0f));
                planes.Add(planeObject);
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
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began || IsUiTouch(touch))
            {
                return;
            }

            // Add objects on touch, removes on second touch
            if (tiles.Count==0)
            {
                //Creates array of pickups on the gameboard
                makePickupArray();
                foreach (GameObject plane in planes)
                {
                    plane.SetActive(false);
                }
                _arCoreDevice.m_arSessionConfig.m_enablePlaneFinding = false; // Disable plane tracking in AR CORE
                PlaceElement(touch);

                
            } else
            {
                foreach (GameObject plane in planes)
                {
                    plane.SetActive(true);
                }
                foreach (var tile in tiles)
                {
                    Destroy(tile);
                }
                tiles = new List<GameObject>();
                spawnerScript.DestroyAll();
                _arCoreDevice.m_arSessionConfig.m_enablePlaneFinding = true; // Enable plane tracking in AR CORE
            }
        }

        private bool IsUiTouch(Touch touch)
        {
            int id = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                // ui touched
                return true;
            }
            return false;
        }

        private bool IsObstacle(int i, int j)
        {
            return (
                (i == 0 && j == 5) || 
                (i == 1 && j == 2) ||
                (i == 1 && j == 4) ||
                (i == 2 && j == 1) ||
                (i == 2 && j == 9) ||
                (i == 2 && j == 11) ||
                (i == 3 && j == 4) || 
                (i == 3 && j == 6) ||
                (i == 4 && j == 2) ||
                (i == 4 && j == 3) ||
                (i == 4 && j == 9) ||
                (i == 4 && j == 11) ||
                (i == 5 && j == 3) ||
                (i == 6 && j == 8) ||
                (i == 6 && j == 9) ||
                (i == 7 && j == 3) ||
                (i == 7 && j == 11) ||
                (i == 8 && j == 2) ||
                (i == 8 && j == 6) ||
                (i == 8 && j == 7) ||
                (i == 8 && j == 8) ||
                (i == 8 && j == 11) ||
                (i == 10 && j == 1) ||
                (i == 10 && j == 5) ||
                (i == 10 && j == 7) ||
                (i == 11 && j == 1) ||
                (i == 11 && j == 4) ||
                (i == 11 && j == 5) ||
                (i == 12 && j == 1) ||
                (i == 13 && j == 6) ||
                (i == 14 && j == 6) ||
                (i == 14 && j == 7) ||
                (i == 14 && j == 9) ||
                (i == 15 && j == 9)
                );
        }

        private bool IsWall(int i, int j)
        {
            return (
                (i == 1 && j == 6) ||
                (i == 1 && j == 7) ||
                (i == 2 && j == 6) ||
                (i == 2 && j == 7) ||
                (i == 6 && j == 4) ||
                (i == 6 && j == 5) ||
                (i == 7 && j == 4) ||
                (i == 7 && j == 5) ||
                (i == 8 && j == 4) ||
                (i == 8 && j == 5) ||
                (i == 11 && j == 7) ||
                (i == 11 && j == 8) ||
                (i == 11 && j == 9) ||
                (i == 12 && j == 7) ||
                (i == 12 && j == 8) ||
                (i == 12 && j == 9) ||
                (i == 13 && j == 7) ||
                (i == 13 && j == 8) ||
                (i == 13 && j == 9)
                );
        }

        private bool IsPickup(int i, int j)
        {
            return (
                (i == 1 && j == 0) ||
                (i == 2 && j == 0) ||
                (i == 3 && j == 0) ||
                (i == 4 && j == 0) ||
                (i == 0 && j == 12) || 
                (i == 6 && j == 6) || 
                (i == 10 && j == 2) || 
                (i == 10 && j == 8)
                );
        }

        private void makePickupArray()
        {
            boardItemsArray = new int[levelSizeX, levelSizeZ];

            for (int i = 0; i < boardItemsArray.GetLength(0); i++)
            {
                for (int j = 0; j < boardItemsArray.GetLength(1); j++)
                {
                    if (IsPickup(i,j))
                    {
                        boardItemsArray[i, j] = pickupPlacement;
                    }
                    else if (IsObstacle(i,j))                        
                    {
                        boardItemsArray[i, j] = obstaclePlacement;
                    }
                    else if (IsWall(i,j))
                    {
                        boardItemsArray[i, j] = wallPlacement;
                    }
                    else
                    {
                        boardItemsArray[i, j] = emptyPlacement;
                    }
                }
            }
            boardItemsArray[0, 0] = playerPlacement;
            boardItemsArray[16, 11] = goalPlacement;
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

        private void PlaceElement(Touch touch)
        {
            TrackableHitFlag raycastFilter = TrackableHitFlag.PlaneWithinBounds | TrackableHitFlag.PlaneWithinPolygon;
            
            TrackableHit hit;
            if (Session.Raycast(m_firstPersonCamera.ScreenPointToRay(touch.position), raycastFilter, out hit))
            {
                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                //Anchor anchor = Session.CreateAnchor(hit.Point, Quaternion.identity);

                GameObject parentTile = Instantiate(parentCubePrefab, hit.Point, Quaternion.identity);
                tilesArray = new GameObject[levelSizeX, levelSizeZ];

                // Intanstiate an tile objects as a child of the anchor; it's transform will now benefit
                // from the anchor's tracking.
                for (int i = 0; i < levelSizeX; i++)
                {
                    for (int j = 0; j < levelSizeZ; j++)
                    {
                        if (boardItemsArray[i,j]!=2)
                        {
                            Vector3 tilePos = hit.Point + new Vector3(0 + i * 0.104f, 0f, 0 + j * 0.104f);
                            GameObject tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, parentTile.transform);
                            tile.transform.parent = parentTile.transform;
                            tiles.Add(tile);
                            tilesArray[i, j] = tile;
                        }
                    }
                }
                parentTile.transform.LookAt(m_firstPersonCamera.transform);
                parentTile.transform.rotation = Quaternion.Euler(0.0f, parentTile.transform.rotation.eulerAngles.y + 180, parentTile.transform.rotation.z);
                spawnerScript.SpawnAll(boardItemsArray, tilesArray,m_firstPersonCamera);
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
