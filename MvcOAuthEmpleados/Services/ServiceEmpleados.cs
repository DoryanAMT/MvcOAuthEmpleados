using MvcOAuthEmpleados.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.Net.Http.Headers;

namespace MvcOAuthEmpleados.Services
{
    public class ServiceEmpleados
    {
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue header;
        private IHttpContextAccessor contextAccessor;
        public ServiceEmpleados(IConfiguration configuration,
            IHttpContextAccessor contextAccessor)
        {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiEmpleados");
            this.header = new MediaTypeWithQualityHeaderValue("application/json");
            this.contextAccessor = contextAccessor;
        }

        public async Task<string> GetTokenAsync
            (string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content = new StringContent
                    (json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content
                        .ReadAsStringAsync();
                    JObject keys = JObject.Parse(data);
                    string token = keys.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return "Petición incorrecta: " + response.StatusCode;
                }
            }
        }
        public async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }
        public async Task<T> CallApiAsync<T>(string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                //  AÑADIMOS EL TOKEN A LA CABECERA
                client.DefaultRequestHeaders.Add
                    ("Authorization", "Bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            string request = "api/empleados";
            List<Empleado> empleados = await this.CallApiAsync<List<Empleado>>(request);
            return empleados;
        }

        //  ALMACENAR EL TOKEN EN SESSION
        //  POR AHORA, RECIBIMOS EL TOKEN EN EL METODO
        public async Task<Empleado> FindEmpleadoAsync
            (int idEmpleado)
        {

            string token = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type == "TOKEN").Value;
            string request = "api/empleados/"+idEmpleado;
            Empleado empleado = await
                this.CallApiAsync<Empleado>(request, token);
            return empleado;
        }

        public async Task<Empleado> GetPerfilAsync()
        {
            string token = this.contextAccessor.HttpContext.User
                .FindFirst(x => x.Type == "TOKEN").Value;
            string request = "api/empleados/perfil";
            Empleado empleado = await
                this.CallApiAsync<Empleado>(request, token);
            return empleado;
        }
        public async Task<List<Empleado>> GetCompisAsync()
        {
            string token = this.contextAccessor.HttpContext.User
                .FindFirst(x => x.Type == "TOKEN").Value;
            string request = "api/empleados/Compis";
            List<Empleado> empleados = await 
                this.CallApiAsync<List<Empleado>>(request,token);
            return empleados;
        }
        public async Task<List<string>> GetOficiosAsync()
        {
            string request = "api/empleados/oficios";
            List<string> oficios = await this.CallApiAsync<List<string>>(request);
            return oficios;
        }
        private string TransformCollectionToQuery
            (List<string> collection)
        {
            string resultado = "";
            foreach (string elem in collection)
            {
                resultado += "oficio=" + elem + "&";
            }
            resultado = resultado.TrimEnd('&');
            return resultado;
        }
        public async Task<List<Empleado>> GetEmpleadosByOficioAsync
            (List<string> oficios)
        {
            string request = "api/empleados/empleadosoficio";
            string data = this.TransformCollectionToQuery(oficios);
            List<Empleado> empleados = await
                this.CallApiAsync<List<Empleado>>
                (request + "?" + data);
            return empleados;
        }
        public async Task UpdateEmpleadosOficioAsync
            (int incremento, List<string> oficios)
        {
            string request = "api/empleados/incrementarsalarios/"+incremento;
            string data = this.TransformCollectionToQuery(oficios);
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.header);
                HttpResponseMessage response =
                    await client.PutAsync(request + "?" + data, null);
            }

        }
    }

}
