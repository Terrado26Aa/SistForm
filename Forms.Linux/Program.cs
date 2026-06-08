using Platform.Maui.Linux.Gtk4;

namespace Forms.Linux;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        GtkMauiApplication.Run<App>("com.sistform.app", "SistForm", args);
    }
}
