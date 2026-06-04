using Microsoft.AspNetCore.Http;

namespace Models.Dto;

public class XlsxFile
{
    public IFormFile File { get; set; }
    public string FileName { get; set; }
}