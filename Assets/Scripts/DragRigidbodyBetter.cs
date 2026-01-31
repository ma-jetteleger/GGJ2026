using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DevLocker.PhysicsUtils
{
	public class DragRigidbodyBetter : MonoBehaviour
	{
		[Tooltip("The spring force applied when dragging rigidbody. The dragging is implemented by attaching an invisible spring joint.")]
		public float Spring = 50.0f;
		public float Damper = 5.0f;
		public float Drag = 10.0f;
		public float AngularDrag = 5.0f;
		public float Distance = 0.2f;

		private int m_SpringCount = 1;
		private SpringJoint m_SpringJoint;
		private LineRenderer m_SpringRenderer;

		private void Update()
		{
			UpdatePinnedSprings();

			// Mouse not available (e.g. gamepad-only setup)
			if (Mouse.current == null)
				return;

			// Left mouse button pressed this frame
			if (!Mouse.current.leftButton.wasPressedThisFrame)
				return;

			var mainCamera = FindCamera();

			Vector2 mousePos = Mouse.current.position.ReadValue();
			Ray ray = mainCamera.ScreenPointToRay(mousePos);

			if (!Physics.Raycast(ray, out RaycastHit hit, 100f, Physics.DefaultRaycastLayers))
				return;

			if (!hit.rigidbody || hit.rigidbody.isKinematic)
				return;

			if (!m_SpringJoint)
			{
				var go = new GameObject("Rigidbody dragger-" + m_SpringCount);
				go.transform.parent = transform;
				go.transform.localPosition = Vector3.zero;

				Rigidbody body = go.AddComponent<Rigidbody>();
				body.isKinematic = true;

				m_SpringJoint = go.AddComponent<SpringJoint>();
				m_SpringCount++;
			}

			m_SpringJoint.transform.position = hit.point;
			m_SpringJoint.anchor = Vector3.zero;
			m_SpringJoint.spring = Spring;
			m_SpringJoint.damper = Damper;
			m_SpringJoint.maxDistance = Distance;
			m_SpringJoint.connectedBody = hit.rigidbody;

			if (m_SpringRenderer)
				m_SpringRenderer.enabled = true;

			UpdatePinnedSprings();
			StartCoroutine(DragObject(hit.distance));
		}

		private IEnumerator DragObject(float distance)
		{
			var body = m_SpringJoint.connectedBody;

			float oldDrag = body.linearDamping;
			float oldAngularDrag = body.angularDamping;

			body.linearDamping = Drag;
			body.angularDamping = AngularDrag;

			var mainCamera = FindCamera();

			while (
				Mouse.current != null &&
				Mouse.current.leftButton.isPressed
			)
			{
				float scroll = Mouse.current.scroll.ReadValue().y;

				Vector2 mousePos = Mouse.current.position.ReadValue();
				Ray ray = mainCamera.ScreenPointToRay(mousePos);
				m_SpringJoint.transform.position = ray.GetPoint(distance);

				Vector3 connectedPosition =
					body.transform.position +
					body.rotation * m_SpringJoint.connectedAnchor;

				Vector3 axis = m_SpringJoint.transform.position - connectedPosition;

				yield return null;
			}

			if (m_SpringJoint.connectedBody)
			{
				body.linearDamping = oldDrag;
				body.angularDamping = oldAngularDrag;

				m_SpringJoint.connectedBody = null;

				if (m_SpringRenderer)
					m_SpringRenderer.enabled = false;
			}
		}

		private void UpdatePinnedSprings()
		{
			foreach (Transform child in transform)
			{
				var spring = child.GetComponent<SpringJoint>();
				var renderer = child.GetComponentInChildren<LineRenderer>();

				if (!spring || !spring.connectedBody)
					continue;

				Vector3 connectedPosition =
					spring.connectedBody.transform.TransformPoint(spring.connectedAnchor);

				if (renderer && renderer.positionCount >= 2)
				{
					renderer.SetPosition(0, spring.transform.position);
					renderer.SetPosition(1, connectedPosition);
				}
			}
		}

		private Camera FindCamera()
		{
			if (TryGetComponent(out Camera cam))
				return cam;

			return Camera.main;
		}
	}
}
