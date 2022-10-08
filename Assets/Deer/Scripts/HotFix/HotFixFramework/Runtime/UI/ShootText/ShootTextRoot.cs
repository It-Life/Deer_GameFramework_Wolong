// ================================================
//描 述:
//作 者:AlanDu
//创建时间:2022-09-23 15-56-02
//修改作者:AlanDu
//修改时间:2022-09-23 15-56-02
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Please modify the description.
/// </summary>
public class ShootTextRoot : MonoBehaviour
{
	public List<Transform> m_ShootText = new List<Transform>(); 
	public Camera CurBaseCamera
	{
		get
		{
			return GameEntry.Camera.CurUseCamera;
		}
	}
	public Transform CurBaseCameraTrans
	{
		get
		{
			return CurBaseCamera.transform;
		}
	}

	public void AddShootText(Transform shootText) 
	{
		m_ShootText.Add(shootText);
	}
	public void RemoveShootText(Transform shootText)
	{
		m_ShootText.Remove(shootText.transform);
	}
	void Update () 
	{
		if (m_ShootText.Count == 0)
			return;
		m_ShootText.Sort(DistanceCompare);

		for(int i = 0; i < m_ShootText.Count; i++)
			m_ShootText[i].SetSiblingIndex(m_ShootText.Count - (i+1));
	}

	private int DistanceCompare(Transform a, Transform b)
	{
		return Mathf.Abs((WorldPos(a.position) - CurBaseCameraTrans.position).sqrMagnitude).CompareTo(Mathf.Abs((WorldPos(b.position) - CurBaseCameraTrans.position).sqrMagnitude));
	}

	private Vector3 WorldPos(Vector3 pos)
	{
		return CurBaseCamera.ScreenToWorldPoint(pos);
	}
}