using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class DynamicDataTable : MonoBehaviour
{
    private const string FileName = "JsonChallenge.json";
    private const string TitleGameObjectName = "Title";
    private const string GridGameObjectName = "Grid";
    private const string HeaderGameObjectName = "Header";

    [SerializeField]
    private GameObject dataTablePrefab;
    [SerializeField]
    private GameObject rowPrefab;
    [SerializeField]
    private GameObject cellPrefab;

    private FileSystemWatcher _fileSystemWatcher;
    private GameObject _dataTableGameObject;
    private bool _updateRequired;

    private void Start()
    {
        ReadFileAndBuildDataTable();
        AddFileWatcher();
    }

    private void ReadFileAndBuildDataTable()
    {
        var filePath = Path.Combine (Application.streamingAssetsPath, FileName);
        var dataTable = GetDataTableFromJsonFile(filePath);
        if (dataTable != null) BuildDataTableUi(dataTable.Value);
    }

    #region Json file deserialization

    private static DataTable? GetDataTableFromJsonFile(string filePath)
    {
        try
        {
            string dataAsJson = File.ReadAllText(filePath, Encoding.UTF8);
            return JsonConvert.DeserializeObject<DataTable>(dataAsJson);
        }
        catch (Exception e)
        {
            Debug.LogError ($"Error reading {FileName}: {e}");
        }

        return null;
    }

    #endregion

    #region Build UI datatable

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
        Destroy(_dataTableGameObject);

        _dataTableGameObject = Instantiate(dataTablePrefab, transform);

        var titleTransform = _dataTableGameObject.transform.Find(TitleGameObjectName);
        if (titleTransform == null) Debug.LogWarning($"DataTable Prefab doesn't have a child called {TitleGameObjectName}");

        var gridTransform = _dataTableGameObject.transform.Find(GridGameObjectName);
        if (gridTransform == null) throw new Exception($"DataTable Prefab doesn't have a child called {GridGameObjectName}");

        var headerTransform = gridTransform.Find(HeaderGameObjectName);
        if (headerTransform == null) throw new Exception($"{GridGameObjectName} doesn't have a child called {HeaderGameObjectName}");

        return (titleTransform, gridTransform, headerTransform);
    }

    private void BuildRowsAndCells(DataTable dataTable, Transform gridTransform)
    {
        foreach (var dictionary in dataTable.Data)
        {
            var rowGameObject = Instantiate(rowPrefab, gridTransform);
            foreach (var header in dataTable.ColumnHeaders)
            {
                dictionary.TryGetValue(header, out var value);
                InstantiateText(value, rowGameObject.transform);
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

    #region File modification listener

    private void AddFileWatcher()
    {
        _fileSystemWatcher = new FileSystemWatcher
        {
            Path = Application.streamingAssetsPath,
            Filter = FileName,
            NotifyFilter = NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };

        _fileSystemWatcher.Changed += OnJsonFileModified;
    }

    private void OnJsonFileModified(object sender, FileSystemEventArgs e)
    {
        Debug.Log("DataTable: OnJsonFileModified - Applying new values");
        _updateRequired = true;
    }

    private void Update()
    {
        if (!_updateRequired) return;
        _updateRequired = false;
        ReadFileAndBuildDataTable();
    }

    public void OnDestroy()
    {
        _fileSystemWatcher?.Dispose();
        _fileSystemWatcher = null;
    }

    #endregion
}
