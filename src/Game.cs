using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace mathbattle
{
    public class Game
    {
        public Question Question;
        public GamePlayer[] GamePlayers;
        public int QuestionNum;
        public Game(Player[] players)
        {
            var gameplayers = new GamePlayer[players.Length];

            for (int i = 0; i < players.Length; i++)
            {
                gameplayers[i] = new GamePlayer();
                gameplayers[i].Player = players[i];
            }

            GamePlayers = gameplayers;

            Start();
        }
        async void Start()
        {
            await SendForAll("Game is starting \n Your opponents:");
            await SendScore();
            await ChangeQuestion();
        }
        public async void OnMessage(Message message)
        {
            GamePlayer from = PlayerById(message.From.Id);

            if (!from.Answered)
            {
                from.Answered = true;

                var result = Question.Compare(message.Text);

                if (result)
                {
                    await from.Player.SendMessage("You're right");
                    from.Score++;
                }
                else
                {
                    await from.Player.SendMessage("Noo!");
                }

                if (!AllAnswered())
                {
                    await from.Player.SendMessage("Waiting for other players");
                }
            }
            else
            {
                await from.Player.SendMessage("You've answered this already. Wait for others");
            }

            if (AllAnswered())
            {
                foreach (var gameplayer in GamePlayers)
                {
                    await Program.Server.Client.SendChatActionAsync(gameplayer.Player.ChatId, ChatAction.Typing);
                }

                await SendScore();

                if (QuestionNum < 10)
                {
                    await ChangeQuestion();
                }
                else if (HaveWinner())
                {
                    // End the game
                    await SendForAll("Game has ended. Final Score:");
                    await SendScore();

                    foreach (var player in GamePlayers)
                    {
                        player.Player.Game = null;
                    }

                    Program.Server.Games.Remove(this);
                }
            }
        }

        bool HaveWinner()
        {
            int max = 0;

            foreach (var player in GamePlayers)
            {
                if (player.Score > max)
                {
                    max = player.Score;
                }
                else if (player.Score == max)
                {
                    return false;
                }
            }

            return true;
        }

        async Task ChangeQuestion()
        {
            foreach (var gameplayer in GamePlayers)
            {
                await Program.Server.Client.SendChatActionAsync(gameplayer.Player.ChatId, ChatAction.Typing);
                gameplayer.Answered = false;
            }

            await Task.Delay(500);

            if (QuestionNum == 0)
            {
                await SendForAll("First question:");
            }
            else if (QuestionNum == 10)
            {
                await SendForAll("Last question:");
            }
            else
            {
                await SendForAll("Question " + QuestionNum + ":");
            }

            Question = QuestionSelector.SelectQuestion();
            QuestionNum++;
            await SendForAll(Question.QuestionText);

            TimerTask(60f, () => ChangeQuestion());
        }

        async void TimerTask(float time, Action action)
        {
            /*
            1 sec = 100 timer = 1000 ms
             */
            for (int timer = (int)(time * 100); timer > 0; timer--)
            {
                await Task.Delay(10);

                if (timer == 30 * 100)
                {
                    SendForAll("30 seconds left", 0);
                }

                if (timer == 15 * 100)
                {
                    SendForAll("15 seconds left", 0);
                }

                if (timer == 5 * 100)
                {
                    SendForAll("5 seconds left", 0);
                }
            }

            action();
        }

        async Task SendScore()
        {
            await Task.Delay(500);

            var scoretext = "";

            foreach (var gamePlayer in GamePlayers)
            {
                scoretext += gamePlayer.Player.Name + " - " + gamePlayer.Score + ". \n";
            }

            await SendForAll(scoretext, 1000);
        }

        async Task SendForAll(string text, int timeout = 500)
        {
            foreach (var gamePlayer in GamePlayers)
            {
                await Program.Server.Client.SendChatActionAsync(gamePlayer.Player.ChatId, ChatAction.Typing);
            }

            await Task.Delay(500);

            foreach (var gamePlayer in GamePlayers)
            {
                await gamePlayer.Player.SendMessage(text, 0);
            }
        }

        bool AllAnswered()
        {
            var res = true;
            foreach (var gameplayer in GamePlayers)
            {
                res = gameplayer.Answered && res;

                if (!res)
                {
                    return res;
                }
            }

            return res;
        }

        GamePlayer PlayerById(int id)
        {
            foreach (var gameplayer in GamePlayers)
            {
                if (gameplayer.Player.Id == id)
                {
                    return gameplayer;
                }
            }

            return null;
        }
    }

    public class GamePlayer
    {
        public Player Player;
        public bool Answered;
        public int Score;
    }
}