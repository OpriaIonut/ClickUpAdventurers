using UnityEngine;

public class DataSaver : MonoBehaviour
{
    public int intDefault;
    public float floatDefault;
    public string stringDefault;
    
    public void SetSaveData(string key, int data)
    {
        PlayerPrefs.SetInt(key, data);
    }

    public void SetSaveData(string key, float data)
    {
        PlayerPrefs.SetFloat(key, data);
    }

    public void SetSaveData(string key, string data)
    {
        PlayerPrefs.SetString(key, data);
    }

    public void SaveModifiedData()
    {
        PlayerPrefs.Save();
    }

    public int LoadInt(string key)
    {
        return PlayerPrefs.GetInt(key, intDefault);
    }

    public float LoadFloat(string key)
    {
        return PlayerPrefs.GetFloat(key, floatDefault);
    }

    public string LoadString(string key)
    {
        return PlayerPrefs.GetString(key, stringDefault);
    }
}
