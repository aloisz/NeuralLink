using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private CarController _carController;
    
    void Update()
    {
        _carController.horizontalInput = Input.GetAxis("Horizontal") ;
        _carController.verticalInput = Input.GetAxis("Vertical");
    }
}
