using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Agenda
{
    public class Seguridad
    {
        // Genera hash SHA256 (string hexadecimal)
        public static string HashSha256(string texto)
        {
            if (texto == null) texto = "";

            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(texto);
                byte[] hash = sha.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        // Enmascara correos: ye****ci@gmail.com
        public static string EnmascararCorreo(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return "";

            email = email.Trim();

            int arroba = email.IndexOf('@');
            if (arroba <= 0) return "*****";

            string usuario = email.Substring(0, arroba);
            string dominio = email.Substring(arroba); // incluye @

            if (usuario.Length <= 2)
            {
                return usuario.Substring(0, 1) + "*****" + dominio;
            }

            int mostrarInicio = 2;
            int mostrarFin = Math.Min(2, usuario.Length - mostrarInicio);

            string inicio = usuario.Substring(0, mostrarInicio);
            string fin = usuario.Substring(usuario.Length - mostrarFin, mostrarFin);

            int estrellas = usuario.Length - (mostrarInicio + mostrarFin);
            if (estrellas < 1) estrellas = 1;

            return inicio + new string('*', estrellas) + fin + dominio;
        }
    }
}