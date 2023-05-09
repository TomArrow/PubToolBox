
using System.Collections.Generic;

public class TableJson
{
    public Dictionary<string,TableEntry> vq3 { get; set; }
    public Dictionary<string, TableEntry> cpm { get; set; }
}


public class TableEntry
{
    public string rank { get; set; }
    public string flag { get; set; }
    public string name { get; set; }
    public string demo { get; set; }
    public string time { get; set; }
    public string points { get; set; }
    public string pid { get; set; }
    public string ref_id { get; set; }
    public string time_ms { get; set; }
    public string status { get; set; }
}
