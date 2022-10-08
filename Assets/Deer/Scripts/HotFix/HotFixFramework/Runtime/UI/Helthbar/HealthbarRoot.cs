using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HealthbarRoot : MonoBehaviour {
    public List<Transform> healthBars = new List<Transform>(); //List of helthbars;
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

    public void AddHealthBar(Transform healthBar) 
    {
         healthBars.Add(healthBar);
    }
    public void RemoveHealthBar(Transform healthBar)
    {
        healthBars.Remove(healthBar.transform);
    }
    void Update () 
    {
        if (healthBars.Count == 0)
            return;
        healthBars.Sort(DistanceCompare);

        for(int i = 0; i < healthBars.Count; i++)
            healthBars[i].SetSiblingIndex(healthBars.Count - (i+1));
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
