using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using System.Data.Sql;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace water_checker_api.Controllers
{
    public class ValuesController : ApiController
    {


        // GET api/values
        [SwaggerOperation("GetAll")]
        public void Get(check_values[] chv,TimeSpan lastts)
        {
           
        }

        // POST api/values
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public async void Post()
        {
            HttpStatusCode response;
            try
            {
                string[] obj = (await Request.Content.ReadAsStringAsync()).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(check_values[]));
                MemoryStream stream1 = new MemoryStream();
                byte[] barr = Encoding.UTF8.GetBytes(obj[0]);
                stream1.Write(barr, 0, barr.Length);
                stream1.Position = 0;
                check_values[] chv = (check_values[])ser.ReadObject(stream1);

                ser = new DataContractJsonSerializer(typeof(TimeSpan));
                stream1 = new MemoryStream();
                barr = Encoding.UTF8.GetBytes(obj[1]);
                stream1.Write(barr, 0, barr.Length);
                stream1.Position = 0;
                TimeSpan lastts = (TimeSpan)ser.ReadObject(stream1); ;

                ser = new DataContractJsonSerializer(typeof(int));
                stream1 = new MemoryStream();
                barr = Encoding.UTF8.GetBytes(obj[2]);
                stream1.Write(barr, 0, barr.Length);
                stream1.Position = 0;
                int device = (int)ser.ReadObject(stream1);

                DateTime cur_date = DateTime.Now;
                cur_date -= lastts;

                SqlConnection conn = new SqlConnection("Data Source = waterchecker.database.windows.net; Initial Catalog = watercheckerdb; Integrated Security = False; User ID = markevich; Password = XzVcNb1984; Connect Timeout = 60; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
                DataTable dt = new DataTable();
                dt.Columns.Add("date_time");
                dt.Columns.Add("value");
                dt.Columns.Add("device");
                foreach (check_values cv in chv)
                {
                    cur_date -= cv.time;
                    dt.Rows.Add(cur_date, cv.value, device);
                }

                conn.Open();
                SqlBulkCopy cp = new SqlBulkCopy(conn);
                cp.DestinationTableName = "check_values";
                cp.WriteToServer(dt);
                conn.Close();
                response = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response = HttpStatusCode.UnsupportedMediaType;
            }
            Request.CreateResponse(response);
            //throw new HttpResponseException(response);
        }

        // PUT api/values/5
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public void Delete(int id)
        {
        }
    }
}
