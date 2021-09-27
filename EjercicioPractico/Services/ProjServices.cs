using EjercicioPractico.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EjercicioPractico.Services
{
    public class ProjServices
    {
        private static int id = 1;
        public async Task<MainViewModel> GetFileData(MemoryStream memory)
        {
            var model = new MainViewModel();
            var array = memory.ToArray();

            try
            {
                var list = await ProcessFile(array);
                if (list == null  || !list.Any())
                    model.Calificaciones = new List<Calificaciones>();
                else
                    model.Calificaciones = list;

                var grafica = list.Select(m => new DataItem 
                { 
                    Alumno = m.Nombres,
                    Calificacion = m.Calificacion
                }).ToArray();

                model.Grafica = grafica;

                var auxbestgrade = model.Calificaciones.OrderByDescending(m => m.Calificacion).FirstOrDefault();
                model.BestGrade = $"{auxbestgrade.Nombres} {auxbestgrade.ApellidosPaterno} {auxbestgrade.ApellidoMaterno}";

                var auxworstgrade = model.Calificaciones.OrderBy(m => m.Calificacion).FirstOrDefault();
                model.WorstGrade = $"{auxworstgrade.Nombres} {auxworstgrade.ApellidosPaterno} {auxworstgrade.ApellidoMaterno}";

                model.Average = model.Calificaciones.Sum(m => m.Calificacion) / model.Calificaciones.Count;
                return model;
            }
            catch (Exception ex)
            {
                //TODO: handle
                throw ex;
            }
        }

        private async Task<List<Calificaciones>> ProcessFile(byte[] array)
        {
            var data = new List<Calificaciones>();
            try
            {
                var ms = new MemoryStream(array);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (ExcelPackage package = new ExcelPackage(ms))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets[0];

                    for (int i = sheet.Dimension.Start.Row; i <= sheet.Dimension.End.Row; i++)
                    {
                        if (i == 1)
                        {
                            continue;
                        }

                        if (sheet.Cells[i, 1].Value == null)
                        {
                            continue;
                        }

                        var auxnombre = string.IsNullOrEmpty(sheet.Cells[i, 1].Value.ToString()) ? null : sheet.Cells[i, 1].Value.ToString();
                        var auxmaterno = string.IsNullOrEmpty(sheet.Cells[i, 3].Value.ToString()) ? null : sheet.Cells[i, 3].Value.ToString();
                        var auxcali = string.IsNullOrEmpty(sheet.Cells[i, 7].Value.ToString()) ? null : sheet.Cells[i, 7].Value.ToString();
                        var auxfecha = string.IsNullOrEmpty(sheet.Cells[i, 4].Value.ToString()) ? null : sheet.Cells[i, 4].Value.ToString();
                        DateTime date = DateTime.ParseExact(auxfecha, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        var edad = DateTime.Today.Year - date.Year;
                        var claveaux = $"{auxnombre.Substring(0, 2) + auxmaterno.Substring(auxmaterno.Length - 2)}";
                        var clavecrypted = ChangePositions(claveaux);
                        var model = new Calificaciones
                        {
                            Id = id++,
                            Nombres = auxnombre,
                            ApellidosPaterno = string.IsNullOrEmpty(sheet.Cells[i, 2].Value.ToString()) ? null : sheet.Cells[i, 2].Value.ToString(),
                            ApellidoMaterno = auxmaterno,
                            FechaNacimiento = auxfecha,
                            Grado = string.IsNullOrEmpty(sheet.Cells[i, 5].Value.ToString()) ? null : sheet.Cells[i, 5].Value.ToString(),
                            Grupo = string.IsNullOrEmpty(sheet.Cells[i, 6].Value.ToString()) ? null : sheet.Cells[i, 6].Value.ToString(),
                            Calificacion = Convert.ToDouble(auxcali),
                            Clave = clavecrypted + edad,
                            ClaveAux = clavecrypted + edad
                        };

                        data.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return data;
        }
        public string ChangePositions(string text)
        {
            var alphabet = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
            var newtext = "";
            foreach (var letter in text.ToUpper())
            {
                var index = alphabet.IndexOf(letter);
                var ans = alphabet.Substring(alphabet.Length - 3, alphabet.Length - (alphabet.Length - 3)) + alphabet.Substring(0, (alphabet.Length - 3));
                newtext += ans[index];
            }
            return newtext;
        }

        public Root GetWeather()
        {
            var client = new RestClient("https://community-open-weather-map.p.rapidapi.com/weather?q=Hermosillo&lat=0&lon=0&callback=test&id=2172797&lang=null&units=imperial&mode=xml");
            var request = new RestRequest(Method.GET);
            request.AddHeader("x-rapidapi-host", "community-open-weather-map.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "4f4fa22486msh86b83d5ce28e1b5p176e3ejsn250fbb541421");
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string value = response.Content.Replace("test(", "");
                string value2 = value.Replace(")", "");

                var test = JsonConvert.DeserializeObject<Root>(value2);
                return test;
            }
            return new Root();
        }
    }
}
