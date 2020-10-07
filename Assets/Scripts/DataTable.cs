// ReSharper disable UnassignedField.Global
// ReSharper disable CollectionNeverUpdated.Global
using System.Collections.Generic;

public struct DataTable
{
    public string Title;
    public List<string> ColumnHeaders;
    public List<Dictionary<string, string>> Data;
}
