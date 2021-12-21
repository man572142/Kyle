using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullPowerVFX : MonoBehaviour
{
    [SerializeField] ParticleSystem particle = null;
    [SerializeField] Light light = null;
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var shape = particle.shape;
        shape.radius = Mathf.Lerp(shape.radius, 0.5f, Time.deltaTime * 3f);
        light.intensity = Mathf.Lerp(light.intensity, 1f, Time.deltaTime * 3f);
    }
}
