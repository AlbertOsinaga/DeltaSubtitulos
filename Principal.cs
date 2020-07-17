#region Header
using static System.Console; 
namespace CursoCSharp  {
#endregion

public static class Principal 
{
    static void Main(string[] args)
    {
        string archivo = args.Length > 0 ? args[0] : "";
        string delta = args.Length > 1 ? args[1] : "";
        DeltaSubtitulo.AplicaDelta(archivo, delta);
        
        WriteLine();
        if(!DeltaSubtitulo.HayError)
            WriteLine($"Archivo '{DeltaSubtitulo.PathArchivoIn}' ajustado en {DeltaSubtitulo.PathArchivoOut}!!!!");
    }
}

#region Footer
}
#endregion