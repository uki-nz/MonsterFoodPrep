using System;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    private float _totalTime;
    private float _timeLeft;

    public Transform CountdownDisplay;
    public Transform CountdownSlider;

    Text _countdownDisplayText;
    Image _countdownSlider;

    // Use this for initialization
    void Start ()
    {
        _totalTime = 5*2.0f;
        _timeLeft = _totalTime;
        
        _countdownDisplayText = CountdownDisplay.GetComponent<Text>();
        _countdownSlider = CountdownSlider.GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
	    if (_timeLeft > 0.0)
	    {
	        _timeLeft -= Time.deltaTime;
	        _countdownDisplayText.text = Math.Round(_timeLeft, 2).ToString();
	        _countdownSlider.fillAmount = _timeLeft / _totalTime;
	    }
	    else
	    {
	        _countdownDisplayText.text = "Time up!";
            // GameOver();
        }
    }
}
