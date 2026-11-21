using System.Collections.Specialized;

namespace semestrovka1_oris.framework.core;

public class UploadedFile
{
    public string FileName { get; set; } = "";
    public byte[] Data { get; set; } = Array.Empty<byte>();
}

public class FormData
{
    public NameValueCollection Fields { get; set; } = new();
    public UploadedFile? Image { get; set; }
}
