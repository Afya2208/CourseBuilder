using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Models.Dto
{
    public class CsvFile
    {
        public IFormFile FormFile { get; set; }
    }
}