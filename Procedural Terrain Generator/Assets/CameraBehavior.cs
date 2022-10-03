using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    CharacterController controller;
    float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0f);

        if (Input.GetKey(KeyCode.W))
            controller.Move(transform.forward * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S))
            controller.Move(-transform.forward * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            controller.Move(-transform.right * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            controller.Move(transform.right * speed * Time.deltaTime);


        float mouseX = Input.GetAxis("Mouse X"); //get x
        float mouseY = Input.GetAxis("Mouse Y"); //get y

        Vector3 movementVector = new Vector3(-mouseY, mouseX, 0f);
        controller.transform.Rotate(movementVector * 1f);



        controller.Move(Vector3.down * 9.8f * Time.deltaTime);
        
       

    }
}
