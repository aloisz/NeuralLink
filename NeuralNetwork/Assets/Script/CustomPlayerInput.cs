using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPlayerInput : MonoBehaviour
{
    [SerializeField] private CustomCarController _carController;
    
    void Update()
    {
        _carController.horizontalInput = Input.GetAxis("Horizontal") ;
        _carController.verticalInput = Input.GetAxis("Vertical");

        _carController.isDrifting = Input.GetKey(KeyCode.Space);    
    }
}
