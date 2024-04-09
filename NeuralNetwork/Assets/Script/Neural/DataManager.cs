using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;


[XmlRoot("Data")]
public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public string path;

    private XmlSerializer serializer = new(typeof(Data));
    Encoding encoding = Encoding.UTF8;
    
    private void Awake()
    {
        Instance = this;
        SetPath();
    }

    private void SetPath()
    {
        path = Path.Combine(Application.persistentDataPath, "Data.xml");
    }

    public void Save(Data data)
    {
        StreamWriter streamWriter = new(path, false, encoding);
        serializer.Serialize(streamWriter, data);
        streamWriter.Close();
    }

    public Data Load()
    {
        if (File.Exists(path))
        {
            FileStream fileStream = new FileStream(path, FileMode.Open);
            Data data = serializer.Deserialize(fileStream) as Data;
            fileStream.Close();

            return data;
        }

        return null;
    }
}
