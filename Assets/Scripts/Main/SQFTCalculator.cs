using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SQFTCalculator : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown m_MassUnitDropDown;

    [SerializeField] private TMP_Dropdown m_FilmTypeRegularCoreDropDown;

    [SerializeField] private TMP_InputField m_WeightInputField;
    [SerializeField] private TextMeshProUGUI m_WeightPlaceholderText;

    [SerializeField] private TMP_Dropdown m_CoreLengthDropDown;

    [SerializeField] private TextMeshProUGUI m_ResultText;

    [SerializeField] private Button m_CalculateButton;

    private float m_CalculationFactor = 2.204622f;

    private void OnEnable()
    {
        m_ResultText.text = "";

        int massUnit = PlayerPrefs.GetInt("MassUnit", 0);
        if (massUnit == 0)
        {
            m_MassUnitDropDown.value = 0;
            m_WeightPlaceholderText.text = "Enter Weight (kg) with Core";
        }
        else
        {
            m_MassUnitDropDown.value = 1;
            m_WeightPlaceholderText.text = "Enter Weight (lbs) with Core";
        }
    }

    public void MassUnitChanged(int index)
    {
        if (index == 0)
            SetKgAsDefaultUnit();
        else
            SetLbsAsDefaultUnit();
    }

    private void SetKgAsDefaultUnit()
    {
        PlayerPrefs.SetInt("MassUnit", 0);

        m_WeightPlaceholderText.text = "Enter Weight (kg) with Core";

        if (m_WeightInputField.text.Length > 0)
        {
            bool can = float.TryParse(m_WeightInputField.text, out float lbsValue);
            if (can == true && lbsValue != 0)
            {
                float kgValue = lbsValue / m_CalculationFactor;
                m_WeightInputField.text = kgValue.ToString("F2");
            }
            else
                m_WeightInputField.text = "0";
        }

        if (m_ResultText.text.Length > 0)
        {
            bool can = float.TryParse(m_ResultText.text, out float lbsValue);
            if (can == true && lbsValue != 0)
            {
                float kgValue = lbsValue / m_CalculationFactor;
                m_ResultText.text = (Convert.ToInt32(kgValue)).ToString();
            }
            else
                m_ResultText.text = "0";
        }
    }

    private void SetLbsAsDefaultUnit()
    {
        PlayerPrefs.SetInt("MassUnit", 1);

        m_WeightPlaceholderText.text = "Enter Weight (lbs) with Core";

        if (m_WeightInputField.text.Length > 0)
        {
            bool can = float.TryParse(m_WeightInputField.text, out float kgValue);
            if (can == true && kgValue != 0)
            {
                float lbsValue = kgValue * m_CalculationFactor;
                m_WeightInputField.text = lbsValue.ToString("F2");
            }
            else
                m_WeightInputField.text = "0";
        }

        if (m_ResultText.text.Length > 0)
        {
            bool can = float.TryParse(m_ResultText.text, out float lbsValue);
            if (can == true && lbsValue != 0)
            {
                float kgValue = lbsValue * m_CalculationFactor;
                m_ResultText.text = (Convert.ToInt32(kgValue)).ToString();
            }
            else
                m_ResultText.text = "0";
        }
    }

    public void ValidateInputFields()
    {
        if (m_WeightInputField.text.Length > 0)
        {
            bool can = float.TryParse(m_WeightInputField.text, out float value);
            if (can == true && value > 0)
            {
                m_CalculateButton.interactable = true;
            }
            else
                m_CalculateButton.interactable = false;
        }
        else
            m_CalculateButton.interactable = false;
    }

    public void Calculate()
    {
        int massUnit = PlayerPrefs.GetInt("MassUnit", 0);
        if (massUnit == 0)
        {
            float.TryParse(m_WeightInputField.text, out float kgValue);
            float result = (kgValue - GlobalReferences._CoreLengthInKg[m_CoreLengthDropDown.value])
                / GlobalReferences._FilmTypeRegularCoreInKg[m_FilmTypeRegularCoreDropDown.value];
            m_ResultText.text = (Convert.ToInt32(result)).ToString();
        }
        else
        {
            float.TryParse(m_WeightInputField.text, out float lbsValue);
            float result = (lbsValue - GlobalReferences._CoreLengthInLbs[m_CoreLengthDropDown.value])
                / GlobalReferences._FilmTypeRegularCoreInLbs[m_FilmTypeRegularCoreDropDown.value];
            m_ResultText.text = (Convert.ToInt32(result)).ToString();
        }
    }
}
