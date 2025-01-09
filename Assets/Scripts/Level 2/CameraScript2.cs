using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript2 : MonoBehaviour
{
    public float scrollSpeed;
    // By what percentage the scroll speed will increase by
    public float percentIncrease;
    void Start() 
    {
        scrollSpeed /= 100;
    }
    private void FixedUpdate()
    {
        Vector3 temp = transform.position;
        temp.y += scrollSpeed;
        transform.position = temp;
    }

    public void IncreaseScrollSpeed()
    {
        scrollSpeed *= (1 + percentIncrease/100);
    }
}
