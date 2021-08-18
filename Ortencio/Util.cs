using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace ConsultaRuc
{
    public class Util
    {
        public static string LeerTextoDeImagen(byte[] imagenBytes)
        {
            //string DataPath = "";
            string TextoResultado = null;
            float porcentaje = 0;
            int numIntentos = 1;
            try
            {
                while (porcentaje <= 0.50 && numIntentos <= 3)
                {
                    using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                    {
                        using (var img = Pix.LoadFromMemory(imagenBytes))
                        {

                            using (var page = engine.Process(img))
                            {

                                TextoResultado = page.GetText().Trim();
                                porcentaje = page.GetMeanConfidence();
                                numIntentos++;

                            }
                        }

                    }
                    if (porcentaje >= 0.50)
                    {
                        break;
                    }
                }
                return TextoResultado;

            }
            catch (Exception ex)
            {
                return null;

            }

        }

    }
}
