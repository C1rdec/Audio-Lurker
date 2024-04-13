using Lurker.Audio;

var service = new AudioSessionService();
foreach (var session in service.GetSessions())
{
    Console.WriteLine(session.Id);
    Console.WriteLine(session.Name);
    Console.WriteLine(session.Volume);
}
