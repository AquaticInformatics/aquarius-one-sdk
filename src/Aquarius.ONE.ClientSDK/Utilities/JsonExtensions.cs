﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ONE.Utilities
{
    public class JsonExtensions
    {
        public static string SerializeObjectNoCache<T>(T obj, JsonSerializerSettings settings = null)
        {
            settings = settings ?? new JsonSerializerSettings();
            bool reset = (settings.ContractResolver == null);
            if (reset)
                // To reduce memory footprint, do not cache contract information in the global contract resolver.
                settings.ContractResolver = new DefaultContractResolver();
            try
            {
                return JsonConvert.SerializeObject(obj, settings);
            }
            finally
            {
                if (reset)
                    settings.ContractResolver = null;
            }
        }
        public static string ToJsonString<T>(T obj)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(obj, jsonSettings);
        }
        public static string ToPrettyJson<T>(T obj)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented

            };
            return JsonConvert.SerializeObject(obj, jsonSettings);
        }
    }
}
