using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public partial class WallGet
    {
        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("items")]
        public Item[] Items { get; set; }
    }

    public partial class Item
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
        public PostType PostType { get; set; }

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
        public string getImageUrl()
        {
            
            foreach (Attachment Attachment in Attachments)
            {
                
                if (Attachment.Photo.Photo604 != null)
                    return Attachment.Photo.Photo604;
                if (Attachment.Photo.Photo807 != null)
                    return Attachment.Photo.Photo807;
                if (Attachment.Photo.Photo1280 != null)
                    return Attachment.Photo.Photo1280;
                if (Attachment.Photo.Photo130 != null)
                    return Attachment.Photo.Photo130;
                if (Attachment.Photo.Photo2560 != null)
                    return Attachment.Photo.Photo2560;

            }
            throw new Exception("No photo");
        }
      
    }

    public partial class Attachment
    {
        [JsonProperty("type")]
        public AttachmentType Type { get; set; }

        [JsonProperty("doc", NullValueHandling = NullValueHandling.Ignore)]
        public Doc Doc { get; set; }

        [JsonProperty("audio", NullValueHandling = NullValueHandling.Ignore)]
        public Audio Audio { get; set; }

        [JsonProperty("photo", NullValueHandling = NullValueHandling.Ignore)]
        public AttachmentPhoto Photo { get; set; }

        [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
        public Link Link { get; set; }

        [JsonProperty("video", NullValueHandling = NullValueHandling.Ignore)]
        public AttachmentVideo Video { get; set; }
    }

    public partial class Audio
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("url")]
        public Url Url { get; set; }

        [JsonProperty("album_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? AlbumId { get; set; }

        [JsonProperty("is_hq")]
        public bool IsHq { get; set; }

        [JsonProperty("genre_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? GenreId { get; set; }
    }

    public partial class Doc
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("ext")]
        public string Ext { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("preview")]
        public Preview Preview { get; set; }

        [JsonProperty("access_key")]
        public string AccessKey { get; set; }
    }

    public partial class Preview
    {
        [JsonProperty("photo")]
        public PreviewPhoto Photo { get; set; }

        [JsonProperty("video")]
        public VideoElement Video { get; set; }
    }

    public partial class PreviewPhoto
    {
        [JsonProperty("sizes")]
        public VideoElement[] Sizes { get; set; }
    }

    public partial class VideoElement
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("file_size", NullValueHandling = NullValueHandling.Ignore)]
        public long? FileSize { get; set; }
    }

    public partial class Link
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("photo")]
        public LinkPhoto Photo { get; set; }
    }

    public partial class LinkPhoto
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

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }
    }

    public partial class AttachmentPhoto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("album_id")]
        public long AlbumId { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("user_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? UserId { get; set; }

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

        [JsonProperty("photo_2560", NullValueHandling = NullValueHandling.Ignore)]
        public string Photo2560 { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("text")]
        public Text Text { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("lat", NullValueHandling = NullValueHandling.Ignore)]
        public double? Lat { get; set; }

        [JsonProperty("long", NullValueHandling = NullValueHandling.Ignore)]
        public double? Long { get; set; }

        [JsonProperty("access_key")]
        public string AccessKey { get; set; }

        [JsonProperty("post_id", NullValueHandling = NullValueHandling.Ignore)]
        public long? PostId { get; set; }
    }

    public partial class AttachmentVideo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("owner_id")]
        public long OwnerId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("comments")]
        public long Comments { get; set; }

        [JsonProperty("views")]
        public long Views { get; set; }

        [JsonProperty("photo_130")]
        public string Photo130 { get; set; }

        [JsonProperty("photo_320")]
        public string Photo320 { get; set; }

        [JsonProperty("photo_640")]
        public string Photo640 { get; set; }

        [JsonProperty("access_key")]
        public string AccessKey { get; set; }

        [JsonProperty("platform")]
        public string Platform { get; set; }

        [JsonProperty("can_add")]
        public long CanAdd { get; set; }
    }

    public partial class Comments
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("groups_can_post")]
        public bool GroupsCanPost { get; set; }

        [JsonProperty("can_post")]
        public long CanPost { get; set; }
    }

    public partial class Likes
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

    public partial class PostSource
    {
        [JsonProperty("type")]
        public PostSourceType Type { get; set; }
        [JsonProperty("data")]
        public PostSourceData Data { get; set; }
    }

    public partial class Reposts
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("user_reposted")]
        public long UserReposted { get; set; }
    }

    public partial class Views
    {
        [JsonProperty("count")]
        public long Count { get; set; }
    }

    public enum Url { HttpsVkComMp3AudioApiUnavailableMp3 };

    public enum Text { Empty };

    public enum AttachmentType { Audio, Doc, Link, Photo, Video };

    public enum PostSourceType { Vk };
    public enum PostSourceData { Empty, Profile_photo };

    public enum PostType { Post };

    public partial class WallGet
    {
        public static WallGet FromJson(string json) => JsonConvert.DeserializeObject<WallGet>(json, QuickType.WallGetConverter.Settings);
    }

    static class UrlExtensions
    {
        public static Url? ValueForString(string str)
        {
            switch (str)
            {
                case "https://vk.com/mp3/audio_api_unavailable.mp3": return Url.HttpsVkComMp3AudioApiUnavailableMp3;
                default: return null;
            }
        }

        public static Url ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            var str = serializer.Deserialize<string>(reader);
            var maybeValue = ValueForString(str);
            if (maybeValue.HasValue) return maybeValue.Value;
            return new Url();
        }

        public static void WriteJson(this Url value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case Url.HttpsVkComMp3AudioApiUnavailableMp3: serializer.Serialize(writer, "audio_api_unavailable.mp3"); break;
            }
        }
    }

    static class TextExtensions
    {
        public static Text? ValueForString(string str)
        {
            switch (str)
            {
                case "": return Text.Empty;
                default: return null;
            }
        }

        public static Text ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            try
            {
                var str = serializer.Deserialize<string>(reader);
                var maybeValue = ValueForString(str);
                return maybeValue.Value;
            }
            catch { return new Text(); }
        }

        public static void WriteJson(this Text value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case Text.Empty: serializer.Serialize(writer, ""); break;
            }
        }
    }

    static class AttachmentTypeExtensions
    {
        public static AttachmentType? ValueForString(string str)
        {
            switch (str)
            {
                case "audio": return AttachmentType.Audio;
                case "doc": return AttachmentType.Doc;
                case "link": return AttachmentType.Link;
                case "photo": return AttachmentType.Photo;
                case "video": return AttachmentType.Video;
                default: return null;
            }
        }

        public static AttachmentType ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            try
            {
                var str = serializer.Deserialize<string>(reader);
                var maybeValue = ValueForString(str);
                return maybeValue.Value;
            }
            catch { return new AttachmentType(); }
        }

        public static void WriteJson(this AttachmentType value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case AttachmentType.Audio: serializer.Serialize(writer, "audio"); break;
                case AttachmentType.Doc: serializer.Serialize(writer, "doc"); break;
                case AttachmentType.Link: serializer.Serialize(writer, "link"); break;
                case AttachmentType.Photo: serializer.Serialize(writer, "photo"); break;
                case AttachmentType.Video: serializer.Serialize(writer, "video"); break;
            }
        }
    }

    static class PostSourceTypeExtensions
    {
        public static PostSourceType? ValueForString(string str)
        {
            switch (str)
            {
                case "vk": return PostSourceType.Vk;
                default: return null;
            }
        }

        public static PostSourceType ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            try
            {
                var str = serializer.Deserialize<string>(reader);
                var maybeValue = ValueForString(str);
                return maybeValue.Value;
            }
            catch { return new PostSourceType(); }

        }

        public static void WriteJson(this PostSourceType value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case PostSourceType.Vk: serializer.Serialize(writer, "vk"); break;
            }
        }
    }

    static class PostTypeExtensions
    {
        public static PostType? ValueForString(string str)
        {
            switch (str)
            {
                case "post": return PostType.Post;
                default: return null;
            }
        }

        public static PostType ReadJson(JsonReader reader, JsonSerializer serializer)
        {
            try
            {
                var str = serializer.Deserialize<string>(reader);
                var maybeValue = ValueForString(str);
                return maybeValue.Value;
            }
            catch
            {
                return new PostType();
            }
        }

        public static void WriteJson(this PostType value, JsonWriter writer, JsonSerializer serializer)
        {
            switch (value)
            {
                case PostType.Post: serializer.Serialize(writer, "post"); break;
            }
        }
    }

    public static class WallGetSerialize
    {
        public static string ToJson(this WallGet self) => JsonConvert.SerializeObject(self, QuickType.WallGetConverter.Settings);
    }

    internal class WallGetConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Url) || t == typeof(Text) || t == typeof(AttachmentType) || t == typeof(PostSourceType) || t == typeof(PostType) || t == typeof(Url?) || t == typeof(Text?) || t == typeof(AttachmentType?) || t == typeof(PostSourceType?) || t == typeof(PostType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (t == typeof(Url))
                return UrlExtensions.ReadJson(reader, serializer);
            if (t == typeof(Text))
                return TextExtensions.ReadJson(reader, serializer);
            if (t == typeof(AttachmentType))
                return AttachmentTypeExtensions.ReadJson(reader, serializer);
            if (t == typeof(PostSourceType))
                return PostSourceTypeExtensions.ReadJson(reader, serializer);
            if (t == typeof(PostType))
                return PostTypeExtensions.ReadJson(reader, serializer);
            if (t == typeof(Url?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return UrlExtensions.ReadJson(reader, serializer);
            }
            if (t == typeof(Text?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return TextExtensions.ReadJson(reader, serializer);
            }
            if (t == typeof(AttachmentType?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return AttachmentTypeExtensions.ReadJson(reader, serializer);
            }
            if (t == typeof(PostSourceType?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return PostSourceTypeExtensions.ReadJson(reader, serializer);
            }
            if (t == typeof(PostType?))
            {
                if (reader.TokenType == JsonToken.Null) return null;
                return PostTypeExtensions.ReadJson(reader, serializer);
            }
            throw new Exception("Unknown type");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var t = value.GetType();
            if (t == typeof(Url))
            {
                ((Url)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(Text))
            {
                ((Text)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(AttachmentType))
            {
                ((AttachmentType)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(PostSourceType))
            {
                ((PostSourceType)value).WriteJson(writer, serializer);
                return;
            }
            if (t == typeof(PostType))
            {
                ((PostType)value).WriteJson(writer, serializer);
                return;
            }
            throw new Exception("Unknown type");
        }

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new WallGetConverter(),
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
