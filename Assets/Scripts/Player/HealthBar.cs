using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    private float targetHealth;
    private float timeScale = 0;

    private bool lerpingHealth;

    
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    
    public void SetHealth(float health)
    {
        targetHealth = health;
        timeScale = 0;

        if (!lerpingHealth)
        {
            StartCoroutine(LerpHealth());
        }
    }

    private IEnumerator LerpHealth()
    {
        float speed = 2;
        float startHealth = slider.value;

        lerpingHealth = true;

        while (timeScale < 1)
        {
            timeScale += Time.deltaTime * speed;
            slider.value = Mathf.Lerp(startHealth, targetHealth, timeScale);

            yield return null;
        }
        lerpingHealth = false;

    }

}
