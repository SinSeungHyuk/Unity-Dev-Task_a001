using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTest : MonoBehaviour
{
    private TruckTest truck;


    // Start is called before the first frame update
    void Awake()
    {
        truck = GetComponentInParent<TruckTest>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -truck.Speed * 20f * Time.deltaTime);
    }
}
