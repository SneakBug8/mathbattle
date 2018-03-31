using System;
using mathbattle.Questions.Selectors;

namespace mathbattle.Questions
{
    public static class QuestionSelector
    {
        public static Question SelectQuestion()
        {
            return OpenTDBRequester.RequestQuestion();
        }
    }
}