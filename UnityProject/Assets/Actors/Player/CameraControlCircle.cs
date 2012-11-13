using UnityEngine;
using System.Collections;

public class CameraControlCircle : MonoBehaviour
{
	#region Public Fields
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2}
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float boundingScale = 0.5f;
	public float turnSpeed = 5f;
	public float sensitivity = 20f;
	//public float minXRotation = -361f;
	//public float maxXRotation = 361f;
	public float minYRotation = -60f;
	public float maxYRotation = 60f;
	public bool travelBack = true;
	public float travelCooldown = 1.5f;
	public float travelSpeed = 0.5f;
	public Texture cursor = null;
	public bool debug = false;
	#endregion
	
	#region Private Fields
	private float centerX = Screen.width/2;
	private float centerY =  Screen.height/2;
	private float minRadius;
	private float maxRadius;
	private float mouseX = 0f;
	private float mouseY = 0f;
	private int frameCount = 0;
	private float rotationX;
	private float rotationY;
	private float elapsedTime = 0f;
	private bool travelling;
	#endregion
	
	#region Private Properties
	private Vector2 MousePosition
	{
		get { return new Vector2(mouseX, mouseY); }
		set { mouseX = value.x; mouseY = value.y; }
	}
	
	private Vector2 CenterScreen
	{
		get { return new Vector2(centerX, centerY); }
		set { centerX = value.x; centerY = value.y; }
	}
	
	private float MouseDistance
	{
		get { return Vector2.Distance (CenterScreen, MousePosition); }
	}
	
	private Vector2 MouseDirection
	{
		get { return (MousePosition - CenterScreen).normalized; }
	}
	#endregion
	
	public void Start()
	{
		Mathf.Clamp (boundingScale, 0f, 1f);
		
		Screen.lockCursor = true;
		
		MousePosition = CenterScreen;
		
		minRadius = centerY * boundingScale;
		maxRadius = Vector2.Distance (CenterScreen, new Vector2(Screen.width, Screen.height));
		
		rotationX = transform.localEulerAngles.y;
		rotationY = transform.localEulerAngles.x;
		
		if (rigidbody)
			rigidbody.freezeRotation = true;
	}
	
	public void Update()
	{
		MoveMouse ();
		
		if (MouseDistance > minRadius)
			RotateSelf (axes);
	}
	
	public void FixedUpdate()
	{
		//TODO: if (debug) DoDebugStuff();
	}
	
	private Vector2 CalculateSpeed()
	{
		Vector2 result = Vector2.zero;
		float percent = MouseDistance / maxRadius;
		
		result = (turnSpeed * MouseDirection) * percent;
		
		/*
		switch (axes)
		{
		case RotationAxes.MouseXAndY:
			return result;
		case RotationAxes.MouseX:
			result.x = 0f;
			return result;
		case RotationAxes.MouseY:
			result.y = 0f;
			return result;
		}
		*/
		
		return result;
	}
	
	private void RotateSelf(RotationAxes axes)
	{
		Vector2 speed = CalculateSpeed ();
		
		switch(axes)
		{
		case RotationAxes.MouseXAndY:
			rotationX = transform.localEulerAngles.y + speed.x;
			//rotationX = Mathf.Clamp (rotationX, minXRotation, maxXRotation);
			
			rotationY += speed.y;
			rotationY = Mathf.Clamp (rotationY, minYRotation, maxYRotation);
			
			transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
			break;
		case RotationAxes.MouseX:
			rotationX = transform.localEulerAngles.y + speed.x;
			//rotationX = Mathf.Clamp (rotationX, minXRotation, maxXRotation);
			
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, rotationX, 0);
			break;
		case RotationAxes.MouseY:
			rotationY += speed.y;
			rotationY = Mathf.Clamp (rotationY, minYRotation, maxYRotation);
			
			transform.localEulerAngles = new Vector3(rotationY, transform.localEulerAngles.y, 0);
			break;
		}
	}

	void MoveMouse ()
	{
		mouseX += Input.GetAxis("Mouse X") * sensitivity;
		mouseX = Mathf.Clamp (mouseX, 0f, (float)Screen.width);
		mouseY += -Input.GetAxis("Mouse Y") * sensitivity;
		mouseY = Mathf.Clamp (mouseY, 0f, (float)Screen.height);
		
		if (travelBack)
		{
			if (Input.GetAxis ("Mouse X") == 0 && Input.GetAxis ("Mouse Y") == 0)
			{
				if (!travelling)
				{
					elapsedTime += Time.deltaTime;
					if (elapsedTime >= travelCooldown)
					{
						elapsedTime = 0f;
						travelling = true;
					}
				}
				if (travelling)
				{
					if (MouseDistance > minRadius / 2)
						MousePosition += -MouseDirection * travelSpeed;
					if (MouseDistance <= minRadius/2)
						travelling = false;
				}
			}
			else
			{
				travelling = false;
				elapsedTime = 0f;
			}
		}
	}
	
	public void OnGUI()
	{
		if (cursor != null)
			GUI.DrawTexture (new Rect(mouseX - (cursor.width), mouseY - (cursor.height), cursor.width * 2, cursor.height * 2), cursor);
		
		if (debug)
		{
			
		}
	}
}

