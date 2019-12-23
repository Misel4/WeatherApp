using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.Net.Http;
using System;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;
using System.IO;
using System.Drawing;
using Android.Graphics;
using Bitmap = Android.Graphics.Bitmap;

namespace WeatherApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button btnWeather;
        TextView txtDegree, txtPlace, txtWeather;
        EditText editCityName;
        ImageView imgWeather;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            editCityName = (EditText)FindViewById(Resource.Id.getWeather);
            btnWeather = (Button)FindViewById(Resource.Id.but_weather);
            txtDegree = (TextView)FindViewById(Resource.Id.txt_degree);
            txtPlace = (TextView)FindViewById(Resource.Id.txt_city);
            txtWeather = (TextView)FindViewById(Resource.Id.txt_weather);
            imgWeather = (ImageView)FindViewById(Resource.Id.WeatherImage);

            btnWeather.Click += BtnWeather_Click;
        }

        private void BtnWeather_Click(object sender, System.EventArgs e)
        {
            string place = editCityName.Text;
            GetWeather(place);
        }

       async void GetWeather(string place)
        {
            string apiKey = "9445c2300f2256b14ec06d059060f82f";
            string apiBase = "https://api.openweathermap.org/data/2.5/weather?q=";
            string unit = "metric";

            if (string.IsNullOrEmpty(place))
            {
                Toast.MakeText(this,"Enter a city place", ToastLength.Short).Show();
                return;
            }

            string url = apiBase + place + "&appid=" + apiKey + "&units=" + unit;

            var handler = new HttpClientHandler();
            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync(url);

            Console.WriteLine(result);

            var resultObject = JObject.Parse(result);
            string weatherDescription = resultObject["weather"][0]["description"].ToString();
            string icon = resultObject["weather"][0]["icon"].ToString();
            string temperature = resultObject["main"]["temp"].ToString();
            string placename = resultObject["name"].ToString();
            string country = resultObject["sys"]["country"].ToString();
            weatherDescription = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(weatherDescription);

            txtDegree.Text = temperature;
            txtWeather.Text = weatherDescription;
            txtPlace.Text = placename + ", " + country;

            string imageUrl = "http://openweathermap.org/img/wn/" + icon + ".png";
            System.Net.WebRequest request = default(System.Net.WebRequest);
            request = WebRequest.Create(imageUrl);
            request.Timeout = int.MaxValue;
            request.Method = "GET";

            WebResponse response = default(WebResponse);
            response = await request.GetResponseAsync();
            MemoryStream ms = new MemoryStream();
            response.GetResponseStream().CopyTo(ms);
            byte[] imageData = ms.ToArray();

            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            imgWeather.SetImageBitmap(bitmap);



        }
    }
}