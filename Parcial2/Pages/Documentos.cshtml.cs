using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Parcial2.Pages
{
    public class DocumentosModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public DocumentosModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string? UsuarioActivo { get; set; }

        [BindProperty]
        public IFormFile? Archivo { get; set; }

        public string? Mensaje { get; set; }
        public string? MensajeError { get; set; }

        public List<string> ListaDocumentos { get; set; } = new();

        public IActionResult OnGet()
        {
            UsuarioActivo = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(UsuarioActivo))
            {
                return RedirectToPage("/Registro");
            }

            CargarListaDocumentos();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            UsuarioActivo = HttpContext.Session.GetString("Usuario");
            if (string.IsNullOrEmpty(UsuarioActivo))
            {
                return RedirectToPage("/Registro");
            }

            if (Archivo is null && Request.Form.Files.Count > 0)
            {
                Archivo = Request.Form.Files[0];
            }

            if (Archivo is null)
            {
                ModelState.AddModelError(nameof(Archivo), "Debe seleccionar un documento legal para subir.");
                CargarListaDocumentos();
                return Page();
            }

            string extension = Path.GetExtension(Archivo.FileName).ToLowerInvariant();
            string[] extensionesPermitidas = { ".pdf", ".docx", ".doc", ".png", ".jpg", ".jpeg" };

            if (!extensionesPermitidas.Contains(extension))
            {
                ModelState.AddModelError(nameof(Archivo), "Solo se permiten documentos en formato PDF, Word (.docx) o imágenes.");
                CargarListaDocumentos();
                return Page();
            }

            string nombreArchivo = Path.GetFileName(Archivo.FileName);
            string carpetaDestino = Path.Combine(_env.WebRootPath, "documentos");

            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

            if (System.IO.File.Exists(rutaCompleta))
            {
                MensajeError = $"El documento '{nombreArchivo}' ya existe en el servidor.";
                CargarListaDocumentos();
                return Page();
            }

            using (FileStream destino = new FileStream(rutaCompleta, FileMode.Create))
            {
                await Archivo.CopyToAsync(destino);
            }

            Mensaje = $"Documento '{nombreArchivo}' cargado exitosamente.";
            CargarListaDocumentos();
            return Page();
        }

        public IActionResult OnPostCerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }

        private void CargarListaDocumentos()
        {
            string carpetaDestino = Path.Combine(_env.WebRootPath, "documentos");
            if (Directory.Exists(carpetaDestino))
            {
                DirectoryInfo dir = new DirectoryInfo(carpetaDestino);
                ListaDocumentos = dir.GetFiles().Select(f => f.Name).ToList();
            }
        }
    }
}