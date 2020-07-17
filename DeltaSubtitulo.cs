#region Header
using System; using static System.Console; using static System.IO.Path; using static System.IO.File;
namespace CursoCSharp {
#endregion

public static class DeltaSubtitulo
{
    #region Campos
    
    public static bool HayError = false;
    public static string MensajeError = "";
    public static string PathArchivoIn = "";
    public static string PathArchivoOut = "";

    #endregion

    #region Metodos

        public static void AplicaDelta(string[] args)
        {
            string archivo = args.Length > 0 ? args[0] : "";
            string delta = args.Length > 1 ? args[1] : "";
            DeltaSubtitulo.AplicaDelta(archivo, delta);
        }

        public static void AplicaDelta(string archivo, string delta)
        {
            #region Input
            WriteTitle();
            DeltaSubtitulo.PathArchivoIn = GetPathArchivoIn(archivo);
            var deltaTiempo = GetDeltaTiempo(delta);            
            #endregion
            
            #region Process
            string[] lineasIn = GetLineasIn(DeltaSubtitulo.PathArchivoIn);
            string[] lineasOut = AplicaDelta(lineasIn, deltaTiempo);
            #endregion
            
            #region Output
            DeltaSubtitulo.PathArchivoOut = GetPathArchivoOut(DeltaSubtitulo.PathArchivoIn, deltaTiempo);
            EscribeArchivoOut(DeltaSubtitulo.PathArchivoOut, lineasOut);
            WriteResult();
            #endregion
        }

        public static string[] AplicaDelta(string[] lineasIn, string deltaTiempo)
        {
            if(DeltaSubtitulo.HayError)
                return new string[] {};
            if(!HayLineas(lineasIn))    // Metodo local
            {
                DeltaSubtitulo.HayError = true;
                DeltaSubtitulo.MensajeError = $"Error en {nameof(AplicaDelta)}. lineasIn no tiene lineas!";
                return new string[] {};
            }    

            TimeSpan spanDelta = CalculaSpan(deltaTiempo);  // Metodo local

            var lineasOut = new string[lineasIn.Length];
            int i = 0;
            while(i < lineasIn.Length)
            {
                lineasOut[i] = AplicaDelta(lineasIn[i]);   // Metodo local
                i++;
            }
            return lineasOut;

            #region Metodos Locales

            string AplicaDelta(string lineaIn)
            {
                // spanDelta definido metodo invocador (closure)
                var lineaOut = lineaIn;

                var i = lineaOut.IndexOf("-->");
                if (i <= 0)
                    return lineaOut;

                var partes = lineaOut.Split(' ', '-', '>');
                 
                var horaAntes = partes[0].Replace(',', '.');
                var horaNuevaInicio = CalculaNuevaHora(horaAntes, spanDelta);

                horaAntes = partes[partes.Length - 1].Replace(',', '.');
                var horaNuevaFin = CalculaNuevaHora(horaAntes, spanDelta);

                lineaOut = horaNuevaInicio + " --> " + horaNuevaFin;
                return lineaOut;
            }

            string CalculaNuevaHora(string horaAntes, TimeSpan spanDelta)
            {
                    TimeSpan spanHoraNueva = TimeSpan.Parse(horaAntes);
                    spanHoraNueva += spanDelta;
                    var horaNueva = spanHoraNueva.ToString();
                    if(horaNueva.Contains('.'))
                        horaNueva = horaNueva.Replace('.',',').Remove(horaNueva.Length-4); 
                    else
                        horaNueva += ",000";
                    return horaNueva;
            }

            TimeSpan CalculaSpan(string delta) => TimeSpan.Parse(delta);

            bool HayLineas(string[] lineas) => lineas.Length > 0;

            #endregion 
        }

        public static void EscribeArchivoOut(string pathArchivoOut, string[] lineasOut)
        {
            if(DeltaSubtitulo.HayError)
                return;
                
            WriteAllLines(pathArchivoOut, lineasOut);
        }

        public static string GetDeltaTiempo(string delta)
        {
            if(DeltaSubtitulo.HayError)
                return "";

            // delta tiempo
            var deltaTiempo = delta;
            if(string.IsNullOrEmpty(delta))
            {
                // Consigue delta tiempo de la consola
                Write("Delta de tiempo ([-]hh:mm:ss.fff): ");
                deltaTiempo = ReadLine();
            }

            if(!DeltaValido(deltaTiempo))   // Metodo local
            {
                DeltaSubtitulo.HayError = true;
                DeltaSubtitulo.MensajeError = $"Error. Delta de tiempo '{deltaTiempo}' debe estar en formato: [-]hh:mm:ss.fff";
            }

            return deltaTiempo;

            #region Metodos Locales

            bool DeltaValido(string deltaTiempo) => TimeSpan.TryParse(deltaTiempo, out TimeSpan tsDeltaTiempo);

            #endregion
        }

        public static string[] GetLineasIn(string pathArchivoIn) =>
            !DeltaSubtitulo.HayError ? ReadAllLines(pathArchivoIn) : new string[] {}; 

        public static string GetPathArchivoIn(string archivo)
        {
            if(DeltaSubtitulo.HayError)
                return "";

            // nombre de archivo de entrada
            var pathArchivoIn = archivo;
            if(pathArchivoIn == "")
            {
                // Consigue nombre de archivo de subtitulos de la consola
                Write("Nombre de archivo de subtítulos: ");
                pathArchivoIn = ReadLine();
            }
            
            // si nombre de archivo no existe
            if(!Exists(pathArchivoIn))
            {
                DeltaSubtitulo.HayError = true;
                DeltaSubtitulo.MensajeError = $"Error. Archivo '{pathArchivoIn}' no existe";
            }

            return pathArchivoIn;
        }
  
        public static string GetPathArchivoOut(string pathArchivoIn, string deltaTiempo) //=>
        {
            if(DeltaSubtitulo.HayError)
                return "";
            
            var strDelta = deltaTiempo.Replace(":", "_");
            var directoryName = GetDirectoryName(pathArchivoIn);
            var pathArchivoOut =   directoryName + (directoryName != "" ? 
                                        DirectorySeparatorChar.ToString() : "") + 
                                    GetFileNameWithoutExtension(pathArchivoIn) + $"_{strDelta}" + "_out" + 
                                    GetExtension(pathArchivoIn);
            return pathArchivoOut;                                            
        }

        public static void WriteResult()
        {
            if(!DeltaSubtitulo.HayError)
            {
                WriteLine($"Archivo '{DeltaSubtitulo.PathArchivoIn}' " + 
                            $"ajustado en {DeltaSubtitulo.PathArchivoOut}!!!!");
            }
            else
            {
                WriteLine(DeltaSubtitulo.MensajeError);
            }
        }
        public static void WriteTitle()
        {
            WriteLine();
            WriteLine("DeltaSubtitulos (dotnet run -- [archivoIn.srt] [hh:mm:ss.fff]");
            WriteLine("-------------------------------------------------------------");
        }

        #endregion
    }

#region Footer
}
#endregion