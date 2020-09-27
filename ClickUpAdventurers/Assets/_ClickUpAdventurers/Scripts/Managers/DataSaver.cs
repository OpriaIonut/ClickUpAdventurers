using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ClickUpAdventurers
{
    public class DataSaver : MonoBehaviour
    {
        public string savePath;

        public int intDefault;
        public float floatDefault;
        public string stringDefault;

        private Dictionary<string, int> intDict;
        private Dictionary<string, float> floatDict;
        private Dictionary<string, string> stringDict;

        private void Awake()
        {
            intDict = new Dictionary<string, int>();
            floatDict = new Dictionary<string, float>();
            stringDict = new Dictionary<string, string>();

            LoadData();
        }

        public void LoadData()
        {
            if (File.Exists(savePath))
            {
                using (StreamReader sw = new StreamReader(savePath))
                {
                    string text = sw.ReadToEnd();
                    text = text.Replace("\r\n", " ");
                    string[] split = text.Split(' ');
                    int dictIndex = 0;
                    for(int index = 0; index < split.Length; index += 2)
                    {
                        if (split[index] == "floatList" || split[index] == "stringList" || split[index] == "")
                        {
                            dictIndex++;
                            index--;
                            continue;
                        }

                        switch(dictIndex)
                        {
                            case 0:
                                intDict[split[index]] = int.Parse(split[index + 1]);
                                break;
                            case 1:
                                floatDict[split[index]] = float.Parse(split[index + 1]);
                                break;
                            case 2:
                                stringDict[split[index]] = split[index + 1];
                                break;
                        }
                    }
                }
            }
        }

        public void SaveModifiedData()
        {
            using (StreamWriter sw = new StreamWriter(savePath))
            {
                foreach (string key in intDict.Keys)
                {
                    sw.WriteLine(key + " " + intDict[key]);
                }
                sw.WriteLine("floatList");
                foreach (string key in floatDict.Keys)
                {
                    sw.WriteLine(key + " " + floatDict[key]);
                }
                sw.WriteLine("stringList");
                foreach (string key in stringDict.Keys)
                {
                    sw.WriteLine(key + " " + stringDict[key]);
                }
            }
        }


        public void ResetSavedData()
        {
            if (File.Exists(savePath))
                File.Delete(savePath);

            intDict.Clear();
            floatDict.Clear();
            stringDict.Clear();
        }

        public void SetSaveData(string key, int data)
        {
            intDict[key] = data;
        }

        public void SetSaveData(string key, float data)
        {
            floatDict[key] = data;
        }

        public void SetSaveData(string key, string data)
        {
            stringDict[key] = data;
        }

        public int LoadInt(string key)
        {
            if (intDict.ContainsKey(key))
                return intDict[key];
            else
                return intDefault;
        }

        public float LoadFloat(string key)
        {
            if (floatDict.ContainsKey(key))
                return floatDict[key];
            else
                return floatDefault;
        }

        public string LoadString(string key)
        {
            if (stringDict.ContainsKey(key))
                return stringDict[key];
            else
                return stringDefault;
        }
    }
}