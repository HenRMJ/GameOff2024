using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchFlicker : MonoBehaviour
{
    Light _light;
    [SerializeField] float maxValue = 8f;
    [SerializeField] float minValue = 3f;
    private float noise = 0;
    [SerializeField] float timeRate = 1f;
    public bool flicker;
   
   
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
        StartCoroutine(LightChanger());

    }

    

    IEnumerator LightChanger()
    {
        while (flicker)
        {
            noise = Random.RandomRange(0f, 1f);
            _light.intensity = Mathf.Lerp(minValue, maxValue, noise);
            yield return new WaitForSeconds(timeRate);
        }
      
        
    }
}
