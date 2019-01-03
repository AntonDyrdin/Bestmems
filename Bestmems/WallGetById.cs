// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var welcome = Welcome.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class WallGetById
    {
        [JsonProperty("response")]
        public Response[] Response { get; set; }
    }

    public partial class WallGetByIdResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("from_id")]
        public long FromId { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("marked_as_ads")]
        public long MarkedAsAds { get; set; }

        [JsonProperty("post_type")]
        public string PostType { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("attachments")]
        public Attachment[] Attachments { get; set; }

        [JsonProperty("post_source")]
        public PostSource PostSource { get; set; }

        [JsonProperty("comments")]
        public Comments Comments { get; set; }

        [JsonProperty("likes")]
        public Likes Likes { get; set; }

        [JsonProperty("reposts")]
        public Reposts Reposts { get; set; }

        [JsonProperty("views")]
        public Views Views { get; set; }
    }

    public partial class WallGetByIdAttachment
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("photo")]
        public Photo Photo { get; set; }
    }

    public partial class Photo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("album_id")]
        public long AlbumId { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("photo_75")]
        public string Photo75 { get; set; }

        [JsonProperty("photo_130")]
        public string Photo130 { get; set; }

        [JsonProperty("photo_604")]
        public string Photo604 { get; set; }

        [JsonProperty("photo_807")]
        public string Photo807 { get; set; }

        [JsonProperty("photo_1280")]
        public string Photo1280 { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("post_id")]
        public long PostId { get; set; }

        [JsonProperty("access_key")]
        public string AccessKey { get; set; }
    }

    public partial class WallGetByIdComments
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("groups_can_post")]
        public bool GroupsCanPost { get; set; }

        [JsonProperty("can_post")]
        public long CanPost { get; set; }
    }

    public partial class WallGetByIdLikes
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("user_likes")]
        public long UserLikes { get; set; }

        [JsonProperty("can_like")]
        public long CanLike { get; set; }

        [JsonProperty("can_publish")]
        public long CanPublish { get; set; }
    }

    public partial class WallGetByIdPostSource
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }
    }

    public partial class WallGetByIdReposts
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("user_reposted")]
        public long UserReposted { get; set; }
    }

    public partial class WallGetByIdViews
    {
        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public partial class WallGetById
    {
        public static WallGetById FromJson(string json) => JsonConvert.DeserializeObject<WallGetById>(json, QuickType.Converter.Settings);
    }


    internal static class Converter
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
