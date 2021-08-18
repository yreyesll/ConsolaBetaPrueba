using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace ConsultaRuc
{
    public partial class FrmConsultaRUCSUNAT : Form
    {
        public FrmConsultaRUCSUNAT()
        {
            InitializeComponent();
        }
        private string ExtraerContenidoEntreTagString(string cadena, int posicion, string nombreInicio, string nombreFin, StringComparison reglaComparacion = StringComparison.OrdinalIgnoreCase)
        {
            string respuesta = "";
            int posicionInicio = cadena.IndexOf(nombreInicio, posicion, reglaComparacion);
            if (posicionInicio > -1)
            {
                posicionInicio += nombreInicio.Length;
                int posFin = cadena.IndexOf(nombreFin, posicionInicio, reglaComparacion);
                if (posFin > -1)
                    respuesta = cadena.Substring(posicionInicio, posFin - posicionInicio);
            }

            return respuesta;
        }
        private string[] ExtraerContenidoEntreTag(string cadena, int posicion, string nombreInicio, string nombreFin, StringComparison reglaComparacion = StringComparison.OrdinalIgnoreCase)
        {
            string[] arrRespuesta = null;
            int posicionInicio = cadena.IndexOf(nombreInicio, posicion, reglaComparacion);
            if (posicionInicio > -1)
            {
                posicionInicio += nombreInicio.Length;
                int posFin = cadena.IndexOf(nombreFin, posicionInicio, reglaComparacion);
                if (posFin > -1)
                {
                    posicion = posFin + nombreFin.Length;
                    arrRespuesta = new string[2];
                    arrRespuesta[0] = posicion.ToString();
                    arrRespuesta[1] = cadena.Substring(posicionInicio, posFin - posicionInicio);
                }
            }

            return arrRespuesta;
        }

        public class DatosRUC
        {
            public short TipoRespuesta { get; set; }
            public string MensajeRespuesta { get; set; }
            public string RUC { get; set; }
            public string TipoContribuyente { get; set; }
            public string NombreComercial { get; set; }
            public string FechaInscripcion { get; set; }
            public string FechaInicioActividades { get; set; }
            public string EstadoContribuyente { get; set; }
            public string CondicionContribuyente { get; set; }
            public string DomicilioFiscal { get; set; }
            public string SistemaEmisionComprobante { get; set; }
            public string ActividadComercioExterior { get; set; }
            public string SistemaContabilidiad { get; set; }
            public string ActividadesEconomicas { get; set; }
            public string ComprobantesPago { get; set; }
            public string SistemaEmisionElectrónica { get; set; }
            public string EmisorElectrónicoDesde { get; set; }
            public string ComprobantesElectronicos { get; set; }
            public string AfiliadoPLEDesde { get; set; }
            public string Padrones { get; set; }
        }

        private DatosRUC ObtenerDatos(string contenidoHTML)
        {
            DatosRUC oDatosRUC = new DatosRUC();
            string nombreInicio = "<HEAD><TITLE>";
            string nombreFin = "</TITLE></HEAD>";
            string contenidoBusqueda = ExtraerContenidoEntreTagString(contenidoHTML, 0, nombreInicio, nombreFin);
            if (contenidoBusqueda == ".:: Pagina de Mensajes ::.")
            {
                nombreInicio = "<p class=\"error\">";
                nombreFin = "</p>";
                oDatosRUC.TipoRespuesta = 2;
                oDatosRUC.MensajeRespuesta = ExtraerContenidoEntreTagString(contenidoHTML, 0, nombreInicio, nombreFin);
            }
            else if (contenidoBusqueda == ".:: Pagina de Error ::.")
            {
                nombreInicio = "<p class=\"error\">";
                nombreFin = "</p>";
                oDatosRUC.TipoRespuesta = 3;
                oDatosRUC.MensajeRespuesta = ExtraerContenidoEntreTagString(contenidoHTML, 0, nombreInicio, nombreFin);
            }
            else
            {
                oDatosRUC.TipoRespuesta = 2;
                nombreInicio = "<div class=\"list-group\">";
                nombreFin = "<div class=\"panel-footer text-center\">";
                contenidoBusqueda = ExtraerContenidoEntreTagString(contenidoHTML, 0, nombreInicio, nombreFin);
                if (contenidoBusqueda == "")
                {
                    nombreInicio = "<strong>";
                    nombreFin = "</strong>";
                    oDatosRUC.MensajeRespuesta = ExtraerContenidoEntreTagString(contenidoHTML, 0, nombreInicio, nombreFin);
                    if(oDatosRUC.MensajeRespuesta == "")
                        oDatosRUC.MensajeRespuesta = "No se encuentra las cabeceras principales del contenido HTML";
                }
                else
                {
                    contenidoHTML = contenidoBusqueda;
                    oDatosRUC.MensajeRespuesta = "Mensaje del inconveniente no especificado";
                    nombreInicio = "<h4 class=\"list-group-item-heading\">";
                    nombreFin = "</h4>";
                    int resultadoBusqueda = contenidoHTML.IndexOf(nombreInicio, 0, StringComparison.OrdinalIgnoreCase);
                    if (resultadoBusqueda > -1)
                    {
                        resultadoBusqueda += nombreInicio.Length;
                        string[] arrResultado = ExtraerContenidoEntreTag(contenidoHTML, resultadoBusqueda,
                            nombreInicio, nombreFin);
                        if (arrResultado != null)
                        {
                            oDatosRUC.RUC = arrResultado[1];

                            // Tipo Contribuyente
                            nombreInicio = "<p class=\"list-group-item-text\">";
                            nombreFin = "</p>";
                            arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                nombreInicio, nombreFin);
                            if (arrResultado != null)
                            {
                                oDatosRUC.TipoContribuyente = arrResultado[1];

                                // Nombre Comercial
                                arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                    nombreInicio, nombreFin);
                                if (arrResultado != null)
                                {
                                    oDatosRUC.NombreComercial = arrResultado[1].Replace("\r\n", "").Replace("\t", "").Trim();

                                    // Fecha de Inscripción
                                    arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                        nombreInicio, nombreFin);
                                    if (arrResultado != null)
                                    {
                                        oDatosRUC.FechaInscripcion = arrResultado[1];

                                        // Estado del Contribuyente
                                        arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                            nombreInicio, nombreFin);
                                        if (arrResultado != null)
                                        {
                                            oDatosRUC.EstadoContribuyente = arrResultado[1].Trim();

                                            // Condición del Contribuyente
                                            arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                nombreInicio, nombreFin);
                                            if (arrResultado != null)
                                            {
                                                oDatosRUC.CondicionContribuyente = arrResultado[1].Trim();

                                                // Domicilio Fiscal
                                                arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                    nombreInicio, nombreFin);
                                                if (arrResultado != null)
                                                {
                                                    oDatosRUC.DomicilioFiscal = arrResultado[1].Trim();

                                                    // Actividad(es) Económica(s)
                                                    nombreInicio = "<tbody>";
                                                    nombreFin = "</tbody>";
                                                    arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                        nombreInicio, nombreFin);
                                                    if (arrResultado != null)
                                                    {
                                                        oDatosRUC.ActividadesEconomicas = arrResultado[1].Replace("\r\n", "").Replace("\t", "").Trim();

                                                        // Comprobantes de Pago c/aut. de impresión (F. 806 u 816)
                                                        arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                            nombreInicio, nombreFin);
                                                        if (arrResultado != null)
                                                        {
                                                            oDatosRUC.ComprobantesPago = arrResultado[1].Replace("\r\n", "").Replace("\t", "").Trim();

                                                            // Sistema de Emisión Electrónica
                                                            arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                                nombreInicio, nombreFin);
                                                            if (arrResultado != null)
                                                            {
                                                                oDatosRUC.SistemaEmisionComprobante = arrResultado[1].Replace("\r\n", "").Replace("\t", "").Trim();

                                                                // Afiliado al PLE desde
                                                                nombreInicio = "<p class=\"list-group-item-text\">";
                                                                nombreFin = "</p>";
                                                                arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                                    nombreInicio, nombreFin);
                                                                if (arrResultado != null)
                                                                {
                                                                    oDatosRUC.AfiliadoPLEDesde = arrResultado[1];

                                                                    // Padrones 
                                                                    nombreInicio = "<tbody>";
                                                                    nombreFin = "</tbody>";
                                                                    arrResultado = ExtraerContenidoEntreTag(contenidoHTML, Convert.ToInt32(arrResultado[0]),
                                                                        nombreInicio, nombreFin);
                                                                    if (arrResultado != null)
                                                                    {
                                                                        oDatosRUC.Padrones = arrResultado[1].Replace("\r\n", "").Replace("\t", "").Trim();

                                                                        oDatosRUC.TipoRespuesta = 1;
                                                                        oDatosRUC.MensajeRespuesta = "Ok";
                                                                    }
                                                                }
                                                            }

                                                        }

                                                    }

                                                }

                                            }

                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }

            return oDatosRUC;
        }

        private async Task<string[]> ValidarImagenCaptcha(HttpClient cliente, short totalIntentos = 3)
        {
            int tipoRespuesta = 2;
            string mensajeRespuesta = "Mensaje de inconveniente no especificado";
            try
            {
                string url = "https://e-consultaruc.sunat.gob.pe/cl-ti-itmrconsruc/captcha?accion=image";
                byte[] arrContenido;

                float porcentajeConfianza = 0;
                int cIntentos = 0;
                while (porcentajeConfianza < 0.51 && cIntentos < totalIntentos + 1)
                {
                    cIntentos++;
                    using (HttpResponseMessage resultadoConsultaImagen = await cliente.GetAsync(url))
                    {
                        if (resultadoConsultaImagen.IsSuccessStatusCode)
                        {
                            using (Stream stmContenido = await resultadoConsultaImagen.Content.ReadAsStreamAsync())
                            {
                                arrContenido = new byte[stmContenido.Length];
                                await stmContenido.ReadAsync(arrContenido, 0, arrContenido.Length);
                            }
                            if (arrContenido.Length > 0)
                            {
                                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                                {
                                    using (var img = Pix.LoadFromMemory(arrContenido))
                                    {
                                        using (var page = engine.Process(img))
                                        {
                                            mensajeRespuesta = page.GetText();
                                            porcentajeConfianza = page.GetMeanConfidence();
                                            if (porcentajeConfianza > 0.50)
                                            {
                                                tipoRespuesta = 1;
                                                mensajeRespuesta = mensajeRespuesta.Replace("\n", "").Trim().ToUpper();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (tipoRespuesta == 2 && cIntentos == totalIntentos)
                    mensajeRespuesta = "No se pudo validar la imagen mayor a 50% de confianza en el proceso";
            }
            catch (Exception ex)
            {
                tipoRespuesta = 3;
                mensajeRespuesta = ex.Message;
            }
            
            return new string[2] { tipoRespuesta.ToString(), mensajeRespuesta};
        }

        private async void btnConsultarRUCMedianteImagen_Click(object sender, EventArgs e)
        {
            int tipoRespuesta = 2;
            string mensajeRespuesta = "";

            txtRUC.Text = "";
            txtTipoContribuyente.Text = "";
            txtNombreComercial.Text = "";
            txtFechaInscripcion.Text = "";
            txtEstadoContribuyente.Text = "";
            txtCondicionContribuyente.Text = "";
            txtDomicilioFiscal.Text = "";
            txtSistemaEmisionComprobante.Text = "";
            txtActividadesEconomicas.Text = "";
            txtComprobantesPago.Text = "";
            txtAfiliadoPLE.Text = "";
            txtPadrones.Text = "";

            string ruc = txtNumeroRUC.Text;
            if (string.IsNullOrWhiteSpace(ruc))
                return;

            btnConsultarRUCMedianteImagen.Enabled = false;

            CookieContainer cookies = new CookieContainer();
            HttpClientHandler controladorMensaje = new HttpClientHandler();
            controladorMensaje.CookieContainer = cookies;
            controladorMensaje.UseCookies = true;
            using (HttpClient cliente = new HttpClient(controladorMensaje))
            {

                cliente.DefaultRequestHeaders.Add("Host", "e-consultaruc.sunat.gob.pe");
                cliente.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.150 Safari/537.36");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                                       SecurityProtocolType.Tls12;

                string[] arrResultadoValidarImagenCaptcha = await ValidarImagenCaptcha(cliente, 5);
                if (arrResultadoValidarImagenCaptcha[0] == "1")
                {
                    var lClaveValor = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("accion", "consPorRuc"),
                        new KeyValuePair<string, string>("nroRuc", ruc),
                        new KeyValuePair<string, string>("contexto", "ti-it"),
                        new KeyValuePair<string, string>("modo", "1"),
                        new KeyValuePair<string, string>("rbtnTipo", "1"),
                        new KeyValuePair<string, string>("search1", ruc),
                        new KeyValuePair<string, string>("tipdoc", "1"),
                        new KeyValuePair<string, string>("codigo", arrResultadoValidarImagenCaptcha[1])
                    };
                    FormUrlEncodedContent contenido = new FormUrlEncodedContent(lClaveValor);
                    string url = "https://e-consultaruc.sunat.gob.pe/cl-ti-itmrconsruc/jcrS03Alias";
                    using (HttpResponseMessage resultadoConsultaDatos = await cliente.PostAsync(url, contenido))
                    {
                        if (resultadoConsultaDatos.IsSuccessStatusCode)
                        {
                            string contenidoHTML = await resultadoConsultaDatos.Content.ReadAsStringAsync();
                            contenidoHTML = WebUtility.HtmlDecode(contenidoHTML);
                            DatosRUC oDatosRUC = ObtenerDatos(contenidoHTML);
                            if (oDatosRUC.TipoRespuesta == 1)
                            {
                                txtRUC.Text = oDatosRUC.RUC;
                                txtTipoContribuyente.Text = oDatosRUC.TipoContribuyente;
                                txtNombreComercial.Text = oDatosRUC.NombreComercial;
                                txtFechaInscripcion.Text = oDatosRUC.FechaInscripcion;
                                txtEstadoContribuyente.Text = oDatosRUC.EstadoContribuyente;
                                txtCondicionContribuyente.Text = oDatosRUC.CondicionContribuyente;
                                txtDomicilioFiscal.Text = oDatosRUC.DomicilioFiscal;
                                txtSistemaEmisionComprobante.Text = oDatosRUC.SistemaEmisionComprobante;
                                txtActividadesEconomicas.Text = oDatosRUC.ActividadesEconomicas;
                                txtComprobantesPago.Text = oDatosRUC.ComprobantesPago;
                                txtAfiliadoPLE.Text = oDatosRUC.AfiliadoPLEDesde;
                                txtPadrones.Text = oDatosRUC.Padrones;

                                tipoRespuesta = 1;
                                mensajeRespuesta =
                                    string.Format("Se realizó exitosamente la consulta del número de RUC {0}",
                                        ruc);
                            }
                            else
                            {
                                tipoRespuesta = oDatosRUC.TipoRespuesta;
                                mensajeRespuesta = string.Format(
                                    "No se pudo realizar la consulta del número de RUC {0}.\r\nDetalle: {1}",
                                    ruc,
                                    oDatosRUC.MensajeRespuesta);
                            }
                        }
                        else
                        {
                            mensajeRespuesta = await resultadoConsultaDatos.Content.ReadAsStringAsync();
                            mensajeRespuesta = string.Format("Ocurrió un inconveniente al consultar los datos del RUC {0}.\r\nDetalle:{1}", ruc, mensajeRespuesta);
                        }

                    }

                }
                else
                {
                    tipoRespuesta = Convert.ToInt16(arrResultadoValidarImagenCaptcha[0]);
                    mensajeRespuesta = arrResultadoValidarImagenCaptcha[1];
                }

            }

            if (tipoRespuesta > 1)
                MessageBox.Show(mensajeRespuesta, "Consultar RUC mediante imagen"
                    , MessageBoxButtons.OK
                    , tipoRespuesta == 2 ? MessageBoxIcon.Warning : MessageBoxIcon.Error);

            btnConsultarRUCMedianteImagen.Enabled = true;
            txtNumeroRUC.Focus();
            txtNumeroRUC.SelectAll();
        }

        
    }
}
