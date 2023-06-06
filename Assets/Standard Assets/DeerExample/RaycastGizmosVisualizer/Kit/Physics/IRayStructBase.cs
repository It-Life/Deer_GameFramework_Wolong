using UnityEngine;

namespace Kit.Physic
{
	public interface IRayStruct : IRayStructBase
	{
		/// <summary>hit result cached from last physics check.</summary>
		RaycastHit hitResult { get; }
		/// <summary>Provide vizualize information (Gizmos)</summary>
		/// <param name="color">main color of Gizmos</param>
		/// <param name="hitColor">hit color of Gizmos</param>
		void DrawGizmos(Color color, Color hitColor);
	}

	public interface IRayAllStruct : IRayStructBase
	{
		int hitCount { get; }
		/// <summary>Provide vizualize information (Gizmos)</summary>
		/// <param name="raycastHits">The cached array of Raycast result.</param>
		/// <param name="validArraySize">The cached hit count from result.</param>
		/// <param name="color">main color of Gizmos</param>
		/// <param name="hitColor">hit color of Gizmos</param>
		void DrawAllGizmos(ref RaycastHit[] raycastHits, int validArraySize, Color color = default(Color), Color hitColor = default(Color));
	}

	public interface IOverlapStruct : IRayStructBase
	{
		/// <summary>hit count from last physics check.</summary>
		int hitCount { get; }
		/// <summary>Provide vizualize information (Gizmos)</summary>
		/// <param name="colliderResult">The cached array of collider result.</param>
		/// <param name="validArraySize">The cached hit count from result.</param>
		/// <param name="color">main color of Gizmos</param>
		/// <param name="hitColor">hit color of Gizmos</param>
		void DrawOverlapGizmos(ref Collider[] colliderResult, int validArraySize, Color color = default(Color), Color hitColor = default(Color));
	}

	public interface IRayStructBase
	{
		/// <summary>bool, True if last physics result are hit.</summary>
		bool hitted { get; }
		/// <summary>Reset parameters</summary>
		void Reset();
		/// <summary>Provide vizualize information (Gizmos)</summary>
		string ToString();
	}
}