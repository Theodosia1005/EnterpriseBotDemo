using EnterpriseBot.Models.Resources;
using System.Collections.Generic;

namespace EnterpriseBot.Models
{
    public enum ConditionType
    {
        ParkingFloor = 0,
        
        DiningBuilding = 1,
        
        DiningCapability = 2
    }

    public enum TableType
    {
        ParkingLots = 0,

        DiningHalls = 1
    }


    public class ConditionModel
    {
        public ConditionModel(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }
            
            Dictionary<ConditionType, string> conditionTypes = ConditionTemplate.GetConditionTypes(content);
            foreach (var condition in conditionTypes)
            {
                ConditionType type = condition.Key;
                string value = condition.Value;
                switch (type)
                {
                    case ConditionType.ParkingFloor:
                        {
                            ParkingFloor = value;
                            QueryType = TableType.ParkingLots;
                            break;
                        }
                    case ConditionType.DiningBuilding:
                        {
                            DiningBuilding = value;
                            QueryType = TableType.DiningHalls;
                            break;
                        }
                    case ConditionType.DiningCapability:
                        {
                            DiningCapability = 60;
                            QueryType = TableType.DiningHalls;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        public string ParkingFloor { get; set; } = "";

        public string DiningBuilding { get; set; } = "";

        public double DiningCapability { get; set; } = -1;

        public TableType QueryType { get; set; } = TableType.ParkingLots;

    }
}
