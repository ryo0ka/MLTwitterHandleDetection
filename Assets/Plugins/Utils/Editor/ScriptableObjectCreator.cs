using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.Editor
{
	public static class ScriptableObjectCreator
	{
		[MenuItem("Assets/Create/Scriptable Object")]
		static void Create()
		{
			var instances = new List<Object>();
			
			foreach (var o in Selection.objects)
			{
				if (Create(o, out var instance))
				{
					instances.Add(instance);
				}
			}
			
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Selection.objects = instances.ToArray();
		}

		static bool Create(Object fileObject, out Object instance)
		{
			string filePath = AssetDatabase.GetAssetPath(fileObject);
			if (!filePath.EndsWith(".cs"))
			{
				Debug.LogWarning($"Failed to instantiate selection: {filePath}, not a class file");
				instance = null;
				return false;
			}

			string fileName = Path.GetFileNameWithoutExtension(filePath);
			instance = ScriptableObject.CreateInstance(fileName);
			if (instance == null)
			{
				Debug.LogWarning($"Failed to instantiate selection: {filePath}, not an Object class");
				return false;
			}

			string path = Path.Combine("Assets", $"{fileName}.asset");
			string uniquePath = AssetDatabase.GenerateUniqueAssetPath(path);
			AssetDatabase.CreateAsset(instance, uniquePath);

			return instance;
		}
	}
}