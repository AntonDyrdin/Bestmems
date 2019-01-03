using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web;
using System.Drawing;
using Newtonsoft.Json;
using QuickType;
namespace Bestmems
{
    public class API

    {   //вечный доступ для Антон
        //https://oauth.vk.com/authorize?client_id=6450316&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=wall,offline&response_type=token&v=5.52

        //вечный доступ для Иван
        //https://oauth.vk.com/authorize?client_id=6715394&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=wall,offline&response_type=token&v=5.52

        //https://oauth.vk.com/authorize?client_id=6450316&group_ids=165233859&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=manage&response_type=token&v=5.74

        string pathPrefics = "";
        public List<string> alredyPosted;
        public string Gauth()
        {
            return "aedb005dc6d1cd8d0dcf48929224f852f6c615a45380e7102f5e6cffac45eaefb163a005899366e578e9d";

        }
        string key = "";
        public void setKey(string ke)
        {
            key = ke;
        }
        public string auth()
        {
            if (key != "")
                return key;
            else
            {     //Антон
                // return "cd0f48d35f3823479bca743f1a5bdeb3d4b3c21a66a9919f24d797a319aed959e4e093949efeb095667d5";
                return "7bb2ab8d8b1f2b70661aa686eb14cec711ad4a498b8f381e02de0cb1dd9da7b73dc5f7740f9ded0cd45f1";
            }
        }
        int delay = 0;
        Form1 form1;
        public API(Form1 form1, string pathPrefics)
        {
            this.pathPrefics = pathPrefics;
            alredyPosted = new List<string>();
            this.form1 = form1; try
            {
                var readed = File.ReadAllLines(pathPrefics + "alredyPosted.txt");

                foreach (string line in readed)
                    alredyPosted.Add(line);
            }
            catch { }
        }
        public string getResponse(string methodName, string parametrs)
        {
            string[] prms = parametrs.Split(',');
            string s = "";
            foreach (string prm in prms)
            { s = s + prm + '&'; }
            return getResponse("https://api.vk.com/method/" + methodName + '?' + s + "&access_token=" + auth() + "&v=" + "5.74");
        }

        public string getResponse(string url)
        {
            string ret = "";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        ret = ret + line;
                    }
                }
            }
            response.Close();
            //  log(ret, Color.White);
            // log("\n", Color.White);
            System.Threading.Thread.Sleep(335);
            if (ret.Contains("denied"))
            {
                log("ACCESS DENIED", Color.Red);
            }
            if (ret.Contains("error"))
            {
                log(ret, Color.Red);
            }
            return ret;
        }

        public void get_reposts(string id)
        {
        }
        public void get_likes(string id)
        {
        }
        public Dictionary<string, string> ParseJson(string res)
        {
            var lines = res.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var ht = new Dictionary<string, string>(20);
            var st = new Stack<string>(20);

            for (int i = 0; i < lines.Length; ++i)
            {
                var line = lines[i];
                var pair = line.Split(":[]".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (pair.Length == 2)
                {
                    var key = ClearString(pair[0]);
                    var val = ClearString(pair[1]);

                    if (val == "{")
                    {
                        st.Push(key);
                    }
                    else
                    {
                        if (st.Count > 0)
                        {
                            key = string.Join("_", st) + "_" + key;
                        }

                        if (ht.ContainsKey(key))
                        {
                            ht[key] += "&" + val;
                        }
                        else
                        {
                            ht.Add(key, val);
                        }
                    }
                }
                else if (line.IndexOf('}') != -1 && st.Count > 0)
                {
                    st.Pop();
                }
            }

            return ht;
        }

        public string ClearString(string str)
        {
            str = str.Trim();
            try
            {
                var ind0 = str.IndexOf("\"");
                var ind1 = str.LastIndexOf("\"");

                if (ind0 != -1 && ind1 != -1)
                {
                    str = str.Substring(ind0 + 1, ind1 - ind0 - 1);
                }
                else if (str[str.Length - 1] == ',')
                {
                    str = str.Substring(0, str.Length - 1);
                }

                str = HttpUtility.UrlDecode(str);
            }
            catch { }
            return str;
        }

        public List<Group> getGropListByDomainsFile(string domainsFilePath)
        {
            List<Group> groupList = new List<Group>();
            string[] groups = File.ReadAllLines(domainsFilePath);
            int inc = 0;

            foreach (string groupDomain in groups)
            {
                try
                {
                    form1.intDelegate = new Form1.IntDelegate(form1.changProgBar);
                    form1.progressBar1.Invoke(form1.intDelegate, Convert.ToInt32((form1.progressBar1.Maximum / groups.Count()) * inc));
                    try
                    {
                        Group group = new Group(groupDomain.Remove(0, "https://vk.com/".Length).Replace(" ", ""), this, 1);
                        groupList.Add(group);
                        log("Added: " + group.name, Color.Green);
                    }
                    catch { log("error: " + groupDomain, Color.Red); }

                    inc++;
                }
                catch { log("проблема при загрузке данных группы " + groupDomain, Color.Red); }
                System.Threading.Thread.Sleep(delay);


            }
            form1.intDelegate = new Form1.IntDelegate(form1.changProgBar);
            form1.progressBar1.Invoke(form1.intDelegate, 0);
            File.WriteAllText(pathPrefics + "groupList.txt", JsonConvert.SerializeObject(groupList));
            return groupList;
        }
        public List<Group> getGropListByIdsFile()
        {
            List<Group> groupList = new List<Group>();
            string[] groups = File.ReadAllLines(pathPrefics + "groups.txt");
            int inc = 0;
            foreach (string groupid in groups)
            {
                try
                {
                    Group group = new Group(groupid, this);
                    groupList.Add(group);
                    inc++;
                    log("Added: " + group.name, Color.Green);
                    System.Threading.Thread.Sleep(delay);
                }
                catch { }
            }
            inc++;
            File.WriteAllText(pathPrefics + "groupList.txt", JsonConvert.SerializeObject(groupList));
            return groupList;
        }
        public string getGroupIdByDomain(string domain)
        {
            WallGet WallGet;
            if (domain.Contains("public"))
                WallGet = WallGet.FromJson(getResponse("wall.get", "owner_id=-" + domain.Replace("public", "") + ",count=2"));
            else
                WallGet = WallGet.FromJson(getResponse("wall.get", "domain=" + domain + ",count=2"));
            return WallGet.Response.Items[0].FromId.ToString().Remove(0, 1);
        }
        public string getGroupNameById(string id)
        {
            GroupsGetById groupsGetById = GroupsGetById.FromJson(getResponse("groups.getById", "group_ids=" + id));
            return groupsGetById.Response[0].Name;
        }
        public string getImageUrlByPostId(Group group, string PostId)
        {
            //Item Item = JsonConvert.DeserializeObject<Item>(getResponse("wall.getById", "posts=" + PostId));

            return group.getPostById(PostId).item.getImageUrl();
        }
        public string posting(Post post)
        {
            //   WallGet WallGet = WallGet.FromJson(getResponse("wall.get", "owner_id=-165233859" + ",count=5"));
            bool is_new_post = true;
            for (int i = 0; i < alredyPosted.Count; i++)
            {
                if (alredyPosted[i].Split('=')[0] == post.groupId + '_' + post.id)
                {
                    is_new_post = false;
                }
            }
            if (is_new_post)
            {
                if (!check_for_repeat(post))
                {///////////////////////////////////////
                    var response = repost_like_a_new(post, false);
                    ///////////////////////////////////////

                    if (response.IndexOf("response") != -1)
                    {
                        string parseId = response.Substring(response.IndexOf("post_id") + 9);
                        parseId = parseId.Split('}')[0];
                        File.AppendAllText(pathPrefics + "alredyPosted.txt", post.groupId + '_' + post.id + "=" + parseId + '\r' + '\n');
                        alredyPosted.Add(post.groupId + '_' + post.id + "=" + parseId);
                        return "1";
                    }
                    else
                    {
                        return '-' + response;
                    }
                }
                return "3";
            }
            else
            {
                return "2";
            }
        }
        public string repost(Post post)
        {
            return getResponse("https://api.vk.com/method/wall.repost?object=wall-" + post.groupId + "_" + post.id + "&group_id=165233859" + "&access_token=" + auth() + "&v=" + "5.74");
        }
        public string repost_like_a_new(Post post, bool debug_mode)
        {

            string attach = "";
            if (post.item.Attachments != null)
            {
                foreach (Attachment attachment in post.item.Attachments)
                {
                    // if (attachment.Type.ToString() != "Link")
                    //   attach = attach + attachment.Type.ToString();
                    if (attachment.Type.ToString() == "Photo")
                    {
                        attach = attach + "photo" + attachment.Photo.OwnerId.ToString() + '_' + attachment.Photo.Id.ToString() + ',';
                    }
                    if (attachment.Type.ToString() == "Video")
                        attach = attach + "video" + attachment.Video.OwnerId.ToString() + '_' + attachment.Video.Id.ToString() + ',';
                    if (attachment.Type.ToString() == "Audio")
                        attach = attach + "audio" + attachment.Audio.OwnerId.ToString() + '_' + attachment.Audio.Id.ToString() + ',';
                    if (attachment.Type.ToString() == "Doc")
                        attach = attach + "doc" + attachment.Doc.OwnerId.ToString() + '_' + attachment.Doc.Id.ToString() + ',';
                    if (attachment.Type.ToString() == "Link")
                        attach = attach + attachment.Link.Url + ',';

                }
                attach = attach.Remove(attach.Length - 1, 1);
            }
            string req = "";
            DateTime pDate = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(post.item.Date);
            if (post.item.Text != "" && post.item.Text != null && post.domain != "webmland")
            {
                string text = post.item.Text;
                if (text.Contains("#"))
                {
                    var words = text.Split(' ');
                    text = "";
                    foreach (string word in words)
                        if (!word.Contains("#"))
                            text = text + word + " ";

                }
                if (debug_mode)
                {
                    req = "https://api.vk.com/method/wall.post?owner_id=-165233859" + "&message= " + text + '\n' + post.domain + '\n' + "https://vk.com/public" + post.groupId + '\n' + "likes = " + post.hist.ElementAt(post.hist.Count - 1).likes.ToString() + ";" + '\n' + "AVG = " + post.AVG.ToString() + "; " + '\n' + "K = " + post.K.ToString() + '\n' + "Wl = " + post.Wl.ToString() + "&from_group=1" + "&attachments=" + attach + "&access_token=" + auth() + "&v=" + "5.74";
                }
                else
                {
                    req = "https://api.vk.com/method/wall.post?owner_id=-165233859" + "&message= " + text + "&from_group=1" + "&attachments=" + attach + "&access_token=" + auth() + "&v=" + "5.74";
                }
            }
            else if (debug_mode)
            {
                req = "https://api.vk.com/method/wall.post?owner_id=-165233859" + "&message=" + post.domain + '\n' + "https://vk.com/public" + post.groupId + '\n' + "likes = " + post.hist.ElementAt(post.hist.Count - 1).likes.ToString() + ";" + '\n' + "AVG = " + post.AVG.ToString() + "; " + '\n' + '\n' + "K = " + post.K.ToString() + '\n' + "Wl = " + post.Wl.ToString() + "&from_group=1" + "&attachments=" + attach + "&access_token=" + auth() + "&v=" + "5.74";
            }
            else
            {
                req = "https://api.vk.com/method/wall.post?owner_id=-165233859" + "&message=" + "&from_group=1" + "&attachments=" + attach + "&access_token=" + auth() + "&v=" + "5.74";
            }
            return getResponse(req);
        }
        public string repost_now(Post post)
        {
            string attach = "";
            if (!check_for_repeat(post))
            {
                if (post.item.Attachments != null)
                {
                    foreach (Attachment attachment in post.item.Attachments)
                    {
                        // if (attachment.Type.ToString() != "Link")
                        //   attach = attach + attachment.Type.ToString();
                        if (attachment.Type.ToString() == "Photo")
                        {
                            attach = attach + "photo" + attachment.Photo.OwnerId.ToString() + '_' + attachment.Photo.Id.ToString() + ',';
                        }
                        if (attachment.Type.ToString() == "Video")
                            attach = attach + "video" + attachment.Video.OwnerId.ToString() + '_' + attachment.Video.Id.ToString() + ',';
                        if (attachment.Type.ToString() == "Audio")
                            attach = attach + "audio" + attachment.Audio.OwnerId.ToString() + '_' + attachment.Audio.Id.ToString() + ',';
                        if (attachment.Type.ToString() == "Doc")
                            attach = attach + "doc" + attachment.Doc.OwnerId.ToString() + '_' + attachment.Doc.Id.ToString() + ',';
                        if (attachment.Type.ToString() == "Link")
                            attach = attach + attachment.Link.Url + ',';

                    }
                    attach = attach.Remove(attach.Length - 1, 1);
                }
                string req = "";
                DateTime pDate = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(post.item.Date);
                if (post.item.Text != "" && post.item.Text != null)
                {
                    string text = post.item.Text;
                    if (text.Contains("#"))
                    {
                        var words = text.Split(' ');
                        text = "";
                        foreach (string word in words)
                            if (!word.Contains("#"))
                                text = text + word + " ";

                    }

                    req = "https://api.vk.com/method/wall.post?owner_id=-165233859" + "&message= " + post.domain + '\n' + "https://vk.com/public" + post.groupId + '\n' + "likes = " + post.hist.ElementAt(post.hist.Count - 1).likes.ToString() + ";" + '\n' + "AVG = " + post.AVG.ToString() + "; " + '\n' + "original publication time = " + pDate.Hour.ToString() + ":" + pDate.Minute.ToString() + ";" + '\n' + "K = " + post.K.ToString() + '\n' + '\n' + text + "&from_group=1" + "&attachments=" + attach + "&access_token=" + auth() + "&v=" + "5.74";

                }
                else
                    req = "https://api.vk.com/method/wall.post?owner_id=-165233859" + "&message=" + post.domain + '\n' + "https://vk.com/public" + post.groupId + '\n' + "likes = " + post.hist.ElementAt(post.hist.Count - 1).likes.ToString() + ";" + '\n' + "AVG = " + post.AVG.ToString() + "; " + '\n' + "original publication time = " + pDate.Hour.ToString() + ":" + pDate.Minute.ToString() + ";" + '\n' + "K = " + post.K.ToString() + '\n' + "&from_group=1" + "&attachments=" + attach + "&access_token=" + auth() + "&v=" + "5.74";
                return getResponse(req);
            }
            else return "repeat";
        }
        bool check_for_repeat(Post post)
        {     /*
            if (post.item.Attachments != null)
            {
                if (post.item.Attachments[0].Type.ToString() == "Photo")
                {
                    string url = post.item.getImageUrl();

                    try
                    {
                        File.Delete("temp1.jpg");
                        File.Delete("temp2.jpg");
                    }
                    catch { }

                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(url, "temp1.jpg");
                    }
                    Image img1 = Image.FromFile("temp1.jpg");
                    Bitmap BM1 = new Bitmap(img1);
                    int incI = 0;
                    int incJ = 0;
                    int volume = 5;
                    var sourceR = new double[volume, volume];
                    var sourceG = new double[volume, volume];
                    var sourceB = new double[volume, volume];
                    for (int x = 0; x < volume; x++)
                    {
                        for (int y = 0; y < volume; y++)
                        {
                            incI = 0;
                            incJ = 0;
                            double r1 = 0;
                            double g1 = 0;
                            double b1 = 0;
                            double count1 = 0;

                            for (int i = x * BM1.Width / volume; i < (x + 1) * BM1.Width / volume; i++)
                            {
                                incJ = 0;
                                for (int j = y * BM1.Height / volume; j < (y + 1) * BM1.Height / volume; j++)
                                {
                                    r1 = r1 + BM1.GetPixel(i, j).R;
                                    g1 = g1 + BM1.GetPixel(i, j).G;
                                    b1 = b1 + BM1.GetPixel(i, j).B;
                                    count1++;
                                    incJ++;
                                }
                                incI++;
                            }
                            r1 = r1 / count1;
                            g1 = g1 / count1;
                            b1 = b1 / count1;
                            sourceR[x, y] = r1;
                            sourceG[x, y] = g1;
                            sourceB[x, y] = b1;
                        }
                    }
                    for (int qqqq = 0; qqqq < 300; qqqq = qqqq + 99)
                    {
                        WallGet response = WallGet.FromJson(getResponse("wall.get", "owner_id=-165233859" + ",offset=" + qqqq.ToString() + ",count=99"));
                        foreach (Item item in response.Response.Items)
                        {
                            if (item.Attachments != null)
                            {
                                if (item.Attachments[0].Type.ToString() == "Photo")
                                {
                                    url = item.getImageUrl();
                                    using (WebClient client = new WebClient())
                                    {
                                        client.DownloadFile(url, "temp2.jpg");
                                    }
                                    Image img2 = Image.FromFile("temp2.jpg");

                                    var points = new Color[3, 3];

                              
                                   
                                    Bitmap BM2 = new Bitmap(img2);
                                      /*
                                   
                                    for (int i = BM1.Width / 6; i < BM1.Width; i = i + BM1.Width / 3)
                                    {
                                        incJ = 0;
                                        for (int j = BM1.Height / 6; j < BM1.Height; j = j + BM1.Height / 3)
                                        {
                                            try
                                            {
                                                if (BM1.GetPixel(i, j) == BM2.GetPixel(i, j))
                                                    count++;
                                            }
                                            catch { }
                                            incJ++;
                                        }
                                        incI++;
                                    }
                                    if (count > 4)
                                    {
                                        log("точечная проверка на повтор дала результат: " + count +" точек", Color.White);
                                        img2.Dispose();
                                        img1.Dispose();
                                        return true;
                                    }     */

            //секторная проверка совпадений
            /*            var forR = new double[volume, volume];
                        var forG = new double[volume, volume];
                        var forB = new double[volume, volume];
                        for (int x = 0; x < volume; x++)
                        {
                            for (int y = 0; y < volume; y++)
                            {
                                incI = 0;
                                incJ = 0;
                                double r1 = 0;
                                double g1 = 0;
                                double b1 = 0;
                                double count1 = 0;

                                for (int i = x * BM2.Width / volume; i < (x + 1) * BM2.Width / volume; i++)
                                {
                                    incJ = 0;
                                    for (int j = y * BM2.Height / volume; j < (y + 1) * BM2.Height / volume; j++)
                                    {
                                        r1 = r1 + BM2.GetPixel(i, j).R;
                                        g1 = g1 + BM2.GetPixel(i, j).G;
                                        b1 = b1 + BM2.GetPixel(i, j).B;
                                        count1++;
                                        incJ++;
                                    }
                                    incI++;
                                }
                                r1 = r1 / count1;
                                g1 = g1 / count1;
                                b1 = b1 / count1;
                                forR[x, y] = r1;
                                forG[x, y] = g1;
                                forB[x, y] = b1;
                            }
                        }
                        double delta = 0;
                        for (int x = 0; x < volume; x++)
                        {
                            for (int y = 0; y < volume; y++)
                            {
                                delta = delta + Math.Abs(sourceR[x, y] - forR[x, y]) + Math.Abs(sourceG[x, y] - forG[x, y]) + Math.Abs(sourceB[x, y] - forB[x, y]);
                            }
                        }
                        if (delta < 400)
                        {
                            log("секторная проверка на повтор дала результат: " + delta,Color.White);
                            return true; }

                        img2.Dispose();
                    }
                }
            }
        }
        img1.Dispose();
    }
}   */
            return false;
        }
        void log(String s, Color col)
        {
            form1.colorStringDelegate = new Form1.ColorStringDelegate(form1.log);
            form1.richTextBox1.Invoke(form1.colorStringDelegate, s, col);
        }
    }
}
