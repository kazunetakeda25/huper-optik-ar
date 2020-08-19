using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MeasurementManager : MonoBehaviour
{
    private ARRaycastManager m_RaycastManager;
    private GameObject visual;
    private bool isIndicatorVisible = false;
    private bool isPlaceable = true;

    [SerializeField] private GameObject m_PointPrefab;
    [SerializeField] private GameObject m_DistanceTextPrefab;
    [SerializeField] private GameObject m_SettingsPanel;
    [SerializeField] private GameObject m_StopButton;
    [SerializeField] private TextMeshProUGUI m_MeasurementUnitText;
    [SerializeField] private ToggleGroup m_Options;

    [SerializeField] private float m_MeasurementFactor = 0.01f;

    private MeasurementUnit m_MeasurementUnit = MeasurementUnit.CENTIMETER;

    private void Start()
    {
        m_RaycastManager = FindObjectOfType<ARRaycastManager>();
        visual = transform.GetChild(0).gameObject;
        visual.SetActive(false);

        ResetGlobalReferences();

        isPlaceable = true;
        m_StopButton.SetActive(false);

        m_MeasurementUnit = (MeasurementUnit)PlayerPrefs.GetInt("MeasurementUnit", (int)MeasurementUnit.CENTIMETER);
        SetMeasurementFactor((int)m_MeasurementUnit);
    }

    private void SetMeasurementFactor(int index)
    {
        m_MeasurementFactor = GlobalReferences._MeasurementFactors[index];
        m_MeasurementUnitText.text = "Measurement Unit: "
            + GlobalReferences._MeasurementUnitNamesAbbr[index];
    }

    public void OpenSettingsPanel()
    {
        m_SettingsPanel.SetActive(true);

        m_Options.transform.GetChild((int)m_MeasurementUnit).GetComponent<Toggle>().isOn = true;

        //m_RaycastManager.enabled = false;
    }

    public void CloseSettingsPanel()
    {
        int index = m_Options.ActiveToggles().FirstOrDefault().transform.GetSiblingIndex();
        m_MeasurementUnit = (MeasurementUnit)index;
        PlayerPrefs.SetInt("MeasurementUnit", index);
        SetMeasurementFactor(index);

        //if (isPlaceable == true)
        //    m_RaycastManager.enabled = true;

        m_SettingsPanel.SetActive(false);

        RestartScene();
    }

    private void ResetGlobalReferences()
    {
        GlobalReferences._PointIndex = 0;
        GlobalReferences._Points = new List<Transform>();
    }

    private void Update()
    {
        if (isPlaceable == true)
            UpdateIndicatorPose();
    }

    public void StopMeasurement()
    {
        m_RaycastManager.enabled = false;
        isPlaceable = false;
        visual.SetActive(false);
        GlobalReferences._Points[GlobalReferences._Points.Count - 1].GetComponent<LineRenderer>().enabled = false;
        DisablePlanes();
    }

    private void DisablePlanes()
    {
        ARPlaneManager planeManager = FindObjectOfType<ARPlaneManager>();
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        planeManager.enabled = false;
    }

    private void PlacePoint(Transform trans)
    {
        GameObject point = Instantiate(m_PointPrefab, trans.position, trans.rotation);

        GlobalReferences._PointIndex++;
        GlobalReferences._Points.Add(point.transform);

        point.name = "Point_" + GlobalReferences._PointIndex;
        point.GetComponent<LineRenderer>().SetPosition(0, point.transform.position);
        point.GetComponent<LineRenderer>().SetPosition(1, point.transform.position);

        if (GlobalReferences._Points.Count >= 2)
        {
            InstantiateDistanceText(
                GlobalReferences._Points[GlobalReferences._Points.Count - 2],
                GlobalReferences._Points[GlobalReferences._Points.Count - 1]
            );

            m_StopButton.SetActive(true);
        }
    }

    private void InstantiateDistanceText(Transform startPoint, Transform endPoint)
    {
        Vector3 instantiatePosition = Vector3.Lerp(endPoint.position, startPoint.position, 0.5f);
        GameObject distanceText = Instantiate(m_DistanceTextPrefab, instantiatePosition, m_DistanceTextPrefab.transform.rotation);
        distanceText.GetComponentInChildren<TextMeshProUGUI>().text
            = $"{(Vector3.Distance(endPoint.position, startPoint.position) * m_MeasurementFactor).ToString("F2")}"
            + GlobalReferences._MeasurementUnitNamesAbbr[(int)m_MeasurementUnit]; ;
        distanceText.tag = "DistanceText";
    }

    private bool IsPointOverUIObject(Vector2 pos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return false;

        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(pos.x, pos.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private void UpdateIndicatorPose()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (m_RaycastManager.enabled == true
            && m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2),
            hits,
            TrackableType.Planes))
        {
            if (Input.touchCount > 0 && !IsPointOverUIObject(Input.GetTouch(0).position)
                || Input.touchCount == 0)
            {
                isIndicatorVisible = hits.Count > 0;
                if (isIndicatorVisible)
                {
                    transform.position = hits[0].pose.position;
                    transform.rotation = hits[0].pose.rotation;

                    if (GlobalReferences._Points.Count > 0)
                    {
                        GlobalReferences._Points[GlobalReferences._Points.Count - 1]
                            .GetComponent<LineRenderer>().SetPosition(1, transform.position);

                        GetComponentInChildren<TextMeshProUGUI>().text
                            = $"{(Vector3.Distance(GlobalReferences._Points[GlobalReferences._Points.Count - 1].position, transform.position) * m_MeasurementFactor).ToString("F2")}"
                            + GlobalReferences._MeasurementUnitNamesAbbr[(int)m_MeasurementUnit];
                    }

                    if (!visual.activeInHierarchy)
                        visual.SetActive(true);

                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        PlacePoint(transform);
                    }
                }
                else
                    visual.SetActive(false);
            }
        }

        GameObject[] distanceTexts = GameObject.FindGameObjectsWithTag("DistanceText");
        if (distanceTexts.Length == 0)
            return;

        for (int i = 0; i < distanceTexts.Length; i++)
        {
            var lookPos = distanceTexts[i].transform.position - Camera.main.transform.position;
            var lookRotation = Quaternion.LookRotation(lookPos);
            distanceTexts[i].transform.rotation = lookRotation;
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

        SceneManager.LoadSceneAsync((int)BuildSceneNames.Measure);
    }
}