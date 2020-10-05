using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class DynamicDataTable : MonoBehaviour
{

    private const string FileName = "JsonChallenge.json";

    private void Start()
    {
        string filePath = Path.Combine (Application.streamingAssetsPath, FileName);
        var dataTable = GetDataTableFromJsonFile(filePath);
        Debug.Log(dataTable);
    }

    #region Json file deserialization

    private static DataTable? GetDataTableFromJsonFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath, Encoding.UTF8);
            dataAsJson = CleanJsonInput(dataAsJson);
            Debug.Log(dataAsJson);
            return JsonUtility.FromJson<DataTable>(dataAsJson);
        }

        Debug.LogError ($"Missing file named {FileName} in StreamingAssets folder");
        return null;
    }

    /*
     * Remove all trailing commas
     * It's required because we are using Unity´s JsonUtility and it only works on a 'clean' json format
     */
    private static string CleanJsonInput(string json)
    {
        return Regex.Replace(json, @"\,(?=\s*?[\}\]])", string.Empty);
    }

    #endregion
}
