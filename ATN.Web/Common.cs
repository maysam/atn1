using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.UI.WebControls;


namespace ATN.Web
{
    public static class Common
    {
        public struct QueryStrings
        {
            public const string Date = "Date";
            public const string LastEigenfactor = "LastEigenfactor";
            public const string LastMachineLearning = "LastMachineLearning";
            public const string LastRun = "LastRun";
            public const string Network = "Network";
            public const string SecondLevel = "SecondLevel";
            public const string SortCol = "SortCol";
            public const string SortOrder = "SortOrder";
            public const string Status = "Status";
            public const string TheoryContributing = "TheoryContributing";
            public const string TheoryId = "TheoryId";
            public const string ThirdLevel = "ThirdLevel";
        }

        public struct Symbols
        {
            public const string Asc = "Asc";
            public const string Des = "Desc";
            public const string Amp = "&";
            public const string Eq = "=";
            public const string Question = "?";
        }

        public struct Pages
        {
            public const string Help = "help.aspx";
            public const string Launcher = "launcher.aspx";
            public const string Networks = "networks.aspx";
            public const string Theory = "theory.aspx";
            
        }

        /// <summary>
        /// Sorts the specified list.
        /// </summary>
        /// <typeparam name="T">this is the type of the list</typeparam>
        /// <param name="list">The list to be sorted.</param>
        /// <param name="sortExpression">The sort expression; format:
        /// @param1 [sortdirection], @param2 [sortdirection], @param3 [sortdirection].
        /// Valid sortDirections are: asc, desc, ascending and descending.
        /// The space between @param# and [sortdirection] is required</param>
        public static void Sort<T>(this List<T> list, string sortExpression)
        {
            string[] sortExpressions = sortExpression.Split(new string[] { "," },
                        StringSplitOptions.RemoveEmptyEntries);

            List<GenericComparer> comparers = new List<GenericComparer>();

            foreach (string sortExpress in sortExpressions)
            {
                string sortProperty = sortExpress.Trim().Split(' ')[0].Trim();
                string sortDirection = sortExpress.Trim().Split(' ')[1].Trim();

                Type type = typeof(T);
                PropertyInfo PropertyInfo = type.GetProperty(sortProperty);
                if (PropertyInfo == null)
                {
                    PropertyInfo[] props = type.GetProperties();
                    foreach (PropertyInfo info in props)
                    {
                        if (info.Name.ToString().ToLower() == sortProperty.ToLower())
                        {
                            PropertyInfo = info;
                            break;
                        }
                    }
                    if (PropertyInfo == null)
                    {
                        throw new Exception(String.Format("{0} is not a valid property of type: \"{1}\"", sortProperty, type.Name));
                    }
                }

                SortDirection SortDirection = SortDirection.Ascending;
                if (sortDirection.ToLower() == "asc" || sortDirection.ToLower() == "ascending")
                {
                    SortDirection = SortDirection.Ascending;
                }
                else if (sortDirection.ToLower() == "desc" || sortDirection.ToLower() == "descending")
                {
                    SortDirection = SortDirection.Descending;
                }
                else
                {
                    throw new Exception("Valid SortDirections are: asc, ascending, desc and descending");
                }

                comparers.Add(new GenericComparer
                {
                    SortDirection = SortDirection,
                    PropertyInfo = PropertyInfo,
                    comparers = comparers
                });
            }
            list.Sort(comparers[0].Compare);
        }

        public class GenericComparer
        {
            public List<GenericComparer> comparers { get; set; }
            int level = 0;

            public SortDirection SortDirection { get; set; }
            public PropertyInfo PropertyInfo { get; set; }

            public int Compare<T>(T t1, T t2)
            {
                int ret = 0;

                if (level >= comparers.Count)
                    return 0;

                object t1Value = comparers[level].PropertyInfo.GetValue(t1, null);
                object t2Value = comparers[level].PropertyInfo.GetValue(t2, null);

                if (t1 == null || t1Value == null)
                {
                    if (t2 == null || t2Value == null)
                    {
                        ret = 0;
                    }
                    else
                    {
                        ret = -1;
                    }
                }
                else
                {
                    if (t2 == null || t2Value == null)
                    {
                        ret = 1;
                    }
                    else
                    {
                        ret = ((IComparable)t1Value).CompareTo(((IComparable)t2Value));
                    }
                }
                if (ret == 0)
                {
                    level += 1;
                    ret = Compare(t1, t2);
                    level -= 1;
                }
                else
                {
                    if (comparers[level].SortDirection == SortDirection.Descending)
                    {
                        ret *= -1;
                    }
                }
                return ret;
            }
        }

    }
}