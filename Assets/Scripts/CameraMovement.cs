using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{

    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    public float speed;

    public GameObject left;
    public GameObject right;
    public GameObject up;
    public GameObject down;
    public GraphicRaycaster graphicRaycaster;


    void Start()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(horizontal, vertical);

        if (moveDirection.sqrMagnitude == 0)
        {
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(pointerEventData, results);

            foreach (RaycastResult hit in results)
            {
                if (hit.gameObject == up) moveDirection += Vector2.up;
                if (hit.gameObject == down) moveDirection += Vector2.down;
                if (hit.gameObject == right) moveDirection += Vector2.right;
                if (hit.gameObject == left) moveDirection += Vector2.left;
            }
        }

       if (moveDirection != Vector2.zero)
        {
            transform.Translate(moveDirection.normalized * speed * Time.deltaTime);
        }
    }

}
