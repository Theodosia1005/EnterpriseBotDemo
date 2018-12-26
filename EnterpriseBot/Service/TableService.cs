using EnterpriseBot.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace EnterpriseBot.Service
{
    public static class TableService
    {

        private class DataBase
        {
            [JsonProperty("ParkingLots")]
            public List<ParkingLot> ParkingLots { get; set; }

            [JsonProperty("DiningHalls")]
            public List<DiningHall> DiningHalls { get; set; }
        }

        private static DataBase dataBase;

        static TableService()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resDir = Path.Combine(dir, @"Service\Resources\");

            StreamReader sr = new StreamReader(resDir + "DemoDataBase.json", Encoding.Default);
            dataBase = JsonConvert.DeserializeObject<DataBase>(sr.ReadToEnd());
        }

        private static List<ParkingLot> FindEmptyParkingLots(string floor = "")
        {
            List<ParkingLot> result = new List<ParkingLot>();
            foreach (ParkingLot parkingLot in dataBase.ParkingLots)
            {
                if ((floor == "" || parkingLot.Floor == floor) && parkingLot.IsEmpty)
                {
                    result.Add(parkingLot);
                }
            }
            return result;
        }

        private static string FindMostEmptyParkingFloor(string currentFloor)
        {
            Dictionary<string, int> emptyNumber = new Dictionary<string, int>();

            foreach (ParkingLot parkingLot in dataBase.ParkingLots)
            {
                if (parkingLot.IsEmpty && parkingLot.Floor != currentFloor)
                {
                    if (!emptyNumber.ContainsKey(parkingLot.Floor))
                    {
                        emptyNumber.Add(parkingLot.Floor, 0);
                    }
                    emptyNumber[parkingLot.Floor]++;
                }
            }
            string result = "";
            int emptyLots = 0;
            foreach (var item in emptyNumber)
            {
                if (item.Value > emptyLots)
                {
                    emptyLots = item.Value;
                    result = item.Key;
                }
            }
            return result;
        }

        private static List<DiningHall> FindDiningHalls(string building = "", double capability = 90)
        {
            List<DiningHall> result = new List<DiningHall>();
            foreach (DiningHall diningHall in dataBase.DiningHalls)
            {
                if ((building == "" || diningHall.Building == building) && diningHall.Capability < capability)
                {
                    result.Add(diningHall);
                }
            }
            return result;
        }

        private static DiningHall FindMostEmptyDiningHalls(string building = "")
        {
            DiningHall result = null;
            double capability = 100;
            foreach (DiningHall diningHall in dataBase.DiningHalls)
            {
                if ((building == "" || diningHall.Building == building) && diningHall.Capability < capability)
                {
                    result = diningHall;
                    capability = diningHall.Capability;
                }
            }
            return result;
        }

        public static List<string> GetReplyMessage(string request)
        {
            ConditionModel condition = new ConditionModel(request);
            List<string> messages = new List<string>();
            switch (condition.QueryType)
            {
                case (TableType.ParkingLots):
                    {
                        if (condition.ParkingFloor == "")
                        {
                            List<ParkingLot> parkinglots = FindEmptyParkingLots();
                            messages.Add("Here is some parking lots.");
                            string message = "";
                            int dispalyCount = 0;
                            foreach (ParkingLot parkinglot in parkinglots)
                            {
                                if (dispalyCount >= 5)
                                {
                                    break;
                                }
                                message += " " + parkinglot.Id;
                                dispalyCount++;
                            }
                            messages.Add(message);
                        }
                        else
                        {
                            List<ParkingLot> parkinglots = FindEmptyParkingLots(condition.ParkingFloor);
                            int preferCount = parkinglots.Count;
                            string message = "";
                            string mostEmptyFloor = "";
                            if (preferCount <= 3)
                            {
                                string currentFloor = "";
                                if (parkinglots.Count == 0)
                                {
                                    message += "Sorry, I can't find parking lot in this floor.";
                                }
                                else
                                {
                                    message += "I find only " + parkinglots.Count + " parking lots in " + condition.ParkingFloor + ".";
                                    currentFloor = condition.ParkingFloor;
                                }

                                mostEmptyFloor = FindMostEmptyParkingFloor(currentFloor);
                                if (mostEmptyFloor == "")
                                {
                                    message += " And sadly there is no parking lot in other floors too.";
                                }
                                else
                                {
                                    parkinglots.AddRange(FindEmptyParkingLots(mostEmptyFloor));
                                    message += " You can also try in " + mostEmptyFloor + ".";
                                }
                            }
                            else
                            {
                                message += "I find " + parkinglots.Count + " parking lots in this floor.";
                            }
                            messages.Add(message);
                            if (parkinglots.Count > 0)
                            {
                                int dispalyCount = 0;
                                if (preferCount > 0)
                                {
                                    messages.Add("Here is some parking lots in " + condition.ParkingFloor + ".");
                                    message = "";
                                    for (int i = 0; i < preferCount && dispalyCount < 5; i++)
                                    {
                                        if (dispalyCount >= 5)
                                        {
                                            break;
                                        }
                                        message += " " + parkinglots[i].Id;
                                        dispalyCount++;
                                    }
                                    messages.Add(message);
                                }
                                if (preferCount < parkinglots.Count && dispalyCount < 5)
                                {
                                    messages.Add("Here is some parking lots in " + mostEmptyFloor + ".");
                                    message = "";
                                    for (int i = preferCount; i < parkinglots.Count && dispalyCount < 5; i++)
                                    {
                                        message += " " + parkinglots[i].Id;
                                        dispalyCount++;
                                    }
                                    messages.Add(message);
                                }
                            }
                        }
                        return messages;
                    }
                case (TableType.DiningHalls):
                    {

                        if (condition.DiningBuilding == "")
                        {
                            List<DiningHall> diningHalls;
                            if (condition.DiningCapability < 0)
                            {
                                diningHalls = FindDiningHalls();
                            }
                            else
                            {
                                diningHalls = FindDiningHalls(capability: condition.DiningCapability);
                            }
                            if (diningHalls.Count == 0)
                            {
                                messages.Add("Sorry, all dining hall is quite busy know.");
                                messages.Add("Here is the least busy one.");
                                DiningHall diningHall = FindMostEmptyDiningHalls();
                                messages.Add(diningHall.Id + " in " + diningHall.Building + " is " + diningHall.Capability + "% busy.");
                            }
                            else
                            {
                                messages.Add("I find " + diningHalls.Count + "dining hall for now.");
                                int dispalyCount = 0;
                                for (int i = 0; i < diningHalls.Count && dispalyCount < 3; i++)
                                {
                                    messages.Add(diningHalls[i].Id + " in " + diningHalls[i].Building + " is " + diningHalls[i].Capability + "% busy.");
                                }
                            
                            }
                        }
                        else
                        {
                            List<DiningHall> diningHalls;
                            if (condition.DiningCapability < 0)
                            {
                                diningHalls = FindDiningHalls(condition.DiningBuilding);
                            }
                            else
                            {
                                diningHalls = FindDiningHalls(condition.DiningBuilding, condition.DiningCapability);
                            }
                            if (diningHalls.Count == 0)
                            {
                                messages.Add("I can not find dining hall you want.");
                                messages.Add("Here is the least busy one in " + condition.DiningBuilding + ".");
                                DiningHall diningHall = FindMostEmptyDiningHalls(condition.DiningBuilding);
                                messages.Add(diningHall.Id + " in " + diningHall.Building + " is " + diningHall.Capability + "% busy.");
                                DiningHall diningHallAll = FindMostEmptyDiningHalls();
                                if (diningHallAll.Building != condition.DiningBuilding)
                                {
                                    messages.Add("Or you may considering go to " + diningHallAll.Building + ", the " + diningHallAll.Id + " there is " + diningHallAll.Capability + "% busy.");
                                }
                                messages.Add("#PIC#");
                            }
                            else
                            {
                                messages.Add("I find " + diningHalls.Count + "dining hall for now.");
                                int dispalyCount = 0;
                                for (int i = 0; i < diningHalls.Count && dispalyCount < 3; i++)
                                {
                                    messages.Add(diningHalls[i].Id + " in " + diningHalls[i].Building + " is " + diningHalls[i].Capability + "% busy.");
                                }
                            }

                        }
                        return messages;
                    }
            }
            messages.Add("Sorry I don't understand");
            return messages;
        }
    }
}
