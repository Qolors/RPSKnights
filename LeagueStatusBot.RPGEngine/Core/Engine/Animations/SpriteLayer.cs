namespace LeagueStatusBot.RPGEngine.Core.Engine.Animations
{
    public class SpriteLayer
    {
        public Image<Rgba32> Sprite { get; set; }
        public Point Position { get; set; }
        public int ZIndex { get; set; }

        public SpriteLayer(int width, int height)
        {
            Sprite = new Image<Rgba32>(width, height);
        }
    }

}