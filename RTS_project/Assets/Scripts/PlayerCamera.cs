using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    PlayerController controller;
    public Transform trans_Controller;
    private void Awake()
    {
        controller = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        transform.position = trans_Controller.position + Vector3.up * 30f;
    }
}
