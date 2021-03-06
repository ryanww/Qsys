﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace QscQsys
{
    public class GetComponents
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        static string method = "Component.GetComponents";
        [JsonProperty("params")]
        static string Params = "crestron";

    }

    public class ComponentResults
    {
        public IList<ComponentProperties> Properties { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class ComponentProperties
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class CreateChangeGroup
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        static string method = "ChangeGroup.AutoPoll";
        [JsonProperty("params")]
        CreateChangeGroupParams Params = new CreateChangeGroupParams();
    }

    public class CreateChangeGroupParams
    {
        [JsonProperty]
        static string Id = "1";
        [JsonProperty]
        static double Rate = 0.2;
    }

    public class AddControlToChangeGroup
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        public string method { get; set; }
        [JsonProperty("params")]
        public AddComponentToChangeGroupParams ComponentParams { get; set; }
    }

    public class AddComponentToChangeGroupParams
    {
        [JsonProperty]
        static string Id = "1";
        public Component Component { get; set; }

    }

    public class Component
    {
        public string Name { get; set; }
        public IList<ControlName> Controls { get; set; }
    }

    public class ControlName
    {
        public string Name { get; set; }
    }

    public class Heartbeat
    {
        [JsonProperty]
        static public string jsonrpc = "2.0";
        [JsonProperty]
        static public string method = "NoOp";
        [JsonProperty("params")]
        HeartbeatParams Params = new HeartbeatParams();
    }

    public class HeartbeatParams
    {
    }

    public class ChangeResult
    {
        public string Component { get; set; }
        public string Name { get; set; }
        public string String { get; set; }
        public double Value { get; set; }
    }

    public class ComponentChange
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        static string method = "Component.Set";
        [JsonProperty("params")]
        public ComponentChangeParams Params { get; set; }
    }

    public class ComponentChangeParams
    {
        public string Name { get; set; }
        public IList<ComponentSetValue> Controls { get; set; }
    }

    public class ComponentSetValue
    {
        public string Name { get; set; }
        public double Value { get; set; }
    }

    public class SetCrossPointMute
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        static string method = "Mixer.SetCrossPointMute";
        [JsonProperty("params")]
        public SetCrossPointMuteParams Params { get; set; }
    }

    public class SetCrossPointMuteParams
    {
        public string Name { get; set; }
        public string Inputs { get; set; }
        public string Outputs { get; set; }
        public bool Value { get; set; }
    }
}