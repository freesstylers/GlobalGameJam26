using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private void Update()
    {
		transform.LookAt(Camera.main.transform.position, Vector3.up);
	}
}
