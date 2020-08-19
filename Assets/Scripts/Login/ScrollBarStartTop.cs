using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarStartTop : MonoBehaviour
{
    private Scrollbar bar;

    private void OnEnable()
    {
        StartCoroutine(SetScrollBar());
    }

    private IEnumerator SetScrollBar()
    {
        yield return null;
        bar = GetComponentInChildren<Scrollbar>();
        bar.value = 1;
    }
}