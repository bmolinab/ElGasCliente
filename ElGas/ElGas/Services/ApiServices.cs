using System;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ElGas.Helpers;
using ElGas.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Text;

namespace ElGas.Services
{
    internal class ApiServices
    {
        public async Task<bool> RegisterUserAsync(string email, string password, string confirmPassword, Cliente cliente)
        {
            var client = new HttpClient();

            var model = new RegisterBindingModel
            {
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var json = JsonConvert.SerializeObject(model);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync(
                Constants.BaseApiAddress + "api/Account/Register", httpContent);
            var result = await response.Content.ReadAsStringAsync();
              var AspNetUSer = JsonConvert.DeserializeObject<AspNetUser>(result);

            cliente.IdAspNetUser = AspNetUSer.Id;
            cliente.Correo = AspNetUSer.Email;


            Debug.WriteLine(result);
            if (response.IsSuccessStatusCode)
            {
                var json2 = JsonConvert.SerializeObject(cliente);
                HttpContent httpContent2 = new StringContent(json2);

                httpContent2.Headers.ContentType = new MediaTypeHeaderValue("application/json");


                var response2 = await client.PostAsync(
                Constants.BaseApiAddress + "api/Clientes/PostClient", httpContent2);

                if (response2.IsSuccessStatusCode)
                {                  
                    return true;
                }

            }

            return false;
        }

        public async Task<string> LoginAsync(string userName, string password)
        {
            var keyValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("username", userName),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", "password")
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post, Constants.BaseApiAddress + "Token");

            request.Content = new FormUrlEncodedContent(keyValues);

            var client = new HttpClient();
            var response = await client.SendAsync(request);

            var content = await response.Content.ReadAsStringAsync();

            JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(content);

            var accessTokenExpiration = jwtDynamic.Value<DateTime>(".expires");
            var accessToken = jwtDynamic.Value<string>("access_token");

            Settings.AccessTokenExpirationDate = accessTokenExpiration;


            
            Debug.WriteLine(accessTokenExpiration);

            Debug.WriteLine(content);

            return accessToken;
        }

        public async Task<List<DistribuidorResponse>> DistribuidoresCercanos(Posicion posicion)
        {
          
                try
                {
                    var request = JsonConvert.SerializeObject(posicion);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");
                    var client = new HttpClient();
                    client.BaseAddress = new Uri(Constants.BaseApiAddress);
                    var url = "api/Distribuidors/NearDistribuidor";
                    var response = await client.PostAsync(url, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;

                    }

                var result = await response.Content.ReadAsStringAsync();
                var distribuidores = JsonConvert.DeserializeObject<List<DistribuidorResponse>>(result);

                return distribuidores;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            
        }

        public static async Task<Response> InsertarAsync<T>(T model, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");
                    var uri = string.Format("{0}/{1}", baseAddress, url);
                    var response = await client.PostAsync(new Uri(uri), content);
                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<Response>(resultado);
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = ex.Message,
                };
            }
        }

        public static async Task<Response> InsertarAsync<T>(object model, Uri baseAddress, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var request = JsonConvert.SerializeObject(model);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var uri = string.Format("{0}/{1}", baseAddress, url);

                    var response = await client.PostAsync(new Uri(uri), content);

                    var resultado = await response.Content.ReadAsStringAsync();
                    var respuesta = JsonConvert.DeserializeObject<Response>(resultado);
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = ex.Message,
                };
            }
        }

    }
}
