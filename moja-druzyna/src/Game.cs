using moja_druzyna.Models;

namespace moja_druzyna.src
{
    public class Game
    {
        public Scout person { get; set; }
        public string gameName { get; set; }
        public int points { get; set; }

        public Game(Scout person, string gameName, int points)
        {
            this.person = person;
            this.gameName = gameName;
            this.points = points;
        }
    }
}