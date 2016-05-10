using UnityEngine;
using System.Collections;

public class SphereCam : MonoBehaviour {

  public Transform target;
	public Vector3 offset;
	public float sensitivity;

	private float dist;
	private Vector3 dir;
	private Vector3 center;

	void Start () {
		center = target.transform.position + new Vector3(0, 0.5f, 0);
		transform.position = center + offset;
		dir = transform.position - center;
		dist = dir.magnitude;
		dir.Normalize();
	}

	void Update () {
		center = target.transform.position + new Vector3(0, 0.5f, 0);
		if (target)
		{
      if (Input.GetMouseButton(0))
      {
				dir = Quaternion.AngleAxis(sensitivity * - Input.GetAxis("Mouse Y"), transform.right) * dir;
				dir = Quaternion.AngleAxis(sensitivity * Input.GetAxis("Mouse X"), Vector3.up) * dir;
				dir.Normalize();
      }
			if (Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				dist -= 1;
				if (dist <= 0)
					dist = 0.01f;
			}
			else if (Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				dist += 1;
			}
			transform.position = center + dir * dist;
			transform.LookAt(center);
			RaycastHit hit;
			var ray = new Ray(center, dir);
			if (Physics.Raycast(ray, out hit, dist + 0.2f))
			{
				transform.position = center + dir * (hit.distance - 0.2f < 0 ? 0 : hit.distance - 0.2f);
			}
		}
	}
}
