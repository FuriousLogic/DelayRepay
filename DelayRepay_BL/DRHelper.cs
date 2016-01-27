using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DelayRepay_BL.Tokenise;
using System.Net.Mail;
using System.Net;

namespace DelayRepay_BL
{
    public class StationPair
    {
        public Station Station1 { get; set; }
        public Station Station2 { get; set; }
    }

    public class DRHelper
    {
        private readonly DR_Entities _db;

        public DRHelper()
        {
            _db = new DR_Entities();
        }

        private enum MsgType
        {
            FunctionCall,
            Error,
            Info
        }

        public void DoSomething(bool isConnected = true)
        {
            ConsoleMsg("DoSomething",MsgType.FunctionCall);
            try
            {
                //Do the emails need to be sent?
                EmailBatch eBatch = (from eb in _db.EmailBatches orderby eb.Created descending select eb).FirstOrDefault();
                if (eBatch == null)
                {
                    //Make fake batch for prev Monday am
                    DateTime fakeTime = DateTime.Today.AddHours(2);
                    fakeTime = fakeTime.AddDays((((int)fakeTime.DayOfWeek) - 1) * -1);
                    fakeTime = fakeTime.AddDays(-7);
                    eBatch = new EmailBatch
                    {
                        Created = fakeTime
                    };
                }
                if ((DateTime.Now - eBatch.Created).TotalDays >= 7)
                    SendClaimEmails();
                else
                    HarvestTrainJourneyInfo(isConnected);
            }
            catch (Exception ex)
            {
                ConsoleMsg(ex.Message,MsgType.Error);
                var db = new DR_Entities();
                LogType ltError = (from lt in db.LogTypes where lt.LogTypeName == "Error" select lt).FirstOrDefault();
                var log = new Log
                {
                    LogType = ltError,
                    Header = ex.Message,
                    Message = ex.StackTrace,
                    Timestamp = DateTime.Now
                };
                db.Logs.Add(log);
                db.SaveChanges();
                db.Dispose();
            }
        }

        private static void ConsoleMsg(string msg, MsgType mt)
        {
            switch (mt)
            {
                case MsgType.FunctionCall:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case MsgType.Info:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case MsgType.Error:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.BackgroundColor=ConsoleColor.White;
                    Console.ForegroundColor=ConsoleColor.Black;
                    msg = $"Unknown Message Type: {mt}. {msg}";
                    break;
            }

            Console.WriteLine(msg);
        }

        private void SendClaimEmails()
        {
            ConsoleMsg("SendClaimEmails", MsgType.FunctionCall);
            //Create new Batch
            var newBatch = new EmailBatch
            {
                Created = DateTime.Today
            };
            _db.EmailBatches.Add(newBatch);

            //Get Station Pairs
            var stationPairs = new List<StationPair>();
            foreach (User user in _db.Users)
            {
                //Build suggested new SP
                var stationPair = new StationPair();
                if (user.HomeStationId > user.DestinationStationId)
                {
                    stationPair.Station1 = user.DestinationStation;
                    stationPair.Station2 = user.HomeStation;
                }
                else
                {
                    stationPair.Station1 = user.HomeStation;
                    stationPair.Station2 = user.DestinationStation;
                }

                //Is it already in list?
                int spCount = (from sp in stationPairs where sp.Station1.Id == stationPair.Station1.Id && sp.Station2.Id == stationPair.Station2.Id select sp).ToList().Count;
                if (spCount == 0)
                    stationPairs.Add(stationPair);
            }

            //Get delayed trains since last email
            var claimableDestinations = new List<Destination>();
            List<Destination> destinationsNotEmailed = (from d in _db.Destinations where d.EmailBatchId == null select d).ToList();
            foreach (Destination destinationNotEmailed in destinationsNotEmailed)
            {
                if (destinationNotEmailed.IsClaimable)
                    claimableDestinations.Add(destinationNotEmailed);
            }

            //Each Station Pair
            var smtpClient = new SmtpClient(Properties.Settings.Default.SmtpClient)
            {
                Port = Properties.Settings.Default.SmtpPort,
                Credentials =
                    new NetworkCredential(Properties.Settings.Default.SmtpUsername,
                        Properties.Settings.Default.SmtpPassword),
                EnableSsl = true
            };
            foreach (StationPair sp in stationPairs)
            {
                //Get Claimable trains for this pair
                var fromStation = new Station();
                var claimableForStationPair = new List<Destination>();
                foreach (Destination claimableDestination in claimableDestinations)
                {
                    if (sp.Station1.Id == claimableDestination.StationId) fromStation = sp.Station2;
                    if (sp.Station2.Id == claimableDestination.StationId) fromStation = sp.Station1;
                    if (fromStation.Id == 0) continue;

                    int fromStationCount = (from fs in claimableDestination.FromStations where fs.StationId == fromStation.Id select fs).ToList().Count;
                    if (fromStationCount > 0)
                        claimableForStationPair.Add(claimableDestination);
                }

                //Build email body
                var mm = new MailMessage
                {
                    Subject = "Train Delays.  Claimable: " + claimableForStationPair.Count,
                    Body =
                        "Travel between " + sp.Station1.StationName + " and " + sp.Station2.StationName +
                        Environment.NewLine
                };
                mm.Body += "Number of claimable trains: " + claimableForStationPair.Count + Environment.NewLine;
                foreach (Destination claimer in claimableForStationPair)
                {
                    //Save data
                    newBatch.Destinations.Add(claimer);

                    //Get FromStation
                    FromStation fsClaimer = (from fs in claimer.FromStations where fs.StationId == fromStation.Id select fs).FirstOrDefault();
                    if (fsClaimer == null)
                        throw new Exception("Can't find FromStation");

                    string line = claimer.Journey.TrainOperator + ". ";
                    line += claimer.MinutesDelay + " minutes delay.  ";
                    line += fsClaimer.ScheduledDeparture.ToString("ddd dd/MM/yy HH:mm") + " from " + fsClaimer.Station.StationName;
                    //line += "Arrived at " + claimer.Station.StationName + " [" + claimer.ScheduledArrival.ToString("ddd dd/MM/yy HH:mm") + "]: " + claimer.MinutesDelay.ToString() + " Minutes late.";
                    mm.Body += line + Environment.NewLine;
                }
                mm.From = new MailAddress("Hell.Yeah@StickItToTheMan.com");
                mm.IsBodyHtml = false;

                //Send to relevant users
                List<User> users = (from u in _db.Users
                                    where (u.HomeStationId == sp.Station1.Id || u.HomeStationId == sp.Station2.Id)
                                    && (u.DestinationStationId == sp.Station1.Id || u.DestinationStationId == sp.Station2.Id)
                                    select u).ToList();
                if (users.Count == 0) throw new Exception("No users found");
                foreach (User recipient in users)
                {
                    mm.To.Clear();
                    mm.To.Add(new MailAddress(recipient.EMail));
                    try
                    {
                        smtpClient.Send(mm);
                    }
                    catch (Exception ex)
                    {
                        ConsoleMsg(ex.Message, MsgType.Error);
                    }
                    _db.SaveChanges();
                }
            }
            _db.SaveChanges();
        }

        public void TestEMail()
        {
            ConsoleMsg("TestEMail", MsgType.FunctionCall);
            try
            {
                var mm = new MailMessage
                {
                    From = new MailAddress("Claims@DelayRepay.dash"),
                    Subject = "Delay Repay Test EMail - " + DateTime.Now.ToString("dd MMM yyyy HH:mm")
                };

                var smtp = new SmtpClient
                {
                    Host = Properties.Settings.Default.SmtpClient,
                    Port = Properties.Settings.Default.SmtpPort,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(Properties.Settings.Default.SmtpUsername, Properties.Settings.Default.SmtpPassword),
                    EnableSsl = true,
                    Timeout = 20000
                };

                mm.Body = "Test email";
                mm.To.Add(new MailAddress("mrbarrydonaldson@gmail.com"));
                smtp.Send(mm);
            }
            catch (Exception ex)
            {
                ConsoleMsg(ex.Message, MsgType.Error);
                var db = new DR_Entities();
                LogType ltError = (from lt in db.LogTypes where lt.LogTypeName == "Error" select lt).FirstOrDefault();
                var log = new Log
                {
                    LogType = ltError,
                    Header = ex.Message,
                    Message = ex.StackTrace,
                    Timestamp = DateTime.Now
                };
                db.Logs.Add(log);
                db.SaveChanges();
                db.Dispose();
            }
        }

        private void HarvestTrainJourneyInfo(bool isConnected)
        {
            ConsoleMsg("HarvestTrainJourneyInfo", MsgType.FunctionCall);
            IEnumerable<Station> stations = GetDistinctListOfStations();
            foreach (Station destinationStation in stations)
            {
                //Get HTML
                string html = getMainPageInfo(destinationStation, isConnected);

                //Tokenise
                var tokeniser = new Tokeniser();
                List<WebItem> page = tokeniser.Tokenise(html);
                if (page.Count == 0) return;

                //Rip out section we want (table, class, "arrivaltable")
                WebItem section = Tokeniser.ExtractSection(page[0], "table", "id", "TrainTable");
                if (section == null) return;

                //Rows
                WebItem rows = Tokeniser.ExtractSection(section, "tbody", "", "");
                foreach (WebItem row in rows.Children)
                {
                    if (row.Text.ToLower() != "tr") continue;

                    string startingStationName;
                    string platform = "";
                    string timetable;
                    string trainOperator;
                    if (row.Children.Count < 5) continue;

                    if (GetContent(row.Children[0]).ToLower().Trim() == "from") continue;

                    if (row.Children.Count == 5)
                    {
                        startingStationName = GetContent(row.Children[0]);
                        timetable = GetContent(row.Children[1]);
                        platform = GetContent(row.Children[3]);
                        trainOperator = GetContent(row.Children[4]);
                    }
                    else
                    {
                        startingStationName = GetContent(row.Children[0]);
                        platform = GetContent(row.Children[1]);
                        timetable = GetContent(row.Children[2]);
                        trainOperator = GetContent(row.Children[4]);
                    }
                    var partialUrlForStationList = row.Children[0].Children[0].Attributes[0].Content.Replace("&amp", "&");
                    if (partialUrlForStationList.Substring(0, 1) == "\"")
                    {
                        partialUrlForStationList = partialUrlForStationList.Substring(1);
                        partialUrlForStationList = partialUrlForStationList.Substring(0, partialUrlForStationList.Length - 1);
                    }
                    var details = row.Children[0].Children[0].Attributes[0].Content.Replace("&amp", "&");

                    //Get J-Code (seems to be unique id for a train journey)
                    string jCode = "";
                    string[] urlParams = partialUrlForStationList.Split('&');
                    foreach (string t in urlParams)
                    {
                        if (t.ToUpper().StartsWith("J="))
                            jCode = t.Substring(2);
                        if (t.ToUpper().StartsWith(";J="))
                            jCode = t.Substring(3);
                    }
                    if (String.IsNullOrEmpty(jCode)) continue;

                    if (String.IsNullOrEmpty(timetable))
                        continue;

                    //Get Train Journey
                    Journey journey = (from j in _db.Journeys where j.JCode == jCode select j).FirstOrDefault() ??
                                      new Journey
                    {
                        JCode = jCode,
                        TrainOperator = trainOperator
                    };

                    //Get Destination
                    Destination destination = (from d in journey.Destinations where d.Station.Id == destinationStation.Id select d).FirstOrDefault();
                    if (destination == null)
                    {
                        //Timetabled arrival time
                        DateTime now = DateTime.Now;
                        int hour = int.Parse(timetable.Substring(0, 2));
                        int minute = int.Parse(timetable.Length == 4 ? timetable.Substring(2, 2) : timetable.Substring(3, 2));
                        var timetabledArrival = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
                        if (timetabledArrival.AddHours(12) < DateTime.Now)
                            timetabledArrival = timetabledArrival.AddDays(1);

                        destination = new Destination
                        {
                            Station = destinationStation,
                            ScheduledArrival = timetabledArrival
                        };
                    }
                    destination.ActualArrival = DateTime.Now;

                    //Which starting stations are we interesting in for this destination?
                    var fromStations = new List<Station>();
                    List<Station> tempStations1 = (from u in _db.Users where u.HomeStation.Id == destinationStation.Id select u.DestinationStation).ToList();
                    List<Station> tempStations2 = (from u in _db.Users where u.DestinationStation.Id == destinationStation.Id select u.HomeStation).ToList();
                    foreach (Station s1 in tempStations1)
                    {
                        if ((from s in fromStations where s.Id == s1.Id select s).FirstOrDefault() == null)
                            fromStations.Add(s1);
                    }
                    foreach (Station s2 in tempStations2)
                    {
                        if ((from s in fromStations where s.Id == s2.Id select s).FirstOrDefault() == null)
                            fromStations.Add(s2);
                    }

                    //Look for fromStations in "Previous Calling Points"
                    IEnumerable<FromStation> validStartingStations = ConfirmStartingStations(partialUrlForStationList, fromStations, details, isConnected);
                    foreach (FromStation fs in validStartingStations)
                    {
                        FromStation fromStation = (from fr in destination.FromStations where fr.Station.Id == fs.StationId select fr).FirstOrDefault();
                        if (fromStation == null)
                            destination.FromStations.Add(fs);
                    }

                    //Is this worth saving?
                    if (destination.FromStations.Count <= 0) continue;

                    if (destination.Journey == null)
                        journey.Destinations.Add(destination);
                    if (journey.Id == 0)
                        _db.Journeys.Add(journey);
                    _db.SaveChanges();
                }
            }
        }

        private IEnumerable<FromStation> ConfirmStartingStations(string parialUrlForStationList, List<Station> fromStations, string details, bool isConnected)
        {
            ConsoleMsg("ConfirmStartingStations", MsgType.FunctionCall);
            var rv = new List<FromStation>();
            var html = "";

            //Get HTML
            FileInfo fiStationList = null;
            if (isConnected)
            {
                var url = Properties.Settings.Default.URLDetailFragment;
                url += details.Replace("\"", "").Replace(";", "");
                html = GetHtmlFromURL(url);
            }
            else
            {
                var partFileName = parialUrlForStationList.Replace("term.aspx?", "");
                partFileName = partFileName.Replace("train.aspx?", "");
                partFileName = partFileName.Replace(";", "");

                foreach (var fi in GetLatestWebPagesFolder().GetFiles())
                {
                    if (!fi.Name.Contains(partFileName)) continue;

                    if (fiStationList == null)
                        fiStationList = fi;
                    else
                    {
                        if (fiStationList.CreationTime < fi.CreationTime)
                            fiStationList = fi;
                    }
                }
                if (fiStationList != null)
                {
                    var sr = fiStationList.OpenText();
                    html = sr.ReadToEnd();
                    sr.Close();
                }
            }

            //Get "Previous Calling Points" stations
            var tokeniser = new Tokeniser();
            var detailPage = tokeniser.Tokenise(html)[0];
            var detailRows = Tokeniser.ExtractSection(detailPage, "table", "title", "Previous Calling Points");
            if (detailRows == null) return rv;

            detailRows = Tokeniser.ExtractSection(detailRows, "tbody", "", "");
            if (detailRows == null) return rv;

            foreach (var detailRow in detailRows.Children)
            {
                if (detailRow.Text.ToLower() != "tr") continue;
                if (detailRow.Children.Count < 2) continue;

                var wiStation = detailRow.Children[0];
                if (wiStation.Text.ToLower() == "th") continue;

                var wiSchedule = detailRow.Children[1];

                //Station Name
                string stationName;
                stationName = GetContent(wiStation.Children.Count > 1 ? wiStation.Children[0] : wiStation);
                if (stationName == "&nbsp;") continue;

                //Scheduled Time
                var strTime = wiSchedule.Children[0].Text;
                if (strTime.Trim().Length != 4)
                    throw new Exception("Couldn't get sceduled departure");
                var hours = int.Parse(strTime.Substring(0, 2));
                var minutes = int.Parse(strTime.Substring(2));
                var scheduledDep = DateTime.Today.AddHours(hours).AddMinutes(minutes);
                if (DateTime.Now < scheduledDep) scheduledDep.AddDays(-1);

                //Station Code
                var station = (from s in _db.Stations where s.StationName == stationName select s).FirstOrDefault();
                if (station == null) continue;

                //Is this one from the fromlist?
                var fromStation = (from fs in fromStations where fs.Id == station.Id select fs).FirstOrDefault();
                if (fromStation != null)
                    rv.Add(new FromStation
                    {
                        ScheduledDeparture = scheduledDep,
                        StationId = station.Id
                    });
            }

            return rv;
        }

        /// <summary>
        /// todo: check this logic
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        private static DateTime GetDatetime(int hour, int minute)
        {
            ConsoleMsg("GetDatetime", MsgType.FunctionCall);
            var dt = DateTime.Today;
            dt = dt.AddHours(hour);
            dt = dt.AddMinutes(minute);

            //If this is more that 12 hours before now then it must be for tomorrow
            var ts = DateTime.Now - dt;
            if (ts.TotalHours > 12)
                dt = dt.AddDays(1);

            //If this is more that 12 hours in the future then it must be for yesterday
            ts = DateTime.Now - dt;
            if (ts.TotalHours < -12)
                dt = dt.AddDays(-1);

            return dt;
        }

        private static DateTime GetDatetime(string timetable)
        {
            ConsoleMsg("GetDatetime", MsgType.FunctionCall);
            var hour = int.Parse(timetable.Substring(0, 2));
            int minute;
            switch (timetable.Trim().Length)
            {
                case 4:
                    minute = int.Parse(timetable.Substring(2, 2));
                    break;
                case 5:
                    minute = int.Parse(timetable.Substring(3, 2));
                    break;
                default:
                    throw new Exception("Unknown Timetable format: " + timetable);
            }
            return GetDatetime(hour, minute);
        }

        private static string GetContent(WebItem wi)
        {
            ConsoleMsg("GetContent", MsgType.FunctionCall);
            var rv = "";
            foreach (var child in wi.Children)
            {
                if (child.Children.Count > 0)
                    return wi.Children.Count != 1 ? "ERROR" : GetContent(child);
            }

            foreach (var child in wi.Children)
            {
                if (child.IsTag) continue;
                if (rv.Length > 0) rv += " ";
                rv += child.Text;
            }

            return rv;
        }

        private string getMainPageInfo(Station destinationStation, bool isConnected)
        {
            ConsoleMsg("getMainPageInfo", MsgType.FunctionCall);
            var rv = "";

            if (isConnected)
            {
                //http://ojp.nationalrail.co.uk/service/ldbboard/arr/NMP
                var url = string.Format(Properties.Settings.Default.URLMain, destinationStation.StationCode);
                rv = GetHtmlFromURL(url);
            }
            else
            {
                //Get latest Folder
                var diWpBase = GetLatestWebPagesFolder();

                //Get latest arrivals page
                FileInfo lastWrittenArrivalFile = null;
                foreach (FileInfo fi in diWpBase.GetFiles())
                {
                    if (!fi.Name.EndsWith("_T=" + destinationStation.StationCode + ".html")) continue;

                    if (lastWrittenArrivalFile == null)
                        lastWrittenArrivalFile = fi;
                    else
                    {
                        if (fi.LastWriteTime > lastWrittenArrivalFile.LastWriteTime)
                            lastWrittenArrivalFile = fi;
                    }
                }

                //Output file contents
                if (lastWrittenArrivalFile != null)
                {
                    StreamReader sr = lastWrittenArrivalFile.OpenText();
                    rv = sr.ReadToEnd();
                    sr.Close();
                }
            }

            return rv;
        }
        
        private static string GetHtmlFromURL(string url)
        {
            ConsoleMsg("GetHtmlFromURL", MsgType.FunctionCall);
            string rv = "";

            var site = new Uri(url);
            WebRequest wReq = WebRequest.Create(site);
            WebResponse wResp = wReq.GetResponse();
            if (wResp != null)
            {
                Stream respStream = wResp.GetResponseStream();
                if (respStream != null)
                {
                    var reader = new StreamReader(respStream, Encoding.ASCII);
                    rv = reader.ReadToEnd();
                }
            }

            //Save Web Page?
            if (Properties.Settings.Default.SaveWebPages)
            {
                string wpFolder = Properties.Settings.Default.WebPagesFolder;
                string subFolder = Path.Combine(wpFolder, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(wpFolder)) Directory.CreateDirectory(wpFolder);
                if (!Directory.Exists(subFolder)) Directory.CreateDirectory(subFolder);

                string wpFilename = DateTime.Now.ToString("HHmmss") + "_" + url.Substring(url.IndexOf("?") + 1) + ".html";
                var sw = new StreamWriter(Path.Combine(subFolder, wpFilename));
                sw.Write(rv);
                sw.Close();
                sw.Dispose();
            }
            return rv;
        }

        private static DirectoryInfo GetLatestWebPagesFolder()
        {
            ConsoleMsg("DirectoryInfo", MsgType.FunctionCall);
            int latestDate = 0;
            var diWPBase = new DirectoryInfo(Properties.Settings.Default.WebPagesFolder);
            foreach (DirectoryInfo di in diWPBase.GetDirectories())
            {
                int currentDate;
                if (int.TryParse(di.Name, out currentDate))
                    if (currentDate > latestDate) latestDate = currentDate;
            }
            diWPBase = new DirectoryInfo(Path.Combine(diWPBase.FullName, latestDate.ToString(CultureInfo.InvariantCulture)));
            return diWPBase;
        }

        private IEnumerable<Station> GetDistinctListOfStations()
        {
            ConsoleMsg("GetDistinctListOfStations", MsgType.FunctionCall);
            //Get diffinative list of stations to monitor 
            List<Station> homeStations = (from u in _db.Users select u.HomeStation).ToList();
            List<Station> destinationStations = (from u in _db.Users select u.DestinationStation).ToList();

            var stations = new List<Station>();
            foreach (Station hs in homeStations)
            {
                foreach (Station s in stations)
                {
                    if (s.Id == hs.Id) break;
                }
                stations.Add(hs);
            }
            foreach (Station ds in destinationStations)
            {
                foreach (Station s in stations)
                {
                    if (s.Id == ds.Id) break;
                }
                stations.Add(ds);
            }

            return stations;
        }
    }
}
