using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using DiscordRPC;
using DiscordRPC.Logging;
using Newtonsoft.Json;
using WebSocketSharp;
using WebSocketSharp.Server;
using Button = DiscordRPC.Button;

namespace YTRPC
{
    public partial class Form1 : Form
    {
        private DiscordRpcClient client;
        private NotifyIcon notifyIcon;
        private WebSocketServer server;
        private string clientId;  // placeholder for client ID

        public Form1()
        {
            //Loads Client ID from user config file
            LoadClientID();

            InitializeComponent();

            // Initialize the Discord RPC client
            client = new DiscordRpcClient(clientId)
            {
                Logger = new ConsoleLogger() { Level = DiscordRPC.Logging.LogLevel.Warning },
            };

            // Set the callback for when Discord is ready
            client.OnReady += (sender, e) =>
            {
                MessageBox.Show("Discord connected: " + e.User.Username, "YTRPC", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // Start the RPC client
            client.Initialize();

            // Start WebSocket
            StartWebSocketServer();

            // Create system tray icon and menu
            notifyIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.app,  // Using your embedded icon
                Visible = true,
                Text = "YTRPC - YouTube Music to Discord RPC",
                ContextMenuStrip = new ContextMenuStrip()
            };
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, Exit_Click);
        }

        private void LoadClientID()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
            if (File.Exists(configPath))
            {
                try
                {
                    var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(configPath));
                    clientId = config["ClientId"];
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "YTRPC");
                }
            }
            else
            {
                DialogResult res = MessageBox.Show("config.json file not found, please ensure it exists in the application folder and try again", "YouTube Music RPC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if(res == DialogResult.OK)
                {
                    Environment.Exit(0);
                }
            }
        }

        public void SetActivity(string state, string details, string imageKey, string url)
        {
            client.SetPresence(new RichPresence()
            {
                State = state,  // The text shown on Discord
                Details = details,  // More detailed info
                Timestamps = Timestamps.Now,
                Type  = ActivityType.Listening,
                Assets = new Assets()
                {
                    LargeImageKey = imageKey,  // Your image key
                    LargeImageText = "Now Playing"  // Image text
                },
                Buttons = new Button[]
                {
                    new Button()
                    {
                        Label = "Listen On YouTube Music",
                        Url = url
                    }
                } 
            });
        }

        private void StartWebSocketServer()
        {
            try
            {
                server = new WebSocketServer(5000);
                server.AddWebSocketService<MediaHandler>("/ws", () => new MediaHandler(this));
                server.Start();
                Debug.WriteLine("WebSocket server started on ws://localhost:5000/ws");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("WebSocket server failed to start: " + ex.Message);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            // Stop the Discord RPC client and exit
            client.Dispose();
            notifyIcon.Visible = false;
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.Hide();
        }
    }

    public class MediaHandler : WebSocketBehavior
    {
        private Form1 _form;
        // Constructor to pass Form1 reference
        public MediaHandler(Form1 form)
        {
            _form = form;
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var data = JsonConvert.DeserializeObject<MediaData>(e.Data);
            Debug.WriteLine(data.Title);
            // Set the presence (activity)
            _form.SetActivity(data.Artist, data.Title, data.Artwork+"?fit=clip", data.Url);
        }
    }

    public class MediaData
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Url { get; set; }
        public string Artwork { get; set; }
    }
}
