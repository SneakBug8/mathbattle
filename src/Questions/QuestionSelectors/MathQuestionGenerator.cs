using System;

namespace mathbattle.Questions.Selectors {
    public class MathQuestionGenerator {
        public static Question GenerateQuestion() {
            var q = new Question();
            var random = new Random();
            var num1 = random.Next(2, 10);
            var num2 = random.Next(2, 10);
            q.QuestionText = num1 + " * " + num2;
            q.CorrectAnswer = (num1 * num2).ToString();
            return q;
        }
    }
}