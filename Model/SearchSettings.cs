using System;
using System.Collections.Generic;
using System.Text;

namespace Sikor.Model
{
    /**
     * <summary>
     * Search settings for Jira search.
     * </summary>
     */
    public class IssuesSearchOptions
    {
        public string Project { get; set; }

        public string Summary { get; set; }

        public string IssueKey { get; set; }

        public string Sorting { get; set; }

        public bool ExcludeResolved { get; set; }

        public bool ExcludeQA { get; set; }


        public bool AssignedToCurrentUser { get; set; }

        public IssuesSearchOptions()
        {
            Project = "";
            Summary = "";
            IssueKey = "";
            Sorting = "";
            AssignedToCurrentUser = true;
        }

        public override string ToString()
        {
            List<string> parameters = new List<string>();

            if (AssignedToCurrentUser)
            {
                parameters.Add("Assignee = currentUser()");
            }

            if (Project != "")
            {
                parameters.Add("Project = " + Project);
            }

            if (IssueKey != "")
            {
                parameters.Add("IssueKey = " + IssueKey);
            }

            if (Summary != "")
            {
                parameters.Add("Summary ~ \"" + Summary.Replace('"', ' ') + "\"");
            }

            if (ExcludeResolved == true)
            {
                parameters.Add("Resolved != \"true\"");
            }


            string jql = string.Join(" && ", parameters);

            if (Sorting != "")
            {
                string[] sort = Sorting.Split('_');
                jql += " ORDER BY " + sort[0] + " " + sort[1];

            }

            return jql;
        }

    }
}


