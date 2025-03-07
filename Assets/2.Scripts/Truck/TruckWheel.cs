using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckWheel : MonoBehaviour
{
    private Truck truck;


    // Start is called before the first frame update
    void Awake()
    {
        truck = GetComponentInParent<Truck>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -truck.Speed * 30f * Time.deltaTime);
    }
}
