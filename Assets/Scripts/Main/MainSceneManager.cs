using NatShare;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject m_FilmVisualizerPanel;
    [SerializeField] private GameObject m_CalculatorPanel;
    [SerializeField] private GameObject m_PhotoGalleryPanel;
    [SerializeField] private Transform m_GalleryContentRoot;
    [SerializeField] private GameObject m_ZoomInView;
    [SerializeField] private GameObject m_GalleryItemPrefab;
    private List<GameObject> m_GalleryItems;
    [Header("Series")]
    [SerializeField] private GameObject m_FusionSeries;
    [SerializeField] private GameObject m_CeramicSeries;
    [SerializeField] private GameObject m_SelectSeries;
    [SerializeField] private GameObject m_DekorativSeries;

    [SerializeField] private GameObject m_LoadingIndicator;

    private void Start()
    {
        m_GalleryItems = new List<GameObject>();

        if (GlobalReferences._LastFilmSeries == FilmType.Fusion)
        {
            m_FilmVisualizerPanel.SetActive(true);
            m_FusionSeries.SetActive(true);
        }
        else if (GlobalReferences._LastFilmSeries == FilmType.Ceramic)
        {
            m_FilmVisualizerPanel.SetActive(true);
            m_CeramicSeries.SetActive(true);
        }
        else if (GlobalReferences._LastFilmSeries == FilmType.Select)
        {
            m_FilmVisualizerPanel.SetActive(true);
            m_SelectSeries.SetActive(true);
        }
        else if (GlobalReferences._LastFilmSeries == FilmType.Dekorativ)
        {
            m_FilmVisualizerPanel.SetActive(true);
            m_DekorativSeries.SetActive(true);
        }
    }

    public void OpenDealerStore()
    {
#if UNITY_EDITOR
        Application.OpenURL(GlobalReferences._DealerStoreLink);
#else
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions
		{
			displayURLAsPageTitle = false,
			pageTitle = "Dealer Store", 
		};

		InAppBrowser.OpenURL(GlobalReferences._DealerStoreLink, options);
#endif
    }

    public void OpenMeasurement()
    {
        GlobalReferences._LastFilmSeries = FilmType.None;

        StartCoroutine(LoadMeasureSceneAsync(1.0f));
    }

    public void OpenCalculator()
    {
        m_CalculatorPanel.SetActive(true);
    }

    public void CloseCalculator()
    {
        m_CalculatorPanel.SetActive(false);
    }

    public void OpenFilmVisualizer()
    {
        m_FilmVisualizerPanel.SetActive(true);
    }

    public void CloseFilmVisualizer()
    {
        m_FilmVisualizerPanel.SetActive(false);
    }

    public void OpenDocuments()
    {
#if UNITY_EDITOR
        Application.OpenURL(GlobalReferences._DocumentsLink);
#else
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions
        {
            displayURLAsPageTitle = false,
            pageTitle = "Specifications",
        };

        InAppBrowser.OpenURL(GlobalReferences._DocumentsLink, options);
#endif
    }

    public void OpenHuperNews()
    {
#if UNITY_EDITOR
        Application.OpenURL(GlobalReferences._HuperNewsLink);
#else
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions
        {
            displayURLAsPageTitle = false,
            pageTitle = "Huper News",
        };

        InAppBrowser.OpenURL(GlobalReferences._HuperNewsLink, options);
#endif
    }

    public void OpenGoogleDrive()
    {
#if UNITY_EDITOR
        Application.OpenURL(GlobalReferences._GoogleDriveLink);
#else
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions
        {
            displayURLAsPageTitle = false,
            pageTitle = "Huper Resources",
        };

        InAppBrowser.OpenURL(GlobalReferences._GoogleDriveLink, options);
#endif
    }

    public void OpenWarrantly()
    {
#if UNITY_EDITOR
        Application.OpenURL(GlobalReferences._WarrantlyLink);
#else
        InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions
        {
            displayURLAsPageTitle = false,
            pageTitle = "Warrantly Portal",
        };

        InAppBrowser.OpenURL(GlobalReferences._WarrantlyLink, options);
#endif
    }

    public void OpenPhotoGallery()
    {
        m_PhotoGalleryPanel.SetActive(true);

        if (GlobalReferences._IsPhotoLoaded == false)
            RefreshPhotoGallery();
    }

    public void RefreshPhotoGallery()
    {
        StartCoroutine(LoadGalleryImages());
    }

    private IEnumerator LoadGalleryImages()
    {
        m_LoadingIndicator.SetActive(true);
        m_GalleryItems = new List<GameObject>();

        WWWForm formData = new WWWForm();
        formData.AddField("secret_key", "4TAVrYMquazQxOIO60IYSHDnOf4kJPzm");

        UnityWebRequest www = UnityWebRequest.Post(GlobalReferences._WPRestAPIEndpoint + "gallery", formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            m_LoadingIndicator.SetActive(false);
            yield break;
        }

        RESTAPI_Response response = JsonUtility.FromJson<RESTAPI_Response>(www.downloadHandler.text);
        if (response == null || response.data == null)
        {
            m_LoadingIndicator.SetActive(false);
            yield break;
        }

        m_LoadingIndicator.SetActive(false);

        string[] imageUrls = response.data.Split(',');

        if (imageUrls == null || imageUrls.Length == 0)
            yield break;

        for (int i = 0; i < imageUrls.Length; i ++)
        {
            GameObject galleryItem = Instantiate(m_GalleryItemPrefab);
            galleryItem.name = (i + 1).ToString();
            galleryItem.transform.SetParent(m_GalleryContentRoot, false);
            m_GalleryItems.Add(galleryItem);
            StartCoroutine(SetGalleryItemImage(imageUrls[i], m_GalleryItems.Count - 1));
            GlobalReferences._IsPhotoLoaded = true;
        }
    }

    private IEnumerator SetGalleryItemImage(string url, int index)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            m_GalleryItems[index].transform.GetChild(0).GetComponent<RawImage>().texture 
                = texture;
            m_GalleryItems[index].transform.GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio 
                = (float)texture.width / (float)texture.height;
            m_GalleryItems[index].GetComponent<Button>().onClick.RemoveAllListeners();
            m_GalleryItems[index].GetComponent<Button>().onClick.AddListener(delegate {
                ZoomInImage(index);
            });
        }

        m_GalleryItems[index].transform.GetChild(2).gameObject.SetActive(false);
    }

    private void ZoomInImage(int index)
    {
        GlobalReferences._SelectedZoomImage = index;
        m_ZoomInView.SetActive(true);
        Texture texture = m_GalleryItems[index].transform.GetChild(0).GetComponent<RawImage>().texture;
        m_ZoomInView.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = texture;
        m_ZoomInView.transform.GetChild(0).GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio
                = (float)texture.width / (float)texture.height;
    }

    public void ShareImage()
    {
        using (var payload = new SharePayload())
        {
            Texture texture = m_ZoomInView.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture;
            payload.AddImage((Texture2D)texture);
        }
    }

    public void DownloadImage()
    {
        using (var payload = new SavePayload())
        {
            Texture texture = m_ZoomInView.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture;
            payload.AddImage((Texture2D)texture);
        }
    }

    public void NextImage()
    {
        if (m_GalleryItems.Count == 0)
            return;

        int index = GlobalReferences._SelectedZoomImage = (GlobalReferences._SelectedZoomImage + 1) % m_GalleryItems.Count;
        Texture texture = m_GalleryItems[index].transform.GetChild(0).GetComponent<RawImage>().texture;
        m_ZoomInView.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = texture;
        m_ZoomInView.transform.GetChild(0).GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio
                = (float)texture.width / (float)texture.height;
    }

    public void PrevImage()
    {
        if (m_GalleryItems.Count == 0)
            return;

        if (GlobalReferences._SelectedZoomImage <= 0)
            GlobalReferences._SelectedZoomImage += m_GalleryItems.Count;

        GlobalReferences._SelectedZoomImage--;

        int index = GlobalReferences._SelectedZoomImage;
        Texture texture = m_GalleryItems[index].transform.GetChild(0).GetComponent<RawImage>().texture;
        m_ZoomInView.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = texture;
        m_ZoomInView.transform.GetChild(0).GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio
                = (float)texture.width / (float)texture.height;
    }

    public void ClosePhotoGallery()
    {
        m_PhotoGalleryPanel.SetActive(false);
    }

    public void ActivateFusionSeries()
    {
        m_FusionSeries.SetActive(true);
    }

    public void ActivateCeramicSeries()
    {
        m_CeramicSeries.SetActive(true);
    }

    public void ActivateSelectSeries()
    {
        m_SelectSeries.SetActive(true);
    }

    public void ActivateDekorativSeries()
    {
        m_DekorativSeries.SetActive(true);
    }

    public void DeactivateFusionSeries()
    {
        m_FusionSeries.SetActive(false);
    }

    public void DeactivateCeramicSeries()
    {
        m_CeramicSeries.SetActive(false);
    }

    public void DeactivateSelectSeries()
    {
        m_SelectSeries.SetActive(false);
    }

    public void DeactivateDekorativSeries()
    {
        m_DekorativSeries.SetActive(false);
    }

    public void LoadARScene(string name)
    {
        if (name.Contains("Fusion"))
        {
            GlobalReferences._MaterialType = FilmType.Fusion;
            GlobalReferences._LastFilmSeries = FilmType.Fusion;
        }
        else if (name.Contains("Ceramic") || name.Contains("Klar"))
        {
            GlobalReferences._MaterialType = FilmType.Ceramic;
            GlobalReferences._LastFilmSeries = FilmType.Ceramic;
        }
        else if (name.Contains("Drei") || name.Contains("Sech"))
        {
            GlobalReferences._MaterialType = FilmType.Select;
            GlobalReferences._LastFilmSeries = FilmType.Select;
        }
        else if (name == "Frost" || name == "Dusted Crystal" || name == "White Out" || name == "Matte Black")
        {
            GlobalReferences._MaterialType = FilmType.Dekorativ;
            GlobalReferences._LastFilmSeries = FilmType.Dekorativ;
        }

        GlobalReferences._Material = name;
        Debug.Log("Material is set to " + GlobalReferences._Material);
        StartCoroutine(LoadARSceneAsync(1.0f));
    }

    private IEnumerator LoadARSceneAsync(float delayedTime)
    {
        yield return new WaitForSeconds(delayedTime);

        SceneManager.LoadScene((int)BuildSceneNames.FilmVisualizer);
    }

    private IEnumerator LoadMeasureSceneAsync(float delayedTime)
    {
        yield return new WaitForSeconds(delayedTime);

        SceneManager.LoadScene((int)BuildSceneNames.Measure);
    }
}