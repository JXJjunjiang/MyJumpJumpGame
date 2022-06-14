using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJump : MonoBehaviour
{
    private float holdTime;
    private Rigidbody _rigi;
    private Vector3 dir;
    private float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        dir = new Vector3(1f, 0.5f, 0).normalized;
        _rigi = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            holdTime = Time.unscaledTime;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            holdTime = Time.unscaledTime - holdTime;
            Debug.Log(holdTime);
            Jump();
        }
    }

    void Jump()
    {
        _rigi.AddForce(dir * speed * holdTime, ForceMode.Impulse);
    }
}
