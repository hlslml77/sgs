using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using SanguoStrategy.Network;

namespace SanguoStrategy.UI
{
    /// <summary>
    /// 登录/注册控制器
    /// </summary>
    public class LoginController : MonoBehaviour
    {
        [Header("UI Panel References")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;
        
        [Header("Login UI")]
        [SerializeField] private TMP_InputField loginUsernameInput;
        [SerializeField] private TMP_InputField loginPasswordInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button showRegisterButton;
        
        [Header("Register UI")]
        [SerializeField] private TMP_InputField registerUsernameInput;
        [SerializeField] private TMP_InputField registerEmailInput;
        [SerializeField] private TMP_InputField registerPasswordInput;
        [SerializeField] private TMP_InputField registerConfirmPasswordInput;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button showLoginButton;
        
        [Header("Feedback")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private GameObject loadingPanel;
        
        [Header("Network")]
        [SerializeField] private string apiUrl = "http://localhost:8080/api/v1";
        
        private ApiClient apiClient;
        
        private void Awake()
        {
            // ⚡ 先修复输入系统（确保 GraphicRaycaster 和 EventSystem 存在）
            var fixer = gameObject.AddComponent<RuntimeInputFixer>();
            fixer.FixInputSystem();
            Destroy(fixer); // 修复后移除组件
        }
        
        private void Start()
        {
            InitializeButtons();
            ShowLoginPanel();
            
            // 初始化 API 客户端
            apiClient = FindObjectOfType<ApiClient>();
            if (apiClient == null)
            {
                GameObject apiObj = new GameObject("ApiClient");
                apiClient = apiObj.AddComponent<ApiClient>();
                DontDestroyOnLoad(apiObj);
            }
            
            // 检查是否已有保存的 token
            string savedToken = PlayerPrefs.GetString("AuthToken", "");
            if (!string.IsNullOrEmpty(savedToken))
            {
                // 直接进入主菜单（服务器会验证token）
                ShowMessage("检测到已保存的登录信息，正在进入游戏...", Color.green);
                Invoke("LoadMainMenu", 1f);
            }
        }
        
        private void InitializeButtons()
        {
            if (loginButton != null)
                loginButton.onClick.AddListener(OnLoginClick);
                
            if (registerButton != null)
                registerButton.onClick.AddListener(OnRegisterClick);
                
            if (showRegisterButton != null)
                showRegisterButton.onClick.AddListener(ShowRegisterPanel);
                
            if (showLoginButton != null)
                showLoginButton.onClick.AddListener(ShowLoginPanel);
        }
        
        private void ShowLoginPanel()
        {
            if (loginPanel != null)
                loginPanel.SetActive(true);
            if (registerPanel != null)
                registerPanel.SetActive(false);
            ClearMessage();
        }
        
        private void ShowRegisterPanel()
        {
            if (loginPanel != null)
                loginPanel.SetActive(false);
            if (registerPanel != null)
                registerPanel.SetActive(true);
            ClearMessage();
        }
        
        private void OnLoginClick()
        {
            string username = loginUsernameInput?.text ?? "";
            string password = loginPasswordInput?.text ?? "";
            
            // 验证输入
            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("请输入用户名", Color.red);
                return;
            }
            
            if (string.IsNullOrEmpty(password))
            {
                ShowMessage("请输入密码", Color.red);
                return;
            }
            
            // 显示加载中
            SetLoading(true);
            
            // 调用登录 API
            StartCoroutine(apiClient.Login(username, password, (success, message) =>
            {
                SetLoading(false);
                
                if (success)
                {
                    ShowMessage("登录成功！正在进入游戏...", Color.green);
                    Invoke("LoadMainMenu", 1f);
                }
                else
                {
                    ShowMessage($"登录失败: {message}", Color.red);
                }
            }));
        }
        
        private void OnRegisterClick()
        {
            string username = registerUsernameInput?.text ?? "";
            string email = registerEmailInput?.text ?? "";
            string password = registerPasswordInput?.text ?? "";
            string confirmPassword = registerConfirmPasswordInput?.text ?? "";
            
            // 验证输入
            if (string.IsNullOrEmpty(username))
            {
                ShowMessage("请输入用户名", Color.red);
                return;
            }
            
            if (username.Length < 3 || username.Length > 20)
            {
                ShowMessage("用户名长度必须在3-20个字符之间", Color.red);
                return;
            }
            
            if (string.IsNullOrEmpty(email))
            {
                ShowMessage("请输入邮箱", Color.red);
                return;
            }
            
            if (!IsValidEmail(email))
            {
                ShowMessage("请输入有效的邮箱地址", Color.red);
                return;
            }
            
            if (string.IsNullOrEmpty(password))
            {
                ShowMessage("请输入密码", Color.red);
                return;
            }
            
            if (password.Length < 6)
            {
                ShowMessage("密码长度至少为6个字符", Color.red);
                return;
            }
            
            if (password != confirmPassword)
            {
                ShowMessage("两次输入的密码不一致", Color.red);
                return;
            }
            
            // 显示加载中
            SetLoading(true);
            
            // 调用注册 API
            StartCoroutine(apiClient.Register(username, email, password, (success, message) =>
            {
                SetLoading(false);
                
                if (success)
                {
                    ShowMessage("注册成功！正在进入游戏...", Color.green);
                    Invoke("LoadMainMenu", 1f);
                }
                else
                {
                    ShowMessage($"注册失败: {message}", Color.red);
                }
            }));
        }
        
        private void LoadMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
        
        private void ShowMessage(string message, Color color)
        {
            if (messageText != null)
            {
                messageText.text = message;
                messageText.color = color;
                messageText.gameObject.SetActive(true);
            }
        }
        
        private void ClearMessage()
        {
            if (messageText != null)
            {
                messageText.text = "";
                messageText.gameObject.SetActive(false);
            }
        }
        
        private void SetLoading(bool isLoading)
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(isLoading);
                
            // 禁用按钮防止重复点击
            if (loginButton != null)
                loginButton.interactable = !isLoading;
            if (registerButton != null)
                registerButton.interactable = !isLoading;
        }
        
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

