using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DynamicDataTable : MonoBehaviour
{
    private const string FileName = "JsonChallenge.json";

    [SerializeField]
    private GameObject dataTablePrefab;
    [SerializeField]
    private GameObject rowPrefab;
    [SerializeField]
    private GameObject cellPrefab;

    private void Start()
    {
        string filePath = Path.Combine (Application.streamingAssetsPath, FileName);
        var dataTable = GetDataTableFromJsonFile(filePath);
        if (dataTable != null) BuildDataTableUi(dataTable.Value);
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

    #region Build UI datatable

    private const string TitleGameObjectName = "Title";
    private const string GridGameObjectName = "Grid";
    private const string HeaderGameObjectName = "Header";

    private void BuildDataTableUi(DataTable dataTable)
    {
        try
        {
            //Prefabs checks
            var (titleTransform, gridTransform, headerTransform) = CheckPrefabStructure();

            //Title (optional)
            if (titleTransform != null) titleTransform.GetComponent<Text>().text = dataTable.Title;

            //Headers
            foreach (var header in dataTable.ColumnHeaders)
                InstantiateText(header, headerTransform, true);

            //Cells
            BuildRowsAndCells(dataTable, gridTransform);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private (Transform titleTransform, Transform gridTransform, Transform headerTransform) CheckPrefabStructure()
    {
        var dataTableGameObject = Instantiate(dataTablePrefab, transform);

        var titleTransform = dataTableGameObject.transform.Find(TitleGameObjectName);

        if (titleTransform == null) Debug.LogWarning($"DataTable Prefab doesn't have a child called {TitleGameObjectName}");

        var gridTransform = dataTableGameObject.transform.Find(GridGameObjectName);

        if (gridTransform == null) throw new Exception($"DataTable Prefab doesn't have a child called {GridGameObjectName}");

        var headerTransform = gridTransform.Find(HeaderGameObjectName);

        if (gridTransform == null) throw new Exception($"Grid doesn't have a child called {HeaderGameObjectName}");

        return (titleTransform, gridTransform, headerTransform);
    }

    /// <summary>
    /// Cell text values are extracted using reflection and casted as string
    /// It could support different types of data elements
    /// </summary>
    private void BuildRowsAndCells(DataTable dataTable, Transform gridTransform)
    {
        foreach (var element in dataTable.Data)
        {
            var rowGameObject = Instantiate(rowPrefab, gridTransform);
            foreach (var header in dataTable.ColumnHeaders)
            {
                var textContent = string.Empty;
                var fieldInfo = element.GetType().GetField(header);
                if (fieldInfo != null)
                    textContent = fieldInfo.GetValue(element) as string;
                InstantiateText(textContent, rowGameObject.transform);
            }
        }
    }

    private void InstantiateText(string textContent, Transform parent, bool bold = false)
    {
        var cellGameObject = Instantiate(cellPrefab, parent);
        var text = cellGameObject.GetComponent<Text>();
        text.text = textContent;
        if(bold) text.fontStyle = FontStyle.Bold;
    }

    #endregion
}
