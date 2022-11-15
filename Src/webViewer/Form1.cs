using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Management;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.ComponentModel;
using EO.WebBrowser;
using System.Media;
using CSCore;
using CSCore.SoundIn;
using CSCore.Codecs.WAV;
using NAudio.Lame;
using NAudio.Wave;
namespace webViewer
{
    public partial class Form1 : Form
    {
        private static int width = Screen.PrimaryScreen.Bounds.Width;
        private static int height = Screen.PrimaryScreen.Bounds.Height;
        public static Form1 form = (Form1)Application.OpenForms["Form1"];
        private static bool onbuttons, recording;
        private static string url, loadedpage;
        private string song;
        private AudioFileReader audioFileReader;
        private IWavePlayer waveOutDevice;
        private static int[] wd = { 2, 2, 2, 2, 2 };
        private static int[] wu = { 2, 2, 2, 2, 2 };
        public Form1()
        {
            InitializeComponent();
        }
        private static void valchanged(int n, bool val)
        {
            if (val)
            {
                if (wd[n] <= 1)
                {
                    wd[n] = wd[n] + 1;
                }
                wu[n] = 0;
            }
            else
            {
                if (wu[n] <= 1)
                {
                    wu[n] = wu[n] + 1;
                }
                wd[n] = 0;
            }
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Size = new System.Drawing.Size(width, height);
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = new System.Drawing.Size(width, height);
            this.TopMost = true;
            this.textBox1.AutoSize = false;
            this.textBox2.AutoSize = false;
            this.textBox1.Location = new System.Drawing.Point(245, height - 21);
            this.textBox2.Location = new System.Drawing.Point(width - 35, height - 21);
            this.textBox1.Size = new System.Drawing.Size(width - 245 - 175, 21);
            this.textBox2.Size = new System.Drawing.Size(35, 21);
            this.button8.Location = new System.Drawing.Point(width - 70, height - 21);
            this.button9.Location = new System.Drawing.Point(width - 175, height - 21);
            this.button11.Location = new System.Drawing.Point(width - 140, height - 21);
            this.button10.Location = new System.Drawing.Point(width - 105, height - 21);
            this.button5.Location = new System.Drawing.Point(0, height - 21);
            this.button6.Location = new System.Drawing.Point(35, height - 21);
            this.button1.Location = new System.Drawing.Point(70, height - 21);
            this.button2.Location = new System.Drawing.Point(105, height - 21);
            this.button7.Location = new System.Drawing.Point(140, height - 21);
            this.button3.Location = new System.Drawing.Point(175, height - 21);
            this.button4.Location = new System.Drawing.Point(210, height - 21);
            EO.WebEngine.BrowserOptions options = new EO.WebEngine.BrowserOptions();
            options.EnableWebSecurity = false;
            EO.WebBrowser.Runtime.DefaultEngineOptions.SetDefaultBrowserOptions(options);
            EO.WebEngine.Engine.Default.Options.AllowProprietaryMediaFormats();
            EO.WebEngine.Engine.Default.Options.SetDefaultBrowserOptions(new EO.WebEngine.BrowserOptions
            {
                EnableWebSecurity = false
            });
            this.webView1.Create(pictureBox1.Handle);
            this.webView1.Engine.Options.AllowProprietaryMediaFormats();
            this.webView1.SetOptions(new EO.WebEngine.BrowserOptions
            {
                EnableWebSecurity = false
            });
            settofrontView();
            this.webView1.Engine.Options.DisableGPU = false;
            this.webView1.Engine.Options.DisableSpellChecker = true;
            this.webView1.Engine.Options.CustomUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko";
            Navigate("https://youtube.com");
            webView1.RegisterJSExtensionFunction("demoAbout", new JSExtInvokeHandler(WebView_JSDemoAbout));
            webView1.RegisterJSExtensionFunction("saveDocument", new JSExtInvokeHandler(WebView_JSSaveDocument));
        }
        private void button5_Click(object sender, EventArgs e)
        {
            this.webView1.Dispose();
            this.Close();
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.webView1.Dispose();
            }
            catch { }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            webView1.GoBack();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            webView1.GoForward();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            webView1.Reload();
        }
        private void GoToLoadedPage()
        {
            if (url == "" & loadedpage == @"webViewer.txt")
            {
                LoadChannelsPage();
            }
            if (url == "" & loadedpage == @"webViewerAudio.txt")
            {
                LoadPlayAudioPage();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Navigate(textBox1.Text);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Navigate(textBox1.Text);
            }
        }
        private void Navigate(string address)
        {
            if (String.IsNullOrEmpty(address))
                return;
            if (address.Equals("about:blank"))
                return;
            if (!address.StartsWith("http://") & !address.StartsWith("https://"))
                address = "https://" + address;
            try
            {
                webView1.Url = address;
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            LoadChannelsPage();
        }
        private void LoadChannelsPage()
        {
            Navigate("");
            string path = @"webViewer.txt";
            string readText =  DecryptFiles(path + ".encrypted", "tybtrybrtyertu50727885");
            webView1.LoadHtml(readText);
            loadedpage = path;
        }
        private void webView1_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            if (webView1.Url.Contains("youtu"))
            {
                string stringinject = @"var script = document.createElement('script'); script.src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js'; document.head.appendChild(script);";
                this.webView1.EvalScript(stringinject);
                stringinject = @"
    <style>
        ::-webkit-scrollbar {
            width: 10px;
        }

        ::-webkit-scrollbar-track {
            background: rgba(0, 0, 0, 0.9);
        }

        ::-webkit-scrollbar-thumb {
            background: #888;
        }

            ::-webkit-scrollbar-thumb:hover {
                background: #eee;
            }
    </style>
".Replace("\r\n", " ");
                stringinject = @"""" + stringinject + @"""";
                stringinject = @"document.getElementsByTagName('head')[0].innerHTML += " + stringinject + @";";
                this.webView1.EvalScript(stringinject);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!onbuttons)
                    form.webView1.SetFocus();
            }
            catch { }
            try
            {
                EO.WebEngine.CookieCollection cookies = this.webView1.Engine.CookieManager.GetCookies(url, true);
                foreach (EO.WebEngine.Cookie cookie in cookies)
                {
                    string name = cookie.Name;
                    this.webView1.Engine.CookieManager.DeleteCookies(url, name);
                    System.Threading.Thread.Sleep(1);
                }
            }
            catch { }
            try
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.pictureBox1.Visible = false;
                }
                else
                {
                    this.pictureBox1.Visible = true;
                }
            }
            catch { }
            if (webView1.Url.Contains("youtu"))
            {
                string stringinject = @"
                    document.cookie = 'VISITOR_INFO1_LIVE = oKckVSqvaGw; path =/; domain =.youtube.com';
                    var cookies = document.cookie.split('; ');
                    for (var i = 0; i < cookies.length; i++)
                    {
                        var cookie = cookies[i];
                        var eqPos = cookie.indexOf('=');
                        var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
                        document.cookie = name + '=;expires=Thu, 01 Jan 1970 00:00:00 GMT';
                    }
                    var el = document.getElementsByClassName('ytp-ad-skip-button');
                    for (var i=0;i<el.length; i++) {
                        el[i].click();
                    }
                    var element = document.getElementsByClassName('ytp-ad-overlay-close-button');
                    for (var i=0;i<element.length; i++) {
                        element[i].click();
                    }
                    var scripts = document.getElementsByTagName('script');
                    for (let i = 0; i < scripts.length; i++)
                    {
                        var content = scripts[i].innerHTML;
                        if (content.indexOf('ytp-ad') > -1) {
                            scripts[i].innerHTML = '';
                        }
                        var src = scripts[i].getAttribute('src');
                        if (src.indexOf('ytp-ad') > -1) {
                            scripts[i].setAttribute('src', '');
                        }
                    }
                    var iframes = document.getElementsByTagName('iframe');
                    for (let i = 0; i < iframes.length; i++)
                    {
                        var content = iframes[i].innerHTML;
                        if (content.indexOf('ytp-ad') > -1) {
                            iframes[i].innerHTML = '';
                        }
                        var src = iframes[i].getAttribute('src');
                        if (src.indexOf('ytp-ad') > -1) {
                            iframes[i].setAttribute('src', '');
                        }
                    }
                    var allelements = document.querySelectorAll('*');
                    for (var i = 0; i < allelements.length; i++) {
	                    var classname = allelements[i].className;
                        if (classname.indexOf('ytp-ad') > -1)  {
                                allelements[i].innerHTML = '';
			            }
                    }
                ".Replace("\r\n", " ");
                this.webView1.QueueScriptCall(stringinject);
            }
        }
        private void webView1_MouseMove(object sender, EO.Base.UI.MouseEventArgs e)
        {
            valchanged(0, e.Y >= height - 31);
            if (wd[0] == 1)
            {
                settofrontButtons();
                onbuttons = true;
                this.textBox2.Text = DateTime.Now.ToString("HH:mm");
            }
            if (wu[0] == 1)
            {
                settofrontView();
                onbuttons = false;
            }
        }
        private void webView1_UrlChanged(object sender, EventArgs e)
        {
            string address = webView1.Url.ToString();
            textBox1.Text = address;
            url = address;
            GoToLoadedPage();
            this.webView1.SetFocus();
        }
        private void button10_Click(object sender, EventArgs e)
        {
            LoadPlayAudioPage();
        }
        private void LoadPlayAudioPage()
        {
            Navigate("");
            string path = @"webViewerAudio.txt";
            string readText = DecryptFiles(path + ".encrypted", "tybtrybrtyertu50727885");
            webView1.LoadHtml(readText);
            loadedpage = path;
        }
        public static void EncryptStringToFile(string contents, string outputFile, string password)
        {
            byte[] salt = new byte[8];
            using (var rng = new RNGCryptoServiceProvider())
                rng.GetBytes(salt);
            using (var encryptedStream = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(encryptedStream);
                sw.Write(contents);
                sw.Flush();
                encryptedStream.Seek(0, SeekOrigin.Begin);
                using (var pbkdf = new Rfc2898DeriveBytes(password, salt))
                using (var aes = new RijndaelManaged())
                using (var encryptor = aes.CreateEncryptor(pbkdf.GetBytes(aes.KeySize / 8), pbkdf.GetBytes(aes.BlockSize / 8)))
                using (var output = File.Create(outputFile))
                {
                    output.Write(salt, 0, salt.Length);
                    using (var cs = new CryptoStream(output, encryptor, CryptoStreamMode.Write))
                        encryptedStream.CopyTo(cs);
                    encryptedStream.Flush();
                }
            }
        }
        public static string DecryptFiles(string inputFile, string password)
        {
            using (var input = File.OpenRead(inputFile))
            {
                byte[] salt = new byte[8];
                input.Read(salt, 0, salt.Length);
                using (var decryptedStream = new MemoryStream())
                using (var pbkdf = new Rfc2898DeriveBytes(password, salt))
                using (var aes = new RijndaelManaged())
                using (var decryptor = aes.CreateDecryptor(pbkdf.GetBytes(aes.KeySize / 8), pbkdf.GetBytes(aes.BlockSize / 8)))
                using (var cs = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
                {
                    string contents;
                    int data;
                    while ((data = cs.ReadByte()) != -1)
                        decryptedStream.WriteByte((byte)data);
                    decryptedStream.Position = 0;
                    using (StreamReader sr = new StreamReader(decryptedStream))
                        contents = sr.ReadToEnd();
                    decryptedStream.Flush();
                    return contents;
                }
            }
        }
        void WebView_JSDemoAbout(object sender, JSExtInvokeArgs e)
        {
            song = e.Arguments[1] as string;
            try
            {
                waveOutDevice.Stop();
                audioFileReader.Dispose();
                waveOutDevice.Dispose();
            }
            catch { }
            waveOutDevice = new WaveOut();
            audioFileReader = new AudioFileReader(song);
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();
        }
        void WebView_JSSaveDocument(object sender, JSExtInvokeArgs e)
        {
            string pageincoming = e.Arguments[0] as string;
            string innerhtml = e.Arguments[1] as string;
            string newstringsearches = e.Arguments[2] as string; 
            int pFrom = innerhtml.IndexOf("var searches = ") + "var searches = ".Length;
            int pTo = innerhtml.LastIndexOf(@"""""];");
            string result = innerhtml.Substring(pFrom, pTo - pFrom);
            innerhtml = innerhtml.Replace(result, newstringsearches);
            string path = pageincoming + ".txt";
            EncryptStringToFile(innerhtml, path + ".encrypted", "tybtrybrtyertu50727885");
            webView1.LoadHtml(innerhtml);
            loadedpage = path;
            MessageBox.Show("The page have been saved.");
        }
        private void button8_Click(object sender, EventArgs e)
        {
            EO.WebEngine.Engine.CleanUpCacheFolders(EO.WebEngine.CacheFolderCleanUpPolicy.AllVersions);
        }
        private void button9_Click(object sender, EventArgs e)
        {
            if (!recording)
            {
                recording = true;
                this.button9.ForeColor = System.Drawing.Color.Red;
                Task.Factory.StartNew(() => recordingSound());
            }
            else
            {
                recording = false;
                button9.ForeColor = System.Drawing.Color.Silver;
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            Process[] processes;
            try
            {
                processes = Process.GetProcessesByName("Editor");
                foreach (Process process in processes)
                    process.Kill();
            }
            catch { }
            if (File.Exists("Editor.exe"))
            {
                this.WindowState = FormWindowState.Minimized;
                Process.Start("Editor.exe");
            }
        }
        private void recordingSound()
        {
            try
            {
                string localDate = DateTime.Now.ToString();
                string audioName = localDate.Replace(" ", " -").Replace("/", "-").Replace(":", "-") + ".wav";
                CSCore.SoundIn.WasapiCapture capture = new CSCore.SoundIn.WasapiLoopbackCapture();
                capture.Initialize();
                using (WaveWriter wavewriter = new WaveWriter(audioName, capture.WaveFormat))
                {
                    capture.DataAvailable += (sound, card) =>
                    {
                        wavewriter.Write(card.Data, card.Offset, card.ByteCount);
                    };
                    capture.Start();
                    for (int count = 0; count <= 60 * 60 * 1000; count++)
                    {
                        if (!recording | count == 60 * 60 * 1000)
                        {
                            capture.Stop();
                            wavewriter.Dispose();
                            ConvertWavMP3(audioName);
                            this.WindowState = FormWindowState.Minimized;
                            Task.Factory.StartNew(() => openDirectory());
                            break;
                        }
                        Thread.Sleep(1);
                    }
                }
            }
            catch { }
        }
        private void openDirectory()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = AppContext.BaseDirectory,
                FileName = "explorer.exe"
            };
            Process.Start(startInfo);
        }
        private void ConvertWavMP3(string wavFile)
        {
            try
            {
                using (var wavRdr = new NAudio.Wave.WaveFileReader(wavFile))
                using (var mp3Writer = new NAudio.Lame.LameMP3FileWriter(wavFile.Replace(".wav", ".mp3"), wavRdr.WaveFormat, 128))
                {
                    wavRdr.CopyTo(mp3Writer);
                    mp3Writer.Close();
                    wavRdr.Close();
                }
                File.Delete(wavFile);
            }
            catch { }
        }
        private void webView1_NewWindow(object sender, NewWindowEventArgs e)
        {
            Navigate(e.TargetUrl);
        }
        private void webView1_RequestPermissions(object sender, RequestPermissionEventArgs e)
        {
            e.Deny();
        }
        private void settofrontButtons()
        {
            this.textBox1.BringToFront();
            this.button5.BringToFront();
            this.button6.BringToFront();
            this.button1.BringToFront();
            this.button2.BringToFront();
            this.button7.BringToFront();
            this.button3.BringToFront();
            this.button4.BringToFront();
            this.pictureBox1.SendToBack();
        }
        private void settofrontView()
        {
            this.textBox1.SendToBack();
            this.button5.SendToBack();
            this.button6.SendToBack();
            this.button1.SendToBack();
            this.button2.SendToBack();
            this.button7.SendToBack();
            this.button3.SendToBack();
            this.button4.SendToBack();
            this.pictureBox1.BringToFront();
            this.webView1.SetFocus();
        }
    }
}