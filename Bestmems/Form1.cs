using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using ZedGraph;
using QuickType;
namespace Bestmems
{
    public partial class Form1 : Form
    {     // C:\\Users\\Общий Доступ\\Desktop\\Debug\\
        //________________________________________________________
        double K = 10;
        int postingRateInMinutes = 60;
        double Kgi = 6;

        //________________________________________________________
        //6450316

        List<Group> groupList;
        API API;
        List<Post> postList1;
        Graphics g1;
        Bitmap bitmap1;
        Graphics g2;
        Bitmap bitmap2;
        string pathPrefics = "C:\\Users\\Общий Доступ\\Desktop\\Debug\\";
       // string pathPrefics = "D:\\Anton\\Desktop\\BESTMEMS\\Bestmems v1.0\\Bestmems\\bin\\Debug\\";
        public Form1()
        {

            InitializeComponent();
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            API = new API(this,pathPrefics);

            bitmap1 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            g1 = Graphics.FromImage(bitmap1);
            bitmap2 = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            g2 = Graphics.FromImage(bitmap2);
            richTextBox5.Text = Kgi.ToString();
            richTextBox2.Text = K.ToString();
            richTextBox3.Text = postingRateInMinutes.ToString();


        }
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void Form1_Load(object sender, EventArgs e)
        {
            ymax = pictureBox2.Height;
            xmax = pictureBox2.Width;

            button9_Click(null, null);
            try
            {
                if (LOAD() == 0)
                {
                    button7_Click(null, null);
                }
            }
            catch { }

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void updateListBox(ListBox listBox, List<Post> postList)
        {
            try
            {
                listBox.Items.Clear();
                foreach (Post post in postList)
                {
                    if (Convert.ToString(Convert.ToDouble(post.K)).Length > 5)
                        if (Convert.ToString(post.AVG).Length > 5)
                            listBox2.Items.Add(Convert.ToString(Convert.ToDouble(post.K)).Substring(0, 5) + "|" + "https://vk.com/" + post.domain + "?w=wall-" + post.groupId + "_" + post.id);
                        else
                            listBox2.Items.Add(Convert.ToString(Convert.ToDouble(post.K)).Substring(0, 5) + "|" + "https://vk.com/" + post.domain + "?w=wall-" + post.groupId + "_" + post.id);

                }
            }
            catch { }
        }
        public void updateHistory(object source, EventArgs e)
        {
            delegatelog("updating posts...", Color.Gray);
            int inc = 0;

            foreach (Group group in groupList)
            {
                try
                {
                    var tampGroup = group;
                    intDelegate = new IntDelegate(changProgBar);
                    progressBar1.Invoke(intDelegate, Convert.ToInt32((progressBar1.Maximum / groupList.Count) * inc));
                    group.updatePosts();
                    inc++;
                    if (tampGroup.posts.Count - group.posts.Count > 0)
                    {
                        for (int i = 0; i < tampGroup.posts.Count - group.posts.Count; i++)
                        { log("added post: " + group.posts.ElementAt(group.posts.Count - 1 - i).groupId + '_' + group.posts.ElementAt(group.posts.Count - 1 - i).id); }
                    }
                }
                catch { }
            }
            intDelegate = new IntDelegate(changProgBar);
            progressBar1.Invoke(intDelegate, 0);
            delegatelog("Posts has updated", Color.Gray);

            updateListBox();

        }
        public void changProgBar(int value)
        {
            progressBar1.Value = value;
        }
        public void updateListBox()
        {
            voidDelegate = new VoidDelegate(updateListBox);
            listBox2.Invoke(voidDelegate, listBox2, postList1);
        }
        public void log(String s, Color col)
        {
            richTextBox1.SelectionColor = col;
            richTextBox1.AppendText(s + '\n');
            richTextBox1.SelectionColor = Color.White;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save(null, null);

        }
        System.Timers.Timer AutoSave;
        public int LOAD()
        {
            //log("https://oauth.vk.com/authorize?client_id=6450316&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=friends&response_type=token&v=5.52", Color.White);
            try
            {
                string json = File.ReadAllText(pathPrefics + "groupList.txt");
                if (json == "")
                {
                    log("попытка создать БД заново", Color.Red);
                    button3_Click(null, null);
                }
                groupList = JsonConvert.DeserializeObject<List<Group>>(json);
                for (int i = 0; i < groupList.Count; i++)
                    groupList[i].load(API);
                listBox1.Items.Clear();
                foreach (Group group in groupList)
                {
                    listBox1.Items.Add(group.name);
                }

                AutoSave = new System.Timers.Timer();
                AutoSave.Elapsed += new System.Timers.ElapsedEventHandler(Save);
                AutoSave.AutoReset = true;
                //day
                AutoSave.Interval = 60000 * 60 * 24;
                AutoSave.Start();
                return 0;
            }
            catch
            {
                try
                {
                    log("попытка загрузить бэкап БД", Color.Red);
                    string json = File.ReadAllText(pathPrefics + "groupListBack.txt");
                    groupList = JsonConvert.DeserializeObject<List<Group>>(json);
                    for (int i = 0; i < groupList.Count; i++)
                        groupList[i].load(API);
                    listBox1.Items.Clear();
                    foreach (Group group in groupList)
                    {
                        listBox1.Items.Add(group.name);
                    }
                    return 0;
                }
                catch
                {
                    try
                    {
                        log("попытка создать БД заново", Color.Red);
                        button3_Click(null, null);
                        return 0;
                    }
                    catch
                    {
                        log("не удалось создать БД заново (возможно префикс абсолютного пути неверен", Color.Red);
                        return 1;
                    }
                }
            }
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {

            listBox2_Click("", null);
        }

        private void optimization_progress_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(listBox2.SelectedItem.ToString().Split('|')[1]);
            }
            catch { }
        }
        private void OnApplicationExit(object sender, EventArgs e)
        {
            Save("", null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            groupList = new List<Group>();
            groupList = API.getGropListByDomainsFile(pathPrefics + "domains.txt");
            listBox1.Items.Clear();
            foreach (Group group in groupList)
            {
                listBox1.Items.Add(group.name);
            }
            try
            {
                File.Delete(pathPrefics + "groupList.txt");
            }
            catch { }
            File.WriteAllText(pathPrefics + "groupList.txt", JsonConvert.SerializeObject(groupList));
            try { File.Delete(pathPrefics + "groupListBack.txt"); }
            catch { }
            File.WriteAllText(pathPrefics + "groupListBack.txt", JsonConvert.SerializeObject(groupList));

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox2_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1 && listBox2.SelectedIndex < postList1.Count)
            {
                try
                {
                    string url = postList1.ElementAt(listBox2.SelectedIndex).item.getImageUrl();
                    pictureBox1.Load(url);
                }
                catch
                {
                    pictureBox1.Image = Image.FromFile("no_photo.png");
                }
            }

        }
        void log(String s)
        {
            colorStringDelegate = new Form1.ColorStringDelegate(log);
            richTextBox1.Invoke(colorStringDelegate, s, Color.White);
        }
        public delegate void ColorStringDelegate(string is_completed, Color col);
        public ColorStringDelegate colorStringDelegate;
        public delegate void StringDelegate(string is_completed);
        public StringDelegate stringDelegate;
        public delegate void IntDelegate(int integer);
        public IntDelegate intDelegate;
        public delegate void VoidDelegate(ListBox listBox, List<Post> postList);
        public VoidDelegate voidDelegate;
        /* void updateGraphs(Post post)
         {
             var points = post.hist;

             GraphPane likesPane = graf.GraphPane;
             GraphPane repostsPane = zedGraphControl1.GraphPane;
             GraphPane viewsPane = zedGraphControl2.GraphPane;
             PointPairList likesList = new PointPairList();
             PointPairList repostsList = new PointPairList();
             PointPairList viewsList = new PointPairList();
             likesPane.CurveList.Clear();
             repostsPane.CurveList.Clear();
             viewsPane.CurveList.Clear();
             int inc = 0;
             for (int i = 0; i < points.Count; i++)
             {
                 likesList.Add(inc, points.ElementAt(i).likes);
                 repostsList.Add(inc, points.ElementAt(i).reposts);
                 viewsList.Add(inc, points.ElementAt(i).views);
                 inc++;
             }

             LineItem myCurve = likesPane.AddCurve("likes", likesList, Color.Green, SymbolType.None);
             LineItem myCurve1 = repostsPane.AddCurve("reposts", repostsList, Color.Red, SymbolType.None);
             LineItem myCurve2 = viewsPane.AddCurve("views", viewsList, Color.Blue, SymbolType.None);
             graf.AxisChange();
             graf.Invalidate();
             zedGraphControl1.AxisChange();
             zedGraphControl1.Invalidate();
             zedGraphControl2.AxisChange();
             zedGraphControl2.Invalidate();
         }     */
        private void set_Wl(object sender, EventArgs e)
        {
            delegatelog("SET Wl...", Color.Cyan);
            WallGet response = WallGet.FromJson(API.getResponse("wall.get", "owner_id=-165233859" + ",count=10"));
            int max = 0;
            int min = 999999;
            var now = new DateTimeOffset(DateTime.Now);
            foreach (Item item in response.Response.Items)
            {
                if (now.ToUnixTimeSeconds() - item.Date < 60 * 60 * 24 * 7)
                {
                    if (item.Likes.Count < min)
                        min = Convert.ToInt32(item.Likes.Count);
                    if (item.Likes.Count > max)
                        max = Convert.ToInt32(item.Likes.Count);
                }
            }
            foreach (Item item in response.Response.Items)
            {

                if (now.ToUnixTimeSeconds() - item.Date < 60 * 60 * 24)
                {

                    foreach (string line in API.alredyPosted)
                    {
                        if (line.Split('=')[1] == item.Id.ToString())
                        {
                            for (int i = 0; i < groupList.Count; i++)
                            {
                                if (line.Split('=')[0].Split('_')[0] == groupList[i].id)
                                {
                                    double old = groupList[i].Wl;
                                    double G;
                                    if (min == max && max == 0)
                                    { G = 0.25; }
                                    else if (min == max && max != 0)
                                    { G = 0.5; }
                                    else
                                        G = (Convert.ToDouble(item.Likes.Count) - Convert.ToDouble(min)) / (Convert.ToDouble(max) - Convert.ToDouble(min));

                                    if (G > 0.5)
                                    {
                                        G = G + (G - 0.5) / 2;
                                    }

                                    if (groupList[i].Wl <= 1)
                                    {
                                        if (!(groupList[i].Wl + (G - 0.5) / Kgi < 0.5))
                                        {
                                            double mew = groupList[i].Wl + (G - 0.5) / Kgi;
                                            groupList[i].Wl = groupList[i].Wl + (G - 0.5) / Kgi;
                                        }
                                    }
                                    else
                                    {
                                        if (!(groupList[i].Wl + (G - 0.5) * 2 / Kgi > 2))
                                        {
                                            double mew = groupList[i].Wl + (G - 0.5) * 2 / Kgi;
                                            groupList[i].Wl = groupList[i].Wl + (G - 0.5) * 2 / Kgi;
                                        }
                                    }
                                    if (item.Likes.Count > 0)
                                        old = groupList[i].Wl;
                                    else
                                        old = groupList[i].Wl;
                                }
                            }
                        }
                    }
                }
            }
            //write Wl changing
            /*  string s = "";
              if (File.ReadAllLines("Gl.txt").Length == 0)
              {
                  s = "";
                  for (int i = 0; i < groupList.Count; i++)
                  { s = s + groupList[i].domain + ';'; }
                  File.WriteAllText("Gl.txt", s + '\r' + '\n');
              }
              s = "";
              for (int i = 0; i < groupList.Count; i++)
              {
                  s = s + groupList[i].Wl + ';';
              }
              File.AppendAllText("Gl.txt", s + '\r' + '\n');
              delegatelog("Success", Color.Cyan);   */
            string[] s = new string[groupList.Count];
            var file = File.ReadAllLines(pathPrefics + "Gl.txt");

            if (file.Length == 0)
            {
                for (int i = 0; i < groupList.Count; i++)
                { s[i] = groupList[i].domain + ';'; }
                File.WriteAllLines(pathPrefics + "Gl.txt", s);

                for (int i = 0; i < groupList.Count; i++)
                {
                    s[i] = s[i] + groupList[i].Wl + ';';
                }
                File.WriteAllLines(pathPrefics + "Gl.txt", s);
            }
            else
            {
                for (int j = 0; j < file.Length; j++)
                    for (int i = 0; i < groupList.Count; i++)
                    {
                        if (file[j].Split(';')[0] == groupList[i].domain)
                        {
                            file[j] = file[j] + groupList[i].Wl + ';';
                            break;
                        }
                    }
                File.WriteAllLines(pathPrefics + "Gl.txt", file);
            }



            delegatelog("Success", Color.Cyan);
        }
        private void Save(object sender, EventArgs e)
        {
            A:
            try
            {
                File.Delete(pathPrefics + "groupList.txt");
                File.WriteAllText(pathPrefics + "groupList.txt", JsonConvert.SerializeObject(groupList));

                File.Delete(pathPrefics + "groupListBack.txt");
                File.WriteAllText(pathPrefics + "groupListBack.txt", JsonConvert.SerializeObject(groupList));

            }
            catch
            {
                goto A;
            }
            delegatelog("save BackUp of data base", Color.Gray);

            var now = new DateTimeOffset(DateTime.Now);
            if (now.Hour == 19)
            {
                if (lastDayOfWlSetting != now.Day)
                {
                    set_Wl(null, null);
                    lastDayOfWlSetting = now.Day;
                }
            }
        }
        int lastDayOfWlSetting;
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://oauth.vk.com/authorize?client_id=6450316&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=friends&response_type=token&v=5.52");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_Click(object sender, EventArgs e)
        {

            try
            {
                postList1 = Group.getSortedPosts(groupList[listBox1.SelectedIndex].posts.ToList(), 3, 0);
                listBox2.Items.Clear();

                for (int i = 0; i < postList1.Count; i++)
                {
                    listBox2.Items.Add("https://vk.com/" + groupList[listBox1.SelectedIndex].domain + "?w=wall-" + groupList[listBox1.SelectedIndex].id + "_" + postList1.ElementAt(i).id);
                }

            }
            catch { log("не удалось отобразить информацию", Color.Red); }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // set_Wl(null, null);
            updateHistory("1", null);
            var allPosts = new List<Post>();
            foreach (Group group in groupList)
            {
                var posts = Group.setAVG(group.posts.ToList(), 3);
                foreach (Post post in posts)
                {
                    Post anpost = post;
                    anpost.Wl = group.Wl;
                    allPosts.Add(anpost);
                }
            }
            postList1 = Group.getSortedPosts(allPosts, 3, 1440);
            updateListBox();


            /*  visualize2();
              visualize();    */




            /*
           allPosts = new List<Post>();
           foreach (Group group in groupList)
           {
               var posts = Group.setAVG(group.posts.ToList(), 2, 1440);
               foreach (Post post in posts)
               {
                   allPosts.Add(post);
               }
           }
           postList2 = Group.getSortedPosts(allPosts, 2, 480, true);
                  */
            /*  if (sender != "1")
              {

                  listBox2.Items.Clear();
                            for (int i = 0; i < postList1.Count; i++)
                  {
                      var del = Convert.ToDouble(postList1[i].first480min.likes) / Convert.ToDouble(postList1[i].first480min.views);
                      if (Convert.ToString(Convert.ToDouble(postList1[i].K)).Length > 5)
                          if (Convert.ToString(postList1[i].AVG).Length > 5)
                              listBox2.Items.Add(Convert.ToString(postList1[i].AVG).Substring(0, 5) + "__" + Convert.ToString(Convert.ToDouble(postList1[i].K)).Substring(0, 5) + "|" + "https://vk.com/" + postList1[i].domain + "?w=wall-" + postList1[i].groupId + "_" + postList1.ElementAt(i).id);
                          else
                              listBox2.Items.Add(Convert.ToString(postList1[i].AVG) + "__" + Convert.ToString(Convert.ToDouble(postList1[i].K)).Substring(0, 5) + "|" + "https://vk.com/" + postList1[i].domain + "?w=wall-" + postList1[i].groupId + "_" + postList1.ElementAt(i).id);
                      else
                          listBox2.Items.Add(Convert.ToString(Convert.ToDouble(postList1[i].K)) + " |" + "https://vk.com/" + postList1[i].domain + "?w=wall-" + postList1[i].groupId + "_" + postList1.ElementAt(i).id);
                                          }
                  /*   for (int i = 0; i < postList2.Count; i++)
                     {
                         if (Convert.ToString(Convert.ToDouble(postList2[i].first480min.reposts) / Convert.ToDouble(postList2[i].first480min.views)).Length > 5)
                             listBox3.Items.Add(Convert.ToString(Convert.ToDouble(postList2[i].first480min.reposts) / Convert.ToDouble(postList2[i].first480min.views)).Substring(0, 5) + "|" + "https://vk.com/" + postList2[i].domain + "?w=wall-" + postList2[i].groupId + "_" + postList2.ElementAt(i).id);
                         else
                             listBox3.Items.Add(Convert.ToString(Convert.ToDouble(postList2[i].first480min.reposts) / Convert.ToDouble(postList2[i].first480min.views)) + "|" + "https://vk.com/" + postList2[i].domain + "?w=wall-" + postList2[i].groupId + "_" + postList2.ElementAt(i).id);

                     }    */
            //  }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            updateHistory("1", null);
        }
        public void posting(object source, EventArgs e)
        {
            /*var now = DateTime.Now;
           if (now.Hour > 11 || now.Hour < 4)
            {    */
            A:
            try
            {
                button5_Click("1", null);
                foreach (Post post in postList1)
                {
                    if (post.K != 0)
                    {
                        if (post.K > K)
                        {
                            if (post.item.PostSource.Data.ToString() != "Profile_photo" && post.item.MarkedAsAds == 0)
                            {
                                if (!check_for_repeat(post))
                                {
                                    string resp = API.posting(post);
                                    if (resp == "3")
                                    {
                                        delegatelog("REPEAT: " + post.groupId + "_" + post.id + " " + post.domain, Color.Red);
                                        drawStringDelegate = new DrawStringDelegate(drawString);
                                        pictureBox2.Invoke(drawStringDelegate, 1, "X", 5, tempX + Xinterval - 3, ymax - (post.K * ymax / 4.0) - 3);

                                    }
                                    if (resp == "2")
                                    {
                                        delegatelog("already posted: " + post.groupId + "_" + post.id + " " + post.domain, Color.Gray);
                                        drawStringDelegate = new DrawStringDelegate(drawString);
                                        pictureBox2.Invoke(drawStringDelegate, 1, "X", 5, tempX + Xinterval - 3, ymax - (post.K * ymax / 4.0) - 3);

                                    }
                                    if (resp[0] == '-')
                                    {
                                        delegatelog("API ERROR: " + post.groupId + "_" + post.id + " " + resp, Color.Red);
                                        drawStringDelegate = new DrawStringDelegate(drawString);
                                        pictureBox2.Invoke(drawStringDelegate, 1, "!", 8, tempX + Xinterval - 10, ymax - (post.K * ymax / 4.0) - 4);


                                    }
                                    if (resp == "1")
                                    {
                                        delegatelog("posted: " + post.groupId + "_" + post.id + " " + post.domain + DateTime.Now.ToString() + '\n' + "https://vk.com/public" + post.groupId + '\n' + "likes = " + post.hist.ElementAt(post.hist.Count - 1).likes.ToString() + ";" + '\n' + "AVG = " + post.AVG.ToString() + "; " + '\n' + "K = " + post.K.ToString() + '\n' + "Wl = " + post.Wl.ToString(), Color.Cyan);

                                        drawStringDelegate = new DrawStringDelegate(drawString);
                                        pictureBox2.Invoke(drawStringDelegate, 1, "O", 10, tempX + Xinterval - 10, ymax - (post.K * ymax / 4.0) - 5);

                                        drawStringDelegate = new DrawStringDelegate(drawString);
                                        pictureBox2.Invoke(drawStringDelegate, 1, post.groupId + "_" + post.id, 10, tempX + Xinterval - 30, ymax - (post.K * ymax / 4.0) - 10);


                                        break;
                                    }
                                }
                                else
                                {
                                    delegatelog("REPEAT: " + post.groupId + "_" + post.id, Color.Red);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

                delegatelog("ERROR: line 474: posting(object source, EventArgs e){...} ", Color.Red);
                goto A;
            }

            /*  }
              else
              {
                  delegatelog("Zzz..", Color.Blue);
              }  */
        }
        Random r;
        int tempX = 0;
        List<Post> temppostList;
        int Xinterval = 30;
        int ymax;
        int xmax;

        void visualize2()
        {

            if (temppostList == null)
            {
                temppostList = postList1;
                drawDelegate = new DrawDelegate(drawLine);
                pictureBox1.Invoke(drawDelegate, 1, Color.FromArgb(50, 170, 160, 255), 400, 0, ymax - K * ymax / 4.0 - 200, xmax, ymax - K * ymax / 4.0 - 200);

                drawDelegate = new DrawDelegate(drawLine);
                pictureBox1.Invoke(drawDelegate, 1, Color.FromArgb(50, 255, 255, 255), 5, 0, ymax - ymax / 4.0, xmax, ymax - ymax / 4.0);


                drawStringDelegate = new DrawStringDelegate(drawString);
                pictureBox1.Invoke(drawStringDelegate, 1, "K=1", 15, 0, ymax - ymax / 4.0);
                drawStringDelegate = new DrawStringDelegate(drawString);
                pictureBox1.Invoke(drawStringDelegate, 1, "K=2", 15, 0, ymax - 2 * ymax / 4.0);
                drawStringDelegate = new DrawStringDelegate(drawString);
                pictureBox1.Invoke(drawStringDelegate, 1, "K=3", 15, 0, ymax - 3 * ymax / 4.0);
                drawStringDelegate = new DrawStringDelegate(drawString);
                pictureBox1.Invoke(drawStringDelegate, 1, "K=4", 15, 0, ymax - 4 * ymax / 4.0);
            }
            else
            {
                r = new Random();
                int inc = 1;
                //  var sortedGroupList = getSortedGroups(groupList);

                foreach (Group group in groupList)
                {
                    drawDelegate = new DrawDelegate(drawLine);
                    pictureBox1.Invoke(drawDelegate, 1, group.color, 20, xmax - 150, inc * 15, xmax - 100, inc * 15);

                    drawStringDelegate = new DrawStringDelegate(drawString);
                    pictureBox1.Invoke(drawStringDelegate, 1, group.name, 10, xmax - 100, inc * 15 - 10);

                    foreach (Post post in postList1)
                    {
                        if (post.K != 0 && group.domain == post.domain)
                        {

                            if (post.item.MarkedAsAds == 0)
                            {
                                foreach (Post tpost in temppostList)
                                {
                                    if (tpost.id == post.id && tpost.groupId == post.groupId)
                                    {
                                        drawDelegate = new DrawDelegate(drawLine);
                                        pictureBox1.Invoke(drawDelegate, 1, group.color, 1, tempX, ymax - tpost.K * ymax / 50.0, tempX + Xinterval, ymax - post.K * ymax / 50.0);
                                    }
                                }
                            }
                        }
                    }
                    inc++;


                }
                tempX = tempX + Xinterval;
                temppostList = postList1;
            }
        }
        void visualize()
        {
            int maxX = pictureBox3.Width;
            int maxY = pictureBox3.Height;
            int inc = 1;
            int h = 0;
            //  var sortedGroupList = getSortedGroups(groupList);
            foreach (Group group in groupList)
            {
                h = inc * 25;

                drawStringDelegate = new DrawStringDelegate(drawString);
                pictureBox3.Invoke(drawStringDelegate, 2, group.domain, 14, maxX - 650, h - 8);
                drawDelegate = new DrawDelegate(drawLine);
                pictureBox3.Invoke(drawDelegate, 2, Color.FromArgb(1, 50, 50, 50), 15, 0, h, maxX, h);
                double score1 = 0;
                double score2 = 0;
                double score3 = 0;
                int inc1 = 0;
                int inc2 = 0;
                int inc3 = 0;
                foreach (Post post in postList1)
                {
                    if (!post.already_posted && post.K != 0 && group.domain == post.domain)
                    {
                        double x = Math.Pow(post.K / post.Wl, 1.0 / 3.0);
                        if (post.item.MarkedAsAds == 0 && x < 4)
                        {
                            if (x > 1 && x < 4)
                            {
                                score2 = score2 + ((x - 1) * (x - 1));
                                inc2++;
                            }
                            else
                            {
                                score3 = score3 + ((x - 1) * (x - 1));
                                inc3++;
                            }

                            score1 = score1 + ((x - 1) * (x - 1));
                            inc1++;

                            drawDelegate = new DrawDelegate(drawLine);
                            pictureBox3.Invoke(drawDelegate, 2, Color.FromArgb(100, 0, 255, 0), 13, x * maxX / 4.0 - 1, h, x * maxX / 4.0 + 1, h);
                        }


                    }
                }
                /*    score1 = Math.Sqrt(score1 / Convert.ToDouble(inc1));
                    drawDelegate = new DrawDelegate(drawLine);
                    pictureBox3.Invoke(drawDelegate,2, Color.Cyan, 15, (score1 + 1) * maxX / 4.0, h, 1 + (score1 + 1) * maxX / 4.0 + 4, h);       */

                score2 = Math.Sqrt(score2 / Convert.ToDouble(inc2));
                drawDelegate = new DrawDelegate(drawLine);
                pictureBox3.Invoke(drawDelegate, 2, Color.Purple, 13, (score2 + 1) * maxX / 4.0, h, 1 + (score2 + 1) * maxX / 4.0 + 4, h);

                score3 = Math.Sqrt(score3 / Convert.ToDouble(inc3));
                drawDelegate = new DrawDelegate(drawLine);
                pictureBox3.Invoke(drawDelegate, 2, Color.Red, 13, (1 - score3) * maxX / 4.0, h, 1 + (1 - score3) * maxX / 4.0 + 4, h);

                drawDelegate = new DrawDelegate(drawLine);
                pictureBox3.Invoke(drawDelegate, 2, Color.White, 13, (score2 / score3) * maxX / 4.0, h, 1 + (score2 / score3) * maxX / 4.0 + 4, h);


                drawDelegate = new DrawDelegate(drawLine);
                pictureBox3.Invoke(drawDelegate, 2, Color.White, 15, maxX / 4.0, h, maxX / 4.0 + 1, h);
                drawDelegate = new DrawDelegate(drawLine);
                pictureBox3.Invoke(drawDelegate, 2, Color.White, 15, 2 * maxX / 4.0, h, 2 * maxX / 4.0 + 1, h);
                drawDelegate = new DrawDelegate(drawLine);
                /* pictureBox3.Invoke(drawDelegate,2, Color.Yellow, 15, 3 * maxX / 4.0, h, 3 * maxX / 4.0 + 2, h);
                 drawDelegate = new DrawDelegate(drawLine);
                 pictureBox3.Invoke(drawDelegate,2, Color.Cyan, 15, 4 * maxX / 4.0, h, 4 * maxX / 4.0 + 2, h);  */
                inc++;
            }
            inc++;
            drawStringDelegate = new DrawStringDelegate(drawString);
            pictureBox3.Invoke(drawStringDelegate, 2, "K=1", 15, maxX / 4.0, h + 5);
            drawStringDelegate = new DrawStringDelegate(drawString);
            pictureBox3.Invoke(drawStringDelegate, 2, "K=2", 15, 2 * maxX / 4.0, h + 5);
            drawStringDelegate = new DrawStringDelegate(drawString);
            pictureBox3.Invoke(drawStringDelegate, 2, "K=3", 15, 3 * maxX / 4.0, h + 5);
            drawStringDelegate = new DrawStringDelegate(drawString);
            pictureBox3.Invoke(drawStringDelegate, 2, "K=4", 15, 4 * maxX / 4.0, h + 5);
        }
        static public List<Group> getSortedGroups(List<Group> groups)
        {
            var res = new List<Group>();
            for (int i = 0; i < groups.Count; i++)
            {
                res.Add(groups[i]);
            }
            var temp = groups[0];
            for (int i = 0; i < res.Count; i++)
            {
                for (int j = i + 1; j < res.Count; j++)
                {
                    double score1 = 0;
                    double score2 = 0;

                    double score12 = 0;
                    double score13 = 0;
                    int inc12 = 0;
                    int inc13 = 0;

                    double score22 = 0;
                    double score23 = 0;
                    int inc22 = 0;
                    int inc23 = 0;

                    double score14 = 0;
                    double score24 = 0;
                    var posts1 = Group.setAVG(res[i].posts.ToList(), 3);
                    posts1 = Group.getSortedPosts(posts1, 3, 0);
                    int inc = 0;
                    foreach (Post post in posts1)
                    {
                        if (post.item.MarkedAsAds == 0 && post.K < 3)
                        {
                            if (post.K > 1 && post.K < 3)
                            {
                                score12 = score12 + ((post.K - 1) * (post.K - 1));
                                inc12++;
                            }
                            else
                            {
                                score13 = score13 + ((post.K - 1) * (post.K - 1));
                                inc13++;
                            }
                            score1 = score1 + ((post.K - 1) * (post.K - 1));
                            inc++;
                        }


                    }
                    score1 = Math.Sqrt(score1 / Convert.ToDouble(inc));
                    score12 = Math.Sqrt(score12 / Convert.ToDouble(inc12));
                    score13 = Math.Sqrt(score13 / Convert.ToDouble(inc13));

                    score14 = score12 / score13;
                    var posts2 = Group.setAVG(res[j].posts.ToList(), 3);
                    posts2 = Group.getSortedPosts(posts2, 3, 0);
                    inc = 0;
                    foreach (Post post in posts2)
                    {

                        if (post.item.MarkedAsAds == 0 && post.K < 3)
                        {
                            if (post.K > 1 && post.K < 3)
                            {
                                score22 = score22 + ((post.K - 1) * (post.K - 1));
                                inc22++;
                            }
                            else
                            {
                                score23 = score23 + ((post.K - 1) * (post.K - 1));
                                inc23++;
                            }
                            score2 = score2 + ((post.K - 1) * (post.K - 1));
                            inc++;
                        }

                    }
                    score2 = Math.Sqrt(score2 / Convert.ToDouble(inc));
                    score22 = Math.Sqrt(score22 / Convert.ToDouble(inc22));
                    score23 = Math.Sqrt(score23 / Convert.ToDouble(inc23));

                    score24 = score22 / score23;
                    if (score14 < score24)
                    {
                        temp = res[i];
                        res[i] = res[j];
                        res[j] = temp;
                    }
                }
            }
            return res;
        }
        System.Timers.Timer time_to_posting;
        private void button7_Click(object sender, EventArgs e)
        {

            posting("", null);

            time_to_posting = new System.Timers.Timer();
            time_to_posting.Elapsed += new System.Timers.ElapsedEventHandler(posting);
            time_to_posting.AutoReset = true;
            time_to_posting.Interval = postingRateInMinutes * 60 * 1000;
            time_to_posting.Start();
            log("Start posting", Color.Cyan);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            time_to_posting.Dispose();
            time_to_posting.AutoReset = false;
            time_to_posting.Stop();
            log("Stop posting", Color.PaleGoldenrod);
        }
        public delegate void DrawDelegate(int picBoxNum, Color col, int H, double x1, double y1, double x2, double y2);
        public DrawDelegate drawDelegate;
        public delegate void DrawStringDelegate(int picBoxNum, string s, int depth, double x, double y);
        public DrawStringDelegate drawStringDelegate;
        public void drawLine(int picBoxNum, Color col, int H, double x1, double y1, double x2, double y2)
        {
            try
            {
                if (picBoxNum == 1)
                {
                    g1.DrawLine(new Pen(col, H), Convert.ToInt16(Math.Round(x1)), Convert.ToInt16(Math.Round(y1)), Convert.ToInt16(Math.Round(x2)), Convert.ToInt16(Math.Round(y2)));
                    pictureBox2.Image = bitmap1;
                    pictureBox2.Refresh();
                }
                if (picBoxNum == 2)
                {
                    g2.DrawLine(new Pen(col, H), Convert.ToInt16(Math.Round(x1)), Convert.ToInt16(Math.Round(y1)), Convert.ToInt16(Math.Round(x2)), Convert.ToInt16(Math.Round(y2)));
                    pictureBox3.Image = bitmap2;
                    pictureBox3.Refresh();
                }
            }
            catch { }
        }
        public void drawString(int picBoxNum, string s, int depth, double x, double y)
        {
            try
            {
                if (picBoxNum == 1)
                {
                    g1.DrawString(s, new Font("Consolas", depth), Brushes.White, new Point(Convert.ToInt16(Math.Round(x)), Convert.ToInt16(Math.Round(y))));
                    pictureBox2.Image = bitmap1;
                    pictureBox2.Refresh();
                }
                if (picBoxNum == 2)
                {
                    g2.DrawString(s, new Font("Consolas", depth), Brushes.White, new Point(Convert.ToInt16(Math.Round(x)), Convert.ToInt16(Math.Round(y))));
                    pictureBox3.Image = bitmap2;
                    pictureBox3.Refresh();
                }
            }
            catch { }
        }
        public bool check_for_repeat(Post post)
        {
            if (post.item.PostType.ToString() == "Photo")
            {
                string url = post.item.getImageUrl();
                pictureBox1.Load(url);

                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, "temp1.jpg");
                }
                Image img1 = Image.FromFile("temp1.jpg");
                try
                {
                    WallGet response = WallGet.FromJson(API.getResponse("wall.get", "owner_id=-165233859" + ",count=20"));
                    foreach (Item item in response.Response.Items)
                    {
                        if (post.item.PostType == item.PostType)
                        {
                            url = item.getImageUrl();
                            using (WebClient client = new WebClient())
                            {
                                client.DownloadFile(url, "temp2.jpg");
                            }
                            Image img2 = Image.FromFile("temp2.jpg");

                            var points = new Color[3, 3];

                            int count = 0;
                            Bitmap BM1 = new Bitmap(img1);
                            Bitmap BM2 = new Bitmap(img2);

                            int incI = 0;
                            int incJ = 0;
                            for (int i = BM1.Width / 6; i < BM1.Width; i = i + BM1.Width / 3)
                            {
                                incJ = 0;
                                for (int j = BM1.Height / 6; j < BM1.Height; j = j + BM1.Height / 3)
                                {
                                    if (BM1.GetPixel(i, j) == BM2.GetPixel(i, j))
                                        count++;
                                    incJ++;
                                }
                                incI++;
                            }
                            if (count > 8)
                                return true;
                        }
                    }
                }
                catch { }
            }
            return false;
        }
        void delegatelog(String s, Color col)
        {
            try
            {
                colorStringDelegate = new Form1.ColorStringDelegate(log);
                richTextBox1.Invoke(colorStringDelegate, s, col);
                string[] lines = new string[1];
                lines[0] = s;
                File.AppendAllLines(pathPrefics + "log.txt", lines);
            }
            catch { }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox6.Text != "" && richTextBox6.Text != " " && groupList[listBox1.SelectedIndex].Wl != Convert.ToDouble(richTextBox6.Text))
                {
                    groupList[listBox1.SelectedIndex].Wl = Convert.ToDouble(richTextBox6.Text);
                    log(groupList[listBox1.SelectedIndex].domain + " Wl = " + groupList[listBox1.SelectedIndex].Wl.ToString());
                }
            }
            catch { log("invalid value"); }
            try
            {
                if (richTextBox5.Text != "" && richTextBox5.Text != " " && Kgi != Convert.ToInt16(richTextBox5.Text))
                {
                    Kgi = Convert.ToInt16(richTextBox5.Text);
                    log("set Kgi to " + Kgi.ToString());
                }
            }
            catch { log("invalid value"); }
            try
            {
                if (richTextBox2.Text != "" && richTextBox2.Text != " " && K != Convert.ToDouble(richTextBox2.Text))
                {
                    K = Convert.ToDouble(richTextBox2.Text);
                    log("set K to " + K.ToString());
                }
            }
            catch { log("invalid value"); }
            try
            {
                if (richTextBox3.Text != "" && richTextBox3.Text != " " && postingRateInMinutes != Convert.ToInt32(richTextBox3.Text))
                {
                    postingRateInMinutes = Convert.ToInt32(richTextBox3.Text);

                    log("set posting rate to " + postingRateInMinutes.ToString() + " minutes");
                    if (time_to_posting != null)
                    {
                        if (time_to_posting.Enabled)
                        {
                            log("restart posting...");

                            button11_Click("", null);
                            button7_Click("", null);
                            log("posting was restarted with new rate", Color.LightGreen);
                        }
                    }
                }
            }
            catch { log("invalid value"); }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            API.posting(postList1.ElementAt(listBox2.SelectedIndex));
            //log(API.repost_now(postList1.ElementAt(listBox2.SelectedIndex)));
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox4.Text != null && richTextBox4.Text != "")
                {
                    API.setKey(richTextBox4.Text);
                    File.WriteAllText(pathPrefics + "token.txt", richTextBox4.Text);
                }
                else
                    API.setKey(File.ReadAllText(pathPrefics + "token.txt"));
            }
            catch { log("Ошибка чтения токена"); }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://oauth.vk.com/authorize?client_id=6715394&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=wall,offline&response_type=token&v=5.52");
            }
            catch { }

        }

        private void button12_Click(object sender, EventArgs e)
        {
            set_Wl(null, null);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < groupList.Count; i++)
            {
                log(groupList[i].domain + " = " + groupList[i].Wl);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                time_to_posting.Stop();
            }
            catch { }
            try
            {
                AutoSave.Stop();
            }
            catch { }
            Environment.Exit(0);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}

/*  var ex1 = new MyClass2();
  ex1.stringPoleinsideMyClass2 = "stringPoleinsideMyClass2";
  var ex2 = new MyClass();
  ex2.integerPole = 12345;
  ex2.StringPole = "StringPole";
  ex2.myClassList = new List<MyClass2>();
  ex2.myClassList.Add(ex1);
  richTextBox1.AppendText(JsonConvert.SerializeObject(ex2));*/
