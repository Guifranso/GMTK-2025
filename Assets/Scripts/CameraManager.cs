using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    // --- VARIÁVEIS DE MOVIMENTO ---
    public float speed;
    public float mushroomMoveDuration;

    // --- VARIÁVEIS DE ZOOM ---
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    // --- VARIÁVEIS DE LIMITE DE CÂMERA ---
    [Header("Camera Limits (at Max Zoom)")] // O nome foi atualizado para clareza
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    // --- REFERÊNCIAS ---
    public GameObject left;
    public GameObject right;
    public GameObject up;
    public GameObject down;
    public GraphicRaycaster graphicRaycaster;
    public GameObject bigMushroom;

    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    private Camera mainCamera;

    // --- ADICIONADO: Variáveis para as bordas do mundo ---
    private float worldBound_Left;
    private float worldBound_Right;
    private float worldBound_Top;
    private float worldBound_Bottom;

    void Start()
    {
        eventSystem = EventSystem.current;
        mainCamera = GetComponent<Camera>();

        // --- ADICIONADO: Calcula as bordas fixas do mundo ---
        // Pega a metade da altura da câmera no zoom máximo (MaxZoom)
        float camHalfHeightAtMaxZoom = maxZoom;
        // Pega a metade da largura, considerando a proporção da tela (aspect ratio)
        float camHalfWidthAtMaxZoom = maxZoom * mainCamera.aspect;

        // Calcula as bordas do mundo com base nos seus limites do Inspector
        worldBound_Left = minX - camHalfWidthAtMaxZoom;
        worldBound_Right = maxX + camHalfWidthAtMaxZoom;
        worldBound_Bottom = minY - camHalfHeightAtMaxZoom;
        worldBound_Top = maxY + camHalfHeightAtMaxZoom;
    }

    void Update()
    {
        CameraMovement();
        HandleZoom();
    }

    void LateUpdate()
    {
        LimitCameraPosition();
    }

    void LimitCameraPosition()
    {
        // --- LÓGICA MODIFICADA PARA LIMITES DINÂMICOS ---

        // Calcula a metade da altura e largura da câmera no zoom ATUAL
        float camHalfHeight = mainCamera.orthographicSize;
        float camHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;

        // Calcula os limites de movimento dinâmicos para a posição central da câmera
        float dynamicMinX = worldBound_Left + camHalfWidth;
        float dynamicMaxX = worldBound_Right - camHalfWidth;
        float dynamicMinY = worldBound_Bottom + camHalfHeight;
        float dynamicMaxY = worldBound_Top - camHalfHeight;

        // Pega a posição atual da câmera
        Vector3 currentPosition = transform.position;

        // Limita os valores de X e Y usando os limites DINÂMICOS calculados
        float clampedX = Mathf.Clamp(currentPosition.x, dynamicMinX, dynamicMaxX);
        float clampedY = Mathf.Clamp(currentPosition.y, dynamicMinY, dynamicMaxY);

        // Define a nova posição da câmera com os valores limitados
        transform.position = new Vector3(clampedX, clampedY, currentPosition.z);
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            float newSize = mainCamera.orthographicSize - scrollInput * zoomSpeed;
            mainCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
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