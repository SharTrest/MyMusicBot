using AngleSharp;
using MyMusicBot;

public class Program
{
    private static void Main()
            => new Bot().MainAsync().GetAwaiter().GetResult();

}
