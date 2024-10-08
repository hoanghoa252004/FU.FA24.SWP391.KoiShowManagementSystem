using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class TestModel
    {
        public IFormFile? file {  get; set; }
        public string? name { get; set; }
    }
}
