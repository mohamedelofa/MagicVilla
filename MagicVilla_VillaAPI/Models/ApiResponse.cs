﻿using System.Net;

namespace MagicVilla_VillaAPI.Models
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public dynamic Result { get; set; }

    }
}
