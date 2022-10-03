using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDA.Interactibles.UserSnap
{
    public class Snap : MonoBehaviour
    {
        CharacterController controller;

        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<CharacterController>();
            
        }

        // Update is called once per frame
        void Update()
        {
            if (controller == null )
                this.enabled = false;
            else
                TrySnap(controller);
        }

        void TrySnap(CharacterController controller)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(controller.transform.position + Vector3.up * 1000, Vector3.down, float.PositiveInfinity,
                LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);

            foreach (RaycastHit hit in hits)
                if (hit.collider.name.Contains("Terrain"))
                    controller.Move(new Vector3(0, -controller.transform.position.y + hit.point.y, 0));
        }//
    }
}