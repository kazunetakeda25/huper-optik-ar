using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PrivacyMainScript : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Camera pCamera;
    [SerializeField] private GameObject[] m_Pages;

    public void OnPointerClick(PointerEventData eventData)
    {
        TextMeshProUGUI proUGUI = GetComponent<TextMeshProUGUI>();

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(proUGUI, Input.mousePosition, pCamera);
        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = proUGUI.textInfo.linkInfo[linkIndex];
            string linkID = linkInfo.GetLinkID();
            switch (linkID)
            {
                case "email":
                    SendEmail("dkratz@huperoptikusa.com");
                    break;
                case "page1":
                    OpenPage(1);
                    break;
                case "page2":
                    OpenPage(2);
                    break;
                case "page3":
                    OpenPage(3);
                    break;
                case "page4":
                    OpenPage(4);
                    break;
                case "page5":
                    OpenPage(5);
                    break;
                case "page6":
                    OpenPage(6);
                    break;
                case "page7":
                    OpenPage(7);
                    break;
                case "page8":
                    OpenPage(8);
                    break;
                case "page9":
                    OpenPage(9);
                    break;
                case "page10":
                    OpenPage(10);
                    break;
                case "page11":
                    OpenPage(11);
                    break;
                case "page12":
                    OpenPage(12);
                    break;
                case "page13":
                    OpenPage(13);
                    break;
                case "privacy":
                    Application.OpenURL("https://www.huperoptikusa.com/privacy-policy/");
                    break;
                case "europa":
                    Application.OpenURL("http://ec.europa.eu/justice/data-protection/bodies/authorities/index_en.htm");
                    break;
                case "choices":
                    Application.OpenURL("http://www.aboutads.info/choices/");
                    break;
                default:
                    break;
            }
        }
    }

    private void SendEmail(string email)
    {
        string subject = CustomEscapeURL("Huper Optik Privacy Policy");
        string body = CustomEscapeURL("");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    private string CustomEscapeURL(string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }

    private void OpenPage(int id)
    {
        for (int i = 0; i < m_Pages.Length; i++)
            m_Pages[i].SetActive(false);

        m_Pages[id].SetActive(true);
    }
}