using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Testing : MonoBehaviour
{
    //public SpriteRenderer cellSprite;

    //public GameObject ball;
    //public GameObject cell;

    //Grid grid;

    //private Camera cam;

    //private void Awake()
    //{
    //    grid = new Grid(10, 10, 0.64f);
    //}

    //void Start()
    //{
    //    cam = Camera.main;
    //    Grid grid = new Grid(20, 10, obj.transform.localScale.x);
    //    grid.InitObj(ball, cell, false);
    //    grid.InitiateObjectAtPosition(ball, 5, 5);
    //}

    //private void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        ClickOnObject();

    //        grid.SetValue(GetMouseWorldPosition());
    //    }
    //}

    //public Vector3 GetMousePos(Vector3 position)
    //{
    //    position.z = 0f;
    //    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(position + new Vector3(0, 0, 6f));
    //    Vector2 worldPoint2d = new Vector2(worldPoint.x, worldPoint.y);
    //    Debug.Log("worldPoint2d " + worldPoint2d);
    //    return worldPoint2d;
    //}

    //public Vector3 GetMouseWorldPosition()
    //{
    //    Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    //    vec.z = 0f;
    //    return vec;
    //}
    //public Vector3 GetMouseWorldPositionWithZ()
    //{
    //    return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    //}

    //public Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    //{
    //    return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    //}

    //public Vector3 GetMouseWorldPositionWithZ(Vector3 screepPosition, Camera worldCamera)
    //{
    //    Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screepPosition);
    //    return worldPosition;
    //}

    //private void OnGUI()
    //{

    //    Vector3 point = new Vector3();
    //    Event currentEvent = Event.current;
    //    Vector2 mousePos = new Vector2();

    //    Get the mouse position from Event.
    //    Note that the y position from Event is inverted.
    //   mousePos.x = currentEvent.mousePosition.x;
    //    mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

    //    point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

    //    GUILayout.BeginArea(new Rect(20, 20, 250, 120));
    //    GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
    //    GUILayout.Label("Mouse position: " + mousePos);
    //    GUILayout.Label("World position: " + point.ToString("F3"));
    //    GUILayout.EndArea();

    //}
    //RaycastHit hit;

    //private void ClickOnObject()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        Debug.Log(hit.transform.gameObject.name + " " + hit.point);

    //        grid.DeleteFromArray(GetMousePos(Input.mousePosition));
    //        Destroy(hit.transform.gameObject);

    //        Debug.Log("hit  = " + hit.transform.gameObject.name);
    //    }
    //}
}
