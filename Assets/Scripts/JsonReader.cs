using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public class JsonReader
{
    static string pathBlockTiles = @"Assets/Scripts/blocks.json";
    public static Dictionary<string, string[]> getBlockTiles()
    {
        string JSONdata = File.ReadAllText(pathBlockTiles);
        TopLevel data = JsonConvert.DeserializeObject<TopLevel>(JSONdata);
        Dictionary<string, string[]> finalData = new Dictionary<string, string[]>();
        foreach(var block in data.Blocks){
         finalData.Add(block.Block, block.Tiles);
        }
        return finalData;
    }
}
public partial class TopLevel
{
    [JsonProperty("blocks")]
    public BlockData[] Blocks { get; set; }
}

public partial class BlockData
{
    [JsonProperty("block")]
    public string Block{ get; set; }

    [JsonProperty("tiles")]
    public string[] Tiles { get; set; }
}

