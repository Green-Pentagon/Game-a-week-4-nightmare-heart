using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerBehaviourScript : MonoBehaviour
{
    
    public TextMeshProUGUI timerReadout;

    private PlayerBehaviourScript Player;
    private Color defaultColour = Color.white;
    private float startTimer = 999.0f;
    private float TimeRemaining;

    public float GetRemainingTime()
    {
        return TimeRemaining;
    }

    public void AppendTime(float amount)
    {
        TimeRemaining += amount;
    }

    // Start is called before the first frame update
    void Start()
    {
        Player = GetComponent<PlayerBehaviourScript>();
        TimeRemaining = startTimer;
    }

    private void FixedUpdate()
    {
        if (TimeRemaining > 0)
        {
            if (TimeRemaining < 10.0f)
            {
                timerReadout.color = Color.red;
            }
            else
            {
                timerReadout.color = defaultColour;
            }

            TimeRemaining -= Time.fixedDeltaTime;
            timerReadout.text = (Mathf.Round(TimeRemaining*100)/100).ToString();
            
        }
        else
        {
            TimeRemaining = 0.00f;
            Player.Kill();
        }
    }
}
