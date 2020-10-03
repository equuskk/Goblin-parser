using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using SuperPuperParser.Models;
using Group = GroupsParser.Models.Group;

namespace GroupsParser
{
    public static class Parser
    {
        private const string EndPoint = "https://ruz.narfu.ru";
        private static readonly HtmlWeb Web = new HtmlWeb();

        public static void Parse()
        {
            var sw = new Stopwatch();
            sw.Start();

            var groups = new List<Group>();
            var schools = GetSchools();
            foreach(var school in schools)
            {
                Console.WriteLine($"Получение инфы от {school.Name}");
                groups.AddRange(GetGroups(school.Id));
            }

            WriteToFile(groups);
            sw.Stop();
            Console.WriteLine();
            Console.WriteLine($"Получено групп: {groups.Count()}");
            Console.WriteLine($"Затрачено времени: {sw.Elapsed.Seconds} секунд {sw.Elapsed.Milliseconds} милисекунд");
        }

        private static List<Group> GetGroups(int schoolId)
        {
            Console.WriteLine($"Получение списка групп из ВШ {schoolId}");
            var url = $"{EndPoint}/?groups&institution={schoolId}";
            var doc = Web.Load(url);

            var groups = doc.DocumentNode
                            .SelectNodes("//a")
                            .Where(x => x.Attributes["href"].Value.StartsWith("?") && int.TryParse(x.ChildNodes[1].InnerText, out _))
                            .Select(x => new Group
                            {
                                RealId = int.Parse(x.ChildNodes[1].InnerText),
                                SiteId = int.Parse(x.Attributes["href"].Value.Split('=')[1]),
                                Name = Regex.Replace(x.ChildNodes[2].InnerText.Trim(), @"\s+", " ")
                            })
                            .Distinct()
                            .ToList();

            Console.WriteLine($"Получено {groups.Count} групп");
            Console.WriteLine("-------------------------------------------");

            return groups;
        }

        private static List<School> GetSchools()
        {
            var url = EndPoint;
            var doc = Web.Load(url);
            var schools = doc.DocumentNode
                             .SelectNodes("//a")
                             .Where(x => x.Attributes["href"].Value.StartsWith("?"))
                             .Select(x => new School
                             {
                                 Id = int.Parse(x.Attributes["href"].Value.Split('=')[1]),
                                 Url = $"{url}{x.Attributes["href"].Value}",
                                 Name = x.InnerText.Trim()
                             })
                             .GroupBy(x => x.Id)
                             .Select(y => y.First())
                             .ToList();

            return schools;
        }

        private static void WriteToFile(List<Group> groups)
        {
            var str = JsonConvert.SerializeObject(groups);
            File.WriteAllText("Data/Groups.json", str);
        }
    }
}