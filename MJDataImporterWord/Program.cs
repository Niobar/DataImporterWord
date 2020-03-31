using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MJDataImporterWord
{
        class Program
    {
        static void Main(string[] args)
        {
            Encoding utf8 = Encoding.UTF8;
            List<int> dayStartsList = new List<int>();
            List<Record> recordsList = new List<Record>();
            Regex exDay = new Regex(@"(?<day>\d{1,2}) wrzesnia 2019");
            Regex exDaySchedule = new Regex(@"(?<hour>\d{1,2})\.(?<minutes>\d{1,2})\t(?<title>[^\n,\r]*)[\n,\r](?:\W*[^\)]*\W*)(?<seasonTest>\W*Sezon\W*)?(?(seasonTest)(?<season>\d*))(?<episodeTest>\W*Odcinek\W*)?(?(episodeTest)(?<episode>\d*))(?:\W*)(?<movieChecker>\w*)(?:[^:]*):(?<actors>[^(]*)(?:[^\n\r]*)(?:[\n\r])(?<text>[^\(]*)");
            var inputstring = System.IO.File.ReadAllText(@"C:\Users\tjtj\Downloads\SCIFIpol Sep 19 FINAL.txt");//Should i load it with some polish letters recognition ?
            MatchCollection mcDay = exDay.Matches(inputstring);



            foreach (Match mDay in mcDay)
            {
                dayStartsList.Add(mDay.Index);
            }
            MatchCollection mcDaySchedule = exDaySchedule.Matches(inputstring, dayStartsList[0]);
            int dayInMonth = 1;
            int dayStopIndex = dayStartsList[dayInMonth];

            foreach (Match mDaySchedule in mcDaySchedule)
            {
                Record rec = new Record();
                if (dayStopIndex < mDaySchedule.Index)
                {
                    dayInMonth++;
                    if (dayInMonth < dayStartsList.Count) dayStopIndex = dayStartsList[dayInMonth];
                    else dayStopIndex = inputstring.Length;
                }

                rec.BroadcastDateAndTime = new DateTime(2019, 09, dayInMonth, Int32.Parse(mDaySchedule.Groups["hour"].Value), Int32.Parse(mDaySchedule.Groups["minutes"].Value), 0);
                string tempTitle = mDaySchedule.Groups["title"].Value;
                string[] tempTitleSplited = tempTitle.Split(':');
                string[] movieTitleArray = tempTitle.Split(' ');
                bool movieTitleTrimmer = false;
                if (Array.Exists(movieTitleArray, e => e == "FILM"))
                {
                    for (int i = 0; i < movieTitleArray.Length; i++)
                    {
                        if (!movieTitleTrimmer)
                        {
                            if (movieTitleArray[i] == "FILM") movieTitleTrimmer = true;
                            movieTitleArray[i] = null;
                        }
                    }
                }
                if (mDaySchedule.Groups["movieChecker"].Value != "serial")
                {
                    rec.Title = string.Join(" ", movieTitleArray).Trim(' ');
                }
                else
                {
                    rec.SubTitle = tempTitleSplited[tempTitleSplited.Length - 1].Trim(' ');
                    rec.Title = tempTitle.Remove(tempTitle.Length - rec.SubTitle.Length - 1);
                    if (mDaySchedule.Groups["season"].Value != "") rec.Sezon = short.Parse(mDaySchedule.Groups["season"].Value);
                    rec.Odcinek = short.Parse(mDaySchedule.Groups["episode"].Value);
                }
                string actors = mDaySchedule.Groups["actors"].Value;
                string[] actorsArray = mDaySchedule.Groups["actors"].Value.Split(',');
                foreach (var actor in actorsArray)
                {
                    rec.Actors.Add(actor.Trim());
                }
                rec.Text = mDaySchedule.Groups["text"].Value;
                recordsList.Add(rec);
            }

            Console.OutputEncoding = Encoding.UTF8;
            foreach (Record rec in recordsList)
            {
                Console.WriteLine(rec);
            }
        }
    }
}
