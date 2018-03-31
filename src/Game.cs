using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using mathbattle.utility;
using mathbattle.config;
using System.Collections.Generic;
using mathbattle.Questions;

namespace mathbattle
{
    public class Game
    {
        public Question Question;
        public GamePlayer[] GamePlayers;
        public int QuestionNum;
        TaskDelayer TillNewQuestionDelayer;
        public bool AcceptMessages = true;
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

            if (!AcceptMessages)
            {
                return;
            }

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


                if (QuestionNum > GameConfig.QuestionsPerGame && HaveWinner())
                {
                    EndGame();
                }
                else
                {
                    await SendScore();
                    await ChangeQuestion();
                }
            }
        }

        async void EndGame()
        {
            StopQuestionTimer();

            SendToAll.SendText(GamePlayers, "Game has ended. Final Score:");

            var playersByScore = (from i in GamePlayers
                                  where i.Score > 0
                                  orderby i.Score descending
                                  select i).ToArray();


            for (int i = 0; i < playersByScore.Length; i++)
            {
                playersByScore[i].Player.ChangeRating(playersByScore.Length - i);
            }

            var playersApplyNegScore = (from i in GamePlayers
                                        where i.Score == 0
                                        select i).ToList();

            foreach (var player in playersApplyNegScore)
            {
                player.Player.ChangeRating(-1);
            }

            await SendScore(true);

            Program.Server.Games.Remove(this);
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
            AcceptMessages = false;
            StopQuestionTimer();

            foreach (var gameplayer in GamePlayers)
            {
                await Program.Server.Client.SendChatActionAsync(gameplayer.Player.ChatId, ChatAction.Typing);
                gameplayer.Answered = false;
            }


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

            await Task.Delay(500);
            SendToAll.SendText(GamePlayers, Question.QuestionText);

            AcceptMessages = true;

            TillNewQuestionDelayer = DelayedTask.DelayTask(new List<DelayedTask>() {
                new DelayedTask(() => ChangeQuestion(), 60),
                new DelayedTask(() => SendToAll.SendText(GamePlayers,"30 seconds remaining"), 30),
                new DelayedTask(() => SendToAll.SendText(GamePlayers,"15 seconds remaining"), 45),
                new DelayedTask(() => SendToAll.SendText(GamePlayers,"5 seconds remaining"), 55)
            });
        }

        async Task SendScore(bool last = false)
        {
            await Task.Delay(500);

            var scoretext = "";

            foreach (var gamePlayer in GamePlayers)
            {
                if (last)
                {
                    scoretext += string.Format("{0} - {1} ({2}).\n",
                    gamePlayer.Player.Name,
                    gamePlayer.Score,
                    gamePlayer.Player.Rating);
                }
                else
                {
                    scoretext += string.Format("{0} - {1}.\n",
                gamePlayer.Player.Name,
                gamePlayer.Score);
                }
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

        void StopQuestionTimer()
        {
            if (TillNewQuestionDelayer != null)
            {
                TillNewQuestionDelayer.Stop();
                TillNewQuestionDelayer = null;
            }
        }
    }




    public class GamePlayer : PlayerWrapper
    {
        public bool Answered;
        public int Score;
    }
}