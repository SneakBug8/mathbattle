using System;

public static class QuestionSelector {
    public static Question SelectQuestion() {
        var q = new Question();
        var random = new Random();
        var num1 = random.Next(2, 15);
        var num2 = random.Next(2, 15);
        q.QuestionText = num1 + " * " + num2;
        q.Answers = new string[1];
        q.Answers[0] = (num1 * num2).ToString();
        return q;
    }
}