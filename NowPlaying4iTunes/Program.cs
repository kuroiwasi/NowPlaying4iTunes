using System;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using iTunesLib;

namespace NowPlaying4iTunes
{
    class Program
    {
        static void Main(string[] args)
        {
            NowPlaying np = new NowPlaying();
        }
    }

    [DataContract]
    public class Payload
    {
        [DataMember(Name = "channel")]
        public string Channel { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "icon_emoji")]
        public string IconEmoji { get; set; }
    }

    class NowPlaying
    {
        public NowPlaying()
        {
            var iTunes = new iTunesApp();
            var aClient = new HttpClient();
            var webhookUri = new Uri("///// FILL IN URI OF SLACK WEBHOOK /////");
            var text = "";
            while(true)
            {
                var track = iTunes.CurrentTrack;
                if (track != null && track.Enabled)
                {
                    if (text != track.Name + " - " + track.Artist)
                    {
                        text = track.Name + " - " + track.Artist;
                        Console.WriteLine(text);

                        var payload = new Payload();
                        payload.Channel = "///// FILL IN CHANNEL_NAME /////";
                        payload.UserName = "Now Playing";
                        payload.Text = text;
                        payload.IconEmoji = ":notes:";

                        var jsonSer = new DataContractJsonSerializer(typeof(Payload));
                        var ms = new MemoryStream();
                        jsonSer.WriteObject(ms, payload);
                        ms.Position = 0;
                        StreamReader sr = new StreamReader(ms);
                        StringContent content = new StringContent(sr.ReadToEnd(), System.Text.Encoding.UTF8, "application/json");

                        var response = aClient.PostAsync(webhookUri, content).Result;
                    }
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }
    }

}
