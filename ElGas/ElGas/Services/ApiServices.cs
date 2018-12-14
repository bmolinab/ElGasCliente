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
using Plugin.Connectivity;

namespace ElGas.Services
{
    internal class ApiServices
        {/// <summary>
        /// Registra nuevos usuarios clientes
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public async Task<bool> RegisterUserAsync(string email, string password, string confirmPassword, Cliente cliente)
        {
            if (CrossConnectivity.Current.IsConnected)
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

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var AspNetUSer = JsonConvert.DeserializeObject<AspNetUser>(result);
                    cliente.IdAspNetUser = AspNetUSer.Id;
                    cliente.Correo = AspNetUSer.Email;
                    Debug.WriteLine(result);

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
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var Mensaje = JsonConvert.DeserializeObject<ErrorModel>(result);

                    var Message = "";

                    foreach (var item in Mensaje.ModelState.Error)
                    {
                        Message = item + "\n";
                    };

                    await App.Current.MainPage.DisplayAlert("El Gas", Message, "Aceptar");

                } 
            }
            else
            {
              await  App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
            }

            return false;
        }
        /// <summary>
        /// Permite el ingreso de lo usuarios autenticados a la aplicación 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> LoginAsync(string userName, string password)
        {
            try
            {
                if (CrossConnectivity.Current.IsConnected)
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
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(content);

                        var accessTokenExpiration = jwtDynamic.Value<DateTime>(".expires");
                        var accessToken = jwtDynamic.Value<string>("access_token");

                        Settings.AccessTokenExpirationDate = accessTokenExpiration;

                        Debug.WriteLine(accessTokenExpiration);

                        Debug.WriteLine(content);

                        return accessToken;
                    }
                    else
                    {
                        //await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.Excepcion, Mensaje.TextoBoton.Aceptar);
                        return null;

                    }

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.Excepcion, Mensaje.TextoBoton.Aceptar);
                return "1";

            }

            return null;
        }
        
        /// <summary>
        /// Obtiene los distribuidores cercanos
        /// </summary>
        /// <param name="posicion"></param>
        /// <returns></returns>
        public async Task<List<DistribuidorResponse>> DistribuidoresCercanos(Posicion posicion)
        {

            if (CrossConnectivity.Current.IsConnected)
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
            else
            {
                await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
            }
            return null;
            
        }

        public static async Task<Response> InsertarAsync<T>(T model, Uri baseAddress, string url)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var request = JsonConvert.SerializeObject(model);
                        var content = new StringContent(request, Encoding.UTF8, "application/json");
                        var uri = string.Format("{0}{1}", baseAddress, url);
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
                        IsSuccess = false,
                        Message = ex.Message,
                    };
                } 
            }
          
            return new Response
            {
                IsSuccess = false,
                Message = Mensaje.Contenido.SinInternet,
            };

        }

        public static async Task<Response> InsertarAsync<T>(object model, Uri baseAddress, string url)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var request = JsonConvert.SerializeObject(model);
                        var content = new StringContent(request, Encoding.UTF8, "application/json");

                        var uri = string.Format("{0}{1}", baseAddress, url);

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
                        IsSuccess = false,
                        Message = ex.Message,
                    };
                }

            }
          
            return new Response
            {
                IsSuccess = false,
                Message = Mensaje.Contenido.SinInternet,
            };

        }
        /// <summary>
        /// Genera un codigo para recuperar la contraseña
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> GenerateCode(string email)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var client = new HttpClient();

                var model = new RegisterBindingModel
                {
                    Email = email,
                };
                var json = JsonConvert.SerializeObject(model);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = await client.PostAsync(
                Constants.BaseApiAddress + "api/Account/GenerateCode", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    return true;

                }

            }
            else
            {
                await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
            }
            return false;
        }
        /// <summary>
        /// Permite cambiar la contraseña
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public async Task<bool> RecoveryPass(string email, string password, string confirmPassword, int codigo)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                var client = new HttpClient();

                if (password == confirmPassword)
                {
                    var model = new PasswordRequest
                    {
                        Email = email,
                        Codigo = codigo,
                        NewPassword = password
                    };

                    var json = JsonConvert.SerializeObject(model);

                    HttpContent httpContent = new StringContent(json);

                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await client.PostAsync(
                        Constants.BaseApiAddress + "api/Account/RecoveryPass", httpContent);



                    if (response.IsSuccessStatusCode)
                    {

                        return true;


                    }

                } 
            }
            else
            {
                await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
            }
            return false;
        }

        /// <summary>
        /// Obtiene todas las ciudades
        /// </summary>
        /// <returns></returns>
        public async Task<List<Ciudad>> GetCiudades()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri(Constants.BaseApiAddress);
                    var url = "api/Ciudades/GetCiudades";
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    var Ciudades = JsonConvert.DeserializeObject<List<Ciudad>>(result);

                    return Ciudades;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }

            }
            return null;
        }

        /// <summary>
        /// Obtiene los sectores segùn la ciudad
        /// </summary>
        /// <param name="idCiudad"></param>
        /// <returns></returns>
        public async Task<List<Sector>> GetSectors(int idCiudad)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    var client = new HttpClient();
                    client.BaseAddress = new Uri(Constants.BaseApiAddress);
                    var url = "api/Sectores/GetSectorsByCity/" + idCiudad;
                    var response = await client.PostAsync(url, null);
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var result = await response.Content.ReadAsStringAsync();
                    var Sector = JsonConvert.DeserializeObject<List<Sector>>(result);

                    return Sector;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }

            }
            else
            {
                await App.Current.MainPage.DisplayAlert(Mensaje.Titulo.Error, Mensaje.Contenido.SinInternet, Mensaje.TextoBoton.Aceptar);
            }
            return null;
        }



        public async Task<Response> Horario(SolicitudesFallidas solicitudes)
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var request = JsonConvert.SerializeObject(solicitudes);
                        var content = new StringContent(request, Encoding.UTF8, "application/json");
                        var uri = string.Format("{0}{1}", Constants.BaseApiAddress, "api/Compras/HorarioDeAtencion");
                        var response = await client.PostAsync(new Uri(uri), content);
                        if (!response.IsSuccessStatusCode)
                        {
                            return new Response
                            {
                                IsSuccess = false,
                                Result = -2
                            };
                        }
                        var resultado = await response.Content.ReadAsStringAsync();
                        var respuesta = JsonConvert.DeserializeObject<Response>(resultado);
                        return respuesta;
                    }

                    //var client = new HttpClient();
                    //client.BaseAddress = new Uri(Constants.BaseApiAddress);
                    //var url = "api/Compras/HorarioDeAtencion";
                    //var response = await client.GetAsync(url);
                    //if (!response.IsSuccessStatusCode)
                    //{
                    //    return new Response {
                    //        IsSuccess=false,
                    //        Result=-2
                    //    };
                    //}

                    //var result = await response.Content.ReadAsStringAsync();
                    //var Respuesta = JsonConvert.DeserializeObject<Response>(result);
                    //return Respuesta;

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return new Response
                    {
                        IsSuccess = false,
                        Message= ex.Message,
                        Result = -2
                    };
                }

            }
             return new Response
            {
                IsSuccess = false,
                Result = -2
            }; ;
        }


    }
}
