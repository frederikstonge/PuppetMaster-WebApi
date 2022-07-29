﻿using System.ComponentModel.DataAnnotations;

namespace PuppetMaster.WebApi.Models.Requests
{
    public class UpdateMapRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string DisplayName { get; set; } = string.Empty;
    }
}
