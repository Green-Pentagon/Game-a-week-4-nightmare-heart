using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehaviourScript : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private Transform stainTransform;
    private float[] rotationRange = { 0.0f,359.0f};
    // Start is called before the first frame update
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        stainTransform = GetComponentInChildren<Transform>();
        stainTransform.Rotate(0.0f,Random.Range(rotationRange[0], rotationRange[1]),0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!particleSystem.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
