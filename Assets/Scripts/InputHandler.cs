using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    public static event EventDelegate OnMouseMoved;
    public static event EventDelegate OnMouseDown;
    public static event EventDelegate OnMouseUp;

    bool onMouseDown;
    Vector3 mousePosition;
    // Start is called before the first frame update
    void Start()
    {
        onMouseDown = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            onMouseDown = true;

            mousePosition = Input.mousePosition;
            OnMouseDown?.Invoke(mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            onMouseDown = false;
            OnMouseUp?.Invoke(mousePosition);
        }
        if (onMouseDown)
        {
            mousePosition = Input.mousePosition;
            OnMouseMoved?.Invoke(mousePosition);
        }
    }
}
