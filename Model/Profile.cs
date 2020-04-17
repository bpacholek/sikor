using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Sikor.Model
{

    [Serializable]
    public class Profile
    {
        public string Host { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public Dictionary<string, ListItem> Projects { get; set; }

        public Dictionary<string, IssueItem> Issues { get; set; }

        public Dictionary<string, IssueStatusItem> Statuses { get; set; }

        public List<FailedStatusUpdateOperation> FailedStatusUpdateOperations { get; }
        public List<FailedWorklog> FailedWorklogs { get; }


        public bool IsTracking { get; set; }

        public TrackingIssueItem TrackingIssue { get; set; }

        public string TrackingComment { get; set; }


        public Profile()
        {
            Projects = new Dictionary<string, ListItem>();
            Issues = new Dictionary<string, IssueItem>();
            Statuses = new Dictionary<string, IssueStatusItem>();
            FailedStatusUpdateOperations = new List<FailedStatusUpdateOperation>();
            FailedWorklogs = new List<FailedWorklog>();
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public string GetId()
        {
            return CalculateMD5Hash(Name);
        }

    }
}
