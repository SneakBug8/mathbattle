using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace mathbattle.Questions.Selectors
{
    public static class OpenTDBRequester
    {
        public static Question RequestQuestion()
        {
            Response response = new Response();
            response.response_code = -1;
            do
            {
                response = TryGetResponse();
            }
            while (response.response_code != 0);

            var q = new Question();
            q.QuestionText = response.results[0].question;
            int correctindex;
            q.Answers = Shuffle(
                response.results[0].incorrect_answers,
                response.results[0].correct_answer,
                out correctindex);
            q.CorrectAnswer = (correctindex + 1).ToString();

            Console.WriteLine(JsonConvert.SerializeObject(q));

            return q;
        }

        static string[] Shuffle(string[] incorrect, string correct, out int correctindex)
        {
            var incorrectlist = incorrect.ToList();
            var res = new List<string>();
            var random = new Random();
            while (incorrectlist.Count > 0 || !res.Contains(correct))
            {
                var index = random.Next(-1, incorrectlist.Count);

                if (index == -1 && !res.Contains(correct))
                {
                    res.Add(correct);
                }
                else if (index != -1)
                {
                    res.Add(incorrectlist[index]);
                    incorrectlist.RemoveAt(index);
                }
            }

            correctindex = res.IndexOf(correct);

            return res.ToArray();
        }

        static Response TryGetResponse()
        {
            var json = Request();
            return JsonConvert.DeserializeObject<Response>(json);
        }

        static string Request()
        {
            string site = string.Format("https://opentdb.com/api.php?amount=1");

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(site);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            using (StreamReader stream = new StreamReader(
                 resp.GetResponseStream(), Encoding.UTF8))
            {
                return stream.ReadToEnd();
            }
        }
    }

    public class Response
    {
        public int response_code;
        public Results[] results;
    }

    public class Results
    {
        public string category;
        public string type;
        public string difficulty;
        public string question;
        public string correct_answer;
        public string[] incorrect_answers;
    }
}