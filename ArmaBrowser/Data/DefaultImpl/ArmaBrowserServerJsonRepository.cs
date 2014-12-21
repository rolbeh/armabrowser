//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;

//namespace ArmaBrowser.Data.DefaultImpl
//{
//    //https://github.com/defuse/password-hashing

//    class ArmaBrowserServerJsonRepository : IArmaBrowserServerRepository
//    {
//        public void Test()
//        {
//            //var item = new TestData
//            //{
//            //    Field1 = "Data 1",
//            //    Field2 = "Data 2",
//            //    Field3 = "Data 3",
//            //    Field4 = "Data 4"
//            //};

//            ////var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(item);


//            ////System.Diagnostics.Debug.WriteLine(jsonString);

//            //const string url = @"http://192.168.20.98/json2mysql/";

//            //using (HttpClient client = new HttpClient())
//            //{
//            //    client.BaseAddress = new Uri(url);


//            //    var requestContent = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
//            //                        {
//            //                            new KeyValuePair<string, string>("data", jsonString)
//            //                        }
//            //        );

//            //    var httpTask = client.PostAsync("data.php", requestContent);
//            //    httpTask.Wait();

//            //    var contentTask = httpTask.Result.Content.ReadAsStringAsync();
//            //    contentTask.Wait();

//            //    var content = contentTask.Result;

//            //    System.Diagnostics.Debug.WriteLine(content);
//            //    System.Diagnostics.Debug.WriteLine("");
//            //}

//        }


//    }


//    class TestData
//    {
//        public string Field1 { get; set; }
//        public string Field2 { get; set; }
//        public string Field3 { get; set; }
//        public string Field4 { get; set; }
//    }
//}
