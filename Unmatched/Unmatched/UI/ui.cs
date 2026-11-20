namespace Unmatched.UI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        var db = Unmatched.Room_Generator.FirestoreDatabaseClient.Create("szoftfejleszt");
        var generator = new Unmatched.Room_Generator.Room_generator(db);
        var code = generator.CreateRoomAsync("Teszt", "Medusa", "Harpies", "B.P.R.D. HQ")
                            .GetAwaiter().GetResult();
        MessageBox.Show($"Szoba létrehozva: {code}");
        Application.Run(new MainForm());
    }
}