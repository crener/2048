using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Profiling;
using UnityEngine.Serialization;

namespace Code.Gameplay
{
    public class GestureController : MonoBehaviour
    {
        [SerializeField]
        private float minMoveDist = 10f;
        [SerializeField]
        private bool debugLogging = false;

        #region Gesture Actions
        [Serializable]
        public class GestureEvent : UnityEvent { }

        #region Up
        [FormerlySerializedAs("gestureUp"), SerializeField]
        private GestureController.GestureEvent m_Up = new GestureController.GestureEvent();

        public GestureController.GestureEvent GestureUp
        {
            get { return this.m_Up; }
            set { this.m_Up = value; }
        }
        #endregion

        #region Right
        [FormerlySerializedAs("gestureRight"), SerializeField]
        private GestureController.GestureEvent m_Right = new GestureController.GestureEvent();

        public GestureController.GestureEvent GestureRight
        {
            get { return this.m_Right; }
            set { this.m_Right = value; }
        }
        #endregion

        #region Down
        [FormerlySerializedAs("gestureDown"), SerializeField]
        private GestureController.GestureEvent m_Down = new GestureController.GestureEvent();

        public GestureController.GestureEvent GestureDown
        {
            get { return this.m_Down; }
            set { this.m_Down = value; }
        }
        #endregion

        #region Left
        [FormerlySerializedAs("gestureLeft"), SerializeField]
        private GestureController.GestureEvent m_Left = new GestureController.GestureEvent();

        public GestureController.GestureEvent GestureLeft
        {
            get { return this.m_Left; }
            set { this.m_Left = value; }
        }
        #endregion
        #endregion

        private Dictionary<int, Vector2> startPos = new Dictionary<int, Vector2>();
        private Vector2 mousePos = Vector2.zero;
        private bool keyboardDown = false;

        void Start() { }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        startPos.Add(touch.fingerId, touch.position);
                        continue;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        if (Vector3.Distance(touch.position, startPos[touch.fingerId]) <= minMoveDist)
                        {
                            //didn't move far enough, so ignore it and remove the point
#if UNITY_EDITOR
                            Debug.Log("Gesture distance too low! dist: " +
                                      Vector3.Distance(touch.position, startPos[touch.fingerId]));
#endif
                            startPos.Remove(touch.fingerId);
                            continue;
                        }

                        Vector2 delta = (touch.position - startPos[touch.fingerId]).normalized;
                        Debug.Log("touch delta X" + delta.x + " Y" + delta.y);

                        CalculateGestureDirection(startPos[touch.fingerId], touch.position);

                        startPos.Remove(touch.fingerId);
                        continue;
                    }
                    else if (touch.phase == TouchPhase.Canceled)
                    {
                        startPos.Remove(touch.fingerId);
                        continue;
                    }
                }
            }
            //check the mouse
            else if (Input.GetMouseButtonDown(0))
            {
                mousePos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (Vector3.Distance(mousePos, Input.mousePosition) <= minMoveDist)
                {
                    //didn't move far enough, so ignore it and remove the point
#if UNITY_EDITOR
                    Debug.Log("Gesture distance too low! dist: " +
                              Vector3.Distance(mousePos, Input.mousePosition));
#endif
                    return;
                }

                CalculateGestureDirection(mousePos, new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            }
            else
            {
                //check buttons
                bool left = Input.GetButtonDown("left");
                bool down = Input.GetButtonDown("down");
                bool up = Input.GetButtonDown("up");
                bool right = Input.GetButtonDown("right");

                if(left && !keyboardDown) MoveLeft();
                else if (down && !keyboardDown) MoveDown();
                else if (up && !keyboardDown) MoveUp();
                else if(right && !keyboardDown) MoveRight();
                else if (keyboardDown) keyboardDown = false;
            }
        }

        void CalculateGestureDirection(Vector2 initialPosition, Vector2 currentPosition)
        {
            Vector2 delta = (currentPosition - initialPosition).normalized;
            CalculateGestureDirection(delta);
        }

        void CalculateGestureDirection(Vector2 NormalizedDelta)
        {
            Vector2 greater = new Vector2(Mathf.Abs(NormalizedDelta.x), Mathf.Abs(NormalizedDelta.y));

            if (greater.x > greater.y)
            {
                //work out if left or right is more prominent
                if (NormalizedDelta.x > 0) MoveRight();
                else MoveLeft();
            }
            else if (greater.x < greater.y)
            {
                //work out if up or down is more prominent
                if (NormalizedDelta.y > 0) MoveUp();
                else MoveDown();
            }
        }

        private void MoveRight()
        {
#if UNITY_EDITOR
            if (debugLogging) Debug.Log("right");
#endif
            Profiler.BeginSample("RightInvoke");
            m_Right.Invoke();
            Profiler.EndSample();
        }

        private void MoveLeft()
        {
#if UNITY_EDITOR
            if (debugLogging) Debug.Log("left");
#endif

            Profiler.BeginSample("LeftInvoke");
            m_Left.Invoke();
            Profiler.EndSample();
        }

        private void MoveUp()
        {
#if UNITY_EDITOR
            if (debugLogging) Debug.Log("up");
#endif

            Profiler.BeginSample("UpInvoke");
            m_Up.Invoke();
            Profiler.EndSample();
        }

        private void MoveDown()
        {
#if UNITY_EDITOR
            if (debugLogging) Debug.Log("down");
#endif
            
            Profiler.BeginSample("DownInvoke");
            m_Down.Invoke();
            Profiler.EndSample();
        }
    }
}