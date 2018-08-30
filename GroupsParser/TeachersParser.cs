// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Net;
// using System.Text.RegularExpressions;
// using System.Threading;
// using HtmlAgilityPack;
// using Newtonsoft.Json;
// using GroupsParser.Models;
//
// namespace GroupsParser
// {
//     public static class TeachersParser
//     {
//         private static HtmlWeb _web;
//         public static List<Teacher> Teachers;
//         private static WebProxy _proxy;
//
//         static TeachersParser()
//         {
//             _web = new HtmlWeb();
//             _web.UserAgent =
//                 "Mozilla/5.0 AppleWebKit/537.36 (KHTML, like Gecko; compatible; Googlebot/2.1; +http://www.google.com/bot.html) Safari/537.36";
//             _proxy = new WebProxy(new Uri("http://88.99.149.188:31288"));
//
//             Teachers = new List<Teacher>();
//         }
//
//         public static void Parse(int start, int end)
//         {
//             Console.WriteLine($"Всего преподов: {end - start}");
//             var c = 0;
//             for (var i = start; i < end; i++)
//             {
//                 var url = $"https://ruz.narfu.ru/?timetable&lecturer={i}";
//                 var doc = _web.Load(url, "GET", _proxy, null);
//                 var teacher = doc.DocumentNode.SelectNodes("//span")[1].InnerText.Trim();
//
//                 if (string.IsNullOrEmpty(teacher)) continue; // значит такой препод удалён
//
//                 var teacherSplit = Regex.Replace(teacher, @"(\s{2,})", "").Split('.');
//
//                 Teachers.Add(new Teacher() { Depart = teacherSplit[1], Id=i, Name = teacherSplit[0]});
//
//                 Console.WriteLine($"Completed {i} / {end}");
//                 if (c++ % 10 == 0)
//                 {
//                     Console.WriteLine("Sleep...");
//                     Thread.Sleep(1500);
//                 }
//             }
//             WriteToFile();
//         }
//
//         private static void WriteToFile()
//         {
//             var str = JsonConvert.SerializeObject(Teachers);
//             File.WriteAllText("../../../Teachers.json", str);
//         }
//     }
// }