using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class SaveManager
{
	private const string fileExtension = ".bin";
	public static SaveManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SaveManager();
			}
			return instance;
		}
	}
	private static SaveManager instance;
	private BinaryFormatter formatter;
	public void SaveData(object obj, string path)
	{
		formatter = new BinaryFormatter();
		FileStream fileStream = new FileStream(Application.persistentDataPath + "/"+ path + fileExtension, FileMode.Create, FileAccess.Write);
		formatter.Serialize(fileStream, obj);
		fileStream.Close();
	}
	public T LoadData<T>(string path)
	{
		object obj = null;
		formatter = new BinaryFormatter();
		if (File.Exists(Application.persistentDataPath + "/" + path + fileExtension))
		{
			FileStream fileStream = new FileStream(Application.persistentDataPath + "/" + path + fileExtension, FileMode.Open, FileAccess.Read);
			obj = formatter.Deserialize(fileStream);
			fileStream.Close();
		}
		return (T)obj;
	}
	public bool DeleteData(string path)
	{
		if (File.Exists(Application.persistentDataPath + "/" + path + fileExtension))
		{
			File.Delete(Application.persistentDataPath + "/" + path + fileExtension);

			return true;
		}
		return false;
	}
}