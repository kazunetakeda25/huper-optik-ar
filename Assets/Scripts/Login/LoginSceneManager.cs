using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_UsernameInputField;
    [SerializeField] private TMP_InputField m_PasswordInputField;
    [SerializeField] private Toggle m_RememberMe;
    [SerializeField] private TextMeshProUGUI m_StatusText;
    [SerializeField] private Button m_LoginButton;
    [SerializeField] private Button m_RegisterButton;
    [SerializeField] private Button m_PrivacyButton;
    [SerializeField] private GameObject m_LoadingIndicator;

    [SerializeField] private GameObject m_LoginPanel;
    [SerializeField] private GameObject m_PrivacyPanel;

    [SerializeField] private GameObject[] m_PrivacySectionList;

    private void Start()
    {
        m_UsernameInputField.text = PlayerPrefs.GetString("Username", "");
        m_PasswordInputField.text = PlayerPrefs.GetString("Password", "");
        ValidateInputFields();
    }

    public void ValidateInputFields()
    {
        if (m_UsernameInputField.text.Length > 0 && m_PasswordInputField.text.Length > 0)
            m_LoginButton.interactable = true;
        else
            m_LoginButton.interactable = false;
    }

    public void LoginAttempt()
    {
        m_StatusText.text = "Logging in...";
        m_LoginButton.interactable = false;
        m_RegisterButton.interactable = false;
        m_PrivacyButton.interactable = false;
        StartCoroutine(LoginRequest());
    }

    private IEnumerator LoginRequest()
    {
        WWWForm formData = new WWWForm();
        formData.AddField("secret_key", "4TAVrYMquazQxOIO60IYSHDnOf4kJPzm");
        formData.AddField("username", m_UsernameInputField.text);
        formData.AddField("password", m_PasswordInputField.text);

        UnityWebRequest www = UnityWebRequest.Post(GlobalReferences._WPRestAPIEndpoint + "login", formData);
        yield return www.SendWebRequest();

        Debug.Log(www.downloadHandler.text);

        if (www.isNetworkError || www.isHttpError)
        {
            m_StatusText.SetText("Network Error.");
            m_LoginButton.interactable = true;
            m_RegisterButton.interactable = true;
            m_PrivacyButton.interactable = true;
            yield break;
        }

        m_LoginButton.interactable = true;
        m_RegisterButton.interactable = true;
        m_PrivacyButton.interactable = true;
        m_StatusText.SetText("Processing...");

        RESTAPI_Response response = JsonUtility.FromJson<RESTAPI_Response>(www.downloadHandler.text);
        if (response != null)
        {
            bool result = int.TryParse(response.data, out int userId);
            if (result == true && userId >= 0)
            {
                Debug.Log("UserID: " + response.data);
                m_StatusText.SetText("Success");
                LoadMainScene();
                yield break;
            }
        }

        m_StatusText.SetText("Incorect username or password.");
    }

    public void LoadMainScene()
    {
        if (m_RememberMe.isOn == true)
        {
            PlayerPrefs.SetString("Username", m_UsernameInputField.text);
            PlayerPrefs.SetString("Password", m_PasswordInputField.text);
        }
        else
        {
            PlayerPrefs.SetString("Username", "");
            PlayerPrefs.SetString("Password", "");
        }

        m_LoadingIndicator.SetActive(true);
        StartCoroutine(LoadMainSceneAsync(1.0f));
    }

    private IEnumerator LoadMainSceneAsync(float delayedTime)
    {
        yield return new WaitForSeconds(delayedTime);

        SceneManager.LoadSceneAsync((int)BuildSceneNames.Main);
        m_StatusText.text = "";
    }

    public void OpenRegisterPanel()
    {
#if UNITY_EDITOR
        Application.OpenURL(GlobalReferences._HuperOptikDealer);
#else
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions
        {
            displayURLAsPageTitle = false,
            pageTitle = "Become A Huper Optik Dealer",
        };

        InAppBrowser.OpenURL(GlobalReferences._HuperOptikDealer, options);
#endif
    }

    public void OpenLoginPanel()
    {
        m_LoginPanel.SetActive(true);
        m_PrivacyPanel.SetActive(false);
    }

    public void OpenPrivacyPanel()
    {
        m_LoginPanel.SetActive(false);
        m_PrivacyPanel.SetActive(true);
    }

    public void Register()
    {
        
    }

    public void BackToPrivacyMain(int index)
    {
        m_PrivacySectionList[0].SetActive(true);
        m_PrivacySectionList[index].SetActive(false);
    }
}
