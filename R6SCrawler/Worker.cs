using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;

namespace R6SCrawler
{
    public class Worker
    {
        public static string mainUrl   = "https://r6stats.com/api/player-search/{0}/pc";
        public static string secondUrl = "https://r6stats.com/stats/{0}/";

        [Obsolete]
        public static string resultPath = ConfigurationSettings.AppSettings["resultPath"];

        [Obsolete]
        static void Main()
        {
            string[] players = ConfigurationSettings.AppSettings["user"].Split('|');
            string path      = ConfigurationSettings.AppSettings["path"];

            foreach (string player in players)
            {
                R6StatsData minervaPlayer = new R6StatsData();

                string token = GetUbiToken(player);

                string html = GetUserHTML(token);

                GetRankedInfo(ref minervaPlayer, html);
                GetCasualInfo(ref minervaPlayer, html);
                GetGeneralInfo(ref minervaPlayer, html);

                minervaPlayer._Nick = player;

                minervaPlayer.TerroristHuntTimePlayed = Utils.DateToMinute(minervaPlayer.GeneralTimePlayed, minervaPlayer.CasualTimePlayed, minervaPlayer.RankedTimePlayed).ToString();

                StoreData(path, minervaPlayer);
                
            }

        }

        public static string GetUbiToken(string player)
        {
            string minervaUser = player;
            string userUrl     = String.Format(mainUrl, minervaUser);

            //Load the json with the user data
            HttpWebRequest getToken = (HttpWebRequest)WebRequest.Create(userUrl);

            getToken.Accept                = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            getToken.UserAgent             = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";
            getToken.ContentType           = "application / json";
            getToken.UseDefaultCredentials = true;

            HttpWebResponse tokenData = (HttpWebResponse)getToken.GetResponse();

            string responseString = new StreamReader(tokenData.GetResponseStream()).ReadToEnd().ToString();

            //Continue if the user was not found
            if (String.IsNullOrWhiteSpace(responseString))
            {
                Console.WriteLine("User: " + minervaUser + "not founded.");
                return "";
            }

            //Treat the entire data and gets the ubisoft id
            string[] idSeparator = new string[] { "\"ubisoft_id\":" };
            string ubisoftID     = responseString.Split(idSeparator, StringSplitOptions.None)[1].Split('\"')[1];

            return ubisoftID;

        }

        public static string GetUserHTML(string token)

        {
            string userUrl = String.Format(secondUrl, token);

            //Load the content
            HttpWebRequest getData = (HttpWebRequest)WebRequest.Create(userUrl);

            getData.Accept                = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            getData.UserAgent             = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";
            getData.ContentType           = "application / json";
            getData.UseDefaultCredentials = true;

            HttpWebResponse tokenData = (HttpWebResponse)getData.GetResponse();

            string responseString = new StreamReader(tokenData.GetResponseStream()).ReadToEnd().ToString();

            return responseString;
        }

        #region GetUserInfo

        public static void GetRankedInfo(ref R6StatsData minervaPlayer, string html)
        {

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNodeCollection rankedStats = htmlDoc.DocumentNode.SelectNodes("//*[@class='card stat-card block__ranked horizontal']/div/div");

            if (rankedStats == null)
            {
                Console.WriteLine("rankedStats for the user: " + minervaPlayer + " is empty.");
                return;
            }

            foreach (HtmlNode rankedData in rankedStats)
            {
                if (rankedData.FirstChild == null)
                {
                    continue;
                }

                string value = rankedData.FirstChild.InnerText;

                switch (rankedData.LastChild.InnerText)
                {
                    case "Time Played":
                        minervaPlayer.RankedTimePlayed = value;
                        break;
                    case "Matches Played":
                        minervaPlayer.RankedMatchesPlayed = value;
                        break;
                    case "Kills / Match":
                        minervaPlayer.RankedKPM = value;
                        break;
                    case "Kills":
                        minervaPlayer.RankedKills = value;
                        break;
                    case "Deaths":
                        minervaPlayer.RankedDeaths = value;
                        break;
                    case "K/D Ratio":
                        minervaPlayer.RankedKD = value;
                        break;
                    case "Wins":
                        minervaPlayer.RankedWins = value;
                        break;
                    case "Losses":
                        minervaPlayer.RankedLosses = value;
                        break;
                    case "W/L Ratio":
                        minervaPlayer.RankedWL = value;
                        break;

                }

            }


        }

        public static void GetCasualInfo(ref R6StatsData minervaPlayer, string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNodeCollection casualStats = htmlDoc.DocumentNode.SelectNodes("//*[@class='card stat-card block__casual horizontal']/div/div");

            if(casualStats == null)
            {
                Console.WriteLine("CasualStats for the user: " + minervaPlayer + " is empty.");
                return;
            }

            foreach (HtmlNode casualData in casualStats)
            {
                if (casualData.FirstChild == null)
                {
                    continue;
                }

                string value = casualData.FirstChild.InnerText;

                switch (casualData.LastChild.InnerText)
                {
                    case "Time Played":
                        minervaPlayer.CasualTimePlayed = value;
                        break;
                    case "Matches Played":
                        minervaPlayer.CasualMatchesPlayed = value;
                        break;
                    case "Kills / Match":
                        minervaPlayer.CasualKPM = value;
                        break;
                    case "Kills":
                        minervaPlayer.CasualKills = value;
                        break;
                    case "Deaths":
                        minervaPlayer.CasualDeaths = value;
                        break;
                    case "K/D Ratio":
                        minervaPlayer.CasualKD = value;
                        break;
                    case "Wins":
                        minervaPlayer.CasualWins = value;
                        break;
                    case "Losses":
                        minervaPlayer.CasualLosses = value;
                        break;
                    case "W/L Ratio":
                        minervaPlayer.CasualWL = value;
                        break;

                }
            }
        }

        public static void GetGeneralInfo(ref R6StatsData minervaPlayer, string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNodeCollection generalStats = htmlDoc.DocumentNode.SelectNodes("//*[@class='card stat-card block__overall horizontal']/div/div");

            if (generalStats == null)
            {
                Console.WriteLine("GeneralStats for the user: " + minervaPlayer + " is empty.");
                return;
            }

            foreach (HtmlNode generalData in generalStats)
            {
                if (generalData.FirstChild == null)
                {
                    continue;
                }

                string value = generalData.FirstChild.InnerText;

                switch (generalData.LastChild.InnerText)
                {
                    case "Time Played":
                        minervaPlayer.GeneralTimePlayed = value;
                        break;
                    case "Matches Played":
                        minervaPlayer.GeneralMatchesPlayed = value;
                        break;
                    case "Kills / Match":
                        minervaPlayer.GeneralKPM = value;
                        break;
                    case "Kills":
                        minervaPlayer.GeneralKills = value;
                        break;
                    case "Deaths":
                        minervaPlayer.GeneralDeaths = value;
                        break;
                    case "K/D Ratio":
                        minervaPlayer.GeneralKD = value;
                        break;
                    case "Wins":
                        minervaPlayer.GeneralWins = value;
                        break;
                    case "Losses":
                        minervaPlayer.GeneralLosses = value;
                        break;
                    case "W/L Ratio":
                        minervaPlayer.GeneralWL = value;
                        break;

                }
            }
        }

        #endregion

        [Obsolete]
        public static void StoreData(string path, R6StatsData minervaPlayer)
        {
            //Checks all the files in the given directory
            DirectoryInfo d  = new DirectoryInfo(path);
            FileInfo[] Files = d.GetFiles("*.json"); 

            foreach (FileInfo file in Files)
            {
                if (file.Name.Contains(minervaPlayer._Nick))
                {
                    UpdateData(path+file.Name, minervaPlayer);
                    return;
                }
            }

            // serialize JSON to a string and then write string to a file
            Guid id = Guid.NewGuid();
            File.WriteAllText(path + minervaPlayer._Nick + " " + id + ".json", JsonConvert.SerializeObject(minervaPlayer));

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(path + minervaPlayer._Nick + " " + id + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, minervaPlayer);
            }
        }

        [Obsolete]
        public static void UpdateData(string path, R6StatsData minervaPlayer)
        {
            // read file into a string and deserialize JSON to a type
            R6StatsData storedPlayer = JsonConvert.DeserializeObject<R6StatsData>(File.ReadAllText(path));         

            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, minervaPlayer);
            }

            PlayerData player = new PlayerData();

            player.RankedWins       = Utils.Parse(minervaPlayer.RankedWins, storedPlayer.RankedWins);
            player.RankedKills      = Utils.Parse(minervaPlayer.RankedKills, storedPlayer.RankedKills);
            player.RankedDeaths     = Utils.Parse(minervaPlayer.RankedDeaths, storedPlayer.RankedDeaths);
            player.RankedLosses     = Utils.Parse(minervaPlayer.RankedLosses, storedPlayer.RankedLosses);
            player.RankedTimePlayed = Utils.Parse(Utils.DateToMinute(minervaPlayer.RankedTimePlayed).ToString(), Utils.DateToMinute(storedPlayer.RankedTimePlayed).ToString()).ToString() + " minutos";

            player.CasualWins       = Utils.Parse(minervaPlayer.CasualWins, storedPlayer.CasualWins);
            player.CasualKills      = Utils.Parse(minervaPlayer.CasualKills, storedPlayer.CasualKills);
            player.CasualDeaths     = Utils.Parse(minervaPlayer.CasualDeaths, storedPlayer.CasualDeaths);
            player.CasualLosses     = Utils.Parse(minervaPlayer.CasualLosses, storedPlayer.CasualLosses);
            player.CasualTimePlayed = Utils.Parse(Utils.DateToMinute(minervaPlayer.CasualTimePlayed).ToString(), Utils.DateToMinute(storedPlayer.CasualTimePlayed).ToString()).ToString() + " minutos";

            player.TerroristHuntTimePlayed = Utils.Parse(minervaPlayer.TerroristHuntTimePlayed, storedPlayer.TerroristHuntTimePlayed).ToString() + " minutos";

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(resultPath + minervaPlayer._Nick + ".json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, player);
            }

        }

    }

}
