using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ToolKit
{
	[MenuItem("Tools/Find Hidden Objects")]
	public static void FindHidden()
	{
		GameObject[] gos = GameObject.FindObjectsOfType<GameObject>();
		foreach (GameObject go in gos)
		{
			if (go.hideFlags != HideFlags.None)
				Debug.Log(go.name + " Hidden = " + go.hideFlags.ToString());
		}
	}
	[MenuItem("Tools/Force GC Collect")]
	public static void GCCollect()
	{
		System.GC.Collect();
	}
	[UnityEditor.MenuItem("Assets/Unload Assets")]
	static void UnloadAssets()
	{
		Resources.UnloadUnusedAssets();
	}


	[MenuItem("Tools/Generate Icon")]
	public static void GenerateIcon()
	{
		Texture2D tex = AssetPreview.GetAssetPreview(Selection.activeGameObject);
		Color[] colors = tex.GetPixels();
		int i = 0;
		Color alpha = colors[i];
		Debug.Log(alpha);
		for (; i < colors.Length; i++)
		{
			if (colors[i] == alpha)
			{
				colors[i].a = 0;
			}
		}
		tex.SetPixels(colors);
		byte[] bytes = tex.EncodeToPNG();
		string path = "Assets/Icons/" + Selection.activeGameObject.name + ".png";
		// For testing purposes, also write to a file in the project folder
		System.IO.File.WriteAllBytes(path, bytes);
		AssetDatabase.ImportAsset(path);
		TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
		ti.textureType = TextureImporterType.Sprite;
		ti.SaveAndReimport();

	}

	public static char[] chars = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
		'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v' };
	public static string GetUniqueID()
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		for (int i = 0; i < 10; i++)
		{
			if (Random.Range(0, 100) > 50)
				sb.Append(Random.Range(0, 10).ToString());
			else
				sb.Append(chars[Random.Range(0, chars.Length)]);
		}
		return sb.ToString();
	}
	[MenuItem("Tools/Bake Mesh")]
	public static void BakeMesh()
	{
		if (Selection.activeGameObject != null)
		{
			SkinnedMeshRenderer smr = Selection.activeGameObject.GetComponent<SkinnedMeshRenderer>();
			if (smr)
			{
				Mesh m = new Mesh();
				smr.BakeMesh(m);
				AssetDatabase.CreateAsset(m, "Assets/TestMesh.asset");
			}
		}
	}

}



