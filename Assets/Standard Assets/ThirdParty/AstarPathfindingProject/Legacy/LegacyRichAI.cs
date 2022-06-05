using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding.RVO;

namespace Pathfinding.Legacy {
	[RequireComponent(typeof(Seeker))]
	[AddComponentMenu("Pathfinding/Legacy/AI/Legacy RichAI (3D, for navmesh)")]
	/// <summary>
	/// Advanced AI for navmesh based graphs.
	///
	/// Deprecated: Use the RichAI class instead. This class only exists for compatibility reasons.
	/// </summary>
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_legacy_1_1_legacy_rich_a_i.php")]
	public class LegacyRichAI : RichAI {
		/// <summary>
		/// Use a 3rd degree equation for calculating slowdown acceleration instead of a 2nd degree.
		/// A 3rd degree equation can also make sure that the velocity when reaching the target is roughly zero and therefore
		/// it will have a more direct stop. In contrast solving a 2nd degree equation which will just make sure the target is reached but
		/// will usually have a larger velocity when reaching the target and therefore look more "bouncy".
		/// </summary>
		public bool preciseSlowdown = true;

		public bool raycastingForGroundPlacement = false;

		/// <summary>
		/// Current velocity of the agent.
		/// Includes eventual velocity due to gravity
		/// </summary>
		new Vector3 velocity;

		Vector3 lastTargetPoint;
		Vector3 currentTargetDirection;

		protected override void Awake () {
			base.Awake();
			if (rvoController != null) {
				if (rvoController is LegacyRVOController) (rvoController as LegacyRVOController).enableRotation = false;
				else Debug.LogError("The LegacyRichAI component only works with the legacy RVOController, not the latest one. Please upgrade this component", this);
			}
		}

		/// <summary>Smooth delta time to avoid getting overly affected by e.g GC</summary>
		static float deltaTime;

		/// <summary>Update is called once per frame</summary>
		protected override void Update () {
			deltaTime = Mathf.Min(Time.smoothDeltaTime*2, Time.deltaTime);

			if (richPath != null) {
				//System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
				//w.Start();
				RichPathPart pt = richPath.GetCurrentPart();
				var fn = pt as RichFunnel;
				if (fn != null) {
					//Clear buffers for reuse
					Vector3 position = UpdateTarget(fn);

					//tr.position = ps;

					//Only get walls every 5th frame to save on performance
					if (Time.frameCount % 5 == 0 && wallForce > 0 && wallDist > 0) {
						wallBuffer.Clear();
						fn.FindWalls(wallBuffer, wallDist);
					}

					/*for (int i=0;i<wallBuffer.Count;i+=2) {
					 *  Debug.DrawLine (wallBuffer[i],wallBuffer[i+1],Color.magenta);
					 * }*/

					//Pick next waypoint if current is reached
					int tgIndex = 0;
					/*if (buffer.Count > 1) {
					 *  if ((buffer[tgIndex]-tr.position).sqrMagnitude < pickNextWaypointDist*pickNextWaypointDist) {
					 *      tgIndex++;
					 *  }
					 * }*/


					//Target point
					Vector3 tg = nextCorners[tgIndex];
					Vector3 dir = tg-position;
					dir.y = 0;

					bool passedTarget = Vector3.Dot(dir, currentTargetDirection) < 0;
					//Check if passed target in another way
					if (passedTarget && nextCorners.Count-tgIndex > 1) {
						tgIndex++;
						tg = nextCorners[tgIndex];
					}

					if (tg != lastTargetPoint) {
						currentTargetDirection = (tg - position);
						currentTargetDirection.y = 0;
						currentTargetDirection.Normalize();
						lastTargetPoint = tg;
						//Debug.DrawRay (tr.position, Vector3.down*2,Color.blue,0.2f);
					}

					//Direction to target
					dir = (tg-position);
					dir.y = 0;
					float magn = dir.magnitude;

					//Write out for other scripts to read
					distanceToSteeringTarget = magn;

					//Normalize
					dir = magn == 0 ? Vector3.zero : dir/magn;
					Vector3 normdir = dir;

					Vector3 force = Vector3.zero;

					if (wallForce > 0 && wallDist > 0) {
						float wLeft = 0;
						float wRight = 0;

						for (int i = 0; i < wallBuffer.Count; i += 2) {
							Vector3 closest = VectorMath.ClosestPointOnSegment(wallBuffer[i], wallBuffer[i+1], tr.position);
							float dist = (closest-position).sqrMagnitude;

							if (dist > wallDist*wallDist) continue;

							Vector3 tang = (wallBuffer[i+1]-wallBuffer[i]).normalized;

							//Using the fact that all walls are laid out clockwise (seeing from inside)
							//Then left and right (ish) can be figured out like this
							float dot = Vector3.Dot(dir, tang) * (1 - System.Math.Max(0, (2*(dist / (wallDist*wallDist))-1)));
							if (dot > 0) wRight = System.Math.Max(wRight, dot);
							else wLeft = System.Math.Max(wLeft, -dot);
						}

						Vector3 norm = Vector3.Cross(Vector3.up, dir);
						force = norm*(wRight-wLeft);

						//Debug.DrawRay (tr.position, force, Color.cyan);
					}

					//Is the endpoint of the path (part) the current target point
					bool endPointIsTarget = lastCorner && nextCorners.Count-tgIndex == 1;

					if (endPointIsTarget) {
						//Use 2nd or 3rd degree motion equation to figure out acceleration to reach target in "exact" [slowdownTime] seconds

						//Clamp to avoid divide by zero
						if (slowdownTime < 0.001f) {
							slowdownTime = 0.001f;
						}

						Vector3 diff = tg - position;
						diff.y = 0;

						if (preciseSlowdown) {
							//{ t = slowdownTime
							//{ diff = vt + at^2/2 + qt^3/6
							//{ 0 = at + qt^2/2
							//{ solve for a
							dir = (6*diff - 4*slowdownTime*velocity)/(slowdownTime*slowdownTime);
						} else {
							dir = 2*(diff -   slowdownTime*velocity)/(slowdownTime*slowdownTime);
						}
						dir = Vector3.ClampMagnitude(dir, acceleration);

						force *= System.Math.Min(magn/0.5f, 1);

						if (magn < endReachedDistance) {
							//END REACHED
							NextPart();
						}
					} else {
						dir *= acceleration;
					}

					//Debug.DrawRay (tr.position+Vector3.up, dir*3, Color.blue);

					velocity += (dir + force*wallForce)*deltaTime;

					if (slowWhenNotFacingTarget) {
						float dot = (Vector3.Dot(normdir, tr.forward)+0.5f)*(1.0f/1.5f);
						//velocity = Vector3.ClampMagnitude (velocity, maxSpeed * Mathf.Max (dot, 0.2f) );
						float xzmagn = Mathf.Sqrt(velocity.x*velocity.x + velocity.z*velocity.z);
						float prevy = velocity.y;
						velocity.y = 0;
						float mg = Mathf.Min(xzmagn, maxSpeed * Mathf.Max(dot, 0.2f));
						velocity = Vector3.Lerp(tr.forward * mg, velocity.normalized * mg, Mathf.Clamp(endPointIsTarget ? (magn*2) : 0, 0.5f, 1.0f));

						velocity.y = prevy;
					} else {
						// Clamp magnitude on the XZ axes
						float xzmagn = Mathf.Sqrt(velocity.x*velocity.x + velocity.z*velocity.z);
						xzmagn = maxSpeed/xzmagn;
						if (xzmagn < 1) {
							velocity.x *= xzmagn;
							velocity.z *= xzmagn;
							//Vector3.ClampMagnitude (velocity, maxSpeed);
						}
					}

					//Debug.DrawLine (tr.position, tg, lastCorner ? Color.red : Color.green);


					if (endPointIsTarget) {
						Vector3 trotdir = Vector3.Lerp(velocity, currentTargetDirection, System.Math.Max(1 - magn*2, 0));
						RotateTowards(trotdir);
					} else {
						RotateTowards(velocity);
					}

					//Applied after rotation to enable proper checks on if velocity is zero
					velocity += deltaTime * gravity;

					if (rvoController != null && rvoController.enabled) {
						//Use RVOController
						tr.position = position;
						rvoController.Move(velocity);
					} else
					if (controller != null && controller.enabled) {
						//Use CharacterController
						tr.position = position;
						controller.Move(velocity * deltaTime);
					} else {
						//Use Transform
						float lasty = position.y;
						position += velocity*deltaTime;

						position = RaycastPosition(position, lasty);

						tr.position = position;
					}
				} else {
					if (rvoController != null && rvoController.enabled) {
						//Use RVOController
						rvoController.Move(Vector3.zero);
					}
				}

				if (pt is RichSpecial) {
					if (!traversingOffMeshLink) {
						StartCoroutine(TraverseSpecial(pt as RichSpecial));
					}
				}
				//w.Stop();
				//Debug.Log ((w.Elapsed.TotalMilliseconds*1000));
			} else {
				if (rvoController != null && rvoController.enabled) {
					//Use RVOController
					rvoController.Move(Vector3.zero);
				} else
				if (controller != null && controller.enabled) {
				} else {
					tr.position = RaycastPosition(tr.position, tr.position.y);
				}
			}

			UpdateVelocity();
			lastDeltaTime = Time.deltaTime;
		}

		new Vector3 RaycastPosition (Vector3 position, float lasty) {
			if (raycastingForGroundPlacement) {
				RaycastHit hit;
				float up = Mathf.Max(height*0.5f, lasty-position.y+height*0.5f);

				if (Physics.Raycast(position+Vector3.up*up, Vector3.down, out hit, up, groundMask)) {
					if (hit.distance < up) {
						//grounded
						position = hit.point;//.up * -(hit.distance-centerOffset);
						velocity.y = 0;
					}
				}
			}
			return position;
		}

		/// <summary>Rotates along the Y-axis the transform towards trotdir</summary>
		bool RotateTowards (Vector3 trotdir) {
			trotdir.y = 0;
			if (trotdir != Vector3.zero) {
				Quaternion rot = tr.rotation;

				Vector3 trot = Quaternion.LookRotation(trotdir).eulerAngles;
				Vector3 eul = rot.eulerAngles;
				eul.y = Mathf.MoveTowardsAngle(eul.y, trot.y, rotationSpeed*deltaTime);
				tr.rotation = Quaternion.Euler(eul);
				//Magic number, should expose as variable
				return Mathf.Abs(eul.y-trot.y) < 5f;
			}
			return false;
		}
	}
}
