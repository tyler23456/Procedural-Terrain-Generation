using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Spawner2.UserSnap
{
	//adds all characters to a list and attempts to snap them to a newly generated terrain each framerate until they are properly grounded.
	class Snap : MonoBehaviour
	{
		List<CharacterController> controllers = new List<CharacterController>();

		public void Add(CharacterController controller)
		{
			controllers.Add(controller);
		}

		public void Update()
		{
			for (int i = 0; i < controllers.Count; i++)
				if (controllers[i] == null || controllers[i].isGrounded)
				{
					controllers.RemoveAt(i);
					i--;
				}
				else
					TrySnap(controllers[i]);
		}

		void TrySnap(CharacterController controller)
		{
			RaycastHit[] hits;
			hits = Physics.RaycastAll(controller.transform.position + Vector3.up * 1000, Vector3.down, float.PositiveInfinity,
				LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);

			foreach (RaycastHit hit in hits)
				if (hit.collider.name == "Terrain")
					controller.Move(new Vector3(0, -controller.transform.position.y + hit.point.y, 0));				
		}//
	}
}
