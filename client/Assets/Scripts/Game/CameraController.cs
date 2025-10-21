using UnityEngine;

namespace SanguoStrategy.Game
{
    /// <summary>
    /// 相机控制器 - 处理游戏中的相机移动、旋转和缩放
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float edgeScrollThreshold = 20f;
        [SerializeField] private bool enableEdgeScrolling = true;
        
        [Header("Rotation Settings")]
        [SerializeField] private float rotateSpeed = 100f;
        [SerializeField] private KeyCode rotateLeftKey = KeyCode.Q;
        [SerializeField] private KeyCode rotateRightKey = KeyCode.E;
        
        [Header("Zoom Settings")]
        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float minZoomDistance = 5f;
        [SerializeField] private float maxZoomDistance = 20f;
        
        [Header("Bounds")]
        [SerializeField] private bool limitMovement = true;
        [SerializeField] private Vector2 minBounds = new Vector2(-20, -20);
        [SerializeField] private Vector2 maxBounds = new Vector2(20, 20);
        
        private Camera cam;
        private Vector3 lastMousePosition;
        private float currentZoom;
        
        private void Start()
        {
            cam = GetComponent<Camera>();
            currentZoom = transform.position.y;
        }
        
        private void Update()
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();
        }
        
        /// <summary>
        /// 处理相机移动
        /// </summary>
        private void HandleMovement()
        {
            Vector3 moveDirection = Vector3.zero;
            
            // WASD键移动
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                moveDirection += Vector3.forward;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                moveDirection += Vector3.back;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                moveDirection += Vector3.left;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                moveDirection += Vector3.right;
            
            // 边缘滚动
            if (enableEdgeScrolling)
            {
                if (Input.mousePosition.x < edgeScrollThreshold)
                    moveDirection += Vector3.left;
                if (Input.mousePosition.x > Screen.width - edgeScrollThreshold)
                    moveDirection += Vector3.right;
                if (Input.mousePosition.y < edgeScrollThreshold)
                    moveDirection += Vector3.back;
                if (Input.mousePosition.y > Screen.height - edgeScrollThreshold)
                    moveDirection += Vector3.forward;
            }
            
            // 鼠标中键拖动
            if (Input.GetMouseButton(2)) // 中键
            {
                Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
                Vector3 dragDirection = new Vector3(-mouseDelta.x, 0, -mouseDelta.y);
                moveDirection += dragDirection * 0.1f;
            }
            
            lastMousePosition = Input.mousePosition;
            
            // 应用移动
            if (moveDirection != Vector3.zero)
            {
                Vector3 newPosition = transform.position + moveDirection.normalized * moveSpeed * Time.deltaTime;
                
                // 限制边界
                if (limitMovement)
                {
                    newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
                    newPosition.z = Mathf.Clamp(newPosition.z, minBounds.y, maxBounds.y);
                }
                
                transform.position = newPosition;
            }
        }
        
        /// <summary>
        /// 处理相机旋转
        /// </summary>
        private void HandleRotation()
        {
            float rotationDelta = 0f;
            
            if (Input.GetKey(rotateLeftKey))
                rotationDelta = rotateSpeed * Time.deltaTime;
            if (Input.GetKey(rotateRightKey))
                rotationDelta = -rotateSpeed * Time.deltaTime;
            
            if (rotationDelta != 0f)
            {
                transform.RotateAround(transform.position, Vector3.up, rotationDelta);
            }
        }
        
        /// <summary>
        /// 处理相机缩放
        /// </summary>
        private void HandleZoom()
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            
            if (scrollDelta != 0f)
            {
                currentZoom -= scrollDelta * zoomSpeed;
                currentZoom = Mathf.Clamp(currentZoom, minZoomDistance, maxZoomDistance);
                
                Vector3 newPosition = transform.position;
                newPosition.y = currentZoom;
                transform.position = newPosition;
            }
        }
        
        /// <summary>
        /// 聚焦到指定位置
        /// </summary>
        public void FocusOn(Vector3 position, float duration = 0.5f)
        {
            // TODO: 使用协程平滑移动到目标位置
            Vector3 targetPosition = new Vector3(position.x, transform.position.y, position.z - 5);
            transform.position = targetPosition;
        }
    }
}

