using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{

    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    public float speed;
    public float mushroomMoveDuration; // Duração em segundos para o movimento suave


    public GameObject left;
    public GameObject right;
    public GameObject up;
    public GameObject down;
    public GraphicRaycaster graphicRaycaster;
    public GameObject bigMushroom;


    void Start()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        CameraMovement();
    }

    public void goToBigMushroom()
    {
        StopAllCoroutines();

        Vector3 targetPosition = new Vector3(bigMushroom.transform.position.x, bigMushroom.transform.position.y, transform.position.z);
        StartCoroutine(MoveToPosition(targetPosition));
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        float elapsedTime = 0;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < mushroomMoveDuration)
        {
            transform.position = Vector3.Lerp(startingPosition, target, elapsedTime / mushroomMoveDuration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        
        transform.position = target;
    }

    void CameraMovement()
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
