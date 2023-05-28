using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public struct ConfigResponse 
{
        public ConfigOrigin requestOrigin;
        public ConfigRequestStatus status;
        public JObject body;
        public Dictionary<string, string> headers;
}
public enum ConfigRequestStatus
{
        None,
        Failed,
        Success,
        Pending,
}
public enum ConfigOrigin
{
        Default,
        Cached,
        Remote,
}
