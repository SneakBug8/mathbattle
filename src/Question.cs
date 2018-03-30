public class Question {
    public string QuestionText;
    public string[] Answers;

    public bool Compare (string useranswer) {
        if (useranswer == null) {
            return false;
        }
        
        foreach (var answer in Answers) {
            if (answer.ToLower() == useranswer.ToLower()) {
                return true;
            }
        }

        return false;
    }
}