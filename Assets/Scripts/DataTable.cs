// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;

[Serializable]
public struct DataTable
{
    public string Title;
    public List<string> ColumnHeaders;
    public List<User> Data;
}
