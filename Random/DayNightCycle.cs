using UnityEngine;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public Slider TimeSlider;
    public TextMeshProUGUI timeText;
    public Transform SunTransform;
    public Color Fogday = Color.gray;
    public Color Fognight = Color.black;

    private int minTime = 5 * 60 * 60;  //5 AM
    private int maxTime = 18 * 60 * 60; //6 PM
    private float time;
    private float intensity;

    private void Start()
    {
        TimeSlider.onValueChanged.AddListener(
            ( val ) =>
            {
                time = minTime + ( maxTime - minTime ) * val;
                UpdateSun();
            } );

        TimeSlider.value = 0.2f; //9 AM
    }

    public void UpdateSun()
    {
        timeText.text = GetFormatedTimeStr( time );

        SunTransform.rotation = Quaternion.Euler(new Vector3((time - 21600) / 86400 * 360, 0, 0));
        if (time > 43200)
            intensity = 1 - (43200 - time) / 43200;
        else
            intensity = 1 - ((43200 - time) / 43200 * -1);

        RenderSettings.fogColor = Color.Lerp(Fognight, Fogday, intensity * intensity);

        SunTransform.GetComponent<Light>().intensity = intensity;

    }

    string GetFormatedTimeStr( float timeInSeconds )
    {
        string[] temptime = TimeSpan.FromSeconds(time).ToString().Split(":"[0]);
        return temptime[0] + ":" + temptime[1];
    }

}
