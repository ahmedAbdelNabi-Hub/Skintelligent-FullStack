﻿using SkinTelIigent.Contracts.DTOs;

namespace SkinTelIigentContracts.CustomResponses
{
    public class ErrorApiResponse : BaseApiResponse
    {
        public Dictionary<string, IEnumerable<string>> Errors { get; set; }

        public ErrorApiResponse(Dictionary<string, IEnumerable<string>> errors)
            : base(400) 
        {
            Errors = errors;
        }
    }

}
