using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown : MonoBehaviour
{

	public float timeLeft = 300.0f;
	private float origTimeleft;
	public bool stop = true;

	private float minutes;
	private float seconds;

	void Start()
	{
		origTimeleft = timeLeft;
	}

	public Text text;

	public void stopTimer()
	{
		stop = true;
		StopCoroutine(updateCoroutine());
	}

	public void startTimer()
	{
		stop = false;
		timeLeft = origTimeleft;
		Update();
		StartCoroutine(updateCoroutine());
	}

	void Update()
	{
		if (stop) return;
		timeLeft -= Time.deltaTime;

		minutes = Mathf.Floor(timeLeft / 60);
		seconds = timeLeft % 60;
		if (seconds > 59) seconds = 59;
		if (minutes < 0 && seconds <= 0)
		{
			stop = true;
			minutes = 0;
			seconds = 0;
			stopTimer();
			GameManager.instance.gameOver();
		}
		//        fraction = (timeLeft * 100) % 100;
	}

	private IEnumerator updateCoroutine()
	{
		bool red = false;
		while (!stop)
		{
			if (minutes < 1 && seconds < 30)
			{
				red = !red;
				if (red)
				{
				text.fontSize = 20;
					text.color = UnityEngine.Color.red;
				} else
				{
				text.fontSize = 14;
					text.color = UnityEngine.Color.black;
				}
			}
			text.text = string.Format("{0:0}:{1:00}", minutes, seconds);
			yield return new WaitForSeconds(0.2f);
		}
	}
}