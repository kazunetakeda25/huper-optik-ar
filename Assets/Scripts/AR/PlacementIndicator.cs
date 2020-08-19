using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementIndicator : MonoBehaviour
{
    private ARRaycastManager m_RaycastManager;
    private GameObject visual;
    private bool isIndicatorVisible = false;
    private bool isPlacable = true;

    [SerializeField] private GameObject m_CornerPointObject;
    [SerializeField] private GameObject m_WindowObject;
    [SerializeField] private GameObject m_GenerateButton;
    [SerializeField] private Toggle m_ViewToggle;

    [SerializeField] private Image m_PreviewButton;
    [SerializeField] private Image m_HomeButton;
    [SerializeField] private Image m_RestartButton;
    [SerializeField] private Image m_ToggleView;

    private void Start()
    {
        m_RaycastManager = FindObjectOfType<ARRaycastManager>();
        visual = transform.GetChild(0).gameObject;
        visual.SetActive(false);

        m_ViewToggle.isOn = GlobalReferences._IsOutside;

        ResetGlobalReferences();
        SetPrimaryButtonColor();
    }

    private void SetPrimaryButtonColor()
    {
        string hexColor = string.Format("#{0:X6}", (int)GlobalReferences._MaterialType);
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            m_PreviewButton.color = color;
            m_HomeButton.color = color;
            m_RestartButton.color = color;
            m_ToggleView.color = color;
        }
    }

    private void ResetGlobalReferences()
    {
        GlobalReferences._PointIndex = 0;
        GlobalReferences._Points = new List<Transform>();
        GlobalReferences._WindowIndex = 0;
        GlobalReferences._Windows = new List<GameObject>();
        GlobalReferences._IsPhotoLoaded = false;
    }
    
    private void Update()
    {
        UpdateIndicatorPose();
    }

    private void PlaceCornerPoint(Transform trans)
    {
        GameObject point = Instantiate(m_CornerPointObject, trans.position, trans.rotation);

        GlobalReferences._PointIndex++;
        GlobalReferences._Points.Add(point.transform);

        point.name = "Point_" + GlobalReferences._PointIndex;

        if (GlobalReferences._Points.Count >= 2)
        {
            EnableGenerateButton();
        }
        else
        {
            DisableGenerateButton();
        }
    }

    private void EnableGenerateButton()
    {
        m_GenerateButton.SetActive(true);
        isPlacable = false;
    }

    private void DisableGenerateButton()
    {
        m_GenerateButton.SetActive(false);
        isPlacable = true;
    }

    public void GenerateWindowMeshs()
    {
        if (GlobalReferences._Points.Count > 2)
        {
            for (int i = 0; i < GlobalReferences._Points.Count - 2; i++)
            {
                GenerateWindowMesh(GlobalReferences._Points[i], GlobalReferences._Points[i + 1]);
            }
        }
        else
        {
            GenerateWindowMesh(GlobalReferences._Points[0], GlobalReferences._Points[1]);
        }

        GlobalReferences._Points = new List<Transform>();
    }

    private void GenerateWindowMesh(Transform startPoint, Transform endPoint)
    {
        GameObject cube = Instantiate(m_WindowObject, m_WindowObject.transform.position, m_WindowObject.transform.rotation);
        float zScale = Vector2.Distance(
            new Vector2(startPoint.position.x, startPoint.position.z),
            new Vector2(endPoint.position.x, endPoint.position.z));

        cube.transform.localScale = new Vector3(cube.transform.localScale.x, cube.transform.localScale.y, zScale);
        cube.transform.position = new Vector3(
            startPoint.position.x + (endPoint.position.x - startPoint.position.x) / 2,
            startPoint.position.y + 1.5f,
            startPoint.position.z + (endPoint.position.z - startPoint.position.z) / 2);

        Vector3 relativePos = endPoint.position - startPoint.position;
        relativePos = new Vector3(relativePos.x, 0, relativePos.z);

        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        cube.transform.rotation = rotation;

        GlobalReferences._WindowIndex++;

        cube.name = "Window_" + GlobalReferences._WindowIndex;
        cube.tag = "Window";

        float windowX = cube.transform.localScale.z;
        float scaleFactor = 10 / windowX;

        Transform canvasTrans = cube.transform.GetChild(0).GetChild(0);
        canvasTrans.localScale = new Vector3(scaleFactor, 3, 1);
        string hexColor = string.Format("#{0:X6}", (int)GlobalReferences._MaterialType);
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
            canvasTrans.GetChild(0).GetComponent<Image>().color = color;

        canvasTrans.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = GlobalReferences._Material;

        if (!string.IsNullOrEmpty(GlobalReferences._Material))
        {
            MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = Resources.Load("Materials/Windows/" + GlobalReferences._Material
                    + (GlobalReferences._IsOutside == false ? "" : "_Outside"), typeof(Material)) as Material;
            }
        }

        if (GlobalReferences._Material == "Dusted Crystal")
            cube.transform.GetChild(1).gameObject.SetActive(true);

        GlobalReferences._Windows.Add(cube);

        DisableGenerateButton();
    }

    public void SwitchMaterial()
    {
        GlobalReferences._IsOutside = !GlobalReferences._IsOutside;
        GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");
        foreach (GameObject window in windows)
        {
            MeshRenderer renderer = window.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = Resources.Load("Materials/Windows/" + GlobalReferences._Material
                    + (GlobalReferences._IsOutside == false ? "" : "_Outside"), typeof(Material)) as Material;
            }
        }
    }

    private bool IsPointOverUIObject(Vector2 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return false;

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(pos.x, pos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void UpdateIndicatorPose()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            if (Input.touchCount > 0 && !IsPointOverUIObject(Input.GetTouch(0).position)
                || Input.touchCount == 0)
            {
                isIndicatorVisible = hits.Count > 0;
                if (isIndicatorVisible)
                {
                    transform.position = hits[0].pose.position;
                    transform.rotation = Quaternion.identity;

                    if (!visual.activeInHierarchy)
                        visual.SetActive(true);

                    if (isPlacable == true && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        PlaceCornerPoint(transform);
                    }
                }
                else
                    visual.SetActive(false);
            }
        }
    }

    public void LoadMainScene()
    {
        ResetGlobalReferences();

        SceneManager.LoadSceneAsync((int)BuildSceneNames.Main);
    }

    public void RestartScene()
    {
        ResetGlobalReferences();

        SceneManager.LoadSceneAsync((int)BuildSceneNames.FilmVisualizer);
    }
}