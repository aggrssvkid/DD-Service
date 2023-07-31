using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DocsVision.Platform.StorageServer.Extensibility;
using Newtonsoft.Json;

namespace TestService
{
    public class TotalCost : StorageServerExtension // Расширение Платформы DigitalDesign
    {
        [ExtensionMethod]
        public decimal GetTotalCost(DateTime s, DateTime po, string portCode)
        {
            var tudaCost = GetCost(s, portCode);
            if (tudaCost == 0 || tudaCost == 1 || tudaCost == 2 || tudaCost == 3)
                return tudaCost;
            var obratnoCost = GetCost(po, portCode);
            if (obratnoCost == 0 || obratnoCost == 1 || obratnoCost == 2 || obratnoCost == 3) 
                return obratnoCost+5;

            return tudaCost + obratnoCost;
        }

        private decimal GetCost(DateTime date, string portCode)
        {
            // Получаем данные с сервера
            string url = GetUrl(date);
            string res = GetContent(url);

            // Обрабатываем эти данные и делаем нужную выборьку
            if (res.Length == 0)
                return 0;
            List<ModelJson> models = JsonConvert.DeserializeObject<List<ModelJson>>(res);
            if (models.Any() == false)
                return 1;
            var datas = models.Where(x => x.depart_date == date);
            if (datas.Any() == false)
                return 2;
            var ports = datas.Where(d => d.destination == portCode);
            if (ports.Any() == false)
                return 3;
            decimal minPrice = ports.Select(p => p.value).Min();
            return minPrice;
        }

        private string GetUrl(DateTime time)
        {
            // Парсим ЮРИЯ, так как делаем запрос динамически, в зависимости от входных данных
            string constStartUrl = @"http://map.aviasales.ru/prices.json?origin_iata=LED&period=";
            string constEndUrl = ":month&direct=true&one_way=true&no_visa=false&schengen=false&need_visa=false&locale=ru";
            string reverseDate = time.ToShortDateString();
            var splitDate = reverseDate.Split('.');
            string date = "";
            for (int i = splitDate.Length - 1; i >= 0; i--)
            {
                date += splitDate[i];
                if (i != 0)
                    date += '-';
            }
            string url = constStartUrl + date + constEndUrl;
            return url;
        }
        
        // Функция запроса данных с сайта с открытым апи
        private string GetContent(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            // Настраиваем параметры запроса
            request.Method = "GET";
            request.Accept = "application/json";
            request.UserAgent = "Mozilla/5.0....";

            // Создаем поток записи данных и считываем данные с сервера
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            StringBuilder output = new StringBuilder();
            output.Append(reader.ReadToEnd());
            response.Close();
            return output.ToString();
        }
    }

    // Моделька для того, чтобы в такую форму парсить JSON
    public class ModelJson
    {
        public decimal value;
        public string destination;
        public DateTime depart_date;
    }
}
