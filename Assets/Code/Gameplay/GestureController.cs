using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

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

    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            foreach(Touch touch in Input.touches)
            {
                if(touch.phase == TouchPhase.Began)
                {
                    startPos.Add(touch.fingerId, touch.position);
                    continue;
                }
                else if(touch.phase == TouchPhase.Ended)
                {
                    if(Vector3.Distance(touch.position, startPos[touch.fingerId]) <= minMoveDist)
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
                else if(touch.phase == TouchPhase.Canceled)
                {
                    startPos.Remove(touch.fingerId);
                    continue;
                }
            }
        }
        else if(Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if(Vector3.Distance(mousePos, Input.mousePosition) <= minMoveDist)
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
    }

    void CalculateGestureDirection(Vector2 initialPosition, Vector2 currentPosition)
    {
        Vector2 delta = (currentPosition - initialPosition).normalized;
        CalculateGestureDirection(delta);
    }

    void CalculateGestureDirection(Vector3 NormalizedDelta)
    {
        Vector2 greater = new Vector2(Mathf.Abs(NormalizedDelta.x), Mathf.Abs(NormalizedDelta.y));

        if(greater.x > greater.y)
        {
            //work out if left or right is more promenant
            if(NormalizedDelta.x > 0)
            {
#if UNITY_EDITOR
                Debug.Log("right");
#endif
                m_Right.Invoke();
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("left");
#endif
                m_Left.Invoke();
            }
        }
        else if(greater.x < greater.y)
        {
            //work out if up or down is more promenant
            if(NormalizedDelta.y > 0)
            {
#if UNITY_EDITOR
                Debug.Log("up");
#endif
                m_Up.Invoke();
            }
            else
            {
#if UNITY_EDITOR
                Debug.Log("down");
#endif
                m_Down.Invoke();
            }
        }
    }
}