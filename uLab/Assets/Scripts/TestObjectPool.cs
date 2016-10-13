﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Locke;

/* 性能测试
 * 1.加载图片无压力，测试加载100个1024 png、bmp，对fps没有影响。飞快。
 * 2.测试加载100个3000面的模型，分散到帧，fps 60。一起加载，需要1秒。
 *   测试加载100个9300面的模型，fps 30。
 *   测试加载100个18648面的模型，fps 3.6。
 * 
 * 
 */

public class TestObjectPool : MonoBehaviour
{
	private const int resCount = 100;
	private const string prefabDir = "prefabs/tmp/";
	private const string rootName = "actived";
	private float progress = 0;
	private string progressText = "";

	void Start()
	{
		var go = new GameObject(rootName);
		go.transform.parent = null;
		go.transform.localPosition = new Vector3(-5000, -5000, -5000);
	}

	float btnW = 90;
	float btnH = 30;
	float btnD = 10;
	float btnX = 20;
	float btnY = 20;
	void OnGUI()
	{
		int btnIdx = 0;

		if (GUI.Button(new Rect(btnX, btnY + (btnH + btnD) * btnIdx, btnW, btnH), "preload"))
			StartCoroutine(Preload());
		GUI.Label(new Rect(btnX + btnW + 10, btnY + (btnH + btnD) * btnIdx, 100, btnH), progressText);

		btnIdx++;
		if (GUI.Button(new Rect(btnX, btnY + (btnH + btnD) * btnIdx, btnW, btnH), "pool.get"))
			PoolGet();

		btnIdx++;
		if (GUI.Button(new Rect(btnX, btnY + (btnH + btnD) * btnIdx, btnW, btnH), "pool.put"))
			PoolPut();

		btnIdx++;
		if (GUI.Button(new Rect(btnX, btnY + (btnH + btnD) * btnIdx, btnW, btnH), "load texture"))
			StartCoroutine(LoadTexture());
	}

	private IEnumerator Preload()
	{
		for (int i = 0; i < resCount; i++)
		{
			string resName = prefabDir + i;
			var go = ObjectPool.Instance.GetGO(resName);
			ObjectPool.Instance.PutGO(resName, go, ObjectPool.ObjectType.Role);
			progressText = "" + (i + 1);
			yield return new WaitForEndOfFrame();
		}
		yield return null;
	}

	private void PoolGet()
	{
		var root = GameObject.Find(rootName);
		for (int i = 0; i < resCount; i++)
		{
			string resName = prefabDir + i;
			var go = ObjectPool.Instance.GetGO(resName);
			go.name = "" + i;
			go.transform.parent = root.transform;
		}
	}

	private void PoolPut()
	{
		var root = GameObject.Find(rootName);
		int count = root.transform.childCount;
		List<GameObject> gos = new List<GameObject>();
		for (int i = 0; i < count; i++)
		{
			gos.Add(root.transform.GetChild(i).gameObject);
		}
		for (int i = 0; i < count; i++)
		{
			string resName = prefabDir + i;
			var go = gos[i];
			ObjectPool.Instance.PutGO(resName, go, ObjectPool.ObjectType.Role);
		}
	}

	private IEnumerator LoadTexture()
	{
		var go = GameObject.Find("Cube");
		for (int i = 0; i < 100; i++)
		{
			var tex = Resources.Load("Textures/tmp/" + i) as Texture2D;
			if (tex != null)
			{
				progressText = "" + i;
				go.GetComponent<MeshRenderer>().material.mainTexture = tex;
			}
			yield return new WaitForEndOfFrame();
		}
		yield return null;
	}

}
