
using SQLite;

/*public class ArtflowImages
{
    public ArtFlowImageData[] images { get; set; }
}*/

public class ArtFlowImageData
{
    [Indexed]
    public int index { get; set; }
    [Indexed]
    public string text_prompt { get; set; }
}
