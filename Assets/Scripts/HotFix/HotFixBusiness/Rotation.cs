// ================================================
//描 述:
//作 者:杜鑫
//创建时间:2022-06-06 23-16-37
//修改作者:杜鑫
//修改时间:2022-06-06 23-16-37
//版 本:0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Please modify the description.
/// </summary>
public class Rotation : MonoBehaviour
{
	void Update()
	{
        transform.Rotate(Vector3.left *10);
	}
}