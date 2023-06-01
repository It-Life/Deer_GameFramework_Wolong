using UnityEngine;
using Lean.Touch;
using System.Collections.Generic;
using UnityEngine.U2D;

//[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour {

	public Transform Focus = default;

	[SerializeField, Range(1f, 20f)]
	float distance = 5f;

	[SerializeField, Min(0f)]
	float focusRadius = 5f;

	[SerializeField, Range(0f, 1f)]
	float focusCentering = 0.5f;

	[SerializeField, Range(1f, 360f)]
	float rotationSpeed = 90f;

	[SerializeField, Range(-89f, 89f)]
	float minVerticalAngle = -45f, maxVerticalAngle = 45f;

	[SerializeField, Min(0f)]
	float alignDelay = 5f;

	[SerializeField, Range(0f, 90f)]
	float alignSmoothRange = 45f;

	[SerializeField, Min(0f)]
	float upAlignmentSpeed = 360f;

	[SerializeField]
	LayerMask obstructionMask = -1;

	Camera regularCamera;

	public Vector3 FocusPoint;
	Vector3 previousFocusPoint;

	Vector2 orbitAngles = new Vector2(45f, 0f);

	float lastManualRotationTime;

	Quaternion gravityAlignment = Quaternion.identity;

	Quaternion orbitRotation;


	//用来找到手指使用此组件的方法 
	public LeanFingerFilter Use = new LeanFingerFilter(true);
	Vector2 camInput;


	Vector3 CameraHalfExtends {
		get {
			Vector3 halfExtends;
			halfExtends.y =
				regularCamera.nearClipPlane *
				Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
			halfExtends.x = halfExtends.y * regularCamera.aspect;
			halfExtends.z = 0f;
			return halfExtends;
		}
	}

	void OnValidate () {
		if (maxVerticalAngle < minVerticalAngle) {
			maxVerticalAngle = minVerticalAngle;
		}
	}

	private void OnEnable()
	{
		SpriteAtlasManager.atlasRequested += RequestAtlas;
	}

	private void OnDisable()
	{
		SpriteAtlasManager.atlasRequested -= RequestAtlas;
	}

	void RequestAtlas(string atlasName, System.Action<SpriteAtlas> callback)
	{
		//SpriteAtlas spatlas = AssetBundleManager.Instance.LoadAsset<SpriteAtlas>(GameConfig.GetSpriteAtlasPath(atlasName));
		//callback(spatlas);
	}

	void Awake () {
		regularCamera = GetComponent<Camera>();
		//FocusPoint = Focus.position;
		transform.localRotation = orbitRotation = Quaternion.Euler(orbitAngles);
	}

    void LateUpdate ()
	{
		if (Focus == null) return;

		var fingers = Use.UpdateAndGetFingers();
		LeanDrag(fingers);

		UpdateGravityAlignment();
		UpdateFocusPoint();
		if (ManualRotation() || AutomaticRotation()) {
			ConstrainAngles();
			orbitRotation = Quaternion.Euler(orbitAngles);
		}
		Quaternion lookRotation = gravityAlignment * orbitRotation;

		Vector3 lookDirection = lookRotation * Vector3.forward;
		Vector3 lookPosition = FocusPoint - lookDirection * distance;

		Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
		Vector3 rectPosition = lookPosition + rectOffset;
		Vector3 castFrom = Focus.position;
		Vector3 castLine = rectPosition - castFrom;
		float castDistance = castLine.magnitude;
		Vector3 castDirection = castLine / castDistance;

		if (Physics.BoxCast(
			castFrom, CameraHalfExtends, castDirection, out RaycastHit hit,
			lookRotation, castDistance, obstructionMask,
			QueryTriggerInteraction.Ignore
		)) {
			rectPosition = castFrom + castDirection * hit.distance;
			lookPosition = rectPosition - rectOffset;
		}
		
		transform.SetPositionAndRotation(lookPosition, lookRotation);
	}

	void LeanDrag(List<LeanFinger> fingers)
	{
		if (fingers.Count > 0 && fingers[0].IsOverGui) return;

		var screenDelta = LeanGesture.GetScreenDelta(fingers);
		camInput = new Vector2(-screenDelta.y, screenDelta.x);
		//Debug.Log(screenDelta);
	}

	void UpdateGravityAlignment () {
		Vector3 fromUp = gravityAlignment * Vector3.up;
		Vector3 toUp = CustomGravity.GetUpAxis(FocusPoint);
		float dot = Mathf.Clamp(Vector3.Dot(fromUp, toUp), -1f, 1f);
		float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
		float maxAngle = upAlignmentSpeed * Time.deltaTime;

		Quaternion newAlignment =
			Quaternion.FromToRotation(fromUp, toUp) * gravityAlignment;
		if (angle <= maxAngle) {
			gravityAlignment = newAlignment;
		}
		else {
			gravityAlignment = Quaternion.SlerpUnclamped(
				gravityAlignment, newAlignment, maxAngle / angle
			);
		}
	}

	void UpdateFocusPoint () {

		previousFocusPoint = FocusPoint;
		Vector3 targetPoint = Focus.position;
		if (focusRadius > 0f) {
			float distance = Vector3.Distance(targetPoint, FocusPoint);
			float t = 1f;
			if (distance > 0.01f && focusCentering > 0f) {
				t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
			}
			if (distance > focusRadius) {
				t = Mathf.Min(t, focusRadius / distance);
			}
			FocusPoint = Vector3.Lerp(targetPoint, FocusPoint, t);
		}
		else {
			FocusPoint = targetPoint;
		}
	}

	bool ManualRotation () {
		//Vector2 camInput = new Vector2(
		//	Input.GetAxis("Vertical Camera"),
		//	Input.GetAxis("Horizontal Camera")
		//);
		const float e = 0.001f;
		if (camInput.x < -e || camInput.x > e || camInput.y < -e || camInput.y > e) {
			orbitAngles += rotationSpeed * Time.unscaledDeltaTime * camInput;
			lastManualRotationTime = Time.unscaledTime;
			return true;
		}
		return false;
	}

	bool AutomaticRotation () {
		if (Time.unscaledTime - lastManualRotationTime < alignDelay) {
			return false;
		}

		Vector3 alignedDelta =
			Quaternion.Inverse(gravityAlignment) *
			(FocusPoint - previousFocusPoint);
		Vector2 movement = new Vector2(alignedDelta.x, alignedDelta.z);
		float movementDeltaSqr = movement.sqrMagnitude;
		if (movementDeltaSqr < 0.0001f) {
			return false;
		}

		float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
		float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
		float rotationChange =
			rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
		if (deltaAbs < alignSmoothRange) {
			rotationChange *= deltaAbs / alignSmoothRange;
		}
		else if (180f - deltaAbs < alignSmoothRange) {
			rotationChange *= (180f - deltaAbs) / alignSmoothRange;
		}
		orbitAngles.y =
			Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
		return true;
	}

	void ConstrainAngles () {
		orbitAngles.x =
			Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

		if (orbitAngles.y < 0f) {
			orbitAngles.y += 360f;
		}
		else if (orbitAngles.y >= 360f) {
			orbitAngles.y -= 360f;
		}
	}

	static float GetAngle (Vector2 direction) {
		float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
		return direction.x < 0f ? 360f - angle : angle;
	}
}
