using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviourScript : MonoBehaviour
{
    public GameObject Player;
    private Transform selfTransform;
    private Vector3 offset = new Vector3(5.0f, 23.0f, -16.0f);
    private float lerpOffset = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        selfTransform = GetComponent<Transform>();
        selfTransform.position = Player.transform.position + offset;
        selfTransform.LookAt(Player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        //selfTransform.LookAt(Player.transform);
        selfTransform.position = Vector3.Lerp(selfTransform.position, Player.transform.position + offset,lerpOffset* Time.deltaTime);
        //selfTransform.position = Player.transform.position + offset;
    }
}
