using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Parcial2.Pages
{
    public class RegistroModel : PageModel
    {
        [Required(ErrorMessage = "El nombre y apellido son obligatorios.")]
        [Display(Name = "Nombre y Apellido")]
        [BindProperty]
        public string? NombreCompleto { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [Display(Name = "Nombre de usuario")]
        [BindProperty]
        public string? NombreUsuario { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [Display(Name = "Correo electrónico")]
        [BindProperty]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar la edad.")]
        [Range(16, 120, ErrorMessage = "Solo se aceptan usuarios mayores de 15 años.")]
        [Display(Name = "Edad")]
        [BindProperty]
        public int? Edad { get; set; }

        [Required(ErrorMessage = "Debe ingresar una contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        [BindProperty]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        [BindProperty]
        public string? ConfirmPassword { get; set; }

        public string? MensajeError { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // validacion 
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // sesion
            HttpContext.Session.SetString("Usuario", NombreUsuario!);


            var cookieOpciones = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(1),
                IsEssential = true,
                HttpOnly = true
            };
            Response.Cookies.Append("PasswordCookie", Password!, cookieOpciones);

            return RedirectToPage("/Documentos");
        }
    }
}