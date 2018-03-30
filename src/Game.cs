using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using mathbattle.utility;
using System.Collections.Generic;

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
            SendToAll.SendText(GamePlayers, "Game is starting \n Your opponents:");

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

                if (QuestionNum < 10 || !HaveWinner())
                {
                    await ChangeQuestion();
                }
                else
                {
                    // End the game
                    SendToAll.SendText(GamePlayers, "Game has ended. Final Score:");

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
                SendToAll.SendText(GamePlayers, "First question:");

            }
            else if (QuestionNum == 10)
            {
                SendToAll.SendText(GamePlayers, "Last question:");
            }
            else
            {
                SendToAll.SendText(GamePlayers, "Question " + QuestionNum + ":");
            }

            Question = QuestionSelector.SelectQuestion();
            QuestionNum++;
            SendToAll.SendText(GamePlayers, Question.QuestionText);


            DelayedTask.DelayTask(new List<DelayedTask>() {
                new DelayedTask(() => ChangeQuestion(), 60),
                new DelayedTask(() => SendToAll.SendText(GamePlayers,"30 seconds remaining"), 30),
                new DelayedTask(() => SendToAll.SendText(GamePlayers,"15 seconds remaining"), 45),
                new DelayedTask(() => SendToAll.SendText(GamePlayers,"5 seconds remaining"), 55)
            });
        }

        async Task SendScore()
        {
            await Task.Delay(500);

            var scoretext = "";

            foreach (var gamePlayer in GamePlayers)
            {
                scoretext += gamePlayer.Player.Name + " - " + gamePlayer.Score + ". \n";
            }

            SendToAll.SendText(GamePlayers, scoretext);
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

    public class GamePlayer : PlayerWrapper
    {
        public bool Answered;
        public int Score;
    }
}