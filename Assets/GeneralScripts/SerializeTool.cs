using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class Serializer
{
    public static void XMLSerialize<T>(T o, string filePath)
    {
        try
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            StreamWriter sw = new StreamWriter(filePath, false);
            formatter.Serialize(sw, o);
            sw.Flush();
            sw.Close();
        }
        catch (Exception e) { Debug.LogError(e); }
    }
    public static T XMLDeSerialize<T>(string filePath)
    {
        try
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            StreamReader sr = new StreamReader(filePath);
            T o = (T)formatter.Deserialize(sr);
            sr.Close();
            return o;
        }
        catch (Exception e) { Debug.LogError(e); }
        return default(T);
    }
    public static void BINSerialize<T>(T o, string filePath)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, o);
            stream.Flush();
            stream.Close();
        }
        catch (Exception e) { Debug.LogError(e); }
    }
    public static T BINDeSerialize<T>(string filePath)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream destream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            T o = (T)formatter.Deserialize(destream);
            destream.Flush();
            destream.Close();
            return o;
        }
        catch (Exception e) { Debug.LogError(e); }
        return default(T);
    }
}
