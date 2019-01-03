using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class GroupsGetById
    {
        [JsonProperty("response")]
        public Response[] Response { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }

        [JsonProperty("is_closed")]
        public long IsClosed { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("photo_50")]
        public string Photo50 { get; set; }

        [JsonProperty("photo_100")]
        public string Photo100 { get; set; }

        [JsonProperty("photo_200")]
        public string Photo200 { get; set; }
    }

    public partial class GroupsGetById
    {
        public static GroupsGetById FromJson(string json) => JsonConvert.DeserializeObject<GroupsGetById>(json, QuickType.GroupsGetByIdConverter.Settings);
    }

    public static class GroupsGetByIdSerialize
    {
        public static string ToJson(this GroupsGetById self) => JsonConvert.SerializeObject(self, QuickType.GroupsGetByIdConverter.Settings);
    }

    internal class GroupsGetByIdConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
