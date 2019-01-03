using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuickType;
using System.IO;

namespace Bestmems
{
    public class Group
    {
        public double Wl=1;
        public System.Drawing.Color color;
        public string id;
        public Queue<Post> posts;
        public string name;
        public int folowersCount;
        public int historyVolume = 1440;
        public int postCapasity = 98;
        public string domain;
        public API API;
        public bool new_post = false;
        [JsonConstructor]
        public Group(string id)
        {
            posts = new Queue<Post>(postCapasity);
            this.id = id;
        }
        public void load(API API)
        {
            this.API = API;
            if (name == null || name == "")
                name = API.getGroupNameById(id.ToString());
            //addPosts();
        }
        public Group(string id, API API)
        {     Random r = new Random();

            this.API = API;
            posts = new Queue<Post>(postCapasity);
            this.id = id;
            name = API.getGroupNameById(id.ToString());
            addPosts();
            
            color = System.Drawing.Color.FromArgb(255, r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
        }

        public Group(string domain, API API, int makaroni)
        {
            this.API = API;
            posts = new Queue<Post>(postCapasity);
            this.domain = domain;
            id = API.getGroupIdByDomain(domain);
            name = API.getGroupNameById(id);
            addPosts();
            Random r = new Random();
            color = System.Drawing.Color.FromArgb(255, r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
        }

        public Post getPostById(string Id)
        {
            for (int i = 0; i < postCapasity; i++)
            {
                if (posts.ElementAt(i).id == Id)
                    return posts.ElementAt(i);
            }
            return new Post();
        }
        public void addPosts()
        {

            int loaded = 0;
            List<Item> items = new List<Item>();
            if (postCapasity <= 99)
            {
                WallGet response = WallGet.FromJson(API.getResponse("wall.get", "owner_id=-" + id+ ",offset=2" + ",count=" + postCapasity.ToString()));
                foreach (Item item in response.Response.Items)
                    items.Add(item);
            }
            else
            {
                while (postCapasity - loaded > 99)
                {
                    WallGet response1 = WallGet.FromJson(API.getResponse("wall.get", "owner_id=-" + id + ",offset=" + loaded.ToString() + ",count=99"));
                    loaded = loaded + 99;
                    foreach (Item item in response1.Response.Items)
                        items.Add(item);
                    if (response1.Response.Items.Length < 99)
                        goto GA;
                }
                WallGet response = WallGet.FromJson(API.getResponse("wall.get", "owner_id=-" + id + ",offset=" + loaded.ToString() + ",count=" + (postCapasity - loaded).ToString()));
                foreach (Item item in response.Response.Items)
                    items.Add(item);
            }
            GA:
            foreach (Item item in items)
            {
                try
                {
                    Post post = new Post();
                    post.id = item.Id.ToString();
                    post.item = item;
                    post.already_posted = false;

                    post.hist = new Queue<HistoryPoint>(historyVolume);
                    HistoryPoint point = new HistoryPoint();
                    point.likes = Convert.ToInt32(item.Likes.Count);
                    point.reposts = Convert.ToInt32(item.Reposts.Count);
                    point.views = Convert.ToInt32(item.Views.Count);
                    point.time = Convert.ToInt32(item.Date);
                    post.hist.Enqueue(point);
                    post = setFirstPoints(post);
                    post.domain = domain;
                    post.groupId = id;
                    posts.Enqueue(post);
                }
                catch { }
            }

        }
        public void updatePosts()
        {
            try
            {
                WallGet response = WallGet.FromJson(API.getResponse("wall.get", "owner_id=-" + id + ",count=" + postCapasity.ToString()));
                foreach (Item item in response.Response.Items)
                {
                    for (int i = 0; i < posts.Count; i++)
                    {
                        if (posts.ElementAt(i).id == item.Id.ToString())
                        {
                            HistoryPoint point = new HistoryPoint();
                            point.likes = Convert.ToInt32(item.Likes.Count);
                            point.reposts = Convert.ToInt32(item.Reposts.Count);
                            if (item.Views == null)
                                point.views = 0;
                            else
                                point.views = Convert.ToInt32(item.Views.Count);
                            point.time = Convert.ToInt32(item.Date);
                            posts.ElementAt(i).hist.Enqueue(point);
                            setFirstPoints(posts.ElementAt(i));
                            if (posts.ElementAt(i).hist.Count > historyVolume)
                            {
                                posts.ElementAt(i).hist.Dequeue();
                            }
                        }

                    }
                }
                foreach (Item item in response.Response.Items)
                {
                    bool is_new = true;
                    for (int i = 0; i < posts.Count; i++)
                    {
                        if (posts.ElementAt(i).id == item.Id.ToString())
                        { is_new = false; }
                    }
                    if (is_new)
                    {
                        new_post = true;
                        Post post = new Post();
                        post.id = item.Id.ToString();
                        post.item = item;
                        post.already_posted = false;

                        HistoryPoint point = new HistoryPoint();
                        point.likes = Convert.ToInt32(item.Likes.Count);
                        point.reposts = Convert.ToInt32(item.Reposts.Count);

                        if (item.Views == null)
                            point.views = 0;
                        else
                            point.views = Convert.ToInt32(item.Views.Count);
                        point.time = Convert.ToInt32(item.Date);
                        post.hist = new Queue<HistoryPoint>(historyVolume);
                        post.hist.Enqueue(point);
                        post = setFirstPoints(post);
                        post.domain = domain;
                        post.groupId = id;
                        posts.Enqueue(post);
                        posts.Dequeue();
                    }
                }
            }
            catch { }
        }
        void updateFolowerCount() { }

        static public Post setFirstPoints(Post post)
        {
            var now = new DateTimeOffset(DateTime.Now);
            if (now.ToUnixTimeSeconds() - post.item.Date < 480 * 60)
                post.first480min = post.hist.Last();
            if (now.ToUnixTimeSeconds() - post.item.Date < 1440 * 60)
                post.first1440min = post.hist.Last();
            if (now.ToUnixTimeSeconds() - post.item.Date < 60 * 60)
                post.first60min = post.hist.Last();
            if (now.ToUnixTimeSeconds() - post.item.Date < 120 * 60)
                post.first120min = post.hist.Last();
            return post;
        }
        static public List<Post> setAVG(List<Post> posts, int SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts)
        {
            double avg = 0;
            for (int i = 0; i < posts.Count; i++)
            {



                if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 1)
                {
                    double score1 = Convert.ToDouble(posts.ElementAt(i).hist.ElementAt(posts.ElementAt(i).hist.Count - 1).likes) / Convert.ToDouble(posts.ElementAt(i).hist.ElementAt(posts.ElementAt(i).hist.Count - 1).views);
                    avg = avg + score1;
                }

                if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 2)
                {
                    double score1 = Convert.ToDouble(posts.ElementAt(i).hist.ElementAt(posts.ElementAt(i).hist.Count - 1).reposts) / Convert.ToDouble(posts.ElementAt(i).hist.ElementAt(posts.ElementAt(i).hist.Count - 1).views);
                    avg = avg + score1;
                }
                if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 3)
                {
                    double score1 = Convert.ToDouble(posts.ElementAt(i).hist.ElementAt(posts.ElementAt(i).hist.Count - 1).likes);
                    avg = avg + score1;
                }

                if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 4)
                {
                    double score1 = Convert.ToDouble(posts.ElementAt(i).hist.ElementAt(posts.ElementAt(i).hist.Count - 1).reposts);
                    avg = avg + score1;
                }

            }
            for (int i = 0; i < posts.Count; i++)
            {
                posts[i] = posts[i].setAVG(avg / posts.Count);
            }
            return posts;
        }
        static public List<Post> getSortedPosts(List<Post> posts, int SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts, int OnFirstMinutes60or120or480or1440orALLis0)
        {
            var res = new List<Post>();
            for (int i = 0; i < posts.Count; i++)
            { res.Add(posts[i]); }
            var temp = new Post();
            for (int i = 0; i < posts.Count; i++)
            {
                if (OnFirstMinutes60or120or480or1440orALLis0 == 0)
                {
                    if (res[i].hist.ElementAt(posts.ElementAt(i).hist.Count - 1).views != 0)
                    {
                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 1)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].hist.ElementAt(posts.ElementAt(i).hist.Count - 1).likes) / Convert.ToDouble(res[i].hist.ElementAt(posts.ElementAt(i).hist.Count - 1).views)) / res[i].AVG);
                        }

                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 2)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].hist.ElementAt(posts.ElementAt(i).hist.Count - 1).reposts) / Convert.ToDouble(res[i].hist.ElementAt(posts.ElementAt(i).hist.Count - 1).views)) / res[i].AVG);
                        }
                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 3)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].hist.ElementAt(posts.ElementAt(i).hist.Count - 1).likes)) / res[i].AVG);
                        }

                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 4)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].hist.ElementAt(posts.ElementAt(i).hist.Count - 1).reposts)) / res[i].AVG);
                        }
                    }
                }
                if (OnFirstMinutes60or120or480or1440orALLis0 == 480)
                {
                    if (res[i].first480min.views != 0)
                    {
                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 1)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first480min.likes) / Convert.ToDouble(res[i].first480min.views)) / res[i].AVG);
                        }

                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 2)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first480min.reposts) / Convert.ToDouble(res[i].first480min.views)) / res[i].AVG);
                        }
                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 3)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first480min.likes)) / res[i].AVG);
                        }

                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 4)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first480min.reposts)) / res[i].AVG);
                        }
                    }
                }
                if (OnFirstMinutes60or120or480or1440orALLis0 == 1440)
                {
                    if (res[i].first1440min.views != 0)
                    {
                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 1)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first1440min.likes) / Convert.ToDouble(res[i].first1440min.views)) / res[i].AVG);
                        }

                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 2)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first1440min.reposts) / Convert.ToDouble(res[i].first1440min.views)) / res[i].AVG);
                        }
                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 3)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first1440min.likes)) / res[i].AVG);
                        }

                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 4)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first1440min.reposts)) / res[i].AVG);
                        }
                    }
                }
                if (OnFirstMinutes60or120or480or1440orALLis0 == 120)
                {
                    if (res[i].first120min.views != 0)
                    {
                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 1)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first120min.likes) / Convert.ToDouble(res[i].first120min.views)) / res[i].AVG);
                        }

                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 2)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first120min.reposts) / Convert.ToDouble(res[i].first120min.views)) / res[i].AVG);
                        }
                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 3)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first120min.likes)) / res[i].AVG);
                        }

                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 4)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first120min.reposts)) / res[i].AVG);
                        }
                    }
                }
                if (OnFirstMinutes60or120or480or1440orALLis0 == 60)
                {
                    if (res[i].first60min.views != 0)
                    {
                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 1)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first60min.likes) / Convert.ToDouble(res[i].first60min.views)) / res[i].AVG);
                        }

                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 2)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first60min.reposts) / Convert.ToDouble(res[i].first60min.views)) / res[i].AVG);
                        }
                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 3)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first60min.likes)) / res[i].AVG);
                        }

                        if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 4)
                        {
                            res[i] = res[i].setK((Convert.ToDouble(res[i].first60min.reposts)) / res[i].AVG);
                        }
                    }
                }
            }

            for (int i = 0; i < posts.Count; i++)
            {
                res[i] = res[i].setK(res[i].K * res[i].K * res[i].K * res[i].Wl);
            }

                for (int i = 0; i < posts.Count; i++)
            {
                for (int j = i + 1; j < posts.Count; j++)
                {

                    if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 1)
                    {

                        double score1 = res[i].K;
                        double score2 = res[j].K;

                        if (score1 < score2)
                        {
                            temp = res[i];
                            res[i] = res[j];
                            res[j] = temp;
                        }

                    }

                    if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 2)
                    {

                        double score1 = res[i].K;
                        double score2 = res[j].K;

                        if (score1 < score2)
                        {
                            temp = res[i];
                            res[i] = res[j];
                            res[j] = temp;
                        }
                    }

                    if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 3)
                    {

                        double score1 = res[i].K;
                        double score2 = res[j].K;

                        if (score1 < score2)
                        {
                            temp = res[i];
                            res[i] = res[j];
                            res[j] = temp;
                        }

                    }

                    if (SortedBy1isLikesPerView2isPostsPerView3isLikes4isPosts == 4)
                    {

                        double score1 = res[i].K;
                        double score2 = res[j].K;

                        if (score1 < score2)
                        {
                            temp = res[i];
                            res[i] = res[j];
                            res[j] = temp;
                        }
                    }
                }
            }

            return res;
        }

        public Post getTop1ofFirst480minByLikes()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first480min.likes;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst1440minByLikes()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first1440min.likes;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst20minByLikes()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first20min.likes;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst60minByLikes()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first60min.likes;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst120minByLikes()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first120min.likes;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst240minByLikes()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first240min.likes;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst480minByReposts()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first480min.reposts;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst1440minByReposts()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first1440min.reposts;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst20minByReposts()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first20min.reposts;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst60minByReposts()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first60min.reposts;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst120minByReposts()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first120min.reposts;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }
        public Post getTop1ofFirst240minByReposts()
        {
            int best = 0;
            var res = new Post();
            foreach (Post post in posts)
            {
                int score = post.first240min.reposts;
                if (score != 0)
                    if (score > best)
                    {
                        best = score;
                        res = post;
                    }
            }
            return res;
        }


    }
    public struct Post
    {
        public Post setK(double K)
        {
            this.K = K;
            return this;
        }
        public Post setAVG(double AVG)
        {
            this.AVG = AVG;
            return this;
        }
        public bool already_posted;
        public double AVG;
        public double K;
        public double Wl;
        public Item item;
        public string id;
        public Queue<HistoryPoint> hist;
        public string domain;
        public string groupId;
        public HistoryPoint first480min;
        public HistoryPoint first1440min;
        public HistoryPoint first20min;
        public HistoryPoint first60min;
        public HistoryPoint first120min;
        public HistoryPoint first240min;
    }
    public struct HistoryPoint
    {
        public int time;
        public int likes;
        public int reposts;
        public int views;
    }

}
