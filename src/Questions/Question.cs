namespace mathbattle.Questions
{

    public class Question
    {
        public string QuestionText;
        public string[] Answers;
        public string CorrectAnswer;
        public bool Compare(string useranswer)
        {
            if (useranswer == null)
            {
                return false;
            }

            return useranswer == CorrectAnswer;
        }
    }
}