#region Header
using System; using static System.Console; using static System.IO.Path; using static System.IO.File;
namespace CursoCSharp {
#endregion

public static class DeltaSubtitulo
{
    #region Campos
    
    public static bool HayError;
    public static string MensajeError;
    public static string PathArchivoIn;
    public static string PathArchivoOut;

    #endregion

    #region Metodos
        public static void AplicaDelta(string archivo, string delta)
        {
            #region Input

            string pathArchivoIn = GetPathArchivoIn(archivo);
            if(DeltaSubtitulo.HayError)
            {
                WriteLine(DeltaSubtitulo.MensajeError);
                return;
            }
            string deltaTiempo = GetDeltaTiempo(delta);            
            if(DeltaSubtitulo.HayError)
            {
                WriteLine(DeltaSubtitulo.MensajeError);
                return;
            }

            #endregion
            
            #region Process
            string[] lineasIn = GetLineasIn(pathArchivoIn);
            string[] lineasOut = AplicaDelta(lineasIn, deltaTiempo);
            #endregion
            
            #region Output
            string pathArchivoOut = GetPathArchivoOut(pathArchivoIn, deltaTiempo);
            EscribeArchivoOut(pathArchivoOut, lineasOut);
            DeltaSubtitulo.PathArchivoIn = pathArchivoIn;
            DeltaSubtitulo.PathArchivoOut = pathArchivoOut;
            #endregion
        }

        public static string[] AplicaDelta(string[] lineasIn, string deltaTiempo)
        {
            TimeSpan spanDelta = CalculaSpan(deltaTiempo);

            var lineasOut = new string[lineasIn.Length];
            int i = 0;
            while(i < lineasIn.Length)
            {
                lineasOut[i] = AplicaDelta(lineasIn[i], deltaTiempo);
                i++;
            }
            return lineasOut;

            #region Metodos Locales

            string AplicaDelta(string lineaIn, string deltaTiempo)
            {
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

            #endregion 
        }

        public static void EscribeArchivoOut(string pathArchivoOut, string[] lineasOut) => 
                WriteAllLines(pathArchivoOut, lineasOut);

        public static string GetDeltaTiempo(string delta)
        {
            // delta tiempo
            var deltaTiempo = delta;
            if(string.IsNullOrEmpty(delta))
            {
                // Consigue delta tiempo de la consola
                WriteLine();
                WriteLine("Delta de tiempo ([-]hh:mm:ss.fff): ");
                deltaTiempo = ReadLine();
            }

            if(!DeltaValido(deltaTiempo))
            {
                DeltaSubtitulo.HayError = true;
                DeltaSubtitulo.MensajeError = $"Error. Delta de tiempo '{deltaTiempo}' debe estar en formato: [-]hh:mm:ss.fff";
            }

            return deltaTiempo;

            #region Metodos Locales

            bool DeltaValido(string deltaTiempo) => TimeSpan.TryParse(deltaTiempo, out TimeSpan tsDeltaTiempo);

            #endregion
        }

        public static string[] GetLineasIn(string pathArchivoIn) => ReadAllLines(pathArchivoIn);

        public static string GetPathArchivoIn(string archivo)
        {
            DeltaSubtitulo.HayError = false;

            // nombre de archivo de entrada
            var pathArchivoIn = archivo;
            if(pathArchivoIn == "")
            {
                // Consigue nombre de archivo de subtitulos de la consola
                WriteLine();
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
            var strDelta = deltaTiempo.Replace(":", "_");
            var directoryName = GetDirectoryName(pathArchivoIn);
            var pathArchivoOut =   directoryName + (directoryName != "" ? 
                                        DirectorySeparatorChar.ToString() : "") + 
                                    GetFileNameWithoutExtension(pathArchivoIn) + $"_{strDelta}" + "_out" + 
                                    GetExtension(pathArchivoIn);
            return pathArchivoOut;                                            
        }
        
    #endregion
}

#region Footer
}
#endregion